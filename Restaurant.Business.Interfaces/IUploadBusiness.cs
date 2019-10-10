using Microsoft.AspNetCore.Http;
using Restaurant.Common.Dtos.AdminAccount;
using Restaurant.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Business.Interfaces
{
    public interface IUploadBusiness
    {
        Task<string> MultipleUpload(IFormFile file, string forwardFolder, string uploadFolder, int restaurantId);
    }
}
