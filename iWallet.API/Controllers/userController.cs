using Microsoft.AspNetCore.Mvc;
namespace iWallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class userController : ControllerBase
    {
        private readonly IUnitofwork _unitofwork;
        public userController(IUnitofwork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _unitofwork.UserRepository.GetAllUsers();
            return Ok(result);
        }    

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(UserDto userDto)
        {
            var user = await _unitofwork.UserRepository.UserRegister(userDto);
            return Ok("sucess, now we send otp code in your email please confierm it");
        }

        [HttpPost("complete-register")]
        public IActionResult CompleteRegister(string otp)
        {
            var user = _unitofwork.UserRepository.CompleteRegister(otp);
            return Ok("Successfly Complete Register");

        }

        [HttpPost("login")]

        public async Task<IActionResult> UserLoginAsync(LoginDto loginDto)
        {
            return Ok(await _unitofwork.UserRepository.UserLoginAsync(loginDto));
        }


        [HttpPost("resend-otp")]
        public IActionResult ResendOtp(string userEmail) 
        {
            var result = _unitofwork.OtpRepository.ResendOtp(userEmail);
            return Ok("Resend Otp Complete, confierm your account by apply new otp code");
        }

        [HttpPatch("reset-email")]

        public IActionResult ResetUserEmail([FromQuery]int id ,[FromBody]UpdateUserEmailDto updateUserEmail)
        {
            var updatedUser = _unitofwork.UserRepository.ResetEmail(id,updateUserEmail);
            return Ok("successfly updated email and send otp to new email , please apply it to complete register");
        }

            
    }
}
