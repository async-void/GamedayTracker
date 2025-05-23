using System.ComponentModel;
using System.Globalization;
using ChalkDotNET;
using DSharpPlus.Commands;
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
    public class BankSlashCommand(ILogger logger, IGuildMemberService memberService)
    {
        #region BALANCE
        [Command("balance")]
        [Description("Get User Bank Balance")]
        public async Task GetUserBalance(CommandContext ctx,
            [Parameter("member")] DiscordUser user)
        {
            await ctx.DeferResponseAsync();
            var member = await ctx.Channel.Guild.GetMemberAsync(user.Id);

            await using var db = new BotDbContextFactory().CreateDbContext();
            var dbUser = await memberService.GetGuildMemberAsync(ctx.Guild!.Id.ToString(), member!.Username!);

            DiscordComponent[] buttons =
            [
                new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate")
            ];

            if (dbUser.IsOk)
            {
                var balance = dbUser.Value.Bank!.Balance.ToString("C");
                DiscordComponent[] components =
                [
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"**Member: {user.GlobalName!}**\r\n\r\n<:money:1337795714855600188> Balance - {balance}\r<:bank:1366390018423390360> Last Deposit - {dbUser.Value.Bank!.DepositTimestamp.ToShortDateString()}"),
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
                logger.Log(LogTarget.Console, LogType.Debug, DateTimeOffset.UtcNow, $"Get Member Balance was used in {ctx.Guild!.Name} by member: {Chalk.Yellow(ctx.Member!.GlobalName!)}");
                return;
            }

            DiscordComponent[] msgComp =
            [
                new DiscordTextDisplayComponent("the member requested was not found in the database, if you ask nicely maybe a ``mod`` will add the requested member! :man_shrugging:"),
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

            await using var db = new BotDbContextFactory().CreateDbContext();
            var dbUser = await memberService.GetGuildMemberAsync(ctx.Guild!.Id.ToString(), member!.Username!);

            //user is in db, run daily command.
            if (dbUser.IsOk)
            {
                var dailyTimeStamp = dbUser.Value.Bank!.DepositTimestamp;
                var currentTime = DateTime.UtcNow;
                var timeElapsed = currentTime - dailyTimeStamp;
                var timeRemaining = TimeSpan.FromHours(24) - timeElapsed;

                if (timeElapsed.Days >= 1)
                {
                    var balance = dbUser.Value.Bank!.Balance + 5.00;
                    timeRemaining = TimeSpan.FromHours(24);

                    var message = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithTitle($"Daily Command")
                            .WithDescription($"Done!  **{dbUser.Value.MemberName}'s** balance is <:money:1337795714855600188> ${balance:#.##}\r\nyou can use daily again in ``{timeRemaining.Humanize(3, minUnit: TimeUnit.Minute)}`` from now")
                            .WithTimestamp(DateTime.UtcNow));

                    dbUser.Value.Bank.Balance = balance;
                    dbUser.Value.Bank.DepositTimestamp = DateTime.UtcNow;
                    db.Members.Update(dbUser.Value);
                    await db.SaveChangesAsync();

                    Console.WriteLine(
                        $"{Chalk.Yellow($"[{DateTimeOffset.UtcNow}]")} {Chalk.Yellow($"[Gameday Tracker]")} {Chalk.DarkBlue("[INFO]")} {Chalk.DarkGray($"[Daily was used in {ctx.Guild!.Name}]")}");
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
                }
                else
                {
                    var message = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder() 
                            .WithDescription($"you can use daily again in ``{timeRemaining.Humanize(4, minUnit: TimeUnit.Second)}`` from now")
                            .WithTimestamp(DateTime.UtcNow));
                    Console.WriteLine(
                        $"{Chalk.Yellow($"[{DateTimeOffset.UtcNow}]")} {Chalk.Yellow($"[Gameday Tracker]")} {Chalk.DarkBlue("[INFO]")} {Chalk.DarkGray($"[Daily was attempted use in {ctx.Guild!.Name}]")}");
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

                var dbMember = new GuildMember()
                {
                    Bank = bank,
                    GuildId = ctx.Guild!.Id.ToString(),
                    MemberName = member!.Username!,
                    MemberId = member.Id.ToString(),
                    PlayerPicks = picks
                };

                db.Members.Add(dbMember);
                await db.SaveChangesAsync();

                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle($"Daily Command")
                        .WithDescription($"Done! **{member.Username}'s** balance is <:money:1337795714855600188> ${dbMember.Bank.Balance:#.##}\r\nyou may use daily again in " +
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
