using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using Restaurant.Repository.Interfaces.Menus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Repository
{
    public class MenuPriceRepository : BaseRepository<MenuPrice>, IMenuPriceRepository
    {
        public MenuPriceRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
