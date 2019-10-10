using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Restaurant.Common.Dtos.AdminAccount;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Restaurant.Common.Enums;
using System.Data.SqlClient;
using Restaurant.Entities.ModelExtensions;

namespace Restaurant.Repository
{
    public class AdminGroupPermissionRepository : BaseRepository<AdminGroupPermission>, IAdminGroupPermissionRepository
    {
        public AdminGroupPermissionRepository(restaurantContext context)
            : base(context)
        {
        }

        public async Task<List<AdminGroupPermission_View00>> GetPermissionByGroupAndModule(int restaurantId, int branchId, int curGroupId,
            int groupId, string module)
        {
            var dbResults = new List<AdminGroupPermission_View00>();
            var restaurantIdP = new SqlParameter("restaurantId", restaurantId);
            var branchIdP = new SqlParameter("branchId", branchId);
            var curGroupIdP = new SqlParameter("curGroupId", curGroupId);
            var groupIdP = new SqlParameter("groupId", groupId);
            var moduleP = new SqlParameter("module", module);

            dbResults = await _context.AdminGroupPermission_View00.FromSql(";EXEC GetPermissionByGroupAndModuleForCloudRestaurant @groupId, @module, @restaurantId, " +
                        "@branchId, @curGroupId", groupIdP, moduleP, restaurantIdP, branchIdP, curGroupIdP).ToListAsync();

            //if result == 0, it mean current user group was created by Restaurant Admin, and this group has branchId == 0
            // So we need to change branchId == 0
            if (dbResults.Count == 0)
            {
                branchIdP = new SqlParameter("branchId", 0);
                dbResults = await _context.AdminGroupPermission_View00.FromSql("EXEC GetPermissionByGroupAndModuleForCloudRestaurant @groupId, @module, @restaurantId, " +
            "@branchId, @curGroupId", groupIdP, moduleP, restaurantIdP, branchIdP, curGroupIdP).ToListAsync();
            }

            return dbResults;
        }
        public async Task<int> InsertOrUpdatePermission(int restaurantId, int branchId, int groupId, int permissionId, int status)
        {
            var dbResults = int.MinValue;
            // Set branchId = 0 because we just create permission by Restaurant
            branchId = 0;
            var curDate = DateTime.Now;

            try
            {
                dbResults = await _context.Database.ExecuteSqlCommandAsync("InsertOrUpdatePermission @p0, @p1, @p2, @p3, " +
                   "@p4, @p5", parameters: new object[] { permissionId, groupId, status, curDate, restaurantId, branchId });
            }
            catch (Exception ex)
            {
                dbResults = 0;
            }

            return dbResults;
        }
        public async Task<string[]> GetPermissionByGroup(int restaurantId, int branchId, int groupId)
        {
            var result = Task.Run(() => new string[0]);
            if (groupId == (int)EAccountType.Admin || groupId == (int)EAccountType.Mod || groupId == (int)EAccountType.RestaurantAdmin)
            {
                result = (from groupPermission in _context.AdminGroupPermission
                          where groupPermission.Status == (int)EStatus.Using &&
                          groupPermission.GroupId == groupId
                          join permission in _context.AdminPermission on groupPermission.PermissionId equals permission.Id
                          select new GroupPermissionDto
                          {
                              Id = groupPermission.Id,
                              RestaurantId = groupPermission.RestaurantId,
                              BranchId = groupPermission.BranchId,
                              GroupId = groupPermission.GroupId,
                              PermissionId = groupPermission.PermissionId,
                              PermissionIdName = permission.Name,
                              Status = groupPermission.Status.GetValueOrDefault()
                          })
                          .Select(c => c.PermissionIdName)
                          .ToArrayAsync();
            }
            else
            {

                result = (from groupPermission in _context.AdminGroupPermission
                          where groupPermission.Status == (int)EStatus.Using &&
                          groupPermission.RestaurantId == restaurantId && groupPermission.GroupId == groupId
                          join permission in _context.AdminPermission on groupPermission.PermissionId equals permission.Id
                          select new GroupPermissionDto
                          {
                              Id = groupPermission.Id,
                              RestaurantId = groupPermission.RestaurantId,
                              BranchId = groupPermission.BranchId,
                              GroupId = groupPermission.GroupId,
                              PermissionId = groupPermission.PermissionId,
                              PermissionIdName = permission.Name,
                              Status = groupPermission.Status.GetValueOrDefault()
                          })
                      .Select(c => c.PermissionIdName)
                      .ToArrayAsync();
            }

            await Task.WhenAll(result);
            return await result;
        }
    }
}
