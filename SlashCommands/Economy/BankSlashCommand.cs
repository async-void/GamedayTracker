using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using GamedayTracker.Helpers;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Utility;
using Humanizer;
using Microsoft.Extensions.Logging;

namespace GamedayTracker.SlashCommands.Economy
{
      
    [Command("bank")]
    [Description("bank group commands")]
    public class BankSlashCommand(IJsonDataService dataService, ILogger<BankSlashCommand> logger)
    {
        private readonly IJsonDataService _dataService = dataService;
        private readonly ILogger<BankSlashCommand> _logger = logger;

        #region BALANCE
        [Command("balance")]
        [Description("Get User Bank Balance")]
        public async Task GetUserBalance(SlashCommandContext ctx,
            [Parameter("member")] DiscordUser user)
        {
            await ctx.DeferResponseAsync();
            var member = await ctx.Channel.Guild.GetMemberAsync(user.Id) as DiscordMember;

            var player = await _dataService.GetMemberFromJsonAsync(member.Id.ToString(), ctx.Channel.Guild.Id.ToString());

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
           
            var _user = await _dataService.GetMemberFromJsonAsync(member!.Id.ToString(), member.Guild.Id.ToString());

            if (_user.IsOk)
            {
                var dailyTimeStamp = _user.Value.Bank?.DepositTimestamp ?? DateTimeOffset.UtcNow;
                var dailyAgainTimestamp = dailyTimeStamp - DateTimeOffset.UtcNow;
                var currentTime = DateTimeOffset.UtcNow;
                DateTimeOffset lastUsed = _user.Value.Bank?.DepositTimestamp ?? DateTimeOffset.UtcNow;
                var nextAvailable = lastUsed + TimeSpan.FromHours(24);
                var timeElapsed = currentTime - dailyTimeStamp;

                _logger.LogInformation("timestamp is {timestamp}", nextAvailable.ToString("MM-dd-yyyy HH:mm:ss tt zzz"));

                if (timeElapsed.TotalDays >= 1)
                {
                   
                    var unixTimestamp = nextAvailable.ToUnixTimeSeconds();

                    var balance = _user.Value.Bank?.Balance + 5.00 ?? 5.00;
                    _user.Value.Bank!.Balance = balance;
                    _user.Value.Bank.DepositTimestamp = DateTimeOffset.UtcNow;
                    var userToUpdate = _user.Value;

                    var updateUserResult = await _dataService.UpdateMemberDataAsync(userToUpdate);

                    if (updateUserResult.IsOk)
                    {
                        var message = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithTitle($"Daily Command")
                            .WithDescription($"Done!  **{_user.Value.MemberName}'s** balance is <:money:1337795714855600188> ${balance:#.##}\r\nyou can use daily again <t:{unixTimestamp}:R> from now")
                            .WithTimestamp(DateTime.UtcNow));

                        await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
                    }
                    else
                        _logger.LogInformation("unable to update {MemberName}'s daily - error: {ErrorMessage}", _user.Value.MemberName, updateUserResult.Error.ErrorMessage);

                    
                }
                else
                {
                    lastUsed = dailyTimeStamp;
                    nextAvailable = lastUsed + TimeSpan.FromHours(24);
                    var unixTimestamp = nextAvailable.ToUnixTimeSeconds();

                    _logger.LogInformation("User {MemberName} has already used daily, next available at {NextAvailable}.", _user.Value.MemberName, nextAvailable.ToString("MM-dd-yyyy hh:mm:ss tt zzz"));

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
                    DepositTimestamp = DateTimeOffset.UtcNow,
                    LastDepositAmount = 5.00
                };

                var user = new GuildMember()
                {
                    Id = Guid.NewGuid(),
                    GuildName = ctx.Guild?.Name ?? "Not Found",
                    GuildId = ctx.Guild?.Id.ToString() ?? "Not Found",
                    MemberName = member.Username,
                    MemberId = member.Id.ToString(),
                    Bank = bank,
                };

                DateTimeOffset lastUsed = _user.Value.Bank?.DepositTimestamp ?? DateTimeOffset.UtcNow;
                var nextAvailable = lastUsed + TimeSpan.FromHours(24);

                var writeResult = await _dataService.WriteMemberToJsonAsync(user);

                _logger.LogInformation("next available Daily at {NextAvailable}.", nextAvailable.ToString("MM-dd-yyyy hh:mm:ss tt zzz"));

                if (!writeResult.IsOk)
                {
                    var errorMessage = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithDescription($"Error writing user data: {writeResult.Error.ErrorMessage}")
                            .WithTimestamp(DateTime.UtcNow));
                    
                    await ctx.EditResponseAsync(errorMessage);
                    return;
                }
                var unixTimestamp = nextAvailable.ToUnixTimeSeconds();

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
