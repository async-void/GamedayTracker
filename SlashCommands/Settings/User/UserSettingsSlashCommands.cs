using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GamedayTracker.ChoiceProviders;
using GamedayTracker.Extensions;
using GamedayTracker.Helpers;
using GamedayTracker.Interfaces;
using GamedayTracker.Services;
using Humanizer;

namespace GamedayTracker.SlashCommands.Settings.User
{
    public class UserSettingsSlashCommands(IJsonDataService jsonDataService)
    {
        #region FAVORITE TEAM
        [Command("favorite-team")]
        [Description("set's the user's favorite NFL team.")]
        public async Task SetFavoriteTeam(CommandContext ctx, [Parameter("team")] string teamName)
        {
            await ctx.DeferResponseAsync();
            var teamNameMatch = NflTeamMatcher.MatchTeam(teamName);
            var abbr = teamNameMatch!.ToAbbr();
            var logoPath = LogoPathService.GetLogoPath(abbr);
            
            var memResult = await jsonDataService.GetMemberFromJsonAsync(ctx.Member.Id.ToString(), ctx.Guild!.Id.ToString());
            if (memResult.IsOk)
            {
                var member = memResult.Value;
                member.FavoriteTeam = teamNameMatch;
                var teamEmoji = NflEmojiService.GetEmoji(abbr);
                var writeResult = await jsonDataService.UpdateMemberDataAsync(member);

                if (writeResult.IsOk)
                {
                    DiscordComponent[] components =
                    [
                        new DiscordTextDisplayComponent("### User Settings - Favorite Team"),
                        new DiscordSeparatorComponent(true),
                        new DiscordTextDisplayComponent($"Favorite Team set to: {teamName.Titleize()} {teamEmoji}"),
                        new DiscordSeparatorComponent(true),
                        new DiscordSectionComponent(new DiscordTextDisplayComponent($"{teamName}"),
                            new DiscordThumbnailComponent(logoPath)),
                        new DiscordSeparatorComponent(true),
                        new DiscordSectionComponent(new DiscordTextDisplayComponent($"Gameday Tracker ©️ {DateTime.UtcNow.ToShortDateString()}"),
                            new DiscordButtonComponent(DiscordButtonStyle.Primary, label: "Donate", customId: "donateId"))
                    ];

                    var container = new DiscordContainerComponent(components, false, DiscordColor.DarkGray);
                    var message = new DiscordMessageBuilder()
                        .EnableV2Components()
                        .AddContainerComponent(container);


                    await ctx.RespondAsync(new DiscordInteractionResponseBuilder(message));

                    await ctx.User.SendMessageAsync($"favorite team ``{teamName.Titleize()}`` {teamEmoji} has been set!");
                }
                else
                {
                    var message = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithTitle($"User Setting - Set Favorite Team Command")
                            .WithDescription($"{writeResult.Error.ErrorMessage}")
                            .WithTimestamp(DateTime.UtcNow));
                    await ctx.EditResponseAsync(message);
                }
            }
            else
            {
                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle($"NOT FOUND")
                        .WithDescription("WIP: member is not in db\r\nwould you like to add the member now?")
                        .WithTimestamp(DateTime.UtcNow));

                await ctx.EditResponseAsync(message);
            }
        }
        #endregion

        #region ENABLE UPDATES
        [Command("user-settings")]
        [Description("user settings")]
        public async Task UserSettings(CommandContext ctx, [SlashChoiceProvider<UserSettingsChoiceProvider>] int choice)
        {
            await ctx.DeferResponseAsync();
            switch (choice)
            {
                case 0:
                    var userName = ctx.Member!.Username;
                    break;
                case 1:
                    break;
                case 2:
                    break;
                default:
                    var message = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithTitle($"User Setting - Enable Updates Command ")
                            .WithDescription("Unknown Command!")
                            .WithTimestamp(DateTime.UtcNow));
                    await ctx.EditResponseAsync(message);
                    break;
            }
        }
        #endregion
    }
}
