using Restaurant.Business.Interfaces.Paginated;
using System.Threading.Tasks;
using Restaurant.Entities.Models;
using Restaurant.Common.Filters;
using System.Collections.Generic;

namespace Restaurant.Business.Interfaces
{
    public interface ICustomerBusiness
    {
        //Task<Customer> Add(Customer model);
        //Task<bool> Update(Customer model);
        //Task<bool> SetActive(int id, int isActive);
        //Task<bool> Delete(int id);
        //Task<IPaginatedList<Customer>> GetAll(int pageIndex, int pageSize);
        //Task<IPaginatedList<Customer>> GetAllByRestaurant(int restaurantId, int branchId, CustomerFilter filter);
        //Task<Customer> GetById(int id);
        Task<List<Customer>> GetAllNotPaginate(int restaurantId, int branchId);
    }
}
