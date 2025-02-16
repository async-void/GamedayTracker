using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Models
{
    public class PlayerPicks
    {
        public int Id { get; set; }
        public int Season { get; set; }
        public int Week { get; set; }
        public virtual ICollection<string>? Picks { get; set; }
    }
}
