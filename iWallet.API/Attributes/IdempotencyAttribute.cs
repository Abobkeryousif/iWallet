using iWallet.Domain.Entities.Enums;

namespace iWallet.API.Attributes
{
    public class IdempotencyAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _ttlHours;

        public IdempotencyAttribute(int ttlHours = 20)
        {
            _ttlHours = ttlHours;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var redis = context.HttpContext.RequestServices
                .GetRequiredService<IIDempotencyService>();

            var http = context.HttpContext;

            var userId = http.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";

            var endpoint = http.Request.Path.ToString();

            var headerKey = http.Request.Headers["Idempotency-Key"].FirstOrDefault();

            var request = context.ActionArguments.Values
                .OfType<TransferRequest>()
                .FirstOrDefault();

            string finalKey;


            if (!string.IsNullOrWhiteSpace(headerKey))
            {
                finalKey = headerKey;
            }

            else if (request != null)
            {
                finalKey = GenerateHash(
                    request.UserId,
                    request.amount,
                    request.type
                );
            }

            else
            {
                finalKey = Guid.NewGuid().ToString();
            }

            var redisKey = $"idem:{userId}:{endpoint}:{finalKey}";

         
            var created = await redis.CreateRequestAsync(
                redisKey,
                TimeSpan.FromHours(_ttlHours)
            );


            if (!created)
            {
                var existing = await redis.GetAsync(redisKey);

                if (existing == "PROCESSING")
                {
                    context.Result = new ContentResult
                    {
                        Content = "Request is still processing",
                        StatusCode = 409
                    };
                    return;
                }

                context.Result = new ContentResult
                {
                    Content = existing,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                return;
            }

            // Execute actual action
            var executed = await next();

            // Save response if success
            if (executed.Exception == null)
            {
                var response = executed.Result is ObjectResult obj
                    ? System.Text.Json.JsonSerializer.Serialize(obj.Value)
                    : "OK";

                await redis.SetResopnseAsync(
                    redisKey,
                    response,
                    TimeSpan.FromHours(_ttlHours)
                );
            }
            else
            {
                await redis.RemoveRequestAsync(redisKey);
            }
        }

        private static string GenerateHash(int userId, decimal amount, TransactionType type)
        {
            var normalizedAmount = amount.ToString("F2");

            var raw = $"{userId}:{type}:{normalizedAmount}";

            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));

            return Convert.ToHexString(bytes);
        }
    }
}