using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Repository
{
    public class StateCityRepository : BaseRepository<StateCity>, IStateCityRepository
    {
        public StateCityRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
