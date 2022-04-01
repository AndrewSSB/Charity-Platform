using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Entities
{
    public class Cases : BaseEntity.BaseEntity
    {
        public string caseName { get; set; }
        public string caseDetails { get; set; }
        public DateTime? startDate { get; set; } 
        public DateTime? endDate { get; set; }
        public bool closed { get; set; }
        public virtual ICollection<Organisation> Organisations { get; set; }
    }
}
