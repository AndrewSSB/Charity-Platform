using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Models.CasesModels
{
    public class CasesGetModel
    {
        public string Id { get; set; }
        public string? Created { get; set; }
        public string? Modified { get; set; }
        public string caseName { get; set; }
        public string caseDetails { get; set; }
        public string? startDate { get; set; }
        public string? endDate { get; set; }
        public bool closed { get; set; }
    }
}
