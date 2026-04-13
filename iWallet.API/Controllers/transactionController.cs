namespace iWallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "USER")]
    public class transactionController : ControllerBase
    {
        private readonly IUnitofwork _unitofwork;

        public transactionController(IUnitofwork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        [HttpPost("deposit")]
    
        public async Task<IActionResult> MakeDepositAsync(int walletId, decimal ammount)
        {
            return Ok(await _unitofwork.TransactionRepository.MakeDepositAsync(walletId, ammount));
        }

        [HttpPost("transfer")]
        

        public async Task<IActionResult> TransferAsync(TransferTransactionDto transferDto)
        {
            int userId = GetUserIdFromToken();
            return Ok (await _unitofwork.TransactionRepository.TransferAsync(transferDto.toAccountNumber,transferDto.amount,userId));

        }


        [HttpPost("withdrawal")]

        public async Task<IActionResult> WithdrawalAsync(WithdrawalDto withdrawalDto)
        {
            return Ok(await _unitofwork.TransactionRepository.MakeWithdrawal(withdrawalDto.walletId , withdrawalDto.amount));
        }


        private int GetUserIdFromToken()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                throw new UnauthorizedAccessException("User Id not found or invalid token");

            return userId;
        }
    }
}
