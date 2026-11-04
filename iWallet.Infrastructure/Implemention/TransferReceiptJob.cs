
namespace iWallet.Infrastructure.Implemention
{
    public class TransferReceiptJob : ITransferReceiptJob
    {
        const string module = "background_job";
        const string operation = "generate";
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TransferReceiptJob> _logger;
        public TransferReceiptJob(ApplicationDbContext context, ILogger<TransferReceiptJob> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task GenerateReceiptAsync(string reference)
        {


            using var activity = BackgroundJobActivitySource.Source.StartActivity("job.generate_receipt");

            Stopwatch stopwatch = Stopwatch.StartNew();

            activity?.AddEvent(new ActivityEvent("generate.receipt.started"));

            var transaction = await _context.Transactions
                   .AsNoTracking()
                   .Include(t => t.FromWallet)
                        .ThenInclude(su => su.User)
                    .Include(t => t.ToWallet)
                        .ThenInclude(ru => ru.User)
                    .FirstOrDefaultAsync(r=> r.Reference == reference);

            if (transaction == null)
            {
                activity?.SetTag("receipt.job.outcome", "failure");
                BusinessMetricsHelper.RecordBusinessOperation(module, operation, "failure");
                _logger.LogWarning("Transaction reference: {Reference} not found", reference);

                throw new Exception($"Transaction {reference} not found");
            }

            var receipt = new TransferReceiptDto
            {
                TransactionReference = transaction.Reference,
                SenderName = transaction.FromWallet.User.UserName,
                SenderWalletNumber = transaction.FromWallet.WalletNumber,

                ReceiverName = transaction.ToWallet.User.UserName,
                ReceiveWalletNumber = transaction.FromWallet.WalletNumber,

                Amount = transaction.Amount,
                Fees = 0,

                Currency = "SRA",
                Date = transaction.CreatedAt,
                Status = transaction.Status.ToString()
            };

            var directory = Path.Combine("receipts");
            Directory.CreateDirectory(directory);

            var logoPath = Path.Combine("wwwroot/assets", "iWallet-logo.png");


            var document = new TransferReceiptDocument(receipt, logoPath);
            document.GeneratePdf(Path.Combine(directory, $"receipt-{reference}.pdf"));
            stopwatch.Stop();

            _logger.LogInformation("Receipt generated Successfly  Reference: {Reference}", reference);

            BusinessMetricsHelper.RecordBusinessOperation(module, operation, "success");
            BusinessMetricsHelper.RecordBusinessOperationDuration(module, operation, stopwatch.ElapsedMilliseconds);

            activity?.SetTag("receipt.job.outcome", "success");
            activity?.AddEvent(new ActivityEvent("generate.receipt.end.successfly"));
            
        }
    }
    }

