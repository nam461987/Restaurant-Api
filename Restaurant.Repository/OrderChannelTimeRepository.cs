using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces.Orders;

namespace Restaurant.Repository
{
    public class OrderChannelTimeRepository : BaseRepository<OrderChannelTime>, IOrderChannelTimeRepository
    {
        public OrderChannelTimeRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
