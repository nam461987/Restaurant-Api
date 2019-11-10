using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces.Orders;

namespace Restaurant.Repository
{
    public class OrderProcessRepository : BaseRepository<OrderProcess>, IOrderProcessRepository
    {
        public OrderProcessRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
