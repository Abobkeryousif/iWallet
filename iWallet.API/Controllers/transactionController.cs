using iWallet.API.Attributes;

namespace iWallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "USER")]
    public class transactionController : ControllerBase
    {
        private readonly IUnitofwork _unitofwork;
        private readonly IGetUserIdFromToken _getUserIdFromToken;

        public transactionController(IUnitofwork unitofwork,IGetUserIdFromToken getUserIdFromToken)
        {
            _unitofwork = unitofwork;
            _getUserIdFromToken = getUserIdFromToken;
        }

        [HttpPost("deposit")]
        [Idempotency(1)]

        public async Task<IActionResult> MakeDepositAsync(int walletId, decimal ammount)
        {
            return Ok(await _unitofwork.TransactionRepository.MakeDepositAsync(walletId, ammount));
        }

        [HttpPost("transfer")]
        [Idempotency(20)]
        
        public async Task<IActionResult> TransferAsync(TransferTransactionDto transferDto)
        {
            int userId = _getUserIdFromToken.UserIdFromToken();
            return Ok (await _unitofwork.TransactionRepository.TransferAsync(transferDto.toAccountNumber,transferDto.amount,userId));

        }


        [HttpPost("withdrawal")]
        [Idempotency(1)]

        public async Task<IActionResult> WithdrawalAsync(WithdrawalDto withdrawalDto)
        {
            return Ok(await _unitofwork.TransactionRepository.MakeWithdrawal(withdrawalDto.walletId , withdrawalDto.amount));
        }


        [HttpPost("beneficiery-transfer")]
        [Idempotency(20)]
        public async Task<IActionResult> TransferToBeneficieryAsync(BeneficieryTransaferDto beneficieryTransafer)
        {
            var userId = _getUserIdFromToken.UserIdFromToken();
            return Ok(await _unitofwork.TransactionRepository.TransferToBeneficiery(beneficieryTransafer.beneficieryName, beneficieryTransafer.amount, userId));    
        }

        [HttpGet("transaction-history")]
        public async Task<IActionResult> GetTransactionHistoryAsync(int walletId) =>
           Ok(await _unitofwork.TransactionRepository.TransactionHistory(walletId));


    }
}
