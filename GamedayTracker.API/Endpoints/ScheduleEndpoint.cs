using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.API.Endpoints
{
    public class ScheduleEndpoint
    {
        public async Task GetSchedule()
        {

            var options = new RestClientOptions("https://api.sportradar.com/nfl/official/trial/v7/en/games/2025/REG/schedule.json");
            var client = new RestClient(options);
            var request = new RestRequest("");
            request.AddHeader("accept", "application/json");
            request.AddHeader("x-api-key", "api key");
            var response = await client.GetAsync(request);

            Console.WriteLine("{0}", response.Content);

        }
    }
}
