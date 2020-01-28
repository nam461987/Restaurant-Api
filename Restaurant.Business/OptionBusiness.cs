using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Business.Filter;
using Restaurant.Business.Interfaces;
using Restaurant.Common.Enums;
using Restaurant.Common.Models;
using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using Restaurant.Repository.Interfaces.Menus;
using Restaurant.Repository.Interfaces.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Business
{
    public class OptionBusiness : IOptionBusiness
    {
        private readonly IMapper _mapper;
        private readonly IAdminGroupRepository _adminGroupRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IRestaurantBranchRepository _restaurantBranchRepository;
        private readonly IMenuCategoryRepository _menuCategoryRepository;
        private readonly IMenuUnitRepository _menuUnitRepository;
        private readonly IMenuSizeRepository _menuSizeRepository;
        private readonly IStateRepository _stateRepository;
        private readonly IStateCityRepository _stateCityRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderChannelRepository _orderChannelRepository;
        private readonly IRestaurantTableRepository _restaurantTableRepository;
        private readonly IAdminAccountRepository _adminAccountRepository;
        private readonly IOrderProcessRepository _orderProcessRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMenuDefinitionRepository _menuDefinitionRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly ITaxRepository _taxRepository;
        public OptionBusiness(IMapper mapper,
            IAdminGroupRepository adminGroupRepository,
            IRestaurantRepository restaurantRepository,
            IRestaurantBranchRepository restaurantBranchRepository,
            IMenuCategoryRepository menuCategoryRepository,
            IMenuUnitRepository menuUnitRepository,
            IMenuSizeRepository menuSizeRepository,
            IStateRepository stateRepository,
            IStateCityRepository stateCityRepository,
            ICustomerRepository customerRepository,
            IOrderChannelRepository orderChannelRepository,
            IRestaurantTableRepository restaurantTableRepository,
            IAdminAccountRepository adminAccountRepository,
            IOrderProcessRepository orderProcessRepository,
            IIngredientRepository ingredientRepository,
            IMenuDefinitionRepository menuDefinitionRepository,
            IMenuRepository menuRepository,
            ITaxRepository taxRepository)
        {
            _mapper = mapper;
            _adminGroupRepository = adminGroupRepository;
            _restaurantRepository = restaurantRepository;
            _restaurantBranchRepository = restaurantBranchRepository;
            _menuCategoryRepository = menuCategoryRepository;
            _menuUnitRepository = menuUnitRepository;
            _menuSizeRepository = menuSizeRepository;
            _stateRepository = stateRepository;
            _stateCityRepository = stateCityRepository;
            _customerRepository = customerRepository;
            _orderChannelRepository = orderChannelRepository;
            _restaurantTableRepository = restaurantTableRepository;
            _adminAccountRepository = adminAccountRepository;
            _orderProcessRepository = orderProcessRepository;
            _ingredientRepository = ingredientRepository;
            _menuDefinitionRepository = menuDefinitionRepository;
            _menuRepository = menuRepository;
            _taxRepository = taxRepository;
        }
        public async Task<List<OptionModel>> GetAdminGroupOptions()
        {
            var options = new List<OptionModel>();

            var item = await _adminGroupRepository.Repo.Where(c => c.Id > (int)EAccountType.Mod && c.Status == (int)EStatus.Using)
                .OrderBy(c => c.Id)
                .ToListAsync();

            if (item != null)
            {
                options.AddRange(item.Select(c => new OptionModel
                {
                    DisplayText = Convert.ToString(c.Name),
                    Value = Convert.ToInt32(c.Id)
                }).ToList());
            }


            return options;
        }
        public async Task<List<OptionModel>> GetRestaurantOptions(int restaurantId)
        {
            var options = new List<OptionModel>();

            var item = await _restaurantRepository.Repo.Where(c => c.Id == restaurantId && c.Status == (int)EStatus.Using)
                .OrderBy(c => c.Id)
                .ToListAsync();

            if (restaurantId == 0)
            {
                item = await _restaurantRepository.Repo.Where(c => c.Status == (int)EStatus.Using)
                .OrderBy(c => c.Id)
                .ToListAsync();
            }

            if (item != null)
            {
                options.AddRange(item.Select(c => new OptionModel
                {
                    DisplayText = Convert.ToString(c.Name),
                    Value = Convert.ToInt32(c.Id)
                }).ToList());
            }

            return options;
        }
        public async Task<List<OptionModel>> GetBranchOptions(int restaurantId, int branchId)
        {
            var options = new List<OptionModel>();

            var item = await _restaurantBranchRepository.Repo.Where(c => c.RestaurantId == restaurantId &&
                c.Id == branchId && c.Status == (int)EStatus.Using)
                .OrderBy(c => c.Id)
                .ToListAsync();

            if (restaurantId > 0 && branchId == 0)
            {
                item = await _restaurantBranchRepository.Repo.Where(c => c.RestaurantId == restaurantId && c.Status == (int)EStatus.Using)
                .OrderBy(c => c.Id)
                .ToListAsync();
            }

            if (item != null)
            {
                options.AddRange(item.Select(c => new OptionModel
                {
                    DisplayText = Convert.ToString(c.Name),
                    Value = Convert.ToInt32(c.Id)
                }).ToList());
            }

            return options;
        }
        public async Task<List<OptionModel>> GetCategoryOptions(int restaurantId, int branchId)
        {
            var options = new List<OptionModel>();

            var item = await _menuCategoryRepository.Repo.Where(c => c.RestaurantId == restaurantId &&
                c.Status == (int)EStatus.Using)
                .OrderBy(c => c.Id)
                .ToListAsync();

            if (item != null)
            {
                options.AddRange(item.Select(c => new OptionModel
                {
                    DisplayText = Convert.ToString(c.Name),
                    Value = Convert.ToInt32(c.Id)
                }).ToList());
            }

            return options;
        }
        public async Task<List<OptionModel>> GetUnitOptions(int restaurantId, int branchId)
        {
            var options = new List<OptionModel>();

            var item = await _menuUnitRepository.Repo.Where(c => c.RestaurantId == restaurantId &&
                c.Status == (int)EStatus.Using)
                .OrderBy(c => c.Id)
                .ToListAsync();

            if (item != null)
            {
                options.AddRange(item.Select(c => new OptionModel
                {
                    DisplayText = Convert.ToString(c.Name),
                    Value = Convert.ToInt32(c.Id)
                }).ToList());
            }

            return options;
        }
        public async Task<List<OptionModel>> GetMenuOptions(int restaurantId, int branchId)
        {
            var options = new List<OptionModel>();

            var item = await _menuRepository.Repo.Where(c => c.RestaurantId == restaurantId &&
                c.Status == (int)EStatus.Using)
                .OrderBy(c => c.Id)
                .ToListAsync();

            if (item != null)
            {
                options.AddRange(item.Select(c => new OptionModel
                {
                    DisplayText = Convert.ToString(c.Name),
                    Value = Convert.ToInt32(c.Id)
                }).ToList());
            }

            return options;
        }
        public async Task<List<OptionModel>> GetSizeOptions(int restaurantId, int branchId)
        {
            var options = new List<OptionModel>();

            var item = await _menuSizeRepository.Repo.Where(c => c.RestaurantId == restaurantId &&
                c.Status == (int)EStatus.Using)
                .OrderBy(c => c.Id)
                .ToListAsync();

            if (item != null)
            {
                options.AddRange(item.Select(c => new OptionModel
                {
                    DisplayText = Convert.ToString(c.Name),
                    Value = Convert.ToInt32(c.Id)
                }).ToList());
            }

            return options;
        }
        public async Task<List<OptionModel>> GetStateOptions()
        {
            var options = new List<OptionModel>();

            var item = await _stateRepository.Repo
                .OrderBy(c => c.Name)
                .ToListAsync();

            if (item != null)
            {
                options.AddRange(item.Select(c => new OptionModel
                {
                    DisplayText = Convert.ToString(c.Name),
                    Value = Convert.ToInt32(c.Id)
                }).ToList());
            }

            return options;
        }
        public async Task<List<OptionModel>> GetCityOptions(int stateId)
        {
            var options = new List<OptionModel>();

            var state = await _stateRepository.Repo.Where(c => c.Id == stateId).FirstOrDefaultAsync();

            if (state != null)
            {
                var item = await _stateCityRepository.Repo.Where(c => c.StateCode == state.Code)
                .OrderBy(c => c.Name)
                .ToListAsync();

                if (item != null)
                {
                    options.AddRange(item.Select(c => new OptionModel
                    {
                        DisplayText = Convert.ToString(c.Name),
                        Value = Convert.ToInt32(c.Id)
                    }).ToList());
                }
            }

            return options;
        }
        public async Task<List<OptionModel>> GetCustomerOptions(int restaurantId, int branchId)
        {
            var options = new List<OptionModel>();

            var item = await _customerRepository.Repo
                .ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0)
                .Where(c => c.Status == (int)EStatus.Using)
                .OrderBy(c => c.Name)
                .ToListAsync();

            if (item != null)
            {
                options.AddRange(item.Select(c => new OptionModel
                {
                    DisplayText = Convert.ToString(c.Name),
                    Value = Convert.ToInt32(c.Id)
                }).ToList());
            }

            return options;
        }
        public async Task<List<OptionModel>> GetOrderChannelOptions(int restaurantId, int branchId)
        {
            var options = new List<OptionModel>();

            var item = await _orderChannelRepository.Repo
                .ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0)
                .Where(c => c.Status == (int)EStatus.Using)
                .OrderBy(c => c.Id)
                .ToListAsync();

            if (item != null)
            {
                options.AddRange(item.Select(c => new OptionModel
                {
                    DisplayText = Convert.ToString(c.Name),
                    Value = Convert.ToInt32(c.Id)
                }).ToList());
            }

            return options;
        }
        public async Task<List<OptionModel>> GetTableOptions(int restaurantId, int branchId)
        {
            var options = new List<OptionModel>();

            var item = await _restaurantTableRepository.Repo
                .ToFilterByRole(f => f.RestaurantId, f => f.BranchId, restaurantId, branchId)
                .Where(c => c.Status == (int)EStatus.Using)
                .OrderBy(c => c.Name)
                .ToListAsync();

            if (item != null)
            {
                options.AddRange(item.Select(c => new OptionModel
                {
                    DisplayText = Convert.ToString(c.Name),
                    Value = Convert.ToInt32(c.Id)
                }).ToList());
            }

            return options;
        }
        public async Task<List<OptionModel>> GetOrderProcessOptions()
        {
            var options = new List<OptionModel>();

            var item = await _orderProcessRepository.Repo
                .Where(c => c.Status == (int)EStatus.Using)
                .OrderBy(c => c.Name)
                .ToListAsync();

            if (item != null)
            {
                options.AddRange(item.Select(c => new OptionModel
                {
                    DisplayText = Convert.ToString(c.Name),
                    Value = Convert.ToInt32(c.Id)
                }).ToList());
            }

            return options;
        }
        public async Task<List<OptionModel>> GetAccountOptions(int restaurantId, int branchId)
        {
            var options = new List<OptionModel>();

            var item = await _adminAccountRepository.Repo
                .ToFilterByRole(f => f.RestaurantId, f => f.BranchId, restaurantId, branchId)
                .Where(c => c.Status == (int)EStatus.Using)
                .OrderBy(c => c.UserName)
                .ToListAsync();

            if (item != null)
            {
                options.AddRange(item.Select(c => new OptionModel
                {
                    DisplayText = Convert.ToString(c.UserName),
                    Value = Convert.ToInt32(c.Id)
                }).ToList());
            }

            return options;
        }
        public async Task<List<OptionModel>> GetIngredientOptions(int restaurantId, int branchId, int id)
        {
            var options = new List<OptionModel>();

            var item = await _ingredientRepository.Repo
                .ToFilterByRole(f => f.RestaurantId, f => f.BranchId.GetValueOrDefault(), restaurantId, branchId)
                .Where(c => c.Status == (int)EStatus.Using)
                .OrderBy(c => c.Name)
                .ToListAsync();

            if (item != null)
            {
                options.AddRange(item.Select(c => new OptionModel
                {
                    DisplayText = Convert.ToString(c.Name),
                    Value = Convert.ToInt32(c.Id)
                }).ToList());
            }

            return options;
        }
        public async Task<List<OptionModel>> GetIngredientWithUnitOptions(int restaurantId, int branchId, int id)
        {
            var options = new List<OptionModel>();

            var IngredientRepo = _ingredientRepository.Repo;

            var item = await (from ingredient in IngredientRepo
                              join unit in _menuUnitRepository.Repo on ingredient.UnitId equals unit.Id into us
                              from unit in us.DefaultIfEmpty()
                              select new Ingredient
                              {
                                  Id = ingredient.Id,
                                  RestaurantId = ingredient.RestaurantId,
                                  BranchId = ingredient.BranchId,
                                  Name = ingredient.Name + " ( " + unit.Name + " )",
                                  UnitId = ingredient.UnitId,
                                  Description = ingredient.Description,
                                  Status = ingredient.Status
                              })
                            .ToFilterByRole(f => f.RestaurantId, f => f.BranchId.GetValueOrDefault(), restaurantId, branchId)
                            .Where(c => c.Status == (int)EStatus.Using)
                            .OrderBy(c => c.Name)
                            .ToListAsync();

            if (item != null)
            {
                options.AddRange(item.Select(c => new OptionModel
                {
                    DisplayText = Convert.ToString(c.Name),
                    Value = Convert.ToInt32(c.Id)
                }).ToList());
            }

            return options;
        }
        public async Task<List<OptionModel>> GetTaxOptions(int restaurantId, int branchId, int id)
        {
            var options = new List<OptionModel>();

            var item = await _taxRepository.Repo
                .ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0)
                .Where(c => c.Status == (int)EStatus.Using)
                .OrderBy(c => c.Name)
                .ToListAsync();

            if (item != null)
            {
                options.AddRange(item.Select(c => new OptionModel
                {
                    DisplayText = Convert.ToString(c.Name + "_" + c.Percentage),
                    Value = Convert.ToInt32(c.Id)
                }).ToList());
            }

            return options;
        }
    }
}
