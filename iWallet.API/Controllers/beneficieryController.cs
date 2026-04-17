
namespace iWallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "USER")]
    public class beneficieryController : ControllerBase
    {
        private readonly IUnitofwork _unitofwork;
        private readonly IGetUserIdFromToken _getUserIdFromToken;
        public beneficieryController(IUnitofwork unitofwork, IGetUserIdFromToken getUserIdFromToken)
        {
            _unitofwork = unitofwork;
            _getUserIdFromToken = getUserIdFromToken;
        }

        [HttpPost]
        public async Task<IActionResult> AddBeneficieryAsync(BeneficieryDto beneficieryDto)
        {
            var userId = _getUserIdFromToken.UserIdFromToken();
            return Ok(await _unitofwork.BeneficiaryRepository.AddBeneficiary(beneficieryDto.Name,beneficieryDto.WalletNumber,userId));
        }
    }
}
