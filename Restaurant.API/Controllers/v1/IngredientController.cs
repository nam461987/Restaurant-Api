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
using Restaurant.Common.Dtos.Ingredient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Restaurant.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [BearerAuthorize]
    [ApiController]
    public class IngredientController : ControllerBase
    {
        private readonly AuthenticationDto _authenticationDto;
        private readonly IIngredientBusiness _ingredientBusiness;

        public IngredientController(IHttpContextAccessor httpContextAccessor,
            IIngredientBusiness ingredientBusiness)
        {
            _authenticationDto = httpContextAccessor.HttpContext.User.ToAuthenticationDto();
            _ingredientBusiness = ingredientBusiness;
        }
        // GET: /restaurant
        [ClaimRequirement("", "category_ingredient_list")]
        [HttpGet]
        public Task<IPaginatedList<IngredientDto>> Get(int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        {
            return _ingredientBusiness.GetAll(_authenticationDto.RestaurantId, _authenticationDto.BranchId, pageIndex, pageSize);
        }
        // GET: /restaurant/5
        [ClaimRequirement("", "category_ingredient_update")]
        [HttpGet("{id}")]
        public Task<IngredientDto> Get(int id)
        {
            return _ingredientBusiness.GetById(_authenticationDto.RestaurantId, _authenticationDto.BranchId, id);
        }
        // POST: /restaurant
        [ClaimRequirement("", "category_ingredient_create")]
        [HttpPost]
        public async Task<int> Post(Ingredient model)
        {

            var result = 0;
            if (ModelState.IsValid)
            {
                model.Status = 1;
                var modelInsert = await _ingredientBusiness.Add(model);
                result = modelInsert.Id;
            }
            return result;
        }
        // PUT: /restaurant/5
        [ClaimRequirement("", "category_ingredient_update")]
        [HttpPut("{id}")]
        public async Task<bool> Put(Ingredient model)
        {
            var result = false;
            if (ModelState.IsValid)
            {
                result = await _ingredientBusiness.Update(model);
            }
            return result;
        }

        // PUT: /restaurant/active
        [ClaimRequirement("", "category_ingredient_delete")]
        [HttpPut("active")]
        public Task<bool> Put(int id, int Status)
        {
            return _ingredientBusiness.SetActive(id, Status);
        }
    }
}
