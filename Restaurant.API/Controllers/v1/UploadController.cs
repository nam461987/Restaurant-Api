using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.API.Attributes;
using Restaurant.Common.Dtos.AdminAccount;
using Restaurant.Business.Interfaces;
using Restaurant.API.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Restaurant.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [BearerAuthorize]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly AuthenticationDto _authenticationDto;
        private readonly IUploadBusiness _uploadBusiness;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _appSetting;
        public UploadController(IHttpContextAccessor httpContextAccessor,
            IUploadBusiness uploadBusiness,
            IHostingEnvironment hostingEnvironment,
            IConfiguration appSetting)
        {
            _authenticationDto = httpContextAccessor.HttpContext.User.ToAuthenticationDto();
            _uploadBusiness = uploadBusiness;
            _hostingEnvironment = hostingEnvironment;
            _appSetting = appSetting;
        }
        [HttpPost]
        public Task<string> MultipleUpload(IFormFile file)
        {
            string forwardFolder = _appSetting.GetValue<string>("AppSettings:ForwardUploadFolderRoot");
            string uploadFolder = _appSetting.GetValue<string>("AppSettings:UploadFolderRoot");

            return _uploadBusiness.MultipleUpload(file, forwardFolder, uploadFolder, _authenticationDto.RestaurantId);
        }
    }
}