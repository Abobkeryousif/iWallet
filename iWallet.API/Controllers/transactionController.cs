using Microsoft.AspNetCore.Mvc;
namespace iWallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class transactionController : ControllerBase
    {
     private readonly IUnitofwork _unitofwork;

        public transactionController(IUnitofwork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        [HttpPost]
        public async Task<IActionResult> MakeDepositAsync(int walletId, decimal ammount)
        {
            return Ok (await _unitofwork.TransactionRepository.MakeDepositAsync(walletId, ammount));
        }
    }
}
