using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using GamedayTracker.Interfaces;
using GamedayTracker.Models.DailyNumbers;
using GamedayTracker.Utility;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace GamedayTracker.Handlers.Modal
{
    public class ModalInteractionHandler(IDailyNumbersCache cache, ILogger<ModalInteractionHandler> logger, IJsonDataService jsonDataService) : IEventHandler<ModalSubmittedEventArgs>
    {
        public async Task HandleEventAsync(DiscordClient sender, ModalSubmittedEventArgs eventArgs)
        {
            await eventArgs.Interaction.DeferAsync();
            var timestamp = DateTimeOffset.UtcNow.ToTimestamp();
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
                var addedUserPicksToJson = await jsonDataService.WriteDailyNumbersPicksToJsonAsync(userPick, eventArgs.Interaction.Guild.Id, today.ToString("yyyyMMdd", CultureInfo.InvariantCulture));
                logger.LogInformation("User {User} numbers have been added to the cache: Numbers: {Numbers} | Date: {date}", eventArgs.Interaction.User.Username, userNumbers, today.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture));

                DiscordComponent[] comps =
                [
                    new DiscordTextDisplayComponent($"Daily Numbers Submitted for {eventArgs.Interaction.User.Username}"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"Your numbers have been submitted: [{string.Join(", ", userNumbers)}]"),
                    new DiscordTextDisplayComponent($"Daily Numbers are drawn every day at midnight **GOOD LUCK**"),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordTextDisplayComponent($"Gameday Tracker ©️ {timestamp}")
                ];
                var container = new DiscordContainerComponent(comps, false, DiscordColor.SpringGreen);
                var msgBuilder = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);
               // await eventArgs.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder(new DiscordMessageBuilder(msgBuilder)));
                await eventArgs.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder(msgBuilder));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "User {User} submitted invalid numbers: {Numbers} | {date}", eventArgs.Interaction.User.Username, userNumbers, today.ToString("yyyyMMdd", CultureInfo.InvariantCulture));
                await eventArgs.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"Invalid numbers submitted | error: {ex.Message}"));
                return;
            }
        }
    }
}
