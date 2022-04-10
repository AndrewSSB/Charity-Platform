using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Wrappers
{
    public class PaginationFilter
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? searchName { get; set; }
        public string? orderBy { get; set; }
        public bool descending { get; set; }

        public PaginationFilter()
        {
            this.PageNumber = 1;
            this.PageSize = 50;
        }

        public PaginationFilter(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber == 0 ? 1 : pageNumber;
            this.PageSize = pageSize > 50 ? 50 : pageSize;
        }

        public PaginationFilter(int pageNumber, int pageSize, string? searchName, string? orderBy, bool descending)
        {
            this.PageNumber = pageNumber == 0 ? 1 : pageNumber;
            this.PageSize = pageSize > 50 ? 50 : pageSize;
            this.searchName = searchName;
            this.orderBy = orderBy;
            this.descending = descending;
        }
    }
}
