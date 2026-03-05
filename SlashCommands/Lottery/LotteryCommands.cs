using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GamedayTracker.AutoCompleteProvider;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Utility;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace GamedayTracker.SlashCommands.Lottery
{
    [Command("lottery")]
    [Description("lottery commands")]
    public class LotteryCommands(ILogger<LotteryCommands> logger, IJsonDataService jsonDataService, ILotteryService lotteryService, IDailyNumbersCache cache)
    {
        [Command("add-numbers")]
        [Description("add your daily numbers for the lottery drawing")]
        public async Task SaveDailyNumbers(SlashCommandContext ctx, [SlashAutoCompleteProvider<DailyNumbersTypeAutoCompleteProvider>] string choice, [Parameter("one")] int numOne,
            [Parameter("two")] int numTwo, [Parameter("three")] int numThree)
        {
            await ctx.DeferResponseAsync();
            var timestamp = DateTimeOffset.UtcNow.ToTimestamp();
            IReadOnlyList<int> userNumbers = [ numOne, numTwo, numThree ];
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (cache.HasUserSubmitted(ctx.Guild.Id, ctx.User.Id, today))
            {
                DiscordComponent[] comps =
                [
                    new DiscordTextDisplayComponent("**Daily Numbers**"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"{ctx.User.Username} already submitted their numbers today.\r\nDaily Number's Drawing is at 10pm est every evening\r\n**Good luck**"),
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

            try
            {
                cache.AddUserPick(
                new DailyNumberPick(
                    ctx.Guild.Id,
                    ctx.User.Id,
                    today,
                    userNumbers,
                    choice,
                    DateTimeOffset.UtcNow
                ));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "User {UserId} submitted invalid numbers: {Numbers}", ctx.User.Id, userNumbers);
                await ctx.RespondAsync($"Invalid numbers submitted: {ex.Message}");
                return;
            }
               

            var keys = cache.GetActiveGuildIds(today);
            var guildName = await ctx.Client.GetGuildAsync(keys[0]);
            await ctx.FollowupAsync($"{ctx.Member.DisplayName}, Your numbers have been recorded! Guild: {ctx.Guild.Name} **Good luck** in tonights drawing!");
           // await ctx.RespondAsync($"{ctx.User.Username}'s numbers have been recorded! {guildName}");
        }

        [Command("help")]
        [Description("get help for lottery daily numbers [straight, box, boxstraight]")]
        public async Task GetLotteryHelp(SlashCommandContext ctx)
        {

        }
    }
}
