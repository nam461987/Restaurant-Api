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
using Restaurant.Common.Dtos.RestaurantTable;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Restaurant.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [BearerAuthorize]
    [ApiController]
    public class RestaurantTableController : ControllerBase
    {
        private readonly AuthenticationDto _authenticationDto;
        private readonly IRestaurantTableBusiness _restaurantTableBusiness;

        public RestaurantTableController(IHttpContextAccessor httpContextAccessor,
            IRestaurantTableBusiness restaurantTableBusiness)
        {
            _authenticationDto = httpContextAccessor.HttpContext.User.ToAuthenticationDto();
            _restaurantTableBusiness = restaurantTableBusiness;
        }
        // GET: /RestaurantTable
        [ClaimRequirement("", "category_restaurant_table_list")]
        [HttpGet]
        public Task<IPaginatedList<RestaurantTableDto>> Get(int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        {
            return _restaurantTableBusiness.GetAll(_authenticationDto.RestaurantId, _authenticationDto.BranchId, pageIndex, pageSize);
        }
        // GET: /RestaurantTable/5
        [ClaimRequirement("", "category_restaurant_table_update")]
        [HttpGet("{id}")]
        public Task<RestaurantTableDto> Get(int id)
        {
            return _restaurantTableBusiness.GetById(_authenticationDto.RestaurantId, _authenticationDto.BranchId, id);
        }
        // POST: /RestaurantTable
        [ClaimRequirement("", "category_restaurant_table_create")]
        [HttpPost]
        public async Task<int> Post(RestaurantTable model)
        {

            var result = 0;
            if (ModelState.IsValid)
            {
                model.Status = 1;
                var modelInsert = await _restaurantTableBusiness.Add(model);
                result = modelInsert.Id;
            }
            return result;
        }
        // PUT: /RestaurantTable/5
        [ClaimRequirement("", "category_restaurant_table_update")]
        [HttpPut("{id}")]
        public async Task<bool> Put(RestaurantTable model)
        {
            var result = false;
            if (ModelState.IsValid)
            {
                result = await _restaurantTableBusiness.Update(model);
            }
            return result;
        }

        // PUT: /RestaurantTable/active
        [ClaimRequirement("", "category_restaurant_table_delete")]
        [HttpPut("active")]
        public Task<bool> Put(int id, int Status)
        {
            return _restaurantTableBusiness.SetActive(id, Status);
        }
        // GET: /RestaurantTable/getall
        [ClaimRequirement("", "quick_order_page")]
        [Route("getall")]
        [HttpGet]
        public async Task<List<RestaurantTable>> GetAllNotPaginate()
        {
            return await _restaurantTableBusiness.GetAllNotPaginate(_authenticationDto.RestaurantId, _authenticationDto.BranchId);
        }
    }
}
