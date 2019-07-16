using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
    public class AdminPermissionBusiness : IAdminPermissionBusiness
    {
        private readonly IAdminPermissionRepository _adminPermissionRepository;

        public AdminPermissionBusiness(IAdminPermissionRepository adminPermissionRepository)
        {
            _adminPermissionRepository = adminPermissionRepository;
        }

        public async Task<AdminPermission> Add(AdminPermission model)
        {
            var entity = _adminPermissionRepository.Add(model);
            await _adminPermissionRepository.SaveChangeAsync();
            model.Id = entity.Id;

            return model;
        }

        public async Task<bool> Delete(int id)
        {
            _adminPermissionRepository.Delete(id);
            var recordUpdated = await _adminPermissionRepository.SaveChangeAsync();
            return recordUpdated > 0;
        }

        public async Task<bool> SetActive(int id, int Active)
        {
            var result = false;
            var record = await _adminPermissionRepository.Repo.FirstOrDefaultAsync(c => c.Id == id);
            if (record != null)
            {
                record.Status = Active == 1 ? 0 : 1;
                await _adminPermissionRepository.SaveChangeAsync();
                result = true;
            }
            return result;
        }

        public Task<IPaginatedList<AdminPermission>> GetAll(int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        {
            var result = _adminPermissionRepository.Repo.Where(c => c.Status < (int)EStatus.All)
                .OrderByDescending(c => c.Id)
                .ToPaginatedListAsync(pageIndex, pageSize);
            return result;
        }

        public async Task<AdminPermission> GetById(int id)
        {
            var result = await _adminPermissionRepository.Repo.Where(c => c.Id == id)
                .FirstOrDefaultAsync();
            return result;
        }

        public async Task<bool> Update(AdminPermission model)
        {
            var result = false;
            var record = await _adminPermissionRepository.Repo.FirstOrDefaultAsync(c => c.Id == model.Id);

            if (record != null)
            {
                record.Name = model.Name;
                record.Code = model.Code;
                record.Description = model.Description;
                record.UpdatedDate = model.UpdatedDate;
                record.UpdatedStaffId = model.UpdatedStaffId;

                await _adminPermissionRepository.SaveChangeAsync();

                result = true;
            }
            return result;
        }
    }
}
