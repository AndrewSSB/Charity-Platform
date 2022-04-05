using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Models.CasesModels
{
    public class CasesPutModel
    {
        public string caseName { get; set; }
        public string caseDetails { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public bool closed { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
