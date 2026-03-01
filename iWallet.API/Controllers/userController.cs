using iWallet.Application.DTOs;
using iWallet.Application.Interface;

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

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(UserDto userDto)
        {
            var user = await _unitofwork.UserRepository.UserRegister(userDto);
            return Ok("user successfly added");
        }
    }
}
