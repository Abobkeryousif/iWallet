namespace iWallet.Infrastructure.Data
{
    public static class DatabaseExtension
    {
        public static async Task ApplyMigrationAsync<TContext>(
            this IServiceProvider service,
            int retryCount = 7,
            int delaySeconds = 5) where TContext : DbContext
        {
            using var scope = service.CreateScope();

            var dbContext = scope.ServiceProvider
            .GetRequiredService<TContext>();

            for (var attempt = 1; attempt <= retryCount; attempt++)
            {
                try
                {
                    Console.WriteLine(
                        $"Applying database migrations... Attempt {attempt}/{retryCount}"
                    );

                    await dbContext.Database.MigrateAsync();

                    Console.WriteLine(
                        "Database migrations applied successfully."
                    );

                    return;
                }
                catch (Exception ex) when (attempt < retryCount)
                {
                    Console.WriteLine(
                        $"Database is not ready yet. Retrying in {delaySeconds} seconds..."
                    );

                    await Task.Delay(
                        TimeSpan.FromSeconds(delaySeconds)
                    );
                }
            }

            throw new Exception(
                "Could not connect to the database after multiple attempts."
            );
        }
    }

    }
