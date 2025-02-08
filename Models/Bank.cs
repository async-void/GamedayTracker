using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Models
{
    public class Bank
    {
        public int Id { get; set; }
        public double Balance { get; set; }
        public double DepositAmount { get; set; }
        public double LastDeposit { get; set; }
        public DateTime DepositTimestamp { get; set; }
    }
}
