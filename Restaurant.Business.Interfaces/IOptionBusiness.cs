using Restaurant.Business.Interfaces.Paginated;
using System.Threading.Tasks;
using Restaurant.Entities.Models;
using Restaurant.Common.Filters;
using System.Collections.Generic;
using Restaurant.Common.Models;

namespace Restaurant.Business.Interfaces
{
    public interface IOptionBusiness
    {
        Task<List<OptionModel>> GetAdminGroupOptions();
        Task<List<OptionModel>> GetRestaurantOptions(int restaurantId);
        Task<List<OptionModel>> GetBranchOptions(int restaurantId, int branchId);
        Task<List<OptionModel>> GetCategoryOptions(int restaurantId, int branchId);
        Task<List<OptionModel>> GetUnitOptions(int restaurantId, int branchId);
        Task<List<OptionModel>> GetSizeOptions(int restaurantId, int branchId);
        Task<List<OptionModel>> GetStateOptions();
        Task<List<OptionModel>> GetCityOptions(int stateId);
        Task<List<OptionModel>> GetCustomerOptions(int restaurantId, int branchId);
        Task<List<OptionModel>> GetOrderChannelOptions(int restaurantId, int branchId);
        Task<List<OptionModel>> GetTableOptions(int restaurantId, int branchId);
        Task<List<OptionModel>> GetOrderProcessOptions();
        Task<List<OptionModel>> GetAccountOptions(int restaurantId, int branchId);
    }
}
