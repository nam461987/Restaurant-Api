using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;

namespace Restaurant.Repository
{
    public class RegistrationRepository : BaseRepository<Registration>, IRegistrationRepository
    {
        public RegistrationRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
