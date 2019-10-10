using Restaurant.Common.Dtos.AdminAccount;
using Restaurant.Common.Responses.AdminAccount;
using Restaurant.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.Business.Interfaces
{
    public interface IAdminBusiness
    {
        AdminAccount Add(AdminAccount model);
        bool Update(AdminAccount model);
        bool Delete(int id);
        IEnumerable<AdminAccount> GetAll();
        AdminAccount GetById(int id);

        Task<LoginResponse> Login(LoginDto model);
        AuthenticationDto CheckAuthentication(string accessToken);
    }
}
