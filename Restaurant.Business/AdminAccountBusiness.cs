using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Restaurant.Business.Filter;
using Restaurant.Business.Interfaces;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Business.Paginated;
using Restaurant.Common.Constants;
using Restaurant.Common.Dtos.AdminAccount;
using Restaurant.Common.Enums;
using Restaurant.Common.Responses.AdminAccount;
using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Business
{
    public class AdminAccountBusiness : IAdminAccountBusiness
    {
        private readonly IMapper _mapper;
        private readonly IDataProtector _protector;
        private readonly IAdminAccountRepository _adminAccountRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IRestaurantBranchRepository _restaurantBranchRepository;
        private readonly IAdminGroupRepository _adminGroupRepository;
        private readonly IAdminPermissionRepository _adminPermissionRepository;
        private readonly IAdminGroupPermissionRepository _adminGroupPermissionRepository;
        private readonly IStateRepository _stateRepository;
        private readonly IStateCityRepository _stateCityRepository;

        public AdminAccountBusiness(IMapper mapper,
            IDataProtectionProvider provider,
            IAdminAccountRepository adminAccountRepository,
            IRestaurantRepository restaurantRepository,
            IRestaurantBranchRepository restaurantBranchRepository,
            IAdminGroupRepository adminGroupRepository,
            IAdminPermissionRepository adminPermissionRepository,
            IAdminGroupPermissionRepository adminGroupPermissionRepository,
            IStateRepository stateRepository,
            IStateCityRepository stateCityRepository)
        {
            _mapper = mapper;
            _protector = provider.CreateProtector("Test");
            _adminAccountRepository = adminAccountRepository;
            _restaurantRepository = restaurantRepository;
            _restaurantBranchRepository = restaurantBranchRepository;
            _adminGroupRepository = adminGroupRepository;
            _adminPermissionRepository = adminPermissionRepository;
            _adminGroupPermissionRepository = adminGroupPermissionRepository;
            _stateRepository = stateRepository;
            _stateCityRepository = stateCityRepository;
        }

        public async Task<AdminAccount> Add(AdminAccount model)
        {
            var entity = _adminAccountRepository.Add(model);
            await _adminAccountRepository.SaveChangeAsync();
            model.Id = entity.Id;

            return model;
        }

        public AuthenticationDto CheckAuthentication(string accessToken)
        {
            var descryptToken = string.Empty;
            try
            {
                descryptToken = _protector.Unprotect(accessToken);
            }
            catch (Exception ex)
            {
                return null;
            }
            var model = JsonConvert.DeserializeObject<AuthenticationDto>(descryptToken);
            if (model == null || model.UserId <= 0)
            {
                model = null;
            }
            return model;
        }

        public async Task<bool> Delete(int id)
        {
            _adminAccountRepository.Delete(id);
            var recordUpdated = await _adminAccountRepository.SaveChangeAsync();
            return recordUpdated > 0;
        }

        public async Task<bool> SetActive(int id, int Active)
        {
            var result = false;
            var record = await _adminAccountRepository.Repo.FirstOrDefaultAsync(c => c.Id == id);
            if (record != null)
            {
                record.Status = Active == 1 ? 0 : 1;
                await _adminAccountRepository.SaveChangeAsync();
                result = true;
            }
            return result;
        }

        public Task<IPaginatedList<AccountDto>> GetAll(int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        {
            var result = GetAll(0, 0, pageIndex, pageSize);
            return result;
        }
        public Task<IPaginatedList<AccountDto>> GetAllByRestaurant(int restaurantId, int branchId,
            int typeId, int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        {
            var result = GetAll(restaurantId, branchId, pageIndex, pageSize);

            return result;
        }

        public async Task<AccountDto> GetById(int restaurantId, int branchId, int id)
        {
            var result = await _adminAccountRepository.Repo.ToFilterByRole(f => f.RestaurantId, f => f.BranchId, restaurantId, branchId).Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            result.PasswordHash = string.Empty;

            return _mapper.Map<AccountDto>(result);
        }

        private Task<IPaginatedList<AccountDto>> GetAll(int restaurantId, int branchId, int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        {
            var adminAccountRepo = _adminAccountRepository.Repo;

            var result = (from adminAccount in adminAccountRepo
                          join restaurant in _restaurantRepository.Repo on adminAccount.RestaurantId equals restaurant.Id into rs
                          from restaurant in rs.DefaultIfEmpty()
                          join branch in _restaurantBranchRepository.Repo on adminAccount.BranchId equals branch.Id into bs
                          from branch in bs.DefaultIfEmpty()
                          join adminGroup in _adminGroupRepository.Repo on adminAccount.TypeId equals adminGroup.Id into gs
                          from adminGroup in gs.DefaultIfEmpty()
                          join state in _stateRepository.Repo on adminAccount.StateId equals state.Id into ss
                          from state in ss.DefaultIfEmpty()
                          join city in _stateCityRepository.Repo on adminAccount.CityId equals city.Id into cs
                          from city in cs.DefaultIfEmpty()
                          select new AccountDto
                          {
                              Id = adminAccount.Id,
                              RestaurantId = adminAccount.RestaurantId,
                              RestaurantIdName = restaurant.Name,
                              BranchId = adminAccount.BranchId,
                              BranchIdName = branch.Name,
                              TypeId = adminAccount.TypeId.GetValueOrDefault(),
                              TypeIdName = adminGroup.Name,
                              UserName = adminAccount.UserName,
                              Email = adminAccount.Email,
                              Mobile = adminAccount.Mobile,
                              FullName = adminAccount.FullName,
                              Gender = adminAccount.Gender,
                              BirthDate = adminAccount.BirthDate,
                              StateId = adminAccount.StateId,
                              StateIdName = state.Name,
                              CityId = adminAccount.CityId,
                              CityIdName = city.Name,
                              Zip = adminAccount.Zip,
                              Address = adminAccount.Address,
                              Avatar = adminAccount.Avatar,
                              Active = adminAccount.Active,
                              Status = adminAccount.Status,
                              CreatedDate = adminAccount.CreatedDate
                          })
                          .ToFilterByRole(f => f.RestaurantId, f => f.BranchId, restaurantId, branchId)
                          .Where(c => c.Status < (int)EStatus.All)
                          .OrderBy(c => c.Id)
                          .ToPaginatedListAsync(pageIndex, pageSize);
            return result;
        }

        public async Task<LoginResponse> Login(LoginDto model)
        {
            var result = new LoginResponse
            {
                Status = EResponseStatus.Fail
            };

            if (!string.IsNullOrWhiteSpace(model.UserName) && !string.IsNullOrWhiteSpace(model.Password))
            {
                var user = await _adminAccountRepository.Repo.FirstOrDefaultAsync(c => c.UserName.Equals(model.UserName));
                if (user != null && user.Active == (int)EActive.Active && user.Status == (int)EStatus.Using && user.PasswordHash.Equals(model.Password))
                {
                    var authentication = new AuthenticationDto
                    {
                        UserId = user.Id,
                        RestaurantId = user.RestaurantId,
                        BranchId = user.BranchId,
                        TypeId = user.TypeId.GetValueOrDefault(),
                        Username = user.UserName,
                        CreatedTime = DateTime.Now
                    };
                    var accessToken = _protector.Protect(JsonConvert.SerializeObject(authentication));
                    user.Description = accessToken;
                    await _adminAccountRepository.SaveChangeAsync();
                    result.Status = EResponseStatus.Success;
                    result.AccessToken = accessToken;
                    result.Displayname = user.FullName != null && user.FullName.Length > 0 ? user.FullName : "admin";
                    result.Email = user.Email != null && user.Email.Length > 0 ? user.Email : "";
                    result.Avatar = user.Avatar != null && user.Avatar.Length > 0 ? user.Avatar : "";
                    result.Permissions = await _adminGroupPermissionRepository.GetPermissionByGroup(user.RestaurantId, user.BranchId, user.TypeId.GetValueOrDefault());
                }
            }

            return result;
        }

        public async Task<bool> Update(AccountDto model)
        {
            var result = false;
            var record = await _adminAccountRepository.Repo.FirstOrDefaultAsync(c => c.Id == model.Id);

            if (record != null)
            {
                record.RestaurantId = model.RestaurantId;
                record.BranchId = model.BranchId;
                record.TypeId = model.TypeId;
                record.UserName = model.UserName;
                record.Email = model.Email;
                record.Mobile = model.Mobile;
                record.FullName = model.FullName;
                record.Gender = model.Gender;
                if (!string.IsNullOrEmpty(model.PasswordHash))
                {
                    record.PasswordHash = model.PasswordHash;
                }
                record.BirthDate = model.BirthDate;
                record.StateId = model.StateId;
                record.CityId = model.CityId;
                record.Zip = model.Zip;
                record.Address = model.Address;
                record.Avatar = model.Avatar;
                record.CreatedDate = model.CreatedDate;
                record.UpdatedDate = model.UpdatedDate;
                record.UpdatedStaffId = model.UpdatedStaffId;

                await _adminAccountRepository.SaveChangeAsync();

                result = true;
            }
            return result;
        }
        public async Task<bool> CheckUserNameExist(string username)
        {
            var result = true;
            var record = await _adminAccountRepository.Repo.FirstOrDefaultAsync(c => c.UserName == username);

            if (record != null)
            {
                result = false;
            }
            return result;
        }
        public async Task<bool> CheckEmailExist(string email)
        {
            var result = true;
            var record = await _adminAccountRepository.Repo.FirstOrDefaultAsync(c => c.Email == email);

            if (record != null)
            {
                result = false;
            }
            return result;
        }
        public async Task<int> ActiveAccount(string token)
        {
            var result = int.MinValue;
            var descryptToken = string.Empty;

            try
            {
                descryptToken = _protector.Unprotect(token);
            }
            catch (Exception ex)
            {
                return (int)EActiveAccountStatus.Fail;
            }
            var model = JsonConvert.DeserializeObject<AuthenticationDto>(descryptToken);
            if (model == null || model.UserId <= 0)
            {
                return (int)EActiveAccountStatus.Fail;
            }
            else
            {
                // Check token expired or not
                DateTime expiredTime = model.CreatedTime.AddDays(1);
                if (expiredTime < DateTime.Now)
                {
                    return (int)EActiveAccountStatus.Expired;
                }

                var account = await _adminAccountRepository.Repo.FirstOrDefaultAsync(c => c.Id == model.UserId && c.RestaurantId == model.RestaurantId &&
                c.BranchId == model.BranchId && c.UserName == model.Username && c.Active == (int)EActive.Deactive);
                if (account != null)
                {
                    account.Active = 1;
                    await _adminAccountRepository.SaveChangeAsync();
                    result = (int)EActiveAccountStatus.Success;
                    return result;
                }
                result = (int)EActiveAccountStatus.WasActivated;
            }
            return result;
        }
    }
}
