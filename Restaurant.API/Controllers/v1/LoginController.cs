using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Restaurant.Business.Interfaces;
using Restaurant.Common.Responses.AdminAccount;
using Restaurant.Common.Dtos.AdminAccount;
using Restaurant.API.Extensions;

namespace Restaurant.API.Controllers.v1
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IAdminAccountBusiness _adminAccountBusiness;

        public LoginController(ILogger<LoginController> logger,
            IAdminAccountBusiness adminAccountBusiness)
        {
            _logger = logger;
            _adminAccountBusiness = adminAccountBusiness;
        }

        [HttpPost]
        public async Task<LoginResponse> Login(LoginDto model)
        {
            model.Password = WebsiteExtension.EncryptPassword(model.Password);
            return await _adminAccountBusiness.Login(model);
        }

        [HttpGet]
        public async Task<LoginResponse> LoginWithToken(string token)
        {
            return await _adminAccountBusiness.LoginWithToken(token);
        }

        [HttpGet]
        public async Task<int> ActiveAccount(string token)
        {
            return await _adminAccountBusiness.ActiveAccount(token);
        }
    }
}