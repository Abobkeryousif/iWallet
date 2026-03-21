using Microsoft.AspNetCore.Mvc;

namespace iWallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class walletController : ControllerBase
    {
        private readonly IUnitofwork _unitofwork;

        public walletController(IUnitofwork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        [HttpPost]
        public async Task<IActionResult> CreateWalletAsync(CreateWalletDto createWalletDto)
        {
            var wallet = await _unitofwork.WalletRepository.CreateAsync(createWalletDto);
            return Ok(wallet);
        }
    }
}
