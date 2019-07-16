using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Restaurant.Business.Interfaces;
using Restaurant.Common.Dtos.AdminAccount;
using Restaurant.Entities.Models;
using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Business
{
    public class EmailBusiness : IEmailBusiness
    {
        private readonly IMapper _mapper;
        private readonly IDataProtector _protector;
        public EmailBusiness(IMapper mapper,
            IDataProtectionProvider provider,
            IConfiguration appSetting)
        {
            _mapper = mapper;
            _protector = provider.CreateProtector("Test");
        }
        public async Task<bool> SendEmailToActiveAccount(AdminAccount model, string user, string password, string activeUrl)
        {
            var result = false;

            var tokenToActive = new AuthenticationDto
            {
                UserId = model.Id,
                RestaurantId = model.RestaurantId,
                BranchId = model.BranchId,
                TypeId = model.TypeId.GetValueOrDefault(),
                Username = model.UserName,
                CreatedTime = DateTime.Now
            };
            var activeToken = _protector.Protect(JsonConvert.SerializeObject(tokenToActive));

            try
            {                
                // add from,to mailaddresses
                MailAddress from = new MailAddress(user, "Cloud Restaurant System");
                MailAddress to = new MailAddress(model.Email, model.FullName.Length > 0 ? model.FullName : model.UserName);
                MailMessage myMail = new MailMessage(from, to);

                // add ReplyTo
                MailAddress replyto = new MailAddress("reply@example.com");
                myMail.ReplyToList.Add(replyto);

                // set subject and encoding
                myMail.Subject = "Active your account";
                myMail.SubjectEncoding = Encoding.UTF8;

                // set body-message and encoding
                myMail.Body = string.Format("<body itemscope itemtype='http://schema.org/EmailMessage' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: none; width: 100% !important; height: 100%; line-height: 1.6em; background-color: #f6f6f6; margin: 0;' bgcolor='#f6f6f6'><table class='body-wrap' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; width: 100%; background-color: #f6f6f6; margin: 0;' bgcolor='#f6f6f6'><tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0;' valign='top'></td><td class='container' width='600' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; display: block !important; max-width: 600px !important; clear: both !important; margin: 0 auto;' valign='top'><div class='content' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; max-width: 600px; display: block; margin: 0 auto; padding: 20px;'><table class='main' width='100%' cellpadding='0' cellspacing='0' itemprop='action' itemscope itemtype='http://schema.org/ConfirmAction' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; border-radius: 3px; background-color: #fff; margin: 0; border: 1px solid #e9e9e9;' bgcolor='#fff'><tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-wrap' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 20px;' valign='top'><meta itemprop='name' content='Confirm Email' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'/><table width='100%' cellpadding='0' cellspacing='0' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>Please active your account by clicking the link below.</td></tr><tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>Then, you can use your account with the usename <b>{0}</b> to login to your account</td></tr><tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' itemprop='handler' itemscope itemtype='http://schema.org/HttpActionHandler' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'><a href='{1}{2}' class='btn-primary' itemprop='url' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; color: #FFF; text-decoration: none; line-height: 2em; font-weight: bold; text-align: center; cursor: pointer; display: inline-block; border-radius: 5px; text-transform: capitalize; background-color: #348eda; margin: 0; border-color: #348eda; border-style: solid; border-width: 10px 20px;'>Active Account</a></td></tr><tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>&mdash; Vangie House Application</td></tr></table></td></tr></table><div class='footer' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; width: 100%; clear: both; color: #999; margin: 0; padding: 20px;'><table width='100%' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='aligncenter content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 12px; vertical-align: top; color: #999; text-align: center; margin: 0; padding: 0 0 20px;' align='center' valign='top'>Follow <a href='https://facebook.com/namto87' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 12px; color: #999; text-decoration: underline; margin: 0;'>@NamTo</a> on Facebook.</td></tr></table></div></div></td><td style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0;' valign='top'></td></tr></table></body>", model.UserName, activeUrl, activeToken);
                myMail.BodyEncoding = Encoding.UTF8;
                // text or html
                myMail.IsBodyHtml = true;

                using (SmtpClient mySmtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    // set smtp-client with basicAuthentication
                    mySmtpClient.UseDefaultCredentials = false;
                    NetworkCredential basicAuthenticationInfo = new
                    NetworkCredential(user, password);
                    mySmtpClient.Credentials = basicAuthenticationInfo;
                    mySmtpClient.EnableSsl = true;
                    await mySmtpClient.SendMailAsync(myMail);
                };

                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
    }
}
