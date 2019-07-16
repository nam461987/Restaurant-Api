﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Business.Interfaces;
using Restaurant.Common.Enums;
using Restaurant.Common.Models;
using Restaurant.Repository.Interfaces;
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
        public OptionBusiness(IMapper mapper,
            IAdminGroupRepository adminGroupRepository,
            IRestaurantRepository restaurantRepository,
            IRestaurantBranchRepository restaurantBranchRepository,
            IMenuCategoryRepository menuCategoryRepository,
            IMenuUnitRepository menuUnitRepository,
            IMenuSizeRepository menuSizeRepository,
            IStateRepository stateRepository,
            IStateCityRepository stateCityRepository)
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

            if(state!= null)
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
    }
}