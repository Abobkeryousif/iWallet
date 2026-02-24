
namespace iWallet.Infrastructure.Configruations
{
    public class LedgerEntryConfiguration : IEntityTypeConfiguration<LedgerEntry>
    {
        public void Configure(EntityTypeBuilder<LedgerEntry> builder)
        {
            builder.HasKey(l => l.Id);

            builder.Property(d => d.Debit)
               .IsRequired()
               .HasColumnType("decimal(18,2)");

            builder.Property(c => c.Credit)
               .IsRequired()
               .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Particulars)
                .IsRequired()
                .HasMaxLength(300);

            builder.HasOne(t => t.Transaction);
            builder.HasOne(w => w.Wallet);
        }
    }
}
