using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces.Orders;

namespace Restaurant.Repository
{
    public class OrderChannelRepository : BaseRepository<OrderChannel>, IOrderChannelRepository
    {
        public OrderChannelRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
