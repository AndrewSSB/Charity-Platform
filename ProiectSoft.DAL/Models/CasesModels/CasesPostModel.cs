using ProiectSoft.DAL.Entities.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Models.CasesModels
{
    public class CasesPostModel
    {
        public string caseName { get; set; }
        public string caseDetails { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public bool closed { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; } 
    }
}
