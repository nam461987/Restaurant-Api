using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces.Orders;

namespace Restaurant.Repository
{
    public class PlacedOrderDetailRepository : BaseRepository<PlacedOrderDetail>, IPlacedOrderDetailRepository
    {
        public PlacedOrderDetailRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
