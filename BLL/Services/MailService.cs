using BLL.Interfaces;
using System.Net;
using System.Net.Mail;

namespace BLL.Services
{
    public class MailService : IMailService
    {

        public async Task<bool> SendEmailAsync(string toEmail, string FromEmail, string Password, string subject, string content)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient("smtp.office365.com");
                smtpClient.Port = 587;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;
                NetworkCredential networkCredential = new NetworkCredential(FromEmail, Password);
                smtpClient.Credentials = networkCredential;
                smtpClient.EnableSsl = true;
                MailMessage mailMessage = new MailMessage(FromEmail, toEmail);
                mailMessage.Subject = subject;
                mailMessage.Body = content;
                mailMessage.IsBodyHtml = true;
                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
