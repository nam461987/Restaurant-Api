using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Repository
{
    public class AdminAccountRepository: BaseRepository<AdminAccount>, IAdminAccountRepository
    {
        public AdminAccountRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
