using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Restaurant.Business.Interfaces;
using Restaurant.Common.Dtos.AdminAccount;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Common.Constants;
using Restaurant.API.Attributes;
using Restaurant.API.Extensions;
using Restaurant.Common.Enums;
using Restaurant.Entities.Models;
using Microsoft.Extensions.Configuration;
using AutoMapper;

namespace Restaurant.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [BearerAuthorize]
    [ApiController]
    public class AdminAccountController : ControllerBase
    {
        private readonly AuthenticationDto _authenticationDto;
        private readonly ILogger<AdminAccountController> _logger;
        private readonly IAdminAccountBusiness _adminAccountBusiness;
        private readonly IEmailBusiness _emailBusiness;
        private readonly IConfiguration _appSetting;
        private readonly IMapper _mapper;

        public AdminAccountController(IHttpContextAccessor httpContextAccessor,
            ILogger<AdminAccountController> logger,
            IAdminAccountBusiness adminAccountBusiness,
            IEmailBusiness emailBusiness,
            IConfiguration appSetting,
            IMapper mapper)
        {
            _authenticationDto = httpContextAccessor.HttpContext.User.ToAuthenticationDto();
            _logger = logger;
            _adminAccountBusiness = adminAccountBusiness;
            _emailBusiness = emailBusiness;
            _appSetting = appSetting;
            _mapper = mapper;
        }

        // GET: /adminaccount
        [ClaimRequirement("", "admin_user_list")]
        [HttpGet]
        public async Task<IPaginatedList<AccountDto>> Get(int restaurantId, int branchId,
            int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        {
            if (_authenticationDto.TypeId == (int)EAccountType.Admin || _authenticationDto.TypeId == (int)EAccountType.Mod)
            {
                if (restaurantId > 0)
                {
                    return await _adminAccountBusiness.GetAllByRestaurant(restaurantId,
                branchId, _authenticationDto.TypeId, pageIndex, pageSize);
                }
                return await _adminAccountBusiness.GetAll(pageIndex, pageSize);
            }
            return await _adminAccountBusiness.GetAllByRestaurant(_authenticationDto.RestaurantId,
                _authenticationDto.BranchId, _authenticationDto.TypeId, pageIndex, pageSize);
        }
        // GET: /adminaccount/5
        [ClaimRequirement("", "admin_user_update")]
        [HttpGet("{id}")]
        public async Task<AccountDto> Get(int id)
        {
            return await _adminAccountBusiness.GetById(_authenticationDto.RestaurantId,
                _authenticationDto.BranchId, id);
        }
        // POST: /adminaccount
        [ClaimRequirement("", "admin_user_create")]
        [HttpPost]
        public async Task<int> Post(AdminAccount model)
        {
            string user = _appSetting.GetValue<string>("AppSettings:EmailAuthentication:UserName");
            string password = _appSetting.GetValue<string>("AppSettings:EmailAuthentication:Password");
            string activeUrl = _appSetting.GetValue<string>("AppSettings:ActiveUrl");
            //restaurant owner create account
            if (_authenticationDto.RestaurantId > 0)
            {
                model.RestaurantId = _authenticationDto.RestaurantId;
            }
            var result = 0;
            if (ModelState.IsValid)
            {
                var dateTimeUtcNow = DateTime.Now;
                model.PasswordHash = WebsiteExtension.EncryptPassword(model.PasswordHash);
                model.CreatedStaffId = _authenticationDto.UserId;
                model.CreatedDate = dateTimeUtcNow;
                model.Status = 1;
                model.Active = 0;
                //recheck eamil and username is existed
                if (!await _adminAccountBusiness.CheckEmailExist(model.Email) ||
                    !await _adminAccountBusiness.CheckUserNameExist(model.UserName))
                {
                    return result;
                }
                var modelInsert = await _adminAccountBusiness.Add(model);
                result = modelInsert.Id;
                if (result > 0)
                {
                    await _emailBusiness.SendEmailToActiveAccount(modelInsert, user, password, activeUrl);
                }
            }
            return result;
        }

        // PUT: /adminaccount/5
        [ClaimRequirement("", "admin_user_update")]
        [HttpPut("{id}")]
        public async Task<bool> Put(AccountDto model)
        {
            var result = false;
            if (ModelState.IsValid)
            {
                var dateTimeUtcNow = DateTime.Now;
                if (!string.IsNullOrEmpty(model.PasswordHash))
                {
                    model.PasswordHash = WebsiteExtension.EncryptPassword(model.PasswordHash);
                }
                model.UpdatedStaffId = _authenticationDto.UserId;
                model.UpdatedDate = dateTimeUtcNow;
                result = await _adminAccountBusiness.Update(model);
            }
            return result;
        }

        // PUT: /adminaccount/active
        [ClaimRequirement("", "admin_user_delete")]
        [HttpPut("active")]
        public async Task<bool> Put(int id, int Active)
        {
            return await _adminAccountBusiness.SetActive(id, Active);
        }

        // DELETE: /adminaccount/5
        [ClaimRequirement("", "admin_user_delete")]
        [HttpDelete("{id}")]
        public Task<bool> Delete(int id)
        {
            var result = _adminAccountBusiness.Delete(id);
            return result;
        }
        // GET: /adminaccount/checkusername
        [Route("checkusername/{username}")]
        [HttpGet]
        public async Task<bool> CheckUserNameExist(string username)
        {
            return await _adminAccountBusiness.CheckUserNameExist(username);
        }
        // GET: /adminaccount/checkemail
        [Route("checkemail/{email}")]
        [HttpGet]
        public async Task<bool> CheckEmailExist(string email)
        {
            return await _adminAccountBusiness.CheckEmailExist(email);
        }
        // GET: /adminaccount/resendactiveaccountemail
        [ClaimRequirement("", "admin_user_resendactiveemail")]
        [Route("resendactiveaccountemail")]
        [HttpGet]
        public async Task<bool> ResendActiveAccountEmail(int id)
        {
            string user = _appSetting.GetValue<string>("AppSettings:EmailAuthentication:UserName");
            string password = _appSetting.GetValue<string>("AppSettings:EmailAuthentication:Password");
            string activeUrl = _appSetting.GetValue<string>("AppSettings:ActiveUrl");

            var model = await _adminAccountBusiness.GetById(_authenticationDto.RestaurantId,
                _authenticationDto.BranchId, id);

            var convertedModel=_mapper.Map<AdminAccount>(model);

            return await _emailBusiness.SendEmailToActiveAccount(convertedModel, user, password, activeUrl);
        }
    }
}