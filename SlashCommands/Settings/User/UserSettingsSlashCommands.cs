using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GamedayTracker.ChoiceProviders;
using GamedayTracker.Extensions;
using GamedayTracker.Factories;
using GamedayTracker.Services;

namespace GamedayTracker.SlashCommands.Settings.User
{
    public class UserSettingsSlashCommands
    {
        #region FAVORITE TEAM
        [Command("favorite-team")]
        [Description("set's the user's favorite NFL team.")]
        public async Task SetFavoriteTeam(SlashCommandContext ctx, [Parameter("team")] string teamName)
        {
            await ctx.DeferResponseAsync();
            var abbr = teamName.ToAbbr();
            var logoPath = LogoPathService.GetLogoPath(abbr);
            var userName = ctx.Member!.Username;
            await using var db = new BotDbContextFactory().CreateDbContext();
            var dbMember = db.Members.Where(x => x.MemberName.Equals(userName))!.FirstOrDefault();
            if (dbMember is not null)
            {
                dbMember.FavoriteTeam = teamName;

                DiscordComponent[] components =
                [
                    
                    new DiscordTextDisplayComponent("User Settings - Favorite Team"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"Favorite Team set to: {teamName}"),
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

                db.Members.Update(dbMember);
                await db.SaveChangesAsync();
                await ctx.User.SendMessageAsync($"favorite team {teamName} has been set!");
            }
            else
            {
                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle($"Daily Command")
                        .WithDescription("WIP: member is not in db\r\nwould you like to add the member now?")
                        .WithTimestamp(DateTime.UtcNow));

                await ctx.EditResponseAsync(message);
            }
        }
        #endregion

        #region ENABLE UPDATES
        [Command("user-settings")]
        [Description("user settings")]
        public async Task UserSettings(SlashCommandContext ctx, [SlashChoiceProvider<UserSettingsChoiceProvider>] int choice)
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
