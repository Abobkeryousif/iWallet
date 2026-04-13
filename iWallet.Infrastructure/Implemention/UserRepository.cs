
namespace iWallet.Infrastructure.Implemention
{
    public class UserRepository : IUserRepository, IIsExistMethod<User>
    {
        private readonly ApplicationDbContext _context;
        private readonly ISendEmailService _sendEmail;
        private IOtpRepository _otpRepository;
        private readonly ITokenService _tokenService;

        public UserRepository(ApplicationDbContext context , ISendEmailService sendEmail , IOtpRepository otpRepository, ITokenService tokenService)
        {
            _context = context;
            _sendEmail = sendEmail;
            _otpRepository = otpRepository;
            _tokenService = tokenService;
        }

        public string CompleteRegister(string Otp)
        {
            if (Otp == null || string.IsNullOrEmpty(Otp))
                throw new ArgumentException("otp is requierd!");

            var confiermOtp = _context.OTPs.FirstOrDefault(o=> o.otp == Otp );

            if (confiermOtp == null)
                throw new ArgumentException("invalid otp");

            if (confiermOtp.IsExpier)
                throw new ArgumentException("otp is expierd, ask new otp");

            if (confiermOtp.IsUsed)
                throw new ArgumentException("otp already used!");

            var user = _context.Users.FirstOrDefault(u => u.Email == confiermOtp.UserEmail);

            if (user == null)
                throw new ArgumentException("user not found");

            confiermOtp.IsUsed = true;
            user.IsActive = true;

            _otpRepository.UpdateOtp(confiermOtp);
            _context.Users.Update(user);
            _context.SaveChanges();

            return "success";
        }

        public async Task<List<GetUsersDto>> GetAllUsers()
        {
            var users = await _context.Users.Where(u=> u.IsActive == true).AsNoTracking().ToListAsync();
            var result = users.Select( u=> new GetUsersDto
            {
              FirstName = u.FirstName,
              LastName = u.LastName,
              Email = u.Email,
              PhoneNumber = u.PhoneNumber,
              City = u.City,
              BirthDate = u.BirthDate,
            }).ToList();

            return result;
        }

        public bool IsExist(Expression<Func<User, bool>> filter = null)
        {
            return _context.Users.Any(filter);
        }

        public string ResetEmail(int id , UpdateUserEmailDto updateUserEmail)
        {
            var resetUserEmail = _context.Users.FirstOrDefault(i=> i.Id == id);
            if (resetUserEmail == null)
                throw new ArgumentException("user not found");

            resetUserEmail.Email = updateUserEmail.email;
            _context.Users.Update(resetUserEmail);
            _context.SaveChanges();

            // discard old otp from old email and send new otp to new updated email
            _otpRepository.ResendOtp(resetUserEmail.Email);

            return "success update user email";

        }

        public async Task<string> UserLoginAsync(LoginDto loginDto)
            {
            var loginUser = await _context.Users.FirstOrDefaultAsync(e=> e.Email == loginDto.email);
            if (loginUser == null)
                throw new Exception("invalid email or password");

            var checkUserPassword = BCrypt.Net.BCrypt.Verify(loginDto.password,loginUser.Password);
                if (!checkUserPassword)
                    throw new Exception("invalid email or password");

            var token = _tokenService.GenerateJwtToken(loginUser);
            _tokenService.WriteTokenToCookie("ACCESS_TOKEN", token ,DateTime.UtcNow.AddMinutes(15));

            return $"Welcome back: {loginUser.UserName}";
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

            Random random = new Random();
            int otp = random.Next(0, 9999);


            var userOtp = new Otp
            {
                otp = otp.ToString("0000"),
                UserEmail = user.Email,
                ExpirationOn = DateTime.Now.AddMinutes(5),
                IsUsed = false,
            };

            _otpRepository.CreateOtp(userOtp);

            //_sendEmail.SendEmail(user.Email,"Confierm Register",$"Please Confierm this code to complete register {userOtp.otp}");



            return $"Register Complete now confierm your account";
        }


    }
}
