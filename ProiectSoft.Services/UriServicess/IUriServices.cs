using ProiectSoft.DAL.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services.UriServicess
{
    public interface IUriServices
    {
        public Uri GetPageUri(PaginationFilter filter, string route);
    }
}
