namespace iWallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "USER")]
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

        [HttpGet]
        public async Task<IActionResult> GetWalletsAsync() =>
            Ok(await _unitofwork.WalletRepository.GetWalletsAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetWalletByIdAsync(int id)
        {
            var getWallet = await _unitofwork.WalletRepository.GetWalletById(id);
            return Ok(getWallet);
        }

        [HttpGet("get-by-wallet-number")]
        public async Task<IActionResult> GetByWalletNumberAsync(string walletNumber) =>
            Ok( await _unitofwork.WalletRepository.GetByWalletNumber(walletNumber) );

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PatchWalletBalance(int id, decimal balance)
        {
            var updatedWalletBalance = await _unitofwork.WalletRepository.PatchWalletBalance(id, balance);
            return Ok(updatedWalletBalance);
        }
    }
}
