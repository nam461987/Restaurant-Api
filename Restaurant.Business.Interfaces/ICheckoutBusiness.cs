using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Common.Dtos.Ingredient;
using Restaurant.Entities.Models;
using System.Threading.Tasks;

namespace Restaurant.Business.Interfaces
{
    public interface ICheckoutBusiness
    {
        string AcceptPayment();
    }
}
