using System.ComponentModel;
using ChalkDotNET;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using GamedayTracker.Enums;
using GamedayTracker.Factories;
using GamedayTracker.Models;
using Humanizer;
using Humanizer.Localisation;
using GamedayTracker.Utility;
using Microsoft.EntityFrameworkCore;

namespace GamedayTracker.SlashCommands.Economy
{
      
    [Command("bank")]
    [Description("bank group commands")]
    public class BankSlashCommand
    {
        #region BALANCE
        [Command("balance")]
        [Description("Get User Bank Balance")]
        public async Task GetUserBalance(CommandContext ctx,
            [Parameter("member")] DiscordUser user)
        {
            await ctx.DeferResponseAsync();

            await using var db = new BotDbContextFactory().CreateDbContext();

            // check if user is in the db. consider making a util function to do the following.
            var dbUser = db.Members.Where(x => x.MemberName.Equals(user!.Username) && x.GuildId == ctx.Guild!.Id.ToString())!
                .Include(x => x.Bank)
                .FirstOrDefault();

            if (dbUser != null)
            {
                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle($"Balance for member **{user.Username}**")
                        .WithDescription("WIP: this gets member's bank balance")
                        .AddField("<:money:1337795714855600188>Balance<:money:1337795714855600188>", $"{dbUser!.Bank!.Balance}", true)
                        .AddField("Last Deposit", $"{dbUser!.Bank!.DepositTimestamp.ToShortDateString()}", true)
                        .WithColor(DiscordColor.SpringGreen)
                        .WithThumbnail("https://i.imgur.com/iR5m51M.png")
                        .WithTimestamp(DateTime.UtcNow));

                await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
                return;
            }
            var message1 = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle($"member **{user.Username}** not found!")
                    .WithDescription("the member requested was not found in the database, if you ask nicely maybe a ``mod`` will add the requested member! :man_shrugging:")
                    .WithTimestamp(DateTime.UtcNow));

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

            // check if user is in the db. consider making a util function to do the following.
            var dbUser = db.Members.Where(x => x.MemberName.Equals(member!.Username) &&
                                               x.GuildId == ctx.Guild!.Id.ToString())!
                                               .Include(x => x.Bank)
                                               .Include(x => x.PlayerPicks)
                                               .FirstOrDefault();
            //user is in db, run daily command.
            if (dbUser is not null)
            {
                var dailyTimeStamp = dbUser.Bank!.DepositTimestamp;
                var currentTime = DateTime.UtcNow;
                var timeElapsed = currentTime - dailyTimeStamp;
                var timeRemaining = TimeSpan.FromHours(24) - timeElapsed;

                if (timeElapsed.Days >= 1)
                {
                    var balance = dbUser.Bank!.Balance + 5.00;
                    timeRemaining = TimeSpan.FromHours(24);

                    var message = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithTitle($"Daily Command")
                            .WithDescription($"Done!  **{dbUser.MemberName}'s** balance is <:money:1337795714855600188> ${balance:#.##}\r\nyou can use daily again in ``{timeRemaining.Humanize(3, minUnit: TimeUnit.Minute)}`` from now")
                            .WithTimestamp(DateTime.UtcNow));

                    dbUser.Bank.Balance = balance;
                    dbUser.Bank.DepositTimestamp = DateTime.UtcNow;
                    db.Members.Update(dbUser);
                    await db.SaveChangesAsync();

                    Console.WriteLine(
                        $"{Chalk.Yellow($"[{DateTimeOffset.UtcNow}]")} {Chalk.Yellow($"[Gameday Tracker]")} {Chalk.DarkBlue("[INFO]")} {Chalk.DarkGray($"[Daily was used in {ctx.Guild!.Name}]")}");
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
                }
                else
                {
                    var message = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder() 
                            .WithDescription($"you can use daily again in ``{timeRemaining.Humanize(3, minUnit: TimeUnit.Minute)}`` from now")
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
                    Picks = new List<string>()
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
