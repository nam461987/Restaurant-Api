using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Restaurant.API.Attributes;
using Restaurant.Common.Dtos.AdminAccount;
using Restaurant.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Restaurant.API.Extensions;
using Restaurant.Entities.Models;
using Restaurant.Common.Constants;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Common.Dtos.RestaurantBranch;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Restaurant.API.Controllers.v1
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/v1/[controller]")]
    [BearerAuthorize]
    [ApiController]
    public class RestaurantBranchController : ControllerBase
    {
        private readonly AuthenticationDto _authenticationDto;
        private readonly IRestaurantBranchBusiness _restaurantBranchBusiness;

        public RestaurantBranchController(IHttpContextAccessor httpContextAccessor,
            IRestaurantBranchBusiness restaurantBranchBusiness)
        {
            _authenticationDto = httpContextAccessor.HttpContext.User.ToAuthenticationDto();
            _restaurantBranchBusiness = restaurantBranchBusiness;
        }
        // GET: /restaurant
        [ClaimRequirement("", "category_restaurant_branch_list")]
        [HttpGet]
        public Task<IPaginatedList<BranchDto>> Get(int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        {
            return _restaurantBranchBusiness.GetAll(_authenticationDto.RestaurantId, pageIndex, pageSize);
        }
        // GET: /restaurant/5
        [ClaimRequirement("", "category_restaurant_branch_update")]
        [HttpGet("{id}")]
        public Task<BranchDto> Get(int id)
        {
            return _restaurantBranchBusiness.GetById(_authenticationDto.RestaurantId, id);
        }
        // POST: /restaurant
        [ClaimRequirement("", "category_restaurant_branch_create")]
        [HttpPost]
        public async Task<int> Post(RestaurantBranch model)
        {

            var result = 0;
            if (ModelState.IsValid)
            {
                model.Status = 1;
                var modelInsert = await _restaurantBranchBusiness.Add(model);
                result = modelInsert.Id;
            }
            return result;
        }
        // PUT: /restaurant/5
        [ClaimRequirement("", "category_restaurant_branch_update")]
        [HttpPut("{id}")]
        public async Task<bool> Put(RestaurantBranch model)
        {
            var result = false;
            if (ModelState.IsValid)
            {
                result = await _restaurantBranchBusiness.Update(model);
            }
            return result;
        }

        // PUT: /restaurant/active
        [ClaimRequirement("", "category_restaurant_branch_delete")]
        [HttpPut("active")]
        public Task<bool> Put(int id, int Status)
        {
            return _restaurantBranchBusiness.SetActive(id, Status);
        }

        // DELETE: /restaurant/5
        [ClaimRequirement("", "category_restaurant_branch_delete")]
        [HttpDelete("{id}")]
        public Task<bool> Delete(int id)
        {
            var result = _restaurantBranchBusiness.Delete(id);

            return result;
        }
    }
}
