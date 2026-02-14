using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Models.Betting
{
    public class BalanceCheckResult
    {
        public bool Allowed { get; set; }
        public string Reason { get; set; } = "";

    }
}
