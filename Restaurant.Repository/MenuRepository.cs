using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using Restaurant.Repository.Interfaces.Menus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Repository
{
    public class MenuRepository : BaseRepository<Menu>, IMenuRepository
    {
        public MenuRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
