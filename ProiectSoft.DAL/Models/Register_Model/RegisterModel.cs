using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.BLL.Models.RegisterModel
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string? email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Password { get; set; }
        public string? Type { get; set; }
        public string Role { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
