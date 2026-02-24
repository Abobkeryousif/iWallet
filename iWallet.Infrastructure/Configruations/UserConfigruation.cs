namespace iWallet.Infrastructure.EntitiesConfigruation
{
    public class UserConfigruation : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(fn => fn.FirstName)
                .IsRequired()
                .HasMaxLength(15);

            builder.Property(ln => ln.LastName)
                .IsRequired()
                .HasMaxLength(15);

            builder.Property(p => p.PhoneNumber)
              .IsRequired()
              .HasMaxLength(15);

            builder.HasIndex(p => p.PhoneNumber)
               .IsUnique();

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(30);

            builder.HasIndex(e=> e.Email)
                .IsUnique();


            builder.Property(c=> c.City)
                .HasMaxLength(15);

            builder.Property(r=> r.Role)
               .HasMaxLength(15);


            builder.Property(p => p.Password)
                .IsRequired()
                .HasMaxLength(35);
        }
    }
}
