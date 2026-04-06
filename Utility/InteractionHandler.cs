using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using GamedayTracker.Cache;
using GamedayTracker.Enums;
using GamedayTracker.Extensions;
using GamedayTracker.Helpers;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Models.NFL;
using GamedayTracker.Pagination.Registry;
using GamedayTracker.Services;
using GamedayTracker.Utility.Multipliers;
using Humanizer;
using Microsoft.Extensions.Logging;
using System.Text;

namespace GamedayTracker.Utility
{
    public class InteractionHandler(ITeamData teamData, IPlayerData playerDataService, IDiscordEmbedService embedService, 
        IGameData gameService, IJsonDataService jsonDataService, ILogger<InteractionHandler> logger): IEventHandler<InteractionCreatedEventArgs>
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
                        var betMultiplier = new BetMultiplier();
                        var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                        logger.LogInformation("Component interaction received with CustomId: {CustomId}", eventArgs.Interaction.Data.CustomId);
                        DiscordComponent[] backBtns =
                        [
                            new DiscordButtonComponent(DiscordButtonStyle.Secondary, "backId", "⬅️ Back"),
                        ];

                        var id = eventArgs.Interaction.Data.CustomId;

                        #region BETTING AWAY/HOME/OVER/UNDER
                        if (id.StartsWith("betting_away"))
                        {
                            await eventArgs.Interaction.DeferAsync();
                            var details = eventArgs.Interaction.Data.CustomId.Split(",");
                            var teamName = details[1];
                            var eventId = details[2];
                            var wagerAmount = details[3];
                            var gameDate = details[4];
                            var multiplier = betMultiplier.GetMultiplier(BetType.Moneyline);
                            var bet = new Bet
                            {
                                Selection = teamName,
                                EventId = $"{eventId}",
                                GameDate = DateTimeOffset.Parse(gameDate),
                                WagerAmount = decimal.Parse(wagerAmount),
                                PlacedAt = DateTime.UtcNow,
                                UserId = eventArgs.Interaction.User.Id,
                                Type = BetType.Moneyline,
                                Multiplier = multiplier,
                                Payout = decimal.Parse(wagerAmount) + multiplier,
                                Status = BetStatus.Pending,
                                Id = Guid.NewGuid()
                            };

                            var betResult = await jsonDataService.WriteMemberBetToJsonAsync(eventArgs.Interaction.User.Id, eventArgs.Interaction.Guild.Id,  bet);
                            if (betResult.IsOk)
                            {
                                var msg = await embedService.BuildBettingResultEmbed(bet);
                                //here we save the bet to the json file, we can also add a confirmation message to the user that their bet was placed successfully
                                var builder = new DiscordWebhookBuilder(new DiscordMessageBuilder()
                                                .EnableV2Components()
                                                .AddContainerComponent(msg));

                                await eventArgs.Interaction.EditOriginalResponseAsync(builder);
                            }
                            else
                            {
                                var errContainer = await embedService.BuildErrorContainer(sender, $"{betResult.Error.ErrorMessage}", eventArgs.Interaction.Guild.Id, DiscordColor.DarkRed);
                                var builder = new DiscordMessageBuilder()
                                           .EnableV2Components()
                                           .AddContainerComponent(errContainer);
                                await eventArgs.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder(builder));
                                var logChannel = await sender.GetChannelAsync(1384436855524692048);
                                await logChannel.SendMessageAsync(builder);
                            }
                            

                        }
                        else if (id.StartsWith("betting_home"))
                        {
                            await eventArgs.Interaction.DeferAsync();
                            var details = eventArgs.Interaction.Data.CustomId.Split(":");
                            var teamName = details[1];
                            var eventId = details[2];
                            var wagerAmount = details[3];
                            var multiplier = betMultiplier.GetMultiplier(BetType.Moneyline);
                            var bet = new Bet
                            {
                                Selection = teamName,
                                EventId = $"{eventId}",
                                WagerAmount = decimal.Parse(wagerAmount),
                                Multiplier = multiplier,
                                PlacedAt = DateTime.UtcNow,
                                UserId = eventArgs.Interaction.User.Id,
                                Type = BetType.Moneyline,
                                Payout = decimal.Parse(wagerAmount) + multiplier,
                                Status = BetStatus.Pending,
                                Id = Guid.NewGuid()
                            };

                            var msg = await embedService.BuildBettingResultEmbed(bet);
                            //here we save the bet to the json file, we can also add a confirmation message to the user that their bet was placed successfully
                            var builder = new DiscordWebhookBuilder(new DiscordMessageBuilder()
                                            .EnableV2Components()
                                            .AddContainerComponent(msg));


                            await eventArgs.Interaction.EditOriginalResponseAsync(builder);
                        }
                        else if (id.StartsWith("betting_over"))
                        {
                            await eventArgs.Interaction.DeferAsync();
                            //var details = eventArgs.Interaction.Data.CustomId.Split(":");
                            //var teamName = details[1];
                            //var eventId = details[2];
                            //var wagerAmount = details[3];
                            //var multiplier = betMultiplier.GetMultiplier(BetType.OverUnder);
                            //var bet = new Bet
                            //{
                            //    Selection = teamName,
                            //    EventId = $"{eventId}",
                            //    WagerAmount = decimal.Parse(wagerAmount),
                            //    PlacedAt = DateTime.UtcNow,
                            //    UserId = eventArgs.Interaction.User.Id,
                            //    Type = BetType.OverUnder,
                            //    Payout = decimal.Parse(wagerAmount) * multiplier,
                            //    Status = BetStatus.Pending,
                            //    Id = Guid.NewGuid()
                            //};
                            //var msg = await embedService.BuildBettingResultEmbed(bet);
                            ////here we save the bet to the json file, we can also add a confirmation message to the user that their bet was placed successfully
                            //var builder = new DiscordWebhookBuilder(new DiscordMessageBuilder()
                            //                .EnableV2Components()
                            //                .AddContainerComponent(msg));
                            await eventArgs.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                                .AddEmbed(new DiscordEmbedBuilder()
                                    .WithTitle("Over/Under Betting is currently unavailable")
                                    .WithDescription("The Over/Under betting option is currently unavailable as we are working on integrating a new odds provider. We appreciate your patience and understanding as we work to bring you the best betting experience possible.")
                                    .WithColor(DiscordColor.Red)
                                    .Build()));
                        }
                        else if (id.StartsWith("betting_under"))
                        {
                            await eventArgs.Interaction.DeferAsync();
                            //var details = eventArgs.Interaction.Data.CustomId.Split(":");
                            //var teamName = details[1];
                            //var eventId = details[2];
                            //var wagerAmount = details[3];
                            //var multiplier = betMultiplier.GetMultiplier(BetType.OverUnder);
                            //var bet = new Bet
                            //{
                            //    Selection = teamName,
                            //    EventId = $"{eventId}",
                            //    WagerAmount = decimal.Parse(wagerAmount),
                            //    PlacedAt = DateTime.UtcNow,
                            //    UserId = eventArgs.Interaction.User.Id,
                            //    Type = BetType.OverUnder,
                            //    Payout = decimal.Parse(wagerAmount) * multiplier,
                            //    Status = BetStatus.Pending,
                            //    Id = Guid.NewGuid()
                            //};
                            //var msg = await embedService.BuildBettingResultEmbed(bet);
                            ////here we save the bet to the json file, we can also add a confirmation message to the user that their bet was placed successfully
                            //var builder = new DiscordWebhookBuilder(new DiscordMessageBuilder()
                            //                .EnableV2Components()
                            //                .AddContainerComponent(msg));
                            await eventArgs.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                               .AddEmbed(new DiscordEmbedBuilder()
                                   .WithTitle("Over/Under Betting is currently unavailable")
                                   .WithDescription("The Over/Under betting option is currently unavailable as we are working on integrating a new odds provider. We appreciate your patience and understanding as we work to bring you the best betting experience possible.")
                                   .WithColor(DiscordColor.Red)
                                   .Build()));
                        }
                        #endregion

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

                            #region COMMANDS HELP - DONE
                            case "commandsBtn":
                                unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                                var cmdsDescBuilder = SlashCommandHelper.BuildCommandsDescription();
                                
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

                            #region PREVIOUS
                            case "prev":
                                var envelope = PaginationCache.Get(eventArgs.Interaction.Message.Id);
                               
                                if (envelope == null)
                                {
                                    await Expired(eventArgs.Interaction);
                                    return;
                                }

                                var data = envelope.Data;
                                var handler = PaginationHandlerRegistry.GetHandler(data.GetType());
                                if (handler is null)
                                {
                                    await eventArgs.Interaction.CreateFollowupMessageAsync(
                                        new DiscordFollowupMessageBuilder()
                                            .WithContent("Unknown pagination type."));
                                    return;
                                }

                                await handler.HandlePreviousAsync(envelope.Data, eventArgs);
                                break;
                            #endregion

                            #region NEXT
                            case "next":
                                    envelope = PaginationCache.Get(eventArgs.Interaction.Message.Id);
                               
                                    if (envelope == null)
                                    {
                                        await Expired(eventArgs.Interaction);
                                        return;
                                    }

                                    data = envelope.Data;
                                    handler = PaginationHandlerRegistry.GetHandler(data.GetType());
                                    if (handler is null)
                                    {
                                        await eventArgs.Interaction.CreateFollowupMessageAsync(
                                            new DiscordFollowupMessageBuilder()
                                                .WithContent("Unknown pagination type."));
                                        return;
                                    }

                                   await handler.HandleNextAsync(envelope.Data, eventArgs);

                                break;
                            #endregion

                            #endregion

                            #region BETTING
                            case "betting":
                                var betDetails = eventArgs.Interaction.Data.Values[0].Split(":");
                                var pickedGame = await gameService.GetScoreboardByEventId(betDetails[1]);
                                var gameDate = pickedGame.Events[0].Date;
                                var betAmount = betDetails[2];
                                var odds = pickedGame.Events[0].Odds ?? [new() { Moneyline = new Moneyline() { Away = 0, Home = 0 } }];
                                var bettingMsg = await embedService.BuildBettingEmbed(betDetails[0], betAmount); 
                               
                                bettingMsg.AddActionRowComponent(new DiscordActionRowComponent(CreateBettingButtons($"{betDetails[0]},{betDetails[1]},{betDetails[2]}", gameDate)));
                                await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage,
                                       new DiscordInteractionResponseBuilder(bettingMsg));
                                break;
                           
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
                    // await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Auto Complete is in development, the devs are hard at work implementing this feature!"));
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

        #region BUTTON COMPONENT CREATORS

        private DiscordComponent[] CreateBettingButtons(string gameData, DateTime gameDate)
        {
            var gameInfo = gameData.Split(",");
            var teamNames = gameInfo[0].Split("at");
            return
            [
                new DiscordButtonComponent(
                    DiscordButtonStyle.Primary,
                    $"betting_away,{teamNames[0]},{gameInfo[1]},{gameInfo[2]},{gameDate}",
                    $"{teamNames[0].Trim()}"),

                new DiscordButtonComponent(
                    DiscordButtonStyle.Primary,
                    $"betting_home,{teamNames[1]},{gameInfo[1]},{gameInfo[2]},{gameDate}",
                    $"{teamNames[1].Trim()}"),

                new DiscordButtonComponent(
                    DiscordButtonStyle.Primary,
                    $"betting_over",
                    $"Over"),

                new DiscordButtonComponent(
                    DiscordButtonStyle.Primary,
                    $"betting_under",
                    $"Under"),
            ];
        }
        #endregion

        #region EXPIRED
        private async Task Expired(DiscordInteraction interaction)
        {
            await interaction.CreateFollowupMessageAsync(
                new DiscordFollowupMessageBuilder()
                    .WithContent("Pagination expired. Please run the command again."));
        }
        #endregion
        }
    }
