using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Common.Dtos.AdminAccount;
using Restaurant.Common.Responses.AdminAccount;
using Restaurant.Entities.Models;
using System.Threading.Tasks;

namespace Restaurant.Business.Interfaces
{
    public interface IAdminAccountBusiness
    {
        Task<AdminAccount> Add(AdminAccount model);
        Task<bool> Update(AccountDto model);
        Task<bool> Delete(int id);
        Task<bool> SetActive(int id, int Active);
        Task<IPaginatedList<AccountDto>> GetAll(int pageIndex, int pageSize);
        Task<IPaginatedList<AccountDto>> GetAllByRestaurant(int restaurantId, int branchId, int typeId, int pageIndex, int pageSize);
        Task<AccountDto> GetById(int restaurantId, int branchId, int id);
        Task<bool> CheckUserNameExist(string username);
        Task<bool> CheckEmailExist(string email);

        Task<int> ActiveAccount(string token);
        Task<LoginResponse> Login(LoginDto model);
        Task<LoginResponse> LoginWithToken(string token);
        AuthenticationDto CheckAuthentication(string accessToken);
    }
}
