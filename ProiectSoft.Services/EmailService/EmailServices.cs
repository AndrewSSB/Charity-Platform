using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProiectSoft.Services.EmailServices;
using SendGrid;
using SendGrid.Helpers.Mail;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.EmailService
{
    public class EmailServices : IEmailServices
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailServices> _logger;
        public EmailServices(IConfiguration configuration, ILogger<EmailServices> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailLogin(string toEmail, string subject, string content)
        {
            LogContext.PushProperty("IdentificationMessage", $"Login mail failed for {toEmail}");

            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("eneandrei0228@gmail.com", "Big heart backpacks");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
            var response = await client.SendEmailAsync(msg);
        }

        public async Task SendEmailRegister(string toEmail, string name)
        {
            LogContext.PushProperty("IdentificationMessage", $"Register mail failed for {toEmail}");

            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var sendGridClient = new SendGridClient(apiKey);
            var sendGridMessage = new SendGridMessage();
            sendGridMessage.SetFrom("eneandrei0228@gmail.com", "Big heart backpacks");
            sendGridMessage.AddTo(toEmail);
            var templateKey = Environment.GetEnvironmentVariable("SENDGRID_TEMPLATE_ID");
            sendGridMessage.SetSubject($"Welcome, {name}");
            sendGridMessage.SetTemplateId(templateKey);
            sendGridMessage.SetTemplateData(new
            {
                name = "Big heart backpacks",
                url = $"https://mc.sendgrid.com/dynamic-templates/{templateKey}"
            });

            var response = await sendGridClient.SendEmailAsync(sendGridMessage);
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                Console.WriteLine("Email sent");
            }
        }
    }
}
