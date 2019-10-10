using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Repository
{
    public class MenuCategoryRepository : BaseRepository<MenuCategory>, IMenuCategoryRepository
    {
        public MenuCategoryRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
