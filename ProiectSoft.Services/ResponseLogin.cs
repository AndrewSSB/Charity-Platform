﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.BLL
{
    public class ResponseLogin
    {
        public bool Success { get; set; }
        public string AccesToken { get; set; }
        public string RefreshToken { get; set; }
        public string Role { get; set; }
    }
}
