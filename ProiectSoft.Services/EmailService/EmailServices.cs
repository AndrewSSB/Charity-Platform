using Microsoft.Extensions.Configuration;
using ProiectSoft.Services.EmailServices;
using SendGrid;
using SendGrid.Helpers.Mail;
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

        public EmailServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailLogin(string toEmail, string subject, string content)
        {
            var apiKey = _configuration["SendApiKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("marius.ene@s.unibuc.ro", "Big heart backpacks");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
            var response = await client.SendEmailAsync(msg);
        }

        public async Task SendEmailRegister(string toEmail, string name)
        {
            var apiKey = _configuration["SendApiKey"];
            var sendGridClient = new SendGridClient(apiKey);
            var sendGridMessage = new SendGridMessage();
            sendGridMessage.SetFrom("marius.ene@s.unibuc.ro", "Big heart backpacks");
            sendGridMessage.AddTo(toEmail);
            var templateKey = _configuration["TemplateKey"];
            sendGridMessage.SetSubject($"Welcome, {name}");
            sendGridMessage.SetTemplateId(templateKey);
            sendGridMessage.SetTemplateData(new
            {
                name = "Big heart backpacks",
                url = "https://mc.sendgrid.com/dynamic-templates/d-d8a411bbe13d4940b887959baa2f8ab2/version/03a96972-ffb4-4052-aeef-fbee5b8828fe"
            });

            var response = await sendGridClient.SendEmailAsync(sendGridMessage);
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                Console.WriteLine("Email sent");
            }
        }
    }
}
