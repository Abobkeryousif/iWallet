namespace iWallet.Infrastructure.Configruations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(r => r.Reference)
                .IsRequired()
                .HasMaxLength(25);

            builder.Property(a => a.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(d=> d.Description)
                .HasMaxLength(60);

            builder.HasOne(x => x.FromWallet)
             .WithMany()
             .HasForeignKey(x => x.FromWalletId)
             .OnDelete(DeleteBehavior.Restrict);

            
            builder.HasOne(x => x.ToWallet)
                   .WithMany()
                   .HasForeignKey(x => x.ToWalletId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(s => s.Status)
                .HasConversion<string>();


        }
    }
}
