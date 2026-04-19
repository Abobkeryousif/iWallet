
namespace iWallet.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {}

        public DbSet<User> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<LedgerEntry> LedgerEntries { get; set; }
        public DbSet<Otp> OTPs { get; set; }

        public DbSet<Beneficiary> Beneficiaries { get; set; }

        public DbSet<UserLimit> UserLimits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Wallet>()
                .Property(wt => wt.WalletType)
                  .HasConversion<string>();

            modelBuilder.Entity<Wallet>()
                .Property(ws=> ws.Status)
                  .HasConversion<string>();

            modelBuilder.Entity<Transaction>()
                .Property(tt=> tt.TransactionType)
                .HasConversion<string>();
            
            modelBuilder.Entity<Transaction>()
                .Property(ts=> ts.TransactionType)
                  .HasConversion<string>();

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<UserLimit>()
                .Property(l => l.PerTransactionLimit)
                .HasMaxLength(5000);

            modelBuilder.Entity<UserLimit>()
                .Property(l => l.DailyLimit)
                .HasMaxLength(20000);



        }
    }
}
