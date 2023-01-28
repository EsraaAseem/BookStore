using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
          /*  var emailSend = new MimeMessage();
            emailSend.From.Add(MailboxAddress.Parse("Hello@Esraaaseem252.com"));
            emailSend.To.Add(MailboxAddress.Parse(email));
            emailSend.Subject = subject;
            emailSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };
            using(var emailClient=new SmtpClient())
            {
                emailClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                emailClient.Authenticate("Esraaaseem252@gmail.com", "rvqizqplqbesuuhj");
                emailClient.Send(emailSend);
                emailClient.Disconnect(true);
            }*/
            return Task.CompletedTask;
        }
    }
}
