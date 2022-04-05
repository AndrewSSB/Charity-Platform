using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.EmailServices
{
    public interface IEmailServices
    {
        Task SendEmailLogin(string model, string subject, string content);
        Task SendEmailRegister(string model, string name);
    }
}
