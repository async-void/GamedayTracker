using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Handlers.Modal
{
    public class ModalInteractionHandler(IDailyNumbersCache cache, ILogger<ModalInteractionHandler> logger, IJsonDataService jsonDataService) : IEventHandler<ModalSubmittedEventArgs>
    {
        public async Task HandleEventAsync(DiscordClient sender, ModalSubmittedEventArgs eventArgs)
        {
            await eventArgs.Interaction.DeferAsync();
           
            var numOne = int.TryParse(((TextInputModalSubmission)eventArgs.Values["lotteryNum1"]).Value, out int nOne);
            var numTwo = int.TryParse(((TextInputModalSubmission)eventArgs.Values["lotteryNum2"]).Value, out int nTwo);
            var numThree = int.TryParse(((TextInputModalSubmission)eventArgs.Values["lotteryNum3"]).Value, out int nThree);

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var userNumbers = new List<int>();
            if (numOne && numTwo && numThree)
            {
                    userNumbers.Add(nOne);
                    userNumbers.Add(nTwo);
                    userNumbers.Add(nThree);
            }
                
            try
            {
                var userPick = new DailyNumberPick(
                    eventArgs.Interaction.Guild.Id,
                    eventArgs.Interaction.User.Id,
                    today,
                    userNumbers,
                    "Straight",
                    DateTimeOffset.UtcNow
                );

                await cache.AddUserPick(userPick);
                // var addedUserPicksToJson = await jsonDataService.WriteDailyNumbersPicksToJsonAsync(userPick, eventArgs.Interaction.Guild.Id, today.ToString("yyyyMMdd", CultureInfo.InvariantCulture));
                logger.LogInformation("User {UserId} numbers have been added to the cache: Numbers: {Numbers} | Date: {date}", eventArgs.Interaction.User.Id, userNumbers, today.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture));
                await eventArgs.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder(new DiscordMessageBuilder().WithContent("User lottery numbers submitted successfully!")));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "User {UserId} submitted invalid numbers: {Numbers} | {date}", eventArgs.Interaction.User.Id, userNumbers, today.ToString("yyyyMMdd", CultureInfo.InvariantCulture));
                await eventArgs.Interaction.Channel.SendMessageAsync($"Invalid numbers submitted: {ex.Message}");
                return;
            }

            //var keys = cache.GetActiveGuildIds(today);
            //var guildName = await sender.GetGuildAsync(keys[0]);
        }
    }
}
