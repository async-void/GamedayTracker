using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Utility;
using Humanizer;

namespace GamedayTracker.SlashCommands.Economy
{
      
    [Command("bank")]
    [Description("bank group commands")]
    public class BankSlashCommand(IJsonDataService dataService)
    {
        #region BALANCE
        [Command("balance")]
        [Description("Get User Bank Balance")]
        public async Task GetUserBalance(SlashCommandContext ctx,
            [Parameter("member")] DiscordUser user)
        {
            await ctx.DeferResponseAsync();
            var member = await ctx.Channel.Guild.GetMemberAsync(user.Id);

            //await using var db = new BotDbContextFactory().CreateDbContext();
            //var dbUser = await memberService.GetGuildMemberAsync(ctx.Guild!.Id.ToString(), member!.Username!);
            var player = await dataService.GetMemberFromJsonAsync(member.Id.ToString(), member.Guild.Id.ToString());

            DiscordComponent[] buttons =
            [
                new DiscordButtonComponent(DiscordButtonStyle.Secondary, "btnAddPlayer", "Add Player"),
                new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate")
            ];

            if (player.IsOk)
            {
                var balance = player.Value.Bank?.Balance ?? 5.00;
                DiscordComponent[] components =
                [
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"**Member: {user.GlobalName!}**\r\n\r\n<:money:1337795714855600188> Balance - {balance}\r<:bank:1366390018423390360> Last Deposit - {player.Value.Bank!.DepositTimestamp}"),
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
        public async ValueTask RunDaily(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            var member = ctx.Member;
           
            var _user = await dataService.GetMemberFromJsonAsync(member!.Id.ToString(), member.Guild.Id.ToString());

            if (_user.IsOk)
            {
                var dailyTimeStamp = _user.Value.Bank?.DepositTimestamp ?? DateTimeOffset.UtcNow;
                var dailyAgainTimestamp = dailyTimeStamp - DateTimeOffset.UtcNow;
                var currentTime = DateTimeOffset.UtcNow;
                var timeElapsed = currentTime - dailyTimeStamp;
               

                if (timeElapsed.TotalDays >= 1)
                {
                    DateTimeOffset lastUsed = _user.Value.Bank?.DepositTimestamp ?? DateTimeOffset.UtcNow;
                    DateTimeOffset nextAvailable = lastUsed.AddHours(24);
                    long unixTimestamp = nextAvailable.ToUnixTimeSeconds();

                    var balance = _user.Value.Bank?.Balance + 5.00 ?? 5.00;
                    _user.Value.Bank!.Balance = balance;
                    _user.Value.Bank.DepositTimestamp = DateTimeOffset.UtcNow;

                    var updateUserResult = await dataService.UpdateMemberDataAsync(_user.Value);

                    var message = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithTitle($"Daily Command")
                            .WithDescription($"Done!  **{_user.Value.MemberName}'s** balance is <:money:1337795714855600188> ${balance:#.##}\r\nyou can use daily again <t:{unixTimestamp}:R> from now")
                            .WithTimestamp(DateTime.UtcNow));

                   
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
                }
                else
                {
                    DateTimeOffset lastUsed = dailyTimeStamp;
                    DateTimeOffset nextAvailable = lastUsed.AddHours(24);
                    long unixTimestamp = nextAvailable.ToUnixTimeSeconds();

                    var message = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder() 
                            .WithDescription($"you can use daily again <t:{unixTimestamp}:R>")
                            .WithTimestamp(DateTime.UtcNow));
                   
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
                }

            }
            //user is not in json file, add user to the json file then run daily.
            else
            {
                var bank = new Bank()
                {
                    Id = Guid.NewGuid(),
                    Balance = 5.00,
                    DepositTimestamp = DateTime.UtcNow,
                    LastDepositAmount = 5.00
                };

                var user = new GuildMember()
                {
                    Id = Guid.NewGuid(),
                    GuildName = ctx.Guild?.Name ?? "Not Found",
                    GuildId = member.Guild.Id.ToString(),
                    MemberName = member.Username,
                    MemberId = member.Id.ToString(),
                    Bank = bank,
                };

                var writeResult = await dataService.WriteMemberToJsonAsync(user);
                
                if (!writeResult.IsOk)
                {
                    var errorMessage = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithDescription($"Error writing user data: {writeResult.Error.ErrorMessage}")
                            .WithTimestamp(DateTime.UtcNow));
                    
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder(errorMessage));
                    return;
                }
                DateTimeOffset lastUsed = user.Bank?.DepositTimestamp ?? DateTimeOffset.UtcNow;
                DateTimeOffset nextAvailable = lastUsed.AddHours(24);
                long unixTimestamp = nextAvailable.ToUnixTimeSeconds();

                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle($"Daily Command")
                        .WithDescription($"Done! **{member.Username}'s** balance is <:money:1337795714855600188> {user.Bank?.Balance}\r\nyou may use daily again " +
                                         $"<t:{unixTimestamp}:R> from now")
                        .WithTimestamp(DateTime.UtcNow)
                        );
               
                await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
            }
        }
        #endregion
    }
}
