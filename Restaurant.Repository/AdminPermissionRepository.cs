using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Repository
{
    public class AdminPermissionRepository : BaseRepository<AdminPermission>, IAdminPermissionRepository
    {
        public AdminPermissionRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
