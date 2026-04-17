namespace iWallet.Infrastructure.Configruations
{
    public class BeneficiaryConfiguration : IEntityTypeConfiguration<Beneficiary>
    {
        public void Configure(EntityTypeBuilder<Beneficiary> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(n => n.Name)
                .IsRequired()
                .HasMaxLength(24);

            builder.Property(wn=> wn.WalletNumber)
                .IsRequired()
                .HasMaxLength(12);

           builder.HasIndex(wn=> wn.WalletNumber)
                .IsUnique();

            builder.HasIndex(n => n.Name)
                .IsUnique();

        }
    }
}
