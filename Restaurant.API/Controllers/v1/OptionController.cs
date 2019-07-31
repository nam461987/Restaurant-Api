using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.API.Attributes;
using Restaurant.Common.Dtos.AdminAccount;
using Restaurant.Business.Interfaces;
using Restaurant.API.Extensions;
using Restaurant.Common.Models;
using Restaurant.Common.Enums;

namespace Restaurant.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [BearerAuthorize]
    [ApiController]
    public class OptionController : ControllerBase
    {
        private readonly AuthenticationDto _authenticationDto;
        private readonly IOptionBusiness _optionBusiness;
        public OptionController(IHttpContextAccessor httpContextAccessor,
            IOptionBusiness optionBusiness)
        {
            _authenticationDto = httpContextAccessor.HttpContext.User.ToAuthenticationDto();
            _optionBusiness = optionBusiness;
        }
        [Route("getadmingroup")]
        [HttpGet]
        public async Task<List<OptionModel>> GetAdminGroupOptions()
        {
            return await _optionBusiness.GetAdminGroupOptions();
        }
        [Route("getrestaurant")]
        [HttpGet]
        public async Task<List<OptionModel>> GetRestaurantOptions()
        {
            return await _optionBusiness.GetRestaurantOptions(_authenticationDto.RestaurantId);
        }
        [Route("getbranch")]
        [HttpGet]
        public async Task<List<OptionModel>> GetBranchOptions(int id = 0)
        {
            if (_authenticationDto.TypeId == (int)EAccountType.Admin || _authenticationDto.TypeId == (int)EAccountType.Mod)
            {
                if (id > 0)
                {
                    return await _optionBusiness.GetBranchOptions(id, 0);
                }
                return await _optionBusiness.GetBranchOptions(_authenticationDto.RestaurantId, 0);
            }
            return await _optionBusiness.GetBranchOptions(_authenticationDto.RestaurantId, _authenticationDto.BranchId);
        }
        [Route("getcategory")]
        [HttpGet]
        public async Task<List<OptionModel>> GetCategoryOptions(int id = 0)
        {
            if (_authenticationDto.TypeId == (int)EAccountType.Admin || _authenticationDto.TypeId == (int)EAccountType.Mod)
            {
                if (id > 0)
                {
                    return await _optionBusiness.GetCategoryOptions(id, 0);
                }
            }
            return await _optionBusiness.GetCategoryOptions(_authenticationDto.RestaurantId, 0);
        }
        [Route("getunit")]
        [HttpGet]
        public async Task<List<OptionModel>> GetUnitOptions(int id = 0)
        {
            if (_authenticationDto.TypeId == (int)EAccountType.Admin || _authenticationDto.TypeId == (int)EAccountType.Mod)
            {
                if (id > 0)
                {
                    return await _optionBusiness.GetUnitOptions(id, 0);
                }
            }
            return await _optionBusiness.GetUnitOptions(_authenticationDto.RestaurantId, 0);
        }
        [Route("getsize")]
        [HttpGet]
        public async Task<List<OptionModel>> GetSizeOptions(int id = 0)
        {
            if (_authenticationDto.TypeId == (int)EAccountType.Admin || _authenticationDto.TypeId == (int)EAccountType.Mod)
            {
                if (id > 0)
                {
                    return await _optionBusiness.GetSizeOptions(id, 0);
                }
            }
            return await _optionBusiness.GetSizeOptions(_authenticationDto.RestaurantId, 0);
        }
        [Route("getstate")]
        [HttpGet]
        public async Task<List<OptionModel>> GetStateOptions()
        {
            return await _optionBusiness.GetStateOptions();
        }
        [Route("getcity")]
        [HttpGet]
        public async Task<List<OptionModel>> GetCityOptions(int id)
        {
            return await _optionBusiness.GetCityOptions(id);
        }
        [Route("getcustomer")]
        [HttpGet]
        public async Task<List<OptionModel>> GetCustomerOptions(int id=0)
        {
            if (_authenticationDto.TypeId == (int)EAccountType.Admin || _authenticationDto.TypeId == (int)EAccountType.Mod)
            {
                if (id > 0)
                {
                    return await _optionBusiness.GetCustomerOptions(id, 0);
                }
            }
            return await _optionBusiness.GetCustomerOptions(_authenticationDto.RestaurantId, 0);
        }
        [Route("getorderchannel")]
        [HttpGet]
        public async Task<List<OptionModel>> GetOrderChannelOptions(int id = 0)
        {
            if (_authenticationDto.TypeId == (int)EAccountType.Admin || _authenticationDto.TypeId == (int)EAccountType.Mod)
            {
                if (id > 0)
                {
                    return await _optionBusiness.GetOrderChannelOptions(id, 0);
                }
            }
            return await _optionBusiness.GetOrderChannelOptions(_authenticationDto.RestaurantId, 0);
        }
        [Route("gettable")]
        [HttpGet]
        public async Task<List<OptionModel>> GetTableOptions(int id = 0)
        {
            if (_authenticationDto.TypeId == (int)EAccountType.RestaurantAdmin)
            {
                if (id > 0)
                {
                    return await _optionBusiness.GetTableOptions(_authenticationDto.RestaurantId, id);
                }
            }
            return await _optionBusiness.GetTableOptions(_authenticationDto.RestaurantId, _authenticationDto.BranchId);
        }
        [Route("getorderprocess")]
        [HttpGet]
        public async Task<List<OptionModel>> GetOrderProcessOptions()
        {
            return await _optionBusiness.GetOrderProcessOptions();
        }
        [Route("getaccount")]
        [HttpGet]
        public async Task<List<OptionModel>> GetAccountOptions(int id = 0)
        {
            if (_authenticationDto.TypeId == (int)EAccountType.RestaurantAdmin)
            {
                if (id > 0)
                {
                    return await _optionBusiness.GetAccountOptions(_authenticationDto.RestaurantId, id);
                }
            }
            return await _optionBusiness.GetAccountOptions(_authenticationDto.RestaurantId, _authenticationDto.BranchId);
        }
    }
}