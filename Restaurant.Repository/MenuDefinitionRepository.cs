using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Repository
{
    public class MenuDefinitionRepository : BaseRepository<MenuDefinition>, IMenuDefinitionRepository
    {
        public MenuDefinitionRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
