using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using GamedayTracker.Interfaces;
using GamedayTracker.Utility;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Text;

namespace GamedayTracker.SlashCommands.Lottery
{
    [Command("lottery")]
    [Description("lottery commands")]
    public class LotteryCommands(ILogger<LotteryCommands> logger, IJsonDataService jsonDataService, 
        ILotteryService lotteryService, IDailyNumbersCache cache, IGlobalWinningNumberService globalWinningNumberService)
    {
        #region ADD DAILY NUMBERS
        [Command("add-numbers")]
        [Description("add your daily numbers for the lottery drawing")]
        public async Task SaveDailyNumbers(SlashCommandContext ctx)
        {
            var timestamp = DateTimeOffset.UtcNow.ToTimestamp();
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (cache.HasUserSubmitted(ctx.Guild.Id, ctx.User.Id, today))
            {
                await ctx.DeferResponseAsync();
                DiscordComponent[] comps =
                [
                    new DiscordTextDisplayComponent("**Daily Numbers**"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"{ctx.User.Username} already submitted their numbers today.\r\nDaily Number's Drawing is at Midnight est every evening\r\n**Good luck**"),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordTextDisplayComponent($"Gameday Tracker ©️ {timestamp}")
                ];
                var container = new DiscordContainerComponent(comps, false, DiscordColor.Blurple);
                var msg = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);
                await ctx.RespondAsync(msg);
                return;
            }
            var mostRecentUserNumbers = await jsonDataService.GetUserLastLotterySubmissionAsync(ctx.Guild.Id, ctx.User.Id);
            var modal = new DiscordModalBuilder()
                .WithCustomId("lotteryNumbersModal")
                .WithTitle("Lottery Numbers Game")
                .AddTextInput(new DiscordTextInputComponent("lotteryNum1", "0-9", $"{mostRecentUserNumbers?.Numbers[0] ?? 0}", required: true, min_length: 1, max_length: 1), "num one")
                .AddTextInput(new DiscordTextInputComponent("lotteryNum2", "0-9", $"{mostRecentUserNumbers?.Numbers[1] ?? 0}", required: true, min_length: 1, max_length: 1), "num two")
                .AddTextInput(new DiscordTextInputComponent("lotteryNum3", "0-9", $"{mostRecentUserNumbers?.Numbers[2] ?? 0}", required: true, min_length: 1, max_length: 1), "num three");

            await ctx.RespondWithModalAsync(modal);
        }
        #endregion

        #region GET LOTTERY NUMBERS DRAW HISTORY
        [Command("history")]
        [Description("get the lottery drawing history")]
        public async Task GetLotteryHistory(SlashCommandContext ctx, [Parameter("amount")][Description("maximum amount is 5")] int amount = 5)
        {
            await ctx.DeferResponseAsync();
            var history = await globalWinningNumberService.GetLotteryHistory();
            
            if (!history.IsOk)
            {
                await ctx.RespondAsync($"{history.Error.ErrorMessage}");
                return;
            }

            if (amount > 5) amount = 5;
            var sortedHistory = history.Value.OrderByDescending(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            var sb = new StringBuilder();
            var historyData = sortedHistory.Take(amount);

            sb.AppendLine($"**Lottery Drawing History (Last {amount} Drawings)**\n");
            foreach (var item in historyData)
            {
                sb.AppendLine($"**{item.Key.ToString("MM/dd/yyyy")}** {string.Join(", ", item.Value)}");
            }

            DiscordComponent[] comps =
            [
                new DiscordTextDisplayComponent("**Daily Numbers Lottery History**"),
                new DiscordSeparatorComponent(true),
                new DiscordTextDisplayComponent(sb.ToString()),
                new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                new DiscordTextDisplayComponent($"Gameday Tracker ©️ {DateTimeOffset.UtcNow.ToTimestamp()}")
            ];
            var container = new DiscordContainerComponent(comps, false, DiscordColor.Blurple);
            var msg = new DiscordMessageBuilder()
                .EnableV2Components()
                .AddContainerComponent(container);
            await ctx.RespondAsync(msg);
        }
        #endregion

        #region HELP
        [Command("help")]
        [Description("get help for lottery daily numbers [straight, box, boxstraight]")]
        public async Task GetLotteryHelp(SlashCommandContext ctx)
        {

        }
        #endregion
    }
}
