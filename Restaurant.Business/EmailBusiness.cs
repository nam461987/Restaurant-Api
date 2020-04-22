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
        public async Task<bool> SendEmailToRestaurantAdmin(AdminAccount model, string user, string password, string activeUrl)
        {
            var result = false;
            var accountPassword = model.UserName + "123";

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
                //MailAddress replyto = new MailAddress("reply@example.com");
                //myMail.ReplyToList.Add(replyto);

                // set subject and encoding
                myMail.Subject = "Your account was created";
                myMail.SubjectEncoding = Encoding.UTF8;

                // set body-message and encoding
                myMail.Body = string.Format("<body style='margin: 0; '><div style='background:#f7f9fa; display:flex; align-items:center;'><table width='100%' align='center' border='0'><tbody><tr><td align='center'><table border='0' align='center' width='650' style='text-align:center;border-spacing:0px;font-family:Arial; background:#fff;'><tbody><tr><td><table border='0' align='center' width='100%' style='border-spacing:0px; table-layout:fixed; border-collapse:collapse;'><tbody><tr style='border-bottom:1px solid #e7e7e7;'><td align='left' style='background:#fff; padding:20px 0 20px 50px;width:50%'></td><td align='right' style='width:50%; padding-right:50px;'><h3>Account Activation</h3></td></tr><tr style='border-bottom:1px solid #e7e7e7;'><td style='padding:0;' colspan='2'><table border='0' align='center' width='100%' style='border-spacing:0px; padding: 0 50px;'><tbody><tr><td colspan='3' align='center'><p style='color:#2196f3;font-size:24px;margin:0; margin-bottom:10px; margin-top:30px;'>Your register was successful</p></td></tr><tr><td colspan='3' align='left' style='font-size:14px; line-height:20px;'><p>Dear {0},</p><p>Welcome to <span style='color:#2196f3;'>Cloud Restaurant System</span>!</p><p> Thanks for registering with us. To start using out application, you need to active your account first, just enter <a href='{1}{2}' style='color:#2196f3; text-decoration:none;'>Active</a>.</p><p>Use the following values when prompted to log in:</p><p><b>Username: {0}</b></p><p><b>Password: {3}</b></p></td></tr><tr><td colspan='3' align='left' style='font-size:14px;'><p>When you log into your account, you will be able to do the following:</p><ul style='line-height:24px;'><li>Create branch, staff account, menu, table,...</li><li>Set permission for each account group</li><li>Create, edit, delete the order</li><li>Change your password</li></ul></td></tr><tr><td colspan='3' align='left' style='font-size:14px;'><p>If you have any questions, please feel free to contact us at <a href='https://www.facebook.com/namto87' style='color:#025885'>Nam To</a> or by phone at <span style='color:#025885;'>+1 (657) 266 9014</span></p></td></tr><tr><td colspan='3' align='center' style='border-bottom:1px solid #e7e7e7;'> <a href='{1}{2}' style='background-image: linear-gradient(90deg, rgba(33,150,243,1) 50%, rgba(26,120,194,1) 100%); color:#fff; text-transform:uppercase;padding:10px 40px;text-decoration:none; border-radius:5px; margin-top:10px; margin-bottom:20px; display:inline-block;'> ACTIVE ACCOUNT </a></td></tr><tr><td colspan='3' align='center' style='font-size:14px;'><p>We hope to see you again soon.</p><p>Cloud Restaurant System</p></td></tr><tr><td colspan='3' align='left'><p style='color:#898989; font-size:12px; margin-bottom:25px;'>This email was sent from a notification-only address that can not accept incoming email. Please do not reply to this message.</p></td></tr></tbody></table></td></tr><tr><td colspan='3' align='center' style='font-size:12px;'><p> +1 (657) 266 9014 &emsp;|&emsp; <a href='https://www.facebook.com/namto87' style='color:#000000;text-decoration:none;'>Nam To</a> &emsp;|&emsp; <a href='#' style='color:#000000;text-decoration:none;'>FAQ</a></p></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></div></body>", model.UserName, activeUrl, activeToken, accountPassword);
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
                //MailAddress replyto = new MailAddress("reply@example.com");
                //myMail.ReplyToList.Add(replyto);

                // set subject and encoding
                myMail.Subject = "Active your account";
                myMail.SubjectEncoding = Encoding.UTF8;

                // set body-message and encoding
                myMail.Body = string.Format("<body style='margin: 0; '><div style='background:#f7f9fa; display:flex; align-items:center;'><table width='100%' align='center' border='0'><tbody><tr><td align='center'><table border='0' align='center' width='650' style='text-align:center;border-spacing:0px;font-family:Arial; background:#fff;'><tbody><tr><td><table border='0' align='center' width='100%' style='border-spacing:0px; table-layout:fixed; border-collapse:collapse;'><tbody><tr style='border-bottom:1px solid #e7e7e7;'><td align='left' style='background:#fff; padding:20px 0 20px 50px;width:50%'></td><td align='right' style='width:50%; padding-right:50px;'><h3>Account Activation</h3></td></tr><tr style='border-bottom:1px solid #e7e7e7;'><td style='padding:0;' colspan='2'><table border='0' align='center' width='100%' style='border-spacing:0px; padding: 0 50px;'><tbody><tr><td colspan='3' align='center'><p style='color:#2196f3;font-size:24px;margin:0; margin-bottom:10px; margin-top:30px;'>Your register was successful</p></td></tr><tr><td colspan='3' align='left' style='font-size:14px; line-height:20px;'><p>Dear {0},</p><p>Welcome to <span style='color:#2196f3;'>Cloud Restaurant System</span>!</p><p> Thanks for registering with us. To start using out application, you need to active your account first, just enter <a href='{1}{2}' style='color:#2196f3; text-decoration:none;'>Active</a>.</p><p>Use the following values when prompted to log in:</p><p><b>Username: {0}</b></p><p><b>Password: (Ask from the account creator)</b></p></td></tr><tr><td colspan='3' align='left' style='font-size:14px;'><p>When you log into your account, you will be able to do the following:</p><ul style='line-height:24px;'><li>Create branch, staff account, menu, table,...</li><li>Set permission for each account group</li><li>Create, edit, delete the order</li><li>Change your password</li></ul></td></tr><tr><td colspan='3' align='left' style='font-size:14px;'><p>If you have any questions, please feel free to contact us at <a href='https://www.facebook.com/namto87' style='color:#025885'>Nam To</a> or by phone at <span style='color:#025885;'>+1 (657) 266 9014</span></p></td></tr><tr><td colspan='3' align='center' style='border-bottom:1px solid #e7e7e7;'> <a href='{1}{2}' style='background-image: linear-gradient(90deg, rgba(33,150,243,1) 50%, rgba(26,120,194,1) 100%); color:#fff; text-transform:uppercase;padding:10px 40px;text-decoration:none; border-radius:5px; margin-top:10px; margin-bottom:20px; display:inline-block;'> ACTIVE ACCOUNT </a></td></tr><tr><td colspan='3' align='center' style='font-size:14px;'><p>We hope to see you again soon.</p><p>Cloud Restaurant System</p></td></tr><tr><td colspan='3' align='left'><p style='color:#898989; font-size:12px; margin-bottom:25px;'>This email was sent from a notification-only address that can not accept incoming email. Please do not reply to this message.</p></td></tr></tbody></table></td></tr><tr><td colspan='3' align='center' style='font-size:12px;'><p> +1 (657) 266 9014 &emsp;|&emsp; <a href='https://www.facebook.com/namto87' style='color:#000000;text-decoration:none;'>Nam To</a> &emsp;|&emsp; <a href='#' style='color:#000000;text-decoration:none;'>FAQ</a></p></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></div></body>", model.UserName, activeUrl, activeToken);
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

        public async Task<bool> SendEmailToOwnerAfterRegister(Registration model, string user, string password)
        {
            var result = false;

            try
            {
                // add from,to mailaddresses
                MailAddress from = new MailAddress(user, "Cloud Restaurant System");
                MailAddress to = new MailAddress(model.Email, model.Owner.Length > 0 ? model.Owner : model.Owner);
                MailMessage myMail = new MailMessage(from, to);

                // add ReplyTo
                //MailAddress replyto = new MailAddress("reply@example.com");
                //myMail.ReplyToList.Add(replyto);

                // set subject and encoding
                myMail.Subject = "Your registration has been received";
                myMail.SubjectEncoding = Encoding.UTF8;

                // set body-message and encoding
                myMail.Body = string.Format("<body style='margin: 0; '> <div style='background:#f7f9fa; display:flex; align-items:center;'> <table width='100%' align='center' border='0'> <tbody> <tr> <td align='center'> <table border='0' align='center' width='650' style='text-align:center;border-spacing:0px;font-family:Arial; background:#fff;'> <tbody> <tr> <td> <table border='0' align='center' width='100%' style='border-spacing:0px; table-layout:fixed; border-collapse:collapse;'> <tbody> <tr style='border-bottom:1px solid #e7e7e7;'> <td align='left' style='background:#fff; padding:20px 0 20px 50px;width:50%'> </td><td align='right' style='width:50%; padding-right:50px;'> <h3>Registed</h3> </td></tr><tr style='border-bottom:1px solid #e7e7e7;'> <td style='padding:0;' colspan='2'> <table border='0' align='center' width='100%' style='border-spacing:0px; padding: 0 50px;'> <tbody> <tr> <td colspan='3' align='center'> <p style='color:#2196f3;font-size:24px;margin:0; margin-bottom:10px; margin-top:30px;'>Your register was successful</p></td></tr><tr> <td colspan='3' align='left' style='font-size:14px; line-height:20px;'> <p>Dear {0},</p><p>Welcome to <span style='color:#2196f3;'>Cloud Restaurant System</span> !</p><p> Thanks for registering with us. To start using out application, you need to wait until our Admin approve your registration first. </p><p>You're going to receive an email include the account that you can use for your business.</p></td></tr><tr> <td colspan='3' align='left' style='font-size:14px; line-height:20px;'> <p>The infomation that you provided to us:</p><p><b>Owner: {0}</b></p><p><b>Business Name: {1}</b></p><p><b>Email: {2}</b></p><p><b>Address: {3}</b></p><p><b>Phone: {4}</b></p></td></tr><tr> <td colspan='3' align='left' style='font-size:14px;'> <p>If you have any questions, please feel free to contact us at <a href='https://www.facebook.com/namto87' style='color:#025885'>Nam To</a> or by phone at <span style='color:#025885;'>+1 (657) 266 9014</span></p></td></tr><tr> <td colspan='3' align='center' style='font-size:14px;'> <p>We hope to see you again soon.</p><p>Cloud Restaurant System</p></td></tr><tr> <td colspan='3' align='left'> <p style='color:#898989; font-size:12px; margin-bottom:25px;'>This email was sent from a notification-only address that can not accept incoming email. Please do not reply to this message.</p></td></tr></tbody> </table> </td></tr><tr> <td colspan='3' align='center' style='font-size:12px;'> <p> +1 (657) 266 9014 &emsp;|&emsp; <a href='https://www.facebook.com/namto87' style='color:#000000;text-decoration:none;'>Nam To</a> &emsp;|&emsp; <a href='#' style='color:#000000;text-decoration:none;'>FAQ</a> </p></td></tr></tbody> </table> </td></tr></tbody> </table> </td></tr></tbody> </table> </div></body>", model.Owner, model.Name, model.Email, model.Address, model.Phone);
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
