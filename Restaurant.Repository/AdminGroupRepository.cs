using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Repository
{
    public class AdminGroupRepository : BaseRepository<AdminGroup>, IAdminGroupRepository
    {
        public AdminGroupRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
