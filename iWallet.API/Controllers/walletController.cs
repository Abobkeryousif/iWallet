namespace iWallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class walletController : ControllerBase
    {
        private readonly IUnitofwork _unitofwork;
        private readonly IGetUserIdFromToken _getUserIdFromToken;
        public walletController(IUnitofwork unitofwork, IGetUserIdFromToken getUserIdFromToken)
        {
            _unitofwork = unitofwork;
            _getUserIdFromToken = getUserIdFromToken;
        }

        [HttpPost]
        [Authorize(Roles = "USER")]
        public async Task<IActionResult> CreateWalletAsync(CreateWalletDto createWalletDto)
        {
            var userid = _getUserIdFromToken.UserIdFromToken();
            var wallet = await _unitofwork.WalletRepository.CreateAsync(userid, createWalletDto.WalletType,createWalletDto.pin);
            return Created("wallet created successfly",wallet);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]

        public async Task<IActionResult> GetWallets() =>
            Ok(await _unitofwork.WalletRepository.GetWalletsAsync());

        [HttpGet("{id:int}")]
        [Authorize(Roles = "USER")]
        public async Task<IActionResult> GetWalletByIdA(int id)
        {
            var getWallet = await _unitofwork.WalletRepository.GetWalletById(id);
            return Ok(getWallet);
        }

        [HttpGet("get-by-wallet-number")]
        [Authorize(Roles = "USER")]
        public async Task<IActionResult> GetByWalletNumber(string walletNumber) =>
            Ok( await _unitofwork.WalletRepository.GetByWalletNumber(walletNumber) );


        [HttpGet("get-user-wallets")]
        [Authorize(Roles = "USER")]
        public async Task<IActionResult> GetUserWallets()
        {
            var userId = _getUserIdFromToken.UserIdFromToken();
            return Ok(await _unitofwork.WalletRepository.GetUserWalletsAsync(userId));
        }

        [HttpPatch("{id:int}")]
        [Authorize(Roles = "USER")]
        public async Task<IActionResult> PatchWalletBalance(int id, decimal balance)
        {
            var updatedWalletBalance = await _unitofwork.WalletRepository.PatchWalletBalance(id, balance);
            return Ok(updatedWalletBalance);
        }
    }
}
