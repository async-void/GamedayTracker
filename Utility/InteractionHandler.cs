using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using GamedayTracker.Extensions;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Services;
using Humanizer;

namespace GamedayTracker.Utility
{
    public class InteractionHandler(ITeamData teamData, IPlayerData playerDataService, IJsonDataService jsonService): IEventHandler<InteractionCreatedEventArgs>
    {
        public async Task HandleEventAsync(DiscordClient sender, InteractionCreatedEventArgs eventArgs)
        {
            switch (eventArgs.Interaction.Type)
            {
                case DiscordInteractionType.ApplicationCommand:
                {
                    break;
                }

                #region COMPONENTS - BUTTONS
                case DiscordInteractionType.Component:
                {

                        var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                        DiscordComponent[] backBtns =
                        [
                            new DiscordButtonComponent(DiscordButtonStyle.Secondary, "backId", "⬅️ Back"),
                        ];

                        switch (eventArgs.Interaction.Data.CustomId)
                        {
                            #region AFC DROPDOWN
                            case "afcDropdown":
                            {
                                var tName = eventArgs.Interaction.Data.Values[0];
                                var draftResult = await teamData.GetDraftResultForTeamAsync(2025, tName);
                                var msgBuilder = new StringBuilder();

                                foreach (var draftEntity in draftResult.Value)
                                {
                                    msgBuilder.Append($"round **{draftEntity.Round}** | **{draftEntity.PlayerName}** | position **{draftEntity.Pos}** | college **{draftEntity.College}**\r\n");
                                }
                                var shortName = tName.ToShortName();
                                var emoji = NflEmojiService.GetEmoji(shortName.ToAbbr());

                                DiscordComponent[] components =
                                [
                                    new DiscordTextDisplayComponent($"**2025** Draft Results for **{tName}**{emoji}"),
                                    new DiscordSeparatorComponent(true),
                                    new DiscordTextDisplayComponent(msgBuilder.ToString()),
                                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                                    new DiscordMediaGalleryComponent(new DiscordMediaGalleryItem("https://i.imgur.com/i6yCh8q.png"))
                                ];
                                var container = new DiscordContainerComponent(components, false, DiscordColor.DarkGray);

                                var message = new DiscordInteractionResponseBuilder()
                                .EnableV2Components()
                                .AddContainerComponent(container);

                                await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(message));
                                break;
                            }
                            #endregion

                            #region NFC DROPDOWN
                            case "nfcDropdown"://TODO: fix V2 component message
                            {

                                var tName = eventArgs.Interaction.Data.Values[0];
                                var draftResult = await teamData.GetDraftResultForTeamAsync(2025, tName);
                                var msgBuilder = new StringBuilder();

                                foreach (var draftEntity in draftResult.Value)
                                {
                                    msgBuilder.Append($"round **{draftEntity.Round}** | **{draftEntity.PlayerName}** | position **{draftEntity.Pos}** | college **{draftEntity.College}**\r\n");
                                }
                                var shortName = tName.ToShortName();
                                var emoji = NflEmojiService.GetEmoji(shortName.ToAbbr());
                                DiscordComponent[] components =
                                [
                                    new DiscordTextDisplayComponent($"**2025** Draft Results for **{tName}**{emoji}"),
                                    new DiscordSeparatorComponent(true),
                                    new DiscordTextDisplayComponent(msgBuilder.ToString()),
                                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                                    new DiscordMediaGalleryComponent(new DiscordMediaGalleryItem("https://i.imgur.com/i6yCh8q.png"))
                                ];
                                var container = new DiscordContainerComponent(components, false, DiscordColor.DarkGray);

                                var message = new DiscordInteractionResponseBuilder()
                                    .EnableV2Components()
                                    .AddContainerComponent(container);
                                await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(message)); 
                                break;
                            }
                            #endregion

                            #region SCOREBOARD
                            case "scoreboardHelpBtn":
                            {
                                DiscordComponent[] components =
                                [
                                new DiscordTextDisplayComponent("Scoreboard Help Section"),
                                new DiscordSeparatorComponent(true),
                                new DiscordTextDisplayComponent("1. Select the Season\r2. Select the Week"),
                                new DiscordSeparatorComponent(true),
                                new DiscordTextDisplayComponent("this slash command will fetch ``All`` the game totals for the selected ``Season`` and ``Week``"),
                                new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                                new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Powered by Gameday Tracker ©️ <t:{unixTimestamp}:F>"),
                                    new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate")),
                                new DiscordActionRowComponent(backBtns)
                                ];

                                var container = new DiscordContainerComponent(components, false, DiscordColor.DarkGray);
                                var scoreboardMessage = new DiscordInteractionResponseBuilder()
                                    .EnableV2Components()
                                    .AddContainerComponent(container);

                                await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(scoreboardMessage));
                                break;
                            }
                            #endregion

                            #region BUTTONS
                            #region DONATE
                            case "donateId":
                                DiscordComponent[] bComponent =
                                [
                                    new DiscordTextDisplayComponent(
                                        "Donate is in development, the devs are hard at work implementing this feature!"),
                                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                                    new DiscordTextDisplayComponent($"-# Gameday Tracker ©️ <t:{unixTimestamp}:F>")
                                ];
                                var cContainer =
                                    new DiscordContainerComponent(bComponent, false, DiscordColor.LightGray);
                                var bMsg = new DiscordInteractionResponseBuilder()
                                    .EnableV2Components()
                                    .AddContainerComponent(cContainer);
                                await eventArgs.Interaction.CreateResponseAsync(
                                    DiscordInteractionResponseType.UpdateMessage,
                                    new DiscordInteractionResponseBuilder(bMsg));
                                break;
                            #endregion

                            #region HELP - doesn't get fired at all because we don't have a [Help] Button
                            case "helpId":
                                unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                                bComponent =                                 
                                [
                                    new DiscordTextDisplayComponent(
                                        "Help is in development, the devs are hard at work implementing this feature!"),
                                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                                    new DiscordSectionComponent(new DiscordTextDisplayComponent("help keep GamedayTracker alive!"),
                                        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate")),
                                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                                    new DiscordTextDisplayComponent($"-# Gameday Tracker ©️ <t:{unixTimestamp}:F>")
                                ];
                                 cContainer =
                                    new DiscordContainerComponent(bComponent, false, DiscordColor.Goldenrod);
                                 bMsg = new DiscordInteractionResponseBuilder()
                                    .EnableV2Components()
                                    .AddContainerComponent(cContainer);
                                await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage, 
                                    new DiscordInteractionResponseBuilder(bMsg));
                                break;
                            #endregion

                            #region SETTINGS
                            case "settingsId":
                                unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                                DiscordButtonComponent[] btns =
                                [
                                        new DiscordButtonComponent(DiscordButtonStyle.Primary, "headlinesId", "Headlines"),
                                        new DiscordButtonComponent(DiscordButtonStyle.Primary, "realtimeScoresId", "Real Time Scores")
                                ];
                                bComponent =
                                [
                                    new DiscordTextDisplayComponent(
                                        "Settings is in development, the devs are hard at work implementing this feature!"),
                                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Powered by Gameday Tracker ©️ <t:{unixTimestamp}:F>"),
                                        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate")),
                                ];
                                 cContainer =
                                    new DiscordContainerComponent(bComponent, false, DiscordColor.Goldenrod);
                                 bMsg = new DiscordInteractionResponseBuilder()
                                    .EnableV2Components()
                                    .AddContainerComponent(cContainer)
                                    .AddActionRowComponent(btns);
                                await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage,
                                    new DiscordInteractionResponseBuilder(bMsg));
                                break;
                            #endregion

                            #region STANDINGS -DONE
                            case "standingsHelpBtn":
                                unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                                var nowTimestamp = DateTimeOffset.UtcNow.Humanize();
                                bComponent =
                                [
                                    new DiscordTextDisplayComponent("Get's the current divisional standings\r\ncommand: ``/standings``"),
                                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Powered by Gameday Tracker ©️ <t:{unixTimestamp}:R>"),
                                        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate")),
                                    new DiscordActionRowComponent(backBtns)
                                ];
                                 cContainer =
                                    new DiscordContainerComponent(bComponent, false, DiscordColor.DarkGray);
                                 bMsg = new DiscordInteractionResponseBuilder()
                                    .EnableV2Components()
                                    .AddContainerComponent(cContainer);
                                await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage,
                                    new DiscordInteractionResponseBuilder(bMsg));
                                break;
                            #endregion

                            #region DRAFT
                            case "draftHelpBtn":
                                unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                                bComponent =
                                [
                                    new DiscordTextDisplayComponent(
                                        "Draft is in development, the devs are hard at work implementing this feature!"),
                                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Powered by Gameday Tracker ©️ <t:{unixTimestamp}:F>"),
                                        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate")),
                                    new DiscordActionRowComponent(backBtns)
                                ];
                                 cContainer =
                                    new DiscordContainerComponent(bComponent, false, DiscordColor.DarkGray);
                                 bMsg = new DiscordInteractionResponseBuilder()
                                    .EnableV2Components()
                                    .AddContainerComponent(cContainer);
                                await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage,
                                    new DiscordInteractionResponseBuilder(bMsg));
                                break;
                            #endregion

                            #region USER SETTINGS
                            case "userSettingsHelpBtn":
                                unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                                bComponent =
                                [
                                    new DiscordTextDisplayComponent(
                                        "User Settings is in development, the devs are hard at work implementing this feature!"),
                                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Powered by Gameday Tracker ©️ <t:{unixTimestamp}:F>"),
                                        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate")),
                                    new DiscordActionRowComponent(backBtns)
                                ];
                                 cContainer =
                                    new DiscordContainerComponent(bComponent, false, DiscordColor.DarkGray);
                                 bMsg = new DiscordInteractionResponseBuilder()
                                    .EnableV2Components()
                                    .AddContainerComponent(cContainer);
                                await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage,
                                    new DiscordInteractionResponseBuilder(bMsg));
                                break;
                            #endregion

                            #region LIVE FEEDS - DONE
                            case "liveFeedsBtn":
                                unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                                bComponent =
                                [
                                    new DiscordTextDisplayComponent("## Live Feeds Help"),
                                    new DiscordSeparatorComponent(true),
                                    new DiscordTextDisplayComponent("Get real-time updates on your favorite teams directly in Discord."),
                                    new DiscordSeparatorComponent(true),
                                    new DiscordTextDisplayComponent("- Realtime Scores: get up to the minute game scores\r\n- Daily Headlines: get daily NFL news articles.\r\n- Daily Standings: " +
                                    "get daily divisional standings"),
                                    new DiscordTextDisplayComponent("### More Info\r\nrun command ``/live-feeds``"),
                                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Powered by Gameday Tracker ©️ <t:{unixTimestamp}:F>"),
                                        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate")),
                                    new DiscordActionRowComponent(backBtns)
                                ];
                                 cContainer =
                                    new DiscordContainerComponent(bComponent, false, DiscordColor.DarkGray);
                                 bMsg = new DiscordInteractionResponseBuilder()
                                    .EnableV2Components()
                                    .AddContainerComponent(cContainer);
                                await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage,
                                    new DiscordInteractionResponseBuilder(bMsg));
                                break;
                            #endregion

                            #region NEWS
                            case "newsHelpBtn":
                                unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                                bComponent =
                                [
                                    new DiscordTextDisplayComponent(
                                        "News is in development, the devs are hard at work implementing this feature!"),
                                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Powered by Gameday Tracker ©️ <t:{unixTimestamp}:F>"),
                                        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate")),
                                    new DiscordActionRowComponent(backBtns)
                                ];
                                 cContainer =
                                    new DiscordContainerComponent(bComponent, false, DiscordColor.DarkGray);
                                 bMsg = new DiscordInteractionResponseBuilder()
                                    .EnableV2Components()
                                    .AddContainerComponent(cContainer);
                                await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage,
                                    new DiscordInteractionResponseBuilder(bMsg));
                                break;
                            #endregion

                            #region COMMANDS HELP
                            case "commandsBtn":
                                unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                                var cmdsDescBuilder = new StringBuilder();
                                cmdsDescBuilder.AppendLine("GamedayTracker supports auto complete - start typing and I will auto complete the commands available.");
                                cmdsDescBuilder.AppendLine();
                                cmdsDescBuilder.AppendLine("### Utility Commands");
                                cmdsDescBuilder.AppendLine("``/help`` ``/about`` ``/ping``");
                                cmdsDescBuilder.AppendLine("### Bank Commands");
                                cmdsDescBuilder.AppendLine("``/daily`` ``/bet`` ``/leaderboard``");

                                bComponent =
                                [
                                    new DiscordTextDisplayComponent("## Commands Help"),
                                    new DiscordTextDisplayComponent("-# GamedayTracker uses `/` slash commands"),
                                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                                    new DiscordTextDisplayComponent(cmdsDescBuilder.ToString()),
                                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Powered by Gameday Tracker ©️ <t:{unixTimestamp}:R>"),
                                        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate")),
                                    new DiscordActionRowComponent(backBtns)
                                ];
                                 cContainer =
                                    new DiscordContainerComponent(bComponent, false, DiscordColor.DarkGray);
                                 bMsg = new DiscordInteractionResponseBuilder()
                                    .EnableV2Components()
                                    .AddContainerComponent(cContainer);
                                await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage,
                                    new DiscordInteractionResponseBuilder(bMsg));
                                break;
                            #endregion

                            #region BACK
                            case "backId":
                                
                                bComponent =
                                [
                                    new DiscordButtonComponent(DiscordButtonStyle.Secondary, "scoreboardHelpBtn", "Scoreboard"),
                                    new DiscordButtonComponent(DiscordButtonStyle.Secondary, "standingsHelpBtn", "Standings"),
                                    new DiscordButtonComponent(DiscordButtonStyle.Secondary, "draftHelpBtn", "Draft"),
                                    new DiscordButtonComponent(DiscordButtonStyle.Secondary, "userSettingsHelpBtn", "User Settings"),
                                    new DiscordButtonComponent(DiscordButtonStyle.Secondary, "newsHelpBtn", "News"),

                                ];
                                DiscordComponent[] buttons2 =
                                [
                                    new DiscordButtonComponent(DiscordButtonStyle.Secondary, "liveFeedsBtn", "Live Feeds"),
                                    new DiscordButtonComponent(DiscordButtonStyle.Secondary, "commandsBtn", "Commands Help"),
                                ];
                                bComponent =
                                [
                                    new DiscordTextDisplayComponent("Help Section"),
                                    new DiscordSeparatorComponent(true),
                                    new DiscordTextDisplayComponent("below is a list of buttons where you will select a button to get the desired help section."),
                                    new DiscordActionRowComponent(bComponent),
                                    new DiscordActionRowComponent(buttons2),
                                     new DiscordSeparatorComponent(true),
                                        new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Powered by Gameday Tracker ©️ <t:{unixTimestamp}:R>"),
                                            new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))

                                ];

                                cContainer = new DiscordContainerComponent(bComponent, false, DiscordColor.DarkGray);
                                bMsg = new DiscordInteractionResponseBuilder()
                                    .EnableV2Components()
                                    .AddContainerComponent(cContainer);
                                await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage,
                                    new DiscordInteractionResponseBuilder(bMsg));
                                break;
                            #endregion

                            #endregion

                            default:
                                {
                                    break;
                                }

                        }
                    break;
                }
                #endregion

                case DiscordInteractionType.Ping:
                    await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Ping is in development, the devs are hard at work implementing this feature!"));
                    break;
                case DiscordInteractionType.AutoComplete:
                    await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Auto Complete is in development, the devs are hard at work implementing this feature!"));
                    break;

                #region MODAL SUBMIT
                case DiscordInteractionType.ModalSubmit:
                    //modal submit
                    switch (eventArgs.Interaction.Data.CustomId)
                    {
                        case "modAddPlayer":
                            var playerName = eventArgs.Interaction.Data.TextInputComponents?[0].Value ?? "--";
                            var company = eventArgs.Interaction.Data.TextInputComponents?[1].Value ?? "--";
                            var balance = eventArgs.Interaction.Data.TextInputComponents?[2].Value ?? "0";
                            var idResult = await playerDataService.GeneratePlayerIdAsync();
                            var newPlayer = new PoolPlayer()
                            {
                                Id = idResult.Value,
                                PlayerId = playerDataService.GeneratePlayerIdentifier().Value,
                                PlayerName = playerName,
                                Company = company,
                                Balance = double.TryParse(balance, out var result) ? result : 0
                            };
                            var pResult = await playerDataService.WritePlayerToXmlAsync(newPlayer);

                            if (pResult.IsOk)
                            {
                                await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                                    new DiscordInteractionResponseBuilder().WithContent($"[**{playerName}**] Added Successfully!"));
                            }
                            await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                                new DiscordInteractionResponseBuilder().WithContent($"Add Player is in development, the devs are hard at work implementing this feature!\r\n{playerName}"));
                            break;
                    }
                   
                    
                    break;
                #endregion
                
                default:
                    await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Unknown command!"));
                    return;
            }
        }
    }
}
