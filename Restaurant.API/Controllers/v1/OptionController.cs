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
using Microsoft.AspNetCore.Authorization;

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
        //[Route("getadmingroup")]
        [HttpGet("getadmingroup")]
        public async Task<List<OptionModel>> GetAdminGroupOptions()
        {
            return await _optionBusiness.GetAdminGroupOptions();
        }
        //[Route("getrestaurant")]
        [HttpGet("getrestaurant")]
        public async Task<List<OptionModel>> GetRestaurantOptions()
        {
            return await _optionBusiness.GetRestaurantOptions(_authenticationDto.RestaurantId);
        }
        //[Route("getbranch")]
        [HttpGet("getbranch")]
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
        //[Route("getcategory")]
        [HttpGet("getcategory")]
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
        //[Route("getunit")]
        [HttpGet("getunit")]
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
        //[Route("getmenu")]
        [HttpGet("getmenu")]
        public async Task<List<OptionModel>> GetMenuOptions(int id = 0)
        {
            if (_authenticationDto.TypeId == (int)EAccountType.Admin || _authenticationDto.TypeId == (int)EAccountType.Mod)
            {
                if (id > 0)
                {
                    return await _optionBusiness.GetMenuOptions(id, 0);
                }
            }
            return await _optionBusiness.GetMenuOptions(_authenticationDto.RestaurantId, 0);
        }
        //[Route("getsize")]
        [HttpGet("getsize")]
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
        //[Route("getstate")]
        [HttpGet("getstate")]
        public async Task<List<OptionModel>> GetStateOptions()
        {
            return await _optionBusiness.GetStateOptions();
        }
        //[Route("getcity")]
        [HttpGet("getcity")]
        public async Task<List<OptionModel>> GetCityOptions(int id)
        {
            return await _optionBusiness.GetCityOptions(id);
        }
        //[Route("getcustomer")]
        [HttpGet("getcustomer")]
        public async Task<List<OptionModel>> GetCustomerOptions(int id = 0)
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
        //[Route("getorderchannel")]
        [HttpGet("getorderchannel")]
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
        //[Route("gettable")]
        [HttpGet("gettable")]
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
        //[Route("getorderprocess")]
        [HttpGet("getorderprocess")]
        public async Task<List<OptionModel>> GetOrderProcessOptions()
        {
            return await _optionBusiness.GetOrderProcessOptions();
        }
        //[Route("getaccount")]
        [HttpGet("getaccount")]
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
        //[Route("getingredient")]
        [HttpGet("getingredient")]
        public async Task<List<OptionModel>> GetIngredientOptions(int id = 0)
        {
            return await _optionBusiness.GetIngredientOptions(_authenticationDto.RestaurantId, _authenticationDto.BranchId,
                id);
        }
        //[Route("getingredientwithunit")]
        [HttpGet("getingredientwithunit")]
        public async Task<List<OptionModel>> GetIngredientWithUnitOptions(int id = 0)
        {
            return await _optionBusiness.GetIngredientWithUnitOptions(_authenticationDto.RestaurantId, _authenticationDto.BranchId,
                id);
        }
        //[Route("gettax")]
        [HttpGet("gettax")]
        public async Task<List<OptionModel>> GetTaxOptions(int id = 0)
        {
            return await _optionBusiness.GetTaxOptions(_authenticationDto.RestaurantId, _authenticationDto.BranchId,
                id);
        }
    }
}