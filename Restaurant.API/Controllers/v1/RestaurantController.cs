using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.API.Attributes;
using Restaurant.Common.Dtos.AdminAccount;
using Restaurant.Business.Interfaces;
using Restaurant.API.Extensions;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Entities.Models;
using Restaurant.Common.Constants;
using Microsoft.Extensions.Configuration;
using Restaurant.Common.Dtos.Restaurant;

namespace Restaurant.API.Controllers.v1
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/v1/[controller]")]
    [BearerAuthorize]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private readonly AuthenticationDto _authenticationDto;
        private readonly IRestaurantBusiness _restaurantBusiness;
        private readonly IAdminAccountBusiness _adminAccountBusiness;
        private readonly IEmailBusiness _emailBusiness;
        private readonly IConfiguration _appSetting;
        public RestaurantController(IHttpContextAccessor httpContextAccessor,
            IRestaurantBusiness restaurantBusiness,
            IEmailBusiness emailBusiness,
            IConfiguration appSetting,
            IAdminAccountBusiness adminAccountBusiness)
        {
            _authenticationDto = httpContextAccessor.HttpContext.User.ToAuthenticationDto();
            _restaurantBusiness = restaurantBusiness;
            _adminAccountBusiness = adminAccountBusiness;
            _emailBusiness = emailBusiness;
            _appSetting = appSetting;
        }
        // GET: /restaurant
        [ClaimRequirement("", "category_restaurant_list")]
        [HttpGet]
        public async Task<IPaginatedList<RestaurantDto>> Get(int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        {
            return await _restaurantBusiness.GetAll(pageIndex, pageSize);
        }
        // GET: /restaurant/5
        [ClaimRequirement("", "category_restaurant_update")]
        [HttpGet("{id}")]
        public async Task<RestaurantDto> Get(int id)
        {
            return await _restaurantBusiness.GetById(id);
        }
        // POST: /restaurant
        [ClaimRequirement("", "category_restaurant_create")]
        [HttpPost]
        public async Task<int> Post(Entities.Models.Restaurant model)
        {
            string user = _appSetting.GetValue<string>("AppSettings:EmailAuthentication:UserName");
            string password = _appSetting.GetValue<string>("AppSettings:EmailAuthentication:Password");
            string activeUrl = _appSetting.GetValue<string>("AppSettings:ActiveUrl");
            var result = 0;
            if (ModelState.IsValid)
            {
                model.Status = 1;
                var modelInsert = await _restaurantBusiness.Add(model);
                result = modelInsert.Id;
                if (result > 0)
                {
                    var dateTimeUtcNow = DateTime.Now;
                    var accountModel = new AdminAccount
                    {
                        UserName = WebsiteExtension.Slug(modelInsert.Name),
                        RestaurantId = modelInsert.Id,
                        Email = modelInsert.Email,
                        PasswordHash = WebsiteExtension.EncryptPassword(WebsiteExtension.Slug(modelInsert.Name) + "123"),
                        CreatedStaffId = _authenticationDto.UserId,
                        CreatedDate = dateTimeUtcNow,
                        Status = 1,
                        Active = 0
                    };
                    var accountInsert = await _adminAccountBusiness.Add(accountModel);

                    if (accountInsert.Id > 0)
                    {
                        await _emailBusiness.SendEmailToRestaurantAdmin(accountInsert, user, password, activeUrl);
                    }
                }
            }
            return result;
        }
        // PUT: /restaurant/5
        [ClaimRequirement("", "category_restaurant_update")]
        [HttpPut("{id}")]
        public async Task<bool> Put(Entities.Models.Restaurant model)
        {
            var result = false;
            if (ModelState.IsValid)
            {
                result = await _restaurantBusiness.Update(model);
            }
            return result;
        }

        // PUT: /restaurant/active
        [ClaimRequirement("", "category_restaurant_delete")]
        [HttpPut("active")]
        public async Task<bool> Put(int id, int Status)
        {
            return await _restaurantBusiness.SetActive(id, Status);
        }

        // DELETE: /restaurant/5
        [ClaimRequirement("", "category_restaurant_delete")]
        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id)
        {
            var result = await _restaurantBusiness.Delete(id);

            return result;
        }
    }
}