using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;

namespace Restaurant.Repository
{
    public class PlacedOrderProcessStatusRepository : BaseRepository<PlacedOrderProcessStatus>, IPlacedOrderProcessStatusRepository
    {
        public PlacedOrderProcessStatusRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
