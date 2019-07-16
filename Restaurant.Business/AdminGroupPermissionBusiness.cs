using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Restaurant.Business.Interfaces;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Business.Interfaces.Responses;
using Restaurant.Business.Paginated;
using Restaurant.Business.Responses;
using Restaurant.Common.Constants;
using Restaurant.Common.Dtos.AdminAccount;
using Restaurant.Common.Enums;
using Restaurant.Common.Models;
using Restaurant.Common.Responses.AdminAccount;
using Restaurant.Entities.ModelExtensions;
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
    public class AdminGroupPermissionBusiness : IAdminGroupPermissionBusiness
    {
        private readonly IAdminGroupPermissionRepository _adminGroupPermissionRepository;
        private readonly IAdminGroupRepository _adminGroupRepository;
        private readonly IAdminPermissionRepository _adminPermissionRepository;

        public AdminGroupPermissionBusiness(IMapper mapper,
            IAdminGroupPermissionRepository adminGroupPermissionRepository,
            IAdminGroupRepository adminGroupRepository,
            IAdminPermissionRepository adminPermissionRepository)
        {
            _adminGroupPermissionRepository = adminGroupPermissionRepository;
            _adminGroupRepository = adminGroupRepository;
            _adminPermissionRepository = adminPermissionRepository;
        }

        public async Task<List<AdminGroupPermission_View00>> GetPermissionByGroupAndModule(int restaurantId, int branchId, int curGroupId,
            int groupId, string module)
        {
            var dbResults = new List<AdminGroupPermission_View00>();

            try
            {
                dbResults = await _adminGroupPermissionRepository.GetPermissionByGroupAndModule(restaurantId, branchId, curGroupId,
             groupId, module);
            }
            catch (Exception ex)
            {
                return null;
            }

            return dbResults;
        }
        public async Task<int> InsertOrUpdatePermission(int restaurantId, int branchId, int groupId, int permissionId, int status)
        {
            var dbResults = int.MinValue;

            try
            {
                dbResults = await _adminGroupPermissionRepository.InsertOrUpdatePermission(restaurantId, branchId, groupId, permissionId, status);
            }
            catch
            {
                dbResults = 0;
            }

            return dbResults;
        }
        public async Task<string[]> GetPermissionByGroup(int restaurantId, int branchId, int groupId)
        {
            var result = await _adminGroupPermissionRepository.GetPermissionByGroup(restaurantId, branchId, groupId);

            return result;
        }
        public async Task<List<AdminGroup>> GetGroup(int restaurantId, int branchId, int groupId)
        {
            var result = Task.Run(() => new List<AdminGroup>());
            if (restaurantId == 0 && branchId == 0)
            {
                result = _adminGroupRepository.Repo.Where(c => c.Id > (int)EAccountType.Admin).ToListAsync();
            }
            else if (restaurantId > 0 && branchId == 0)
            {
                result = _adminGroupRepository.Repo.Where(c => c.Id > (int)EAccountType.Mod).ToListAsync();
            }
            else if(restaurantId > 0 && branchId > 0)
            {
                result = _adminGroupRepository.Repo.Where(c => c.Id > groupId).ToListAsync();
            }

            return await result;
        }
        public async Task<List<Option2Model>> GetModule()
        {
            var options = new List<Option2Model>();

            var result = await _adminPermissionRepository.Repo.Where(c => c.Status == (int)EStatus.Using).ToListAsync();

            if (result.Any())
            {
                options.AddRange(result.Select(c => c.Code).Distinct().Select(c => new Option2Model
                {
                    DisplayText = Convert.ToString(c),
                    Value = Convert.ToString(c)
                }).ToList());
            }

            return options;
        }
        public async Task<AdminGroupPermission> Add(AdminGroupPermission model)
        {
            var entity = _adminGroupPermissionRepository.Add(model);
            await _adminGroupPermissionRepository.SaveChangeAsync();
            model.Id = entity.Id;

            return model;
        }
    }
}
