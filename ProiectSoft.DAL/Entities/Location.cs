using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Entities
{
    public class Location : BaseEntity.BaseEntity
    {
        public string County { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public int? Number { get; set; }
        public virtual Shelter Shelter { get; set; } //prima medoda de 1:1
    }
}
