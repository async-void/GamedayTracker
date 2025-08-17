using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Models
{
    public class Bank
    {
        public Guid Id { get; set; }
        public double Balance { get; set; }
        public double LastDepositAmount { get; set; }
        public DateTimeOffset DepositTimestamp { get; set; }
    }
}
