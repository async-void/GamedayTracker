using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using GamedayTracker.Factories;
using GamedayTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace GamedayTracker.SlashCommands.Economy
{
    public class DailySlashCommand
    {
        [Command("daily")]
        [Description("runs the daily command")]
        public async ValueTask Daily(CommandContext ctx)
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
                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle($"Daily Command")
                        .WithDescription($"Member **{dbUser.MemberName}'s** balance is ${balance:#.##}.00")
                        .WithTimestamp(DateTime.UtcNow));

                dbUser.Bank.Balance = balance;

                db.Members.Update(dbUser);
                await db.SaveChangesAsync();

                await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
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
                        .WithDescription($"Member **{member.GlobalName}'s** balance is ${dbMember.Bank.Balance:#.##}.00")
                        .WithTimestamp(DateTime.UtcNow)
                        );

                await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
            }
        }
    }
}
