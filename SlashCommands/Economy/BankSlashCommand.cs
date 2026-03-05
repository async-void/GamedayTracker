using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using GamedayTracker.Helpers;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Services;
using GamedayTracker.Utility;
using GamedayTracker.Utility.Ansi;
using Humanizer;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Channels;

namespace GamedayTracker.SlashCommands.Economy
{
      
    [Command("bank")]
    [Description("bank group commands")]
    public class BankSlashCommand(IJsonDataService dataService, ILogger<BankSlashCommand> logger, IDiscordEmbedService embedService)
    {
        private readonly IJsonDataService _dataService = dataService;
        private readonly ILogger<BankSlashCommand> _logger = logger;
        private readonly IDiscordEmbedService _embedService = embedService;

        #region BALANCE
        [Command("balance")]
        [Description("Get User Bank Balance")]
        public async Task GetUserBalance(SlashCommandContext ctx,
            [Parameter("member")] DiscordUser user)
        {
            await ctx.DeferResponseAsync();
            var member = await ctx.Channel.Guild.GetMemberAsync(user.Id);
            var player = await _dataService.GetMemberFromJsonAsync(member.Id, ctx.Channel.Guild.Id);
            var timestamp = DateTimeOffset.UtcNow.ToTimestamp();
            DiscordComponent[] buttons =
            [
                new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate")
            ];

            if (player.IsOk)
            {
                var balance = player.Value.Bank?.Balance ?? 5;
                var depositTimestamp = player.Value.Bank?.DepositTimestamp ?? DateTimeOffset.UtcNow;  
                var depositedTimestamp = depositTimestamp.ToUnixTimeSeconds();

                var embed = new DiscordEmbedBuilder()
                            .WithTitle($"{player.Value.MemberName}'s Bank")
                            .WithColor(DiscordColor.DarkGreen)
                            .WithThumbnail(member.AvatarUrl)
                            .AddField("Balance", balance.ToString(), true)
                            .AddField("Last Deposit", depositTimestamp.Humanize(), true)
                            .WithFooter($"Gameday Tracker ©️ ")
                            .WithTimestamp(DateTimeOffset.UtcNow)
                            .Build();

                var message = new DiscordMessageBuilder()
                    .AddEmbed(embed);

                await ctx.EditResponseAsync(message);
                return;
            }

            var errContainer = await _embedService.BuildErrorContainer(ctx.Client, $"Error fetching user balance: {player.Error.ErrorMessage!} with ErrorCode: {player.Error.ErrorCode}", ctx.Guild?.Id ?? 0, DiscordColor.Red);

            var errMessage = new DiscordMessageBuilder()
                .EnableV2Components()
                .AddContainerComponent(errContainer);
            var logChannel = await ctx.Client.GetChannelAsync(1384436855524692048);
            await logChannel.SendMessageAsync(errMessage);
                
            var consoleMagenta = AnsiColors.GetAnsiCode("magenta");
            var resetConsole = AnsiColors.GetAnsiCode("reset");
            Console.ForegroundColor = ConsoleColor.Magenta;
            _logger.LogInformation("unable to fetch member [{MemberName}] balance - error: {ErrorMessage}", member.Username, player.Error.ErrorMessage);
            Console.ResetColor();
            await ctx.EditResponseAsync(errMessage);
        }
        #endregion

        #region DAILY
        [Command("daily")]
        [Description("adds the daily [$5.00] to the user account")]
        public async ValueTask RunDaily(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync(ephemeral: true);
            TimeSpan TIMESPAN = TimeSpan.FromHours(2);
            var timestamp = DateTimeOffset.UtcNow.ToTimestamp();
            var member = ctx.Member;
            var _user = await _dataService.GetMemberFromJsonAsync(member!.Id, member.Guild.Id);

            if (_user.IsOk)
            {
                var dailyTimeStamp = _user.Value.Bank?.DepositTimestamp ?? DateTimeOffset.UtcNow;
                var currentTime = DateTimeOffset.UtcNow;
                var lastUsed = _user.Value.Bank?.DepositTimestamp ?? DateTimeOffset.UtcNow;
                var nextAvailable = lastUsed + TIMESPAN;
                var timeElapsed = currentTime - dailyTimeStamp;

                if (timeElapsed.TotalHours >= 2)
                {
                    var balance = _user.Value.Bank?.Balance + 5 ?? 5;
                    _user.Value.Bank!.Balance = balance;
                    _user.Value.Bank.DepositTimestamp = DateTimeOffset.UtcNow;
                    var userToUpdate = _user.Value;

                    var updateUserResult = await _dataService.UpdateMemberDataAsync(userToUpdate);

                    if (updateUserResult.IsOk)
                    {
                        var updatedUser = await _dataService.GetMemberFromJsonAsync(member!.Id, member.Guild.Id);

                        if (updatedUser.IsOk)
                        {
                            lastUsed = updatedUser.Value.Bank?.DepositTimestamp ?? DateTimeOffset.UtcNow;
                            nextAvailable = lastUsed + TIMESPAN;
                            var unixTimestamp = nextAvailable.ToTimestamp();

                            //TODO: change this to the V2 embed
                            var message = new DiscordMessageBuilder()
                            .AddEmbed(new DiscordEmbedBuilder()
                                .WithTitle($"Daily Command")
                                .WithDescription($"Done!  **{updatedUser.Value.MemberName}'s** balance is <:money:1337795714855600188> {balance.ToString("C", CultureInfo.CreateSpecificCulture("en-US"))}\r\nyou can use daily again {unixTimestamp} from now")
                                .WithFooter($"Gameday Tracker ©️ {timestamp}"));

                            await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
                        }
                        else
                        {
                            var errContainer = await _embedService.BuildErrorContainer(ctx.Client, $"Error fetching updated user data: {updatedUser.Error.ErrorMessage!} with ErrorCode: {updatedUser.Error.ErrorCode}", ctx.Guild?.Id ?? 0, DiscordColor.Red);
                            var errEmbed = new DiscordMessageBuilder()
                                .EnableV2Components()
                                .AddContainerComponent(errContainer);


                            await ctx.EditResponseAsync(new DiscordMessageBuilder()
                                .AddContainerComponent(errContainer));
                                
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
                    var unixTimestamp = nextAvailable.ToTimestamp();

                    await ctx.FollowupAsync(new DiscordFollowupMessageBuilder()
                        .WithContent($"you can use ``/daily`` again {unixTimestamp}")
                        .AsEphemeral(true));
                }

            }
            //user is not in json file, add user to the json file then run daily.
            else
            {
                var bank = new Bank()
                {
                    BankId = 0,
                    Balance = 5,
                    DepositTimestamp = DateTimeOffset.UtcNow,
                    LastDepositAmount = 5
                };

                var bets = new List<Bet>();

                var user = new GuildMember()
                {
                    MemberId = ctx.Member.Id,
                    GuildName = ctx.Guild?.Name ?? "Not Found",
                    GuildId = ctx.Guild.Id,
                    MemberName = member.Username,
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
                var unixTimestamp = nextAvailable.ToTimestamp();

                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle($"Daily Command")
                        .WithDescription($"Done! **{member.Username}'s** balance is <:money:1337795714855600188> {user.Bank?.Balance.ToString("C", CultureInfo.CreateSpecificCulture("en-US"))}\r\nyou may use daily again " +
                                         $"{unixTimestamp} from now")
                        .WithFooter($"Gameday Tracker ©️ {timestamp}"));
               
                await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
            }
        }
        #endregion
    }
}
