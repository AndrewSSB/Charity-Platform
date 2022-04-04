using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Type { get; set; } //it can be a person or an organisation
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public virtual ICollection<Donation> Donations { get; set; }    
        public virtual ICollection<UserRole> UserRoles { get; set; }
    } 
}
