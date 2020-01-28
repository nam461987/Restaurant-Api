using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Business.Filter;
using Restaurant.Business.Interfaces;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Business.Paginated;
using Restaurant.Common.Dtos.MenuDefinition;
using Restaurant.Common.Enums;
using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using Restaurant.Repository.Interfaces.Menus;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Business
{
    public class MenuDefinitionBusiness : IMenuDefinitionBusiness
    {
        private readonly IMapper _mapper;
        private readonly IMenuDefinitionRepository _menuDefinitionRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IMenuSizeRepository _menuSizeRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMenuUnitRepository _menuUnitRepository;
        public MenuDefinitionBusiness(IMapper mapper,
            IMenuDefinitionRepository menuDefinitionRepository,
            IRestaurantRepository restaurantRepository,
            IMenuRepository menuRepository,
            IMenuSizeRepository menuSizeRepository,
            IIngredientRepository ingredientRepository,
            IMenuUnitRepository menuUnitRepository)
        {
            _mapper = mapper;
            _menuDefinitionRepository = menuDefinitionRepository;
            _restaurantRepository = restaurantRepository;
            _menuRepository = menuRepository;
            _menuSizeRepository = menuSizeRepository;
            _ingredientRepository = ingredientRepository;
            _menuUnitRepository = menuUnitRepository;
        }
        public async Task<MenuDefinition> Add(MenuDefinition model)
        {
            var entity = _menuDefinitionRepository.Add(model);
            await _menuDefinitionRepository.SaveChangeAsync();

            return model;
        }
        public async Task<bool> Update(MenuDefinition model)
        {
            var result = false;
            var record = await _menuDefinitionRepository.Repo
                .FirstOrDefaultAsync(c => c.MenuId == model.MenuId
                && c.SizeId == model.SizeId
                && c.Id == model.Id);

            if (record != null)
            {
                record.Quantity = model.Quantity;
                record.Description = model.Description;

                await _menuDefinitionRepository.SaveChangeAsync();

                result = true;
            }
            return result;
        }
        public async Task<bool> SetActive(int id, int Active)
        {
            var result = false;
            var record = await _menuDefinitionRepository.Repo.FirstOrDefaultAsync(c => c.Id == id);
            if (record != null)
            {
                record.Status = Active == 1 ? 0 : 1;
                await _menuDefinitionRepository.SaveChangeAsync();
                result = true;
            }
            return result;
        }
        public Task<bool> Delete(int id)
        {
            throw new System.NotImplementedException();
        }
        public async Task<IPaginatedList<MenuDefinitionDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize,
            int menuId, int sizeId)
        {
            var MenuDefinitionRepo = _menuDefinitionRepository.Repo;

            var result = await (from menuDefinition in MenuDefinitionRepo
                                join restaurant in _restaurantRepository.Repo on menuDefinition.RestaurantId equals restaurant.Id into rs
                                from restaurant in rs.DefaultIfEmpty()
                                join menu in _menuRepository.Repo on menuDefinition.MenuId equals menu.Id into ms
                                from menu in ms.DefaultIfEmpty()
                                join size in _menuSizeRepository.Repo on menuDefinition.SizeId equals size.Id into ss
                                from size in ss.DefaultIfEmpty()
                                join ingredient in _ingredientRepository.Repo on menuDefinition.IngredientId equals ingredient.Id into iis
                                from ingredient in iis.DefaultIfEmpty()
                                join unit in _menuUnitRepository.Repo on ingredient.UnitId equals unit.Id into us
                                from unit in us.DefaultIfEmpty()
                                select new MenuDefinitionDto
                                {
                                    Id = menuDefinition.Id,
                                    RestaurantId = menuDefinition.RestaurantId,
                                    RestaurantIdName = restaurant.Name,
                                    BranchId = menuDefinition.BranchId,
                                    MenuId = menuDefinition.MenuId,
                                    MenuIdName = menu.Name,
                                    SizeId = menuDefinition.SizeId,
                                    SizeIdName = size.Name,
                                    IngredientId = menuDefinition.IngredientId,
                                    IngredientIdName = ingredient.Name,
                                    IngredientIdUnitIdName = unit.Name,
                                    Quantity = menuDefinition.Quantity,
                                    Description = menuDefinition.Description,
                                    Status = menuDefinition.Status
                                })
                          .ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0)
                          .Where(c => c.MenuId == menuId
                          && c.SizeId == sizeId
                          && c.Status < (int)EStatus.All)
                          .OrderBy(c => c.Id)
                          .ToPaginatedListAsync(pageIndex, pageSize);
            return result;
        }
        public async Task<MenuDefinitionDto> GetById(int restaurantId, int branchId, int id)
        {
            var result = await _menuDefinitionRepository.Repo.ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0)
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            return _mapper.Map<MenuDefinitionDto>(result);
        }
        public async Task<int> CheckExist(int restaurantId, int branchId, int menuId, int sizeId, int ingredientId)
        {
            var exist = 0;

            var result = await _menuDefinitionRepository.Repo.ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0)
                .Where(c => c.MenuId == menuId && c.SizeId == sizeId && c.IngredientId == ingredientId)
                .FirstOrDefaultAsync();

            if (result != null)
            {
                exist = result.Id;
            }

            return exist;
        }
    }
}
