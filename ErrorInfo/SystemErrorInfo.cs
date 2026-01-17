using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.ErrorInfo
{
    public class SystemErrorInfo
    {
        public Guid ErrorId { get; set; } = Guid.NewGuid();
        public required string Info { get; set; }         
        public string? Severity { get; set; }            
        public string? Category { get; set; }            
        public override string ToString()
        {
            return $"{ErrorId} | {Severity} | {Category} | {Info}";
        }

    }
}
