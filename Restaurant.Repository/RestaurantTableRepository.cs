using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Repository
{
    public class RestaurantTableRepository : BaseRepository<RestaurantTable>, IRestaurantTableRepository
    {
        public RestaurantTableRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
