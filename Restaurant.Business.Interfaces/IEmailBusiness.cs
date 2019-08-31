using Restaurant.Common.Dtos.AdminAccount;
using Restaurant.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Business.Interfaces
{
    public interface IEmailBusiness
    {
        Task<bool> SendEmailToActiveAccount(AdminAccount model, string user, string password, string activeUrl);
        Task<bool> SendEmailToRestaurantAdmin(AdminAccount model, string user, string password, string activeUrl); 
    }
}
