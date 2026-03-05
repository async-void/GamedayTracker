using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Pagination
{
    public class PaginationEnvelope
    {
        public required object Data { get; init; }
        public required string Type { get; init; }

    }
}
