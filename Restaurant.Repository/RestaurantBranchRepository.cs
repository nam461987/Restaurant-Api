using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Repository
{
    public class RestaurantBranchRepository : BaseRepository<RestaurantBranch>, IRestaurantBranchRepository
    {
        public RestaurantBranchRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
