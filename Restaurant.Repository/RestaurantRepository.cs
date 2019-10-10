using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Repository
{
    public class RestaurantRepository : BaseRepository<Restaurant.Entities.Models.Restaurant>, IRestaurantRepository
    {
        public RestaurantRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
