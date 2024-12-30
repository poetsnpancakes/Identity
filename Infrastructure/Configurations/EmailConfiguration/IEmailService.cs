using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity_Infrastructure.Configurations.EmailConfiguration
{
    public interface IEmailService
    {
        Task SendEmailAsync(string subject, string body, string toEmail = null, string ccEmail = null, string bccEmail = null);
    }
}
