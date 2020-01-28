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
using Restaurant.Common.Dtos.MenuDefinition;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Restaurant.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [BearerAuthorize]
    [ApiController]
    public class MenuDefinitionController : ControllerBase
    {
        private readonly AuthenticationDto _authenticationDto;
        private readonly IMenuDefinitionBusiness _menuDefinitionBusiness;

        public MenuDefinitionController(IHttpContextAccessor httpContextAccessor,
            IMenuDefinitionBusiness menuDefinitionBusiness)
        {
            _authenticationDto = httpContextAccessor.HttpContext.User.ToAuthenticationDto();
            _menuDefinitionBusiness = menuDefinitionBusiness;
        }
        // GET: /menudefinition
        [ClaimRequirement("", "menu_definition_list")]
        [HttpGet]
        public async Task<IPaginatedList<MenuDefinitionDto>> Get(int menuId, int sizeId, int pageIndex = Constant.PAGE_INDEX_DEFAULT,
            int pageSize = Constant.PAGE_SIZE_DEFAULT)
        {
            return await _menuDefinitionBusiness.GetAll(_authenticationDto.RestaurantId, _authenticationDto.BranchId,
                pageIndex, pageSize, menuId, sizeId);
        }
        // GET: /menudefinition/5
        [ClaimRequirement("", "menu_definition_update")]
        [HttpGet("{id}")]
        public async Task<MenuDefinitionDto> Get(int id)
        {
            return await _menuDefinitionBusiness.GetById(_authenticationDto.RestaurantId, _authenticationDto.BranchId, id);
        }
        // POST: /menudefinition
        [ClaimRequirement("", "menu_definition_create")]
        [HttpPost]
        public async Task<int> Post(MenuDefinition model)
        {
            var result = 0;
            if (ModelState.IsValid)
            {
                // if Add New or Update an definition exist
                // set Status is Using (value = 1)
                model.Status = 1;

                var idExist = await _menuDefinitionBusiness.CheckExist(model.RestaurantId,
                    model.BranchId.GetValueOrDefault(), model.MenuId, model.SizeId, model.IngredientId);

                if (idExist > 0)
                {
                    model.Id = idExist;
                    if (await _menuDefinitionBusiness.Update(model))
                    {
                        result = 1;
                    }
                }
                else
                {
                    var modelInsert = await _menuDefinitionBusiness.Add(model);
                    result = modelInsert.Id;
                }
            }
            return result;
        }
        // PUT: /menudefinition/5
        [ClaimRequirement("", "menu_definition_update")]
        [HttpPut("{id}")]
        public async Task<bool> Put(MenuDefinition model)
        {
            var result = false;
            if (ModelState.IsValid)
            {
                result = await _menuDefinitionBusiness.Update(model);
            }
            return result;
        }

        // PUT: /menudefinition/active
        [ClaimRequirement("", "menu_definition_delete")]
        [HttpPut("active")]
        public async Task<bool> Put(int id, int Status)
        {
            return await _menuDefinitionBusiness.SetActive(id, Status);
        }
    }
}
