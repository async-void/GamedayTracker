using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Records
{
    public record TeamStandingRecord(string TeamName, string Abbr, int Wins, int Loses, double Pct, double GB);
   
}
