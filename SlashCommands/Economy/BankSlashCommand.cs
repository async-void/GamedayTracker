using System.ComponentModel;
using System.Globalization;
using ChalkDotNET;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.MessageCommands;
using DSharpPlus.Entities;
using GamedayTracker.Enums;
using GamedayTracker.Factories;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using Humanizer;

namespace GamedayTracker.SlashCommands.Economy
{
      
    [Command("bank")]
    [Description("bank group commands")]
    public class BankSlashCommand(ILogger logger, IJsonDataService dataService)//TODO: convert from database to json
    {
        #region BALANCE
        [Command("balance")]
        [Description("Get User Bank Balance")]
        public async Task GetUserBalance(MessageCommandContext ctx,
            [Parameter("member")] DiscordUser user)
        {
            await ctx.DeferResponseAsync();
            var member = await ctx.Channel.Guild.GetMemberAsync(user.Id);

            //await using var db = new BotDbContextFactory().CreateDbContext();
            //var dbUser = await memberService.GetGuildMemberAsync(ctx.Guild!.Id.ToString(), member!.Username!);
            var player = await dataService.GetMemberFromJsonAsync(member.Id.ToString());

            DiscordComponent[] buttons =
            [
                new DiscordButtonComponent(DiscordButtonStyle.Secondary, "btnAddPlayer", "Add Player"),
                new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate")
            ];

            if (player.IsOk)
            {
                var balance = player.Value.Balance!;
                DiscordComponent[] components =
                [
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"**Member: {user.GlobalName!}**\r\n\r\n<:money:1337795714855600188> Balance - {balance}\r<:bank:1366390018423390360> Last Deposit - {player.Value.LastDeposit}"),
                        new DiscordThumbnailComponent(member!.AvatarUrl.ToString())),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"Gameday Tracker - {DateTime.UtcNow.ToLongDateString()}"),
                    new DiscordActionRowComponent(buttons)
                   
                ];

                var container = new DiscordContainerComponent(components, false, DiscordColor.DarkGreen);
                var message = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);

                await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
                logger.Log(LogTarget.Console, LogType.Debug, DateTime.UtcNow, $"Get Member Balance was used in {ctx.Guild!.Name} by member: {Chalk.Yellow(ctx.Member!.GlobalName!)}");
                return;
            }

            DiscordComponent[] msgComp =
            [
                new DiscordTextDisplayComponent(player.Error.ErrorMessage!),
                new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                new DiscordTextDisplayComponent($"Gameday Tracker - {DateTime.UtcNow.ToLongDateString()}"),
                new DiscordActionRowComponent(buttons)
            ];
            var msgContainer = new DiscordContainerComponent(msgComp, false, DiscordColor.DarkGray);
            var message1 = new DiscordMessageBuilder()
                .EnableV2Components()
                .AddContainerComponent(msgContainer);

            await ctx.EditResponseAsync(new DiscordWebhookBuilder(message1));
        }
        #endregion

        #region DAILY SLASHCOMMAND
        [Command("daily")]
        [Description("adds the daily [$5.00] to the user account")]
        public async ValueTask RunDaily(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            var member = ctx.Member;


            var dbUser = await dataService.GetMemberFromJsonAsync(member!.Id.ToString());
            logger.Log(LogTarget.Console, LogType.Debug, DateTime.UtcNow,
                $"attempting to fetch user {member!.GlobalName!}");

            //user is in db, run daily command.
            if (dbUser.IsOk)
            {
                var dailyTimeStamp = dbUser.Value.LastDeposit;
                var currentTime = DateTime.UtcNow;
                var timeElapsed = currentTime - dailyTimeStamp;
                var timeRemaining = TimeSpan.FromHours(24) - timeElapsed;

                if (timeElapsed.Value.TotalDays >= 1)
                {
                    var balance = dbUser.Value.Balance + 5.00;
                    timeRemaining = TimeSpan.FromHours(24);

                    var message = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithTitle($"Daily Command")
                            .WithDescription($"Done!  **{dbUser.Value.MemberName}'s** balance is <:money:1337795714855600188> ${balance:#.##}\r\nyou can use daily again in ``{timeRemaining}`` from now")
                            .WithTimestamp(DateTime.UtcNow));

                    dbUser.Value.Balance = balance;
                    dbUser.Value.LastDeposit = DateTime.UtcNow;
                    //TODO: write member data to json

                    logger.Log(LogTarget.Console, LogType.Debug, DateTime.UtcNow,
                        $"{Chalk.Yellow($"[{DateTimeOffset.UtcNow}]")} {Chalk.Yellow($"[Gameday Tracker]")} {Chalk.DarkBlue("[INFO]")} {Chalk.DarkGray($"[Daily was used in {ctx.Guild!.Name}]")}");
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
                }
                else
                {
                    var message = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder() 
                            .WithDescription($"you can use daily again in ``{timeRemaining}`` from now")
                            .WithTimestamp(DateTime.UtcNow));
                    logger.Log(LogTarget.Console, LogType.Debug, DateTime.UtcNow,
                        $"{Chalk.Yellow($"[{DateTimeOffset.UtcNow}]")} {Chalk.Yellow($"[Gameday Tracker]")} {Chalk.DarkBlue("[INFO]")} {Chalk.DarkGray($"[Daily attempted use in {ctx.Guild!.Name}]")}");
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
                }

            }
            //user is not in db, add user to db then run daily.
            else
            {
                var bank = new Bank()
                {
                    Balance = 5.00,
                    DepositAmount = 5.00,
                    DepositTimestamp = DateTime.UtcNow,
                    LastDeposit = 5.00
                };

                var picks = new PlayerPicks()
                {
                    Season = 0,
                    Week = 0,
                };


                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle($"Daily Command")
                        .WithDescription($"Done! **{member.Username}'s** balance is <:money:1337795714855600188> $me\r\nyou may use daily again in " +
                                         $"``{TimeSpan.FromHours(24).Humanize(3, minUnit: TimeUnit.Minute)}`` from now")
                        .WithTimestamp(DateTime.UtcNow)
                        );
                Console.WriteLine(
                    $"{Chalk.Yellow($"[{DateTimeOffset.UtcNow}]")} {Chalk.Yellow($"[Gameday Tracker]")} {Chalk.DarkBlue("[INFO]")} {Chalk.DarkGray($"[Daily was used in {ctx.Guild.Name}]")}");
                await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
            }
        }
        #endregion
    }
}
