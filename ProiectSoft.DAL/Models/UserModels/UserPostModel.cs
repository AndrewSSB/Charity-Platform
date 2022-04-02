using ProiectSoft.DAL.Entities.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Models.UserModels
{
    public class UserPostModel
    {
        public string? Id { get; set; }
        public string UserName { get; set; }
        public string email { get; set; }
        public string Password { get; set; }
        public string confirmPassword { get; set; }
        public string Type { get; set; } //it can be a person or an organisation
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
