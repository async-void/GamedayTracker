using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamedayTracker.Models;

namespace GamedayTracker.Services
{
    public class NflEmojiService
    {
        public static Dictionary<string, string> NflEmojis { get; set; } = new()
        {
            { "ARI", "<:arizona:1331714128317775882>"},
            { "ATL", "<:falcons:1331727989209305172>"},
            { "BAL", "<:ravens:1331729349732204737>" },
            { "BUF", "<:bills:1331720491928522946>" },
            { "CAR", "<:panthers:1331720514858651668>" },
            { "CHI", "<:bears:1331720490074767495>" },
            { "CIN", "<:bengals:1331720491005775964>" },
            { "CLE", "<:browns:1331732518990385202>" },
            { "DAL", "<:cowboys:1331732938152349746>" },
            { "DEN", "<:denver:1331734095457419284>" },
            { "DET", "<:detroit:1331720498060460204>" },
            { "GB", "<:packers:1331735017524559912>" },
            { "HOU", "<:texans:1331735350770532362>" },
            { "IND", "<:colts:1331720495309262868>" },
            { "JAC", "<:jaguars:1331720507481002117>" },
            { "KC", "<:chiefs:1331720494206156911>" },
            { "LAC", "<:chargers:1331720493404917831>" },
            { "LAR", "<:rams:1331720521657880618>" },
            { "LV", "<:raiders:1331736359748243596>" },
            { "MIA", "<:dolphins:1331736916902805535>" },
            { "MIN", "<:vikings:1331720532202229880>" },
            { "NE", "<:patriots:1331720518331662386>" },
            { "NO", "<:saints:1331720525654790295>" },
            { "NYG", "<:giants:1331720504037347401>" },
            { "NYJ", "<:jets:1331737958314672221>" },
            { "PHI", "<:eagles:1331720501210648616>" },
            { "PIT", "<:steelers:1331720528934862971>" },
            { "SEA", "<:seattle:1331738427733053551>" },
            { "SF", "<:49ers:1331738874569035807>" },
            { "TB", "<:buccaneers:1331720492641423370>" },
            { "TEN", "<:titans:1331739858321805454>" },
            { "WAS", "<:washington:1331740108138745866>" },
            { "NFL", "<:nfl:1331742015809130629>" },
            { "AFC", "<:afc:1331745347285811300>" },
            { "NFC", "<:nfc:1331741091636056196>" },
            { "default", ""}
        };

        public static string GetEmoji(string abbr)
        {
            return NflEmojis.TryGetValue(abbr, out var path) ? path : NflEmojis["default"];
        }
    }
}
