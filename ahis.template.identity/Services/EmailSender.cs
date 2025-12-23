using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ahis.template.identity.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // TODO: replace with real email provider (SMTP, SendGrid, etc)
            Console.WriteLine($"Sending email to {email}");
            Console.WriteLine(subject);
            Console.WriteLine(htmlMessage);

            return Task.CompletedTask;
        }
    }
}
