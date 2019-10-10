using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Business.Filter;
using Restaurant.Business.Interfaces;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Business.Paginated;
using Restaurant.Common.Dtos.Ingredient;
using Restaurant.Common.Enums;
using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using Restaurant.Repository.Interfaces.Menus;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Business
{
    public class IngredientBusiness : IIngredientBusiness
    {
        private readonly IMapper _mapper;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IMenuUnitRepository _menuUnitRepository;
        public IngredientBusiness(IMapper mapper,
            IIngredientRepository ingredientRepository,
            IRestaurantRepository restaurantRepository,
            IMenuUnitRepository menuUnitRepository)
        {
            _mapper = mapper;
            _ingredientRepository = ingredientRepository;
            _restaurantRepository = restaurantRepository;
            _menuUnitRepository = menuUnitRepository;
        }
        public async Task<Ingredient> Add(Ingredient model)
        {
            var entity = _ingredientRepository.Add(model);
            await _ingredientRepository.SaveChangeAsync();

            return model;
        }
        public async Task<bool> Update(Ingredient model)
        {
            var result = false;
            var record = await _ingredientRepository.Repo.FirstOrDefaultAsync(c => c.Id == model.Id);

            if (record != null)
            {
                record.Name = model.Name;
                record.UnitId = model.UnitId;
                record.Description = model.Description;

                await _ingredientRepository.SaveChangeAsync();

                result = true;
            }
            return result;
        }
        public async Task<bool> SetActive(int id, int Active)
        {
            var result = false;
            var record = await _ingredientRepository.Repo.FirstOrDefaultAsync(c => c.Id == id);
            if (record != null)
            {
                record.Status = Active == 1 ? 0 : 1;
                await _ingredientRepository.SaveChangeAsync();
                result = true;
            }
            return result;
        }
        public Task<bool> Delete(int id)
        {
            throw new System.NotImplementedException();
        }
        public async Task<IPaginatedList<IngredientDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize)
        {
            var IngredientRepo = _ingredientRepository.Repo;

            var result = await (from ingredient in IngredientRepo
                          join restaurant in _restaurantRepository.Repo on ingredient.RestaurantId equals restaurant.Id into rs
                          from restaurant in rs.DefaultIfEmpty()
                          join unit in _menuUnitRepository.Repo on ingredient.UnitId equals unit.Id into us
                          from unit in us.DefaultIfEmpty()
                          select new IngredientDto
                          {
                              Id = ingredient.Id,
                              RestaurantId = ingredient.RestaurantId,
                              RestaurantIdName = restaurant.Name,
                              BranchId = ingredient.BranchId,
                              Name = ingredient.Name,
                              UnitId = ingredient.UnitId,
                              UnitIdName = unit.Name,
                              Description = ingredient.Description,
                              Status = ingredient.Status,
                          })
                          .ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0)
                          .Where(c => c.Status < (int)EStatus.All)
                          .OrderBy(c => c.Id)
                          .ToPaginatedListAsync(pageIndex, pageSize);
            return result;
        }
        public async Task<IngredientDto> GetById(int restaurantId, int branchId, int id)
        {
            var result = await _ingredientRepository.Repo.ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0).Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            return _mapper.Map<IngredientDto>(result);
        }
    }
}
