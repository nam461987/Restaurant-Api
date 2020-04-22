using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Business.Interfaces;
using Restaurant.API.Extensions;
using Restaurant.Common.Models;
using System.Collections.Generic;
using Restaurant.Entities.Models;
using Microsoft.Extensions.Configuration;
using System;

namespace Restaurant.API.Controllers.v1
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IRegistrationBusiness _registrationBusiness;
        private readonly IOptionBusiness _optionBusiness;
        private readonly IConfiguration _appSetting;
        private readonly IEmailBusiness _emailBusiness;
        private readonly IAdminAccountBusiness _adminAccountBusiness;

        public RegisterController(IOptionBusiness optionBusiness,
            IRegistrationBusiness registrationBusiness,
            IConfiguration appSetting,
            IEmailBusiness emailBusiness,
            IAdminAccountBusiness adminAccountBusiness)
        {
            _optionBusiness = optionBusiness;
            _registrationBusiness = registrationBusiness;
            _appSetting = appSetting;
            _emailBusiness = emailBusiness;
            _adminAccountBusiness = adminAccountBusiness;
        }

        // POST: /Menu
        [HttpPost]
        public async Task<bool> Post(Registration model)
        {
            var result = false;

            var emailExist = await _adminAccountBusiness.CheckEmailExist(model.Email);

            if (!emailExist)
            {
                return result;
            }

            if (ModelState.IsValid)
            {
                model.Status = 1;
                model.CreatedDate = DateTime.Now;
                var modelInsert = await _registrationBusiness.Add(model);

                if (modelInsert)
                {
                    string user = _appSetting.GetValue<string>("AppSettings:EmailAuthentication:UserName");
                    string password = _appSetting.GetValue<string>("AppSettings:EmailAuthentication:Password");

                    await _emailBusiness.SendEmailToOwnerAfterRegister(model, user, password);
                }

                result = modelInsert;
            }
            return result;
        }

        [HttpGet("getstate")]
        public async Task<List<OptionModel>> GetStateOptions()
        {
            return await _optionBusiness.GetStateOptions();
        }
        [HttpGet("getcity")]
        public async Task<List<OptionModel>> GetCityOptions(int id)
        {
            return await _optionBusiness.GetCityOptions(id);
        }
        //[Route("checkemail/{email}")]
        [HttpGet("checkemail/{email}")]
        public async Task<bool> CheckEmailExist(string email)
        {
            return await _adminAccountBusiness.CheckEmailExist(email);
        }
    }
}