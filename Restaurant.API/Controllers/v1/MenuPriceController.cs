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
using Restaurant.Common.Dtos.MenuPrice;
using Restaurant.Common.Enums;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Restaurant.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [BearerAuthorize]
    [ApiController]
    public class MenuPriceController : ControllerBase
    {
        private readonly AuthenticationDto _authenticationDto;
        private readonly IMenuPriceBusiness _menuPriceBusiness;

        public MenuPriceController(IHttpContextAccessor httpContextAccessor,
            IMenuPriceBusiness menuPriceBusiness)
        {
            _authenticationDto = httpContextAccessor.HttpContext.User.ToAuthenticationDto();
            _menuPriceBusiness = menuPriceBusiness;
        }
        // GET: /menuprice
        [ClaimRequirement("", "category_menu_price_list")]
        [HttpGet]
        public async Task<IPaginatedList<MenuPriceDto>> Get(int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        {
            return await _menuPriceBusiness.GetAll(_authenticationDto.RestaurantId, _authenticationDto.BranchId, pageIndex, pageSize);
        }
        // GET: /menuprice/5
        [ClaimRequirement("", "category_menu_price_update")]
        [HttpGet("{id}")]
        public Task<MenuPriceDto> Get(int id)
        {
            return _menuPriceBusiness.GetById(_authenticationDto.RestaurantId, _authenticationDto.BranchId, id);
        }
        // GET: /menuprice/5
        [ClaimRequirement("", "category_menu_price_update")]
        [Route("getpricebymenu")]
        [HttpGet]
        public async Task<IPaginatedList<MenuPriceDto>> GetPriceByMenu(int restaurantId, int branchId, int menuId, int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        {
            return await _menuPriceBusiness.GetByMenuId(restaurantId, branchId, pageIndex, pageSize, menuId);
        }
        // POST: /menuprice
        [ClaimRequirement("", "category_menu_price_create")]
        [HttpPost]
        public async Task<int> Post(MenuPrice model)
        {
            var result = 0;
            if (ModelState.IsValid)
            {
                model.Status = 1;
                //check if price for that size is available, even deleted. just update the exist record
                // and change status to using(in case it was delete)
                var checkExist = await _menuPriceBusiness.CheckExistPrice(model);
                if (checkExist != null)
                {
                    model.Id = checkExist.Id;
                    var modelupdate = await _menuPriceBusiness.Update(model);
                    if (checkExist.Status == (int)EStatus.Delete)
                    {
                        await _menuPriceBusiness.SetActive(checkExist.Id, (int)checkExist.Status);
                    }
                    result = checkExist.Id;
                }
                else
                {
                    var modelInsert = await _menuPriceBusiness.Add(model);
                    result = modelInsert.Id;
                }
            }
            return result;
        }
        // PUT: /menuprice/5
        [ClaimRequirement("", "category_menu_price_update")]
        [HttpPut("{id}")]
        public async Task<bool> Put(MenuPrice model)
        {
            var result = false;
            if (ModelState.IsValid)
            {
                result = await _menuPriceBusiness.Update(model);
            }
            return result;
        }

        // PUT: /menuprice/active
        [ClaimRequirement("", "category_menu_price_delete")]
        [HttpPut("active")]
        public Task<bool> Put(int id, int Status)
        {
            return _menuPriceBusiness.SetActive(id, Status);
        }
    }
}
