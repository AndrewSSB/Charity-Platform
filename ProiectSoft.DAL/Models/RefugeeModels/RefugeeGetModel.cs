using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Models.RefugeeModels
{
    public class RefugeeGetModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string lastName { get; set; }
        public int? Age { get; set; }
        public string Details { get; set; }
        public int ShelterId { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
