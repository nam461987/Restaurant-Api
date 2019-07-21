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
using Restaurant.Common.Enums;

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
        private readonly IRestaurantBusiness _restaurantBusiness;

        public RestaurantBranchController(IHttpContextAccessor httpContextAccessor,
            IRestaurantBranchBusiness restaurantBranchBusiness,
            IRestaurantBusiness restaurantBusiness)
        {
            _authenticationDto = httpContextAccessor.HttpContext.User.ToAuthenticationDto();
            _restaurantBranchBusiness = restaurantBranchBusiness;
            _restaurantBusiness = restaurantBusiness;
        }
        // GET: /restaurant
        [ClaimRequirement("", "category_restaurant_branch_list")]
        [HttpGet]
        public async Task<IPaginatedList<BranchDto>> Get(int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        {
            return await _restaurantBranchBusiness.GetAll(_authenticationDto.RestaurantId, pageIndex, pageSize);
        }
        // GET: /restaurant/5
        [ClaimRequirement("", "category_restaurant_branch_update")]
        [HttpGet("{id}")]
        public async Task<BranchDto> Get(int id)
        {
            return await _restaurantBranchBusiness.GetById(_authenticationDto.RestaurantId, id);
        }
        // POST: /restaurant
        [ClaimRequirement("", "category_restaurant_branch_create")]
        [HttpPost]
        public async Task<int> Post(RestaurantBranch model)
        {
            var result = 0;
            if (ModelState.IsValid)
            {
                //check Restaurant Type
                var restaurant = await _restaurantBusiness.GetById(_authenticationDto.RestaurantId);
                if (restaurant != null && restaurant.TypeId == (int)ERestaurantType.Online)
                {
                    //if Online Restaurant had 1 Branch before, can not add more
                    // check count of Branch
                    var branch = await _restaurantBranchBusiness.GetAll(_authenticationDto.RestaurantId, 0, 10);
                    if (branch.TotalItems > 0)
                    {
                        return result;
                    }
                }

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
        public async Task<bool> Put(int id, int Status)
        {
            return await _restaurantBranchBusiness.SetActive(id, Status);
        }

        // DELETE: /restaurant/5
        [ClaimRequirement("", "category_restaurant_branch_delete")]
        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id)
        {
            var result = await _restaurantBranchBusiness.Delete(id);

            return result;
        }
    }
}
