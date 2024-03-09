using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(string toEmail, string FromEmail, string Password, string subject, string content);
    }
}
