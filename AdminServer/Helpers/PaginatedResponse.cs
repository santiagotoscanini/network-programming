using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminServer.Helpers
{
    public class PaginatedResponse<T> where T : class
    {
        public int TotalElements { get; set; }
        public int TotalPages { get; set; }
        public ICollection<T> Elements { get; set; } = new List<T>();
    }
}
