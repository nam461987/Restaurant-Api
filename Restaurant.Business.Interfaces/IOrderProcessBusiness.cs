using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Entities.Models;
using System.Threading.Tasks;

namespace Restaurant.Business.Interfaces
{
    public interface IOrderProcessBusiness
    {
        Task<OrderProcess> Add(OrderProcess model);
        Task<bool> Update(OrderProcess model);
        Task<bool> SetActive(int id, int Active);
        Task<IPaginatedList<OrderProcess>> GetAll(int pageIndex, int pageSize);
        Task<OrderProcess> GetById(int id);
    }
}
