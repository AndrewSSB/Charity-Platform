using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Models.UserModels
{
    public class UserGetModel
    {
        public string? Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Type { get; set; } //it can be a person or an organisation
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
