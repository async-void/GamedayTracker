using DSharpPlus;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Jobs
{
    public class DailyNumbersJob(DiscordClient client) : IJob
    {
        private readonly DiscordClient _client = client;

        private readonly Random _random = new Random();

        public async Task Execute(IJobExecutionContext context)
        {
            var dailyNumbers = GenerateNumbers();
            var guilds = _client.Guilds.Values;
            foreach (var guild in guilds)
            {
                var defaultChnl =  guild?.GetDefaultChannel();
                if (defaultChnl is not null)
                {
                    await defaultChnl.SendMessageAsync(
                    $"Today's daily numbers are: {dailyNumbers[0]}, {dailyNumbers[1]}, {dailyNumbers[2]}");
                    await Task.Delay(300);
                }
                
            }
        }

        public int[] GenerateNumbers()
        {
            var numbers = new int[3];
            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[i] = _random.Next(0, 10);
            }
            return numbers;
        }
    }
}



