﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Models.LocationModels
{
    public class LocationPutModel
    {
        public string County { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public int? Number { get; set; }
    }
}
