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
using Restaurant.Common.Dtos.MenuUnit;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Restaurant.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [BearerAuthorize]
    [ApiController]
    public class MenuUnitController : ControllerBase
    {
        private readonly AuthenticationDto _authenticationDto;
        private readonly IMenuUnitBusiness _menuUnitBusiness;

        public MenuUnitController(IHttpContextAccessor httpContextAccessor,
            IMenuUnitBusiness menuUnitBusiness)
        {
            _authenticationDto = httpContextAccessor.HttpContext.User.ToAuthenticationDto();
            _menuUnitBusiness = menuUnitBusiness;
        }
        // GET: /MenuUnit
        [ClaimRequirement("", "category_menu_unit_list")]
        [HttpGet]
        public async Task<IPaginatedList<MenuUnitDto>> Get(int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        {
            return await _menuUnitBusiness.GetAll(_authenticationDto.RestaurantId, _authenticationDto.BranchId, pageIndex, pageSize);
        }
        // GET: /MenuUnit/5
        [ClaimRequirement("", "category_menu_unit_update")]
        [HttpGet("{id}")]
        public async Task<MenuUnitDto> Get(int id)
        {
            return await _menuUnitBusiness.GetById(_authenticationDto.RestaurantId, _authenticationDto.BranchId, id);
        }
        // POST: /MenuUnit
        [ClaimRequirement("", "category_menu_unit_create")]
        [HttpPost]
        public async Task<int> Post(MenuUnit model)
        {

            var result = 0;
            if (ModelState.IsValid)
            {
                model.Status = 1;
                var modelInsert = await _menuUnitBusiness.Add(model);
                result = modelInsert.Id;
            }
            return result;
        }
        // PUT: /MenuUnit/5
        [ClaimRequirement("", "category_menu_unit_update")]
        [HttpPut("{id}")]
        public async Task<bool> Put(MenuUnit model)
        {
            var result = false;
            if (ModelState.IsValid)
            {
                result = await _menuUnitBusiness.Update(model);
            }
            return result;
        }

        // PUT: /MenuUnit/active
        [ClaimRequirement("", "category_menu_unit_delete")]
        [HttpPut("active")]
        public async Task<bool> Put(int id, int Status)
        {
            return await _menuUnitBusiness.SetActive(id, Status);
        }
    }
}
