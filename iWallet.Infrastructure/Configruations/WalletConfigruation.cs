namespace iWallet.Infrastructure.Configruations
{
    public class WalletConfigruation : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(wn => wn.WalletNumber)
                .IsRequired()
                .HasMaxLength(12);

            builder.HasIndex(wn => wn.WalletNumber)
                .IsUnique();

            builder.Property(b => b.Balance)
                .IsRequired()
                 .HasColumnType("decimal(18,2)");

            builder.HasOne(u => u.User)
                .WithMany(w => w.Wallets)
                .HasForeignKey(u => u.UserId);
        }
    }
}
