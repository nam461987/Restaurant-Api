using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Business.Interfaces;
using Restaurant.Entities.Models;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Common.Constants;
using Restaurant.Common.Filters;
using Restaurant.API.Attributes;
using Restaurant.API.Extensions;
using Restaurant.Common.Dtos.AdminAccount;
using Restaurant.Common.Enums;
using System.Collections.Generic;

namespace Restaurant.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [BearerAuthorize]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly AuthenticationDto _authenticationDto;
        private readonly ICustomerBusiness _customerBusiness;
        public CustomerController(IHttpContextAccessor httpContextAccessor,
            ICustomerBusiness customerBusiness)
        {
            _authenticationDto = httpContextAccessor.HttpContext.User.ToAuthenticationDto();
            _customerBusiness = customerBusiness;
        }
        //// GET: /Customer
        //[Route("getbyfilter")]
        //[HttpGet]
        //public Task<IPaginatedList<Customer>> Get(int restaurantId, int branchId, string code, string value,
        //    int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        //{
        //    var filter = new CustomerFilter
        //    {
        //        PageIndex = pageIndex,
        //        PageSize = pageSize,
        //        Name = string.Equals("Name", code, StringComparison.OrdinalIgnoreCase) ? value : string.Empty,
        //        Phone = string.Equals("Phone", code, StringComparison.OrdinalIgnoreCase) ? value : string.Empty,
        //    };

        //    if (restaurantId > 0)
        //    {
        //        return _customerBusiness.GetAllByRestaurant(restaurantId, branchId, filter);
        //    }

        //    return _customerBusiness.GetAllByRestaurant(_authenticationDto.RestaurantId,
        //        _authenticationDto.BranchId, filter);
        //}
        //// GET: /Customer
        //[Route("getall")]
        //[HttpGet]
        //public Task<IPaginatedList<Customer>> Get(int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        //{
        //    var filter = new CustomerFilter
        //    {
        //        PageIndex = pageIndex,
        //        PageSize = pageSize,
        //        Name = string.Empty,
        //        Phone = string.Empty,
        //    };

        //    if (_authenticationDto.TypeId == (int)EAccountType.Admin || _authenticationDto.TypeId == (int)EAccountType.Mod)
        //    {
        //        return _customerBusiness.GetAll(pageIndex, pageSize);
        //    }

        //    return _customerBusiness.GetAllByRestaurant(_authenticationDto.RestaurantId,
        //        _authenticationDto.BranchId, filter);
        //}
        //// GET: /customer/5
        //[HttpGet("{id}")]
        //public Task<Customer> Get(int id)
        //{
        //    return _customerBusiness.GetById(id);
        //}
        //// POST: /Customer
        //[HttpPost]
        //public async Task<int> Post(Customer model)
        //{
        //    if (_authenticationDto.RestaurantId > 0)
        //    {
        //        model.RestaurantId = _authenticationDto.RestaurantId;
        //    }

        //    var result = 0;
        //    if (ModelState.IsValid)
        //    {
        //        //Check users add new record for their restaurant
        //        if (WebsiteExtension.CheckRightBeforeAction(_authenticationDto.RestaurantId, _authenticationDto.BranchId, model.RestaurantId, (int)model.BranchId))
        //        {
        //            model.Status = 1;
        //            var modelInsert = await _customerBusiness.Add(model);
        //            result = modelInsert.Id;
        //        }
        //    }
        //    return result;
        //}
        //// PUT: /customer/5
        //[HttpPut("{id}")]
        //public async Task<bool> Put(Customer model)
        //{
        //    var result = false;
        //    if (ModelState.IsValid)
        //    {
        //        if (WebsiteExtension.CheckRightBeforeAction(_authenticationDto.RestaurantId, _authenticationDto.BranchId, model.RestaurantId, (int)model.BranchId))
        //        {
        //            result = await _customerBusiness.Update(model);
        //        }
        //    }
        //    return result;
        //}

        //// PUT: /customer/active
        //[HttpPut("active")]
        //public Task<bool> Put(int id, int Status)
        //{
        //    return _customerBusiness.SetActive(id, Status);
        //}

        //// DELETE: /customer/5
        //[HttpDelete("{id}")]
        //public Task<bool> Delete(int id)
        //{
        //    var result = _customerBusiness.Delete(id);

        //    return result;
        //}
        // GET: /customer/getall
        [ClaimRequirement("", "quick_order_page")]
        [Route("getall")]
        [HttpGet]
        public async Task<List<Customer>> GetAllNotPaginate()
        {
            return await _customerBusiness.GetAllNotPaginate(_authenticationDto.RestaurantId, _authenticationDto.BranchId);
        }
    }
}