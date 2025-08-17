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
            var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            DiscordComponent[] buttons =
            [
                new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate")
            ];

            if (player.IsOk)
            {
                var balance = player.Value.Bank?.Balance ?? 5.00;
                var depositTimestamp = player.Value.Bank?.DepositTimestamp ?? DateTimeOffset.UtcNow;  
                var depositedTimestamp = depositTimestamp.ToUnixTimeSeconds();

                DiscordComponent[] components =
                [
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"**{user.GlobalName!}**\r\n<:money:1337795714855600188> Balance - {balance}\r\n<:bank:1366390018423390360> Last Deposit: {depositTimestamp.Humanize()}"),
                        new DiscordThumbnailComponent(member!.AvatarUrl.ToString())),
                    new DiscordSeparatorComponent(true),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Gameday Tracker ©️ <t:{unixTimestamp}:F>"),
                        new DiscordActionRowComponent(buttons))  
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
                new DiscordTextDisplayComponent($"Error fetching player data: {player.Error.ErrorMessage!} with ErrorCode: {player.Error.ErrorCode}"),
                new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Gameday Tracker ©️ <t:{unixTimestamp}:F>"),
                        new DiscordActionRowComponent(buttons))
            ];
            var msgContainer = new DiscordContainerComponent(msgComp, false, DiscordColor.DarkGray);
            var message1 = new DiscordMessageBuilder()
                .EnableV2Components()
                .AddContainerComponent(msgContainer);

            await ctx.EditResponseAsync(new DiscordWebhookBuilder(message1));
        }
        #endregion

        #region DAILY
        [Command("daily")]
        [Description("adds the daily [$5.00] to the user account")]
        public async ValueTask RunDaily(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync(ephemeral: true);
            TimeSpan TIMESPAN = TimeSpan.FromHours(2);

            var member = ctx.Member;
            var _user = await _dataService.GetMemberFromJsonAsync(member!.Id.ToString(), member.Guild.Id.ToString());

            if (_user.IsOk)
            {
                var dailyTimeStamp = _user.Value.Bank?.DepositTimestamp ?? DateTimeOffset.UtcNow;
                var currentTime = DateTimeOffset.UtcNow;
                var lastUsed = _user.Value.Bank?.DepositTimestamp ?? DateTimeOffset.UtcNow;
                var nextAvailable = lastUsed + TIMESPAN;
                var timeElapsed = currentTime - dailyTimeStamp;

                if (timeElapsed.TotalHours >= 2)
                {
                    var balance = _user.Value.Bank?.Balance + 5.00 ?? 5.00;
                    _user.Value.Bank!.Balance = balance;
                    _user.Value.Bank.DepositTimestamp = DateTimeOffset.UtcNow;
                    var userToUpdate = _user.Value;

                    var updateUserResult = await _dataService.UpdateMemberDataAsync(userToUpdate);

                    if (updateUserResult.IsOk)
                    {
                        var updatedUser = await _dataService.GetMemberFromJsonAsync(member!.Id.ToString(), member.Guild.Id.ToString());

                        if (updatedUser.IsOk)
                        {
                            lastUsed = updatedUser.Value.Bank?.DepositTimestamp ?? DateTimeOffset.UtcNow;
                            nextAvailable = lastUsed + TIMESPAN;
                            var unixTimestamp = nextAvailable.ToUnixTimeSeconds();

                            //TODO: change this to the V2 embed
                            var message = new DiscordMessageBuilder()
                            .AddEmbed(new DiscordEmbedBuilder()
                                .WithTitle($"Daily Command")
                                .WithDescription($"Done!  **{updatedUser.Value.MemberName}'s** balance is <:money:1337795714855600188> ${balance:C}\r\nyou can use daily again <t:{unixTimestamp}:R> from now")
                                .WithTimestamp(DateTime.UtcNow));

                            await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
                        }
                        else
                        {
                            await ctx.FollowupAsync(new DiscordFollowupMessageBuilder()
                                .WithContent($"Error fetching updated user data: {updatedUser.Error.ErrorMessage!} with ErrorCode: {updatedUser.Error.ErrorCode}")
                                .AsEphemeral(true));
                        }
                    }
                    else
                    {
                        await ctx.FollowupAsync(new DiscordFollowupMessageBuilder()
                                .WithContent($"Error fetching updated user data: {updateUserResult.Error.ErrorMessage!} with ErrorCode: {updateUserResult.Error.ErrorCode}")
                                .AsEphemeral(true));
                        _logger.LogInformation("unable to update {MemberName}'s daily - error: {ErrorMessage}", _user.Value.MemberName, updateUserResult.Error.ErrorMessage);
                    }   
                }
                else
                {
                    var unixTimestamp = nextAvailable.ToUnixTimeSeconds();

                    await ctx.FollowupAsync(new DiscordFollowupMessageBuilder()
                        .WithContent($"you can use ``/daily`` again <t:{unixTimestamp}:R>")
                        .AsEphemeral(true));
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

                var bets = new List<Bet>();

                var user = new GuildMember()
                {
                    Id = Guid.NewGuid(),
                    GuildName = ctx.Guild?.Name ?? "Not Found",
                    GuildId = ctx.Guild?.Id.ToString() ?? "Not Found",
                    MemberName = member.Username,
                    MemberId = member.Id.ToString(),
                    Bank = bank,
                    Bets = bets,
                };

                DateTimeOffset lastUsed = user.Bank.DepositTimestamp;
                var nextAvailable = lastUsed + TIMESPAN;

                var writeResult = await _dataService.WriteMemberToJsonAsync(user);

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
