using ProiectSoft.BLL.Models;
using ProiectSoft.BLL.Models.Login_Model;
using ProiectSoft.BLL.Models.RegisterModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.BLL.Interfaces
{
    public interface IAuthManager
    {
        Task<bool> Register(RegisterModel registerModel);
        Task<Response> Login(LoginModel loginModel);
        Task<string> Refresh(RefreshModel refreshModel);
    }
}
