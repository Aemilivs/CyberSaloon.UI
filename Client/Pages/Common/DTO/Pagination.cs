using System.Collections.Generic;

namespace Client.Pages.Common.DTO
{
    public class Pagination<T>
    {
        public IEnumerable<T> Entities { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}