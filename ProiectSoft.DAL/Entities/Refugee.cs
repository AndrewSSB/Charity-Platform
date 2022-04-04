using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Entities
{
    public class Refugee : BaseEntity.BaseEntity
    {
        public string Name { get; set; }
        public string lastName { get; set; }
        public int? Age { get; set; }
        public string Details { get; set; }
        public int ShelterId { get; set; }
        public virtual Shelter Shelter { get; set; }
    }
}
