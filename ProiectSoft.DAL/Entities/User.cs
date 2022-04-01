using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Entities
{
    public class User : BaseEntity.BaseEntity
    {
        public string UserName { get; set; }
        public string email { get; set; }
        public string Password { get; set; }
        public string confirmPassword { get; set; }
        public string Type { get; set; } //it can be a person or an organisation
        public virtual ICollection<Donation> Donations { get; set; }    
    } 
}
