using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProiectSoft.BLL.Interfaces;
using ProiectSoft.BLL.Models;
using ProiectSoft.BLL.Models.Login_Model;
using ProiectSoft.BLL.Models.RegisterModel;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.Services.EmailServices;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.BLL.Managers
{
    public class AuthManager : IAuthManager
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenHelper _tokenHelper;
        private readonly AppDbContext _appDbContext;
        private readonly IEmailServices _emailService;
        private readonly ILogger<AuthManager> _logger;
        public AuthManager(UserManager<User> userManager,
               SignInManager<User> signInManager,
               ITokenHelper tokenHelper,
               AppDbContext appDbContext,
               IEmailServices emailServices,
               ILogger<AuthManager> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenHelper = tokenHelper;
            _appDbContext = appDbContext;
            _emailService = emailServices;
            _logger = logger;
        }

        public async Task<ResponseLogin> Login(LoginModel loginModel)
        {
            LogContext.PushProperty("IdentificationMessage", "Found this problem in the AuthManager:Login");

            var user = await _userManager.FindByNameAsync(loginModel.UserName);

            var role = await _userManager.GetRolesAsync(user);

            if (user == null)
            {
                return new ResponseLogin
                {
                    Success = false
                };

                _logger.LogError($"OPS! {JsonConvert.SerializeObject(loginModel.UserName)} does not exist in our database!");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginModel.Password, false);

            if (result.Succeeded)
            {
                var token = await _tokenHelper.CreateAccesToken(user);
                var refreshToken = _tokenHelper.CreateRefreshToken();

                //user.RefreshToken = refreshToken;
                //await _userManager.UpdateAsync(user);

                var refreshTokenResult = await SetAuthenticationToken(user, "", "refreshToken", refreshToken);

                if (!refreshTokenResult) { return new ResponseLogin { Success = false }; }

                await _emailService.SendEmailLogin(user.Email, "New login!", "<h1>Hey! \nNew login to your account noticed</h1><p>New login to your account at " + DateTime.Now + "</p>");
                //await _emailService.SendEmailRegister(user.Email, loginModel.UserName); //e pentru test mai rapid
                return new ResponseLogin
                {
                    Success = true,
                    AccesToken = token,
                    RefreshToken = refreshToken,
                    Role = role.FirstOrDefault()
                };
            }
            else
            {
                return new ResponseLogin
                {
                    Success = false
                };
                
                _logger.LogError($"OPS! We couldn't log you in, {loginModel.UserName}");
            }
        }

        public async Task<bool> Register(RegisterModel registerModel)
        {
            LogContext.PushProperty("IdentificationMessage", "Found this exception in the AuthManager:Register");

            var user = new User
            {
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                Email = registerModel.email,
                UserName = registerModel.UserName,
                Type = registerModel.Type,
                DateCreated = DateTime.Now,
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, registerModel.Role);

                await _emailService.SendEmailRegister(user.Email, user.UserName); // dupa ce te inregistrezi primesti un email de welcome (am luat un template gratis de pe net sa para mai dragut)

                return true;
            }

            _logger.LogError($"OPS! It seems you have filled in the wrong details, {registerModel.UserName}");

            return false;
        }

        private async Task<bool> SetAuthenticationToken(User user, string loginProvider, string name, string value)
        {
            if (user == null || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value)) { return false; }

            //var existingToken = await _appDbContext.UserTokens.FirstOrDefaultAsync();

            var existingToken = await _appDbContext.UserTokens.FirstOrDefaultAsync(x => x.Name == name &&
                                                                                   x.LoginProvider == loginProvider &&
                                                                                   x.UserId == user.Id);

            if (existingToken == null)
            {
                var newToken = new IdentityUserToken<Guid>()
                {
                    UserId = user.Id,
                    LoginProvider = loginProvider,
                    Name = name,
                    Value = value
                };

                await _appDbContext.UserTokens.AddAsync(newToken);
            }
            else
            {
                existingToken.Value = value;
            }

            await _appDbContext.SaveChangesAsync();

            return true;
        }
    }
}
