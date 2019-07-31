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
using Restaurant.Common.Dtos.Menu;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Restaurant.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [BearerAuthorize]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly AuthenticationDto _authenticationDto;
        private readonly IMenuBusiness _menuBusiness;

        public MenuController(IHttpContextAccessor httpContextAccessor,
            IMenuBusiness menuBusiness)
        {
            _authenticationDto = httpContextAccessor.HttpContext.User.ToAuthenticationDto();
            _menuBusiness = menuBusiness;
        }
        // GET: /Menu
        [ClaimRequirement("", "category_menu_list")]
        [HttpGet]
        public async Task<IPaginatedList<MenuDto>> Get(int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        {
            return await _menuBusiness.GetAll(_authenticationDto.RestaurantId, _authenticationDto.BranchId, pageIndex, pageSize);
        }
        // GET: /Menu/5
        [ClaimRequirement("", "category_menu_update")]
        [HttpGet("{id}")]
        public Task<MenuDto> Get(int id)
        {
            return _menuBusiness.GetById(_authenticationDto.RestaurantId, _authenticationDto.BranchId, id);
        }
        // POST: /Menu
        [ClaimRequirement("", "category_menu_create")]
        [HttpPost]
        public async Task<int> Post(Menu model)
        {

            var result = 0;
            if (ModelState.IsValid)
            {
                model.Status = 1;
                var modelInsert = await _menuBusiness.Add(model);
                result = modelInsert.Id;
            }
            return result;
        }
        // PUT: /Menu/5
        [ClaimRequirement("", "category_menu_update")]
        [HttpPut("{id}")]
        public async Task<bool> Put(Menu model)
        {
            var result = false;
            if (ModelState.IsValid)
            {
                result = await _menuBusiness.Update(model);
            }
            return result;
        }

        // PUT: /Menu/active
        [ClaimRequirement("", "category_menu_delete")]
        [HttpPut("active")]
        public async Task<bool> Put(int id, int Status)
        {
            return await _menuBusiness.SetActive(id, Status);
        }
        // GET: /Menu/getall
        [ClaimRequirement("", "quick_order_page")]
        [Route("getall")]
        [HttpGet]
        public async Task<List<Menu>> GetAllNotPaginate()
        {
            return await _menuBusiness.GetAllNotPaginate(_authenticationDto.RestaurantId, _authenticationDto.BranchId);
        }
    }
}
