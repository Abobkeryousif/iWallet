using System.Linq.Expressions;

namespace iWallet.Infrastructure.Implemention
{
    public class UserRepository : IUserRepository, IIsExistMethod<User>
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool IsExist(Expression<Func<User, bool>> filter = null)
        {
            return _context.Users.Any(filter);
        }

        public async Task<string> UserRegister(UserDto userDto)
        {
            var checkUserIsExist = IsExist(u => u.Email == userDto.Email);
            if (checkUserIsExist)
                throw new ArgumentException("this user already added");

            var encriptedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            var user = new User
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                City = userDto.City,
                BirthDate = userDto.BirthDate,
                IsActive = false,
                Role = "USER",
                Password = encriptedPassword
            };

            
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return $"Register Completed Successfly";
        }
    }
}
