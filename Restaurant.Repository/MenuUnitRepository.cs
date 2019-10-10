using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Repository
{
    public class MenuUnitRepository : BaseRepository<MenuUnit>, IMenuUnitRepository
    {
        public MenuUnitRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
