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
using Restaurant.Common.Dtos.MenuCategory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Restaurant.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [BearerAuthorize]
    [ApiController]
    public class MenuCategoryController : ControllerBase
    {
        private readonly AuthenticationDto _authenticationDto;
        private readonly IMenuCategoryBusiness _menuCategoryBusiness;

        public MenuCategoryController(IHttpContextAccessor httpContextAccessor,
            IMenuCategoryBusiness menuCategoryBusiness)
        {
            _authenticationDto = httpContextAccessor.HttpContext.User.ToAuthenticationDto();
            _menuCategoryBusiness = menuCategoryBusiness;
        }
        // GET: /menucategory
        [ClaimRequirement("", "category_menu_category_list")]
        [HttpGet]
        public Task<IPaginatedList<MenuCategoryDto>> Get(int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        {
            return _menuCategoryBusiness.GetAll(_authenticationDto.RestaurantId, _authenticationDto.BranchId, pageIndex, pageSize);
        }
        // GET: /menucategory/5
        [ClaimRequirement("", "category_menu_category_update")]
        [HttpGet("{id}")]
        public Task<MenuCategoryDto> Get(int id)
        {
            return _menuCategoryBusiness.GetById(_authenticationDto.RestaurantId, _authenticationDto.BranchId, id);
        }
        // POST: /menucategory
        [ClaimRequirement("", "category_menu_category_create")]
        [HttpPost]
        public async Task<int> Post(MenuCategory model)
        {

            var result = 0;
            if (ModelState.IsValid)
            {
                model.Status = 1;
                var modelInsert = await _menuCategoryBusiness.Add(model);
                result = modelInsert.Id;
            }
            return result;
        }
        // PUT: /menucategory/5
        [ClaimRequirement("", "category_menu_category_update")]
        [HttpPut("{id}")]
        public async Task<bool> Put(MenuCategory model)
        {
            var result = false;
            if (ModelState.IsValid)
            {
                result = await _menuCategoryBusiness.Update(model);
            }
            return result;
        }

        // PUT: /menucategory/active
        [ClaimRequirement("", "category_menu_category_delete")]
        [HttpPut("active")]
        public Task<bool> Put(int id, int Status)
        {
            return _menuCategoryBusiness.SetActive(id, Status);
        }
    }
}
