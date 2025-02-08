using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using GamedayTracker.Factories;
using GamedayTracker.Models;
using Humanizer;
using Humanizer.Localisation;
using Microsoft.EntityFrameworkCore;

namespace GamedayTracker.SlashCommands.Economy
{
    [Command("bank")]
    [Description("bank group commands")]
    public class BankSlashCommand
    {
        [Command("balance")]
        [Description("Get User Bank Balance")]
        public async Task GetUserBalance(CommandContext ctx,
            [Parameter("member")] DiscordUser user)
        {
            await ctx.DeferResponseAsync();
            await using var db = new BotDbContextFactory().CreateDbContext();

            // check if user is in the db. consider making a util function to do the following.
            var dbUser = db.Members.Where(x => x.MemberName.Equals(user!.GlobalName) && x.GuildId == ctx.Guild!.Id.ToString())!
                .Include(x => x.Bank)
                .FirstOrDefault();

            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle($"Balance for member **{user.Username}**")
                    .WithDescription("WIP: this gets member's bank balance")
                    .WithTimestamp(DateTime.UtcNow));

            await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
        }

        #region DAILY SLASHCOMMAND
        [Command("daily")]
        [Description("adds the daily [$5.00] to the user account")]
        public async ValueTask RunDaily(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            var member = ctx.Member;
            await using var db = new BotDbContextFactory().CreateDbContext();

            // check if user is in the db. consider making a util function to do the following.
            var dbUser = db.Members.Where(x => x.MemberName.Equals(member!.GlobalName) &&
                                               x.GuildId == ctx.Guild!.Id.ToString())!
                .Include(x => x.Bank)
                .FirstOrDefault();
            //user is in db, run daily command.
            if (dbUser is not null)
            {
                var balance = dbUser.Bank!.Balance + 5.00;
                var dailyTimeStamp = dbUser.Bank.DepositTimestamp;
                var delta = DateTime.UtcNow - dailyTimeStamp;
                var canUse = TimeSpan.FromHours(delta.Hours).Humanize(minUnit: TimeUnit.Minute);

                if (delta.Hours >= 24)
                {
                    var message = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithTitle($"Daily Command")
                            .WithDescription($"Member **{dbUser.MemberName}'s** balance is <:money:1337795714855600188> ${balance:#.##}.00")
                            .WithTimestamp(DateTime.UtcNow));

                    dbUser.Bank.Balance = balance;

                    db.Members.Update(dbUser);
                    await db.SaveChangesAsync();

                    await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
                }
                else
                {
                    var message = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithDescription($"you can use daily in {canUse} from now.")
                            .WithTimestamp(DateTime.UtcNow));
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
                var dbMember = new GuildMember()
                {
                    Bank = bank,
                    GuildId = ctx.Guild!.Id.ToString(),
                    MemberName = member!.GlobalName!,
                    MemberId = member.Id.ToString()
                };

                db.Members.Add(dbMember);
                await db.SaveChangesAsync();

                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle($"Daily Command")
                        .WithDescription($"Member **{member.GlobalName}'s** balance is <:money:1337795714855600188> ${dbMember.Bank.Balance:#.##}.00")
                        .WithTimestamp(DateTime.UtcNow)
                        );

                await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
            }
        }
        #endregion
    }
}
