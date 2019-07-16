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
using Restaurant.Common.Dtos.MenuSize;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Restaurant.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [BearerAuthorize]
    [ApiController]
    public class MenuSizeController : ControllerBase
    {
        private readonly AuthenticationDto _authenticationDto;
        private readonly IMenuSizeBusiness _menuSizeBusiness;

        public MenuSizeController(IHttpContextAccessor httpContextAccessor,
            IMenuSizeBusiness menuSizeBusiness)
        {
            _authenticationDto = httpContextAccessor.HttpContext.User.ToAuthenticationDto();
            _menuSizeBusiness = menuSizeBusiness;
        }
        // GET: /restaurant
        [ClaimRequirement("", "category_menu_size_list")]
        [HttpGet]
        public Task<IPaginatedList<MenuSizeDto>> Get(int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        {
            return _menuSizeBusiness.GetAll(_authenticationDto.RestaurantId, _authenticationDto.BranchId, pageIndex, pageSize);
        }
        // GET: /restaurant/5
        [ClaimRequirement("", "category_menu_size_update")]
        [HttpGet("{id}")]
        public Task<MenuSizeDto> Get(int id)
        {
            return _menuSizeBusiness.GetById(_authenticationDto.RestaurantId, _authenticationDto.BranchId, id);
        }
        // POST: /restaurant
        [ClaimRequirement("", "category_menu_size_create")]
        [HttpPost]
        public async Task<int> Post(MenuSize model)
        {

            var result = 0;
            if (ModelState.IsValid)
            {
                model.Status = 1;
                var modelInsert = await _menuSizeBusiness.Add(model);
                result = modelInsert.Id;
            }
            return result;
        }
        // PUT: /restaurant/5
        [ClaimRequirement("", "category_menu_size_update")]
        [HttpPut("{id}")]
        public async Task<bool> Put(MenuSize model)
        {
            var result = false;
            if (ModelState.IsValid)
            {
                result = await _menuSizeBusiness.Update(model);
            }
            return result;
        }

        // PUT: /restaurant/active
        [ClaimRequirement("", "category_menu_size_delete")]
        [HttpPut("active")]
        public Task<bool> Put(int id, int Status)
        {
            return _menuSizeBusiness.SetActive(id, Status);
        }
    }
}
