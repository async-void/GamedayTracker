﻿using DSharpPlus;
using DSharpPlus.Entities;
using GamedayTracker.Services;
using System.Reflection;
using System.Xml;
using ChalkDotNET;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Processors.TextCommands.Parsing;
using DSharpPlus.Interactivity.Extensions;

namespace GamedayTracker
{
    public class Bot
    { 
        public async Task RunAsync()
        {
            
            var configService = new ConfigurationDataService();
            var token = configService.GetBotToken();
            var prefix = configService.GetBotPrefix();

            var dBuilder = DiscordClientBuilder.CreateDefault(token.Value, TextCommandProcessor.RequiredIntents | SlashCommandProcessor.RequiredIntents);
            dBuilder.UseInteractivity();

            dBuilder.UseCommands ((IServiceProvider serviceProvider, CommandsExtension extension) =>
            {
                extension.AddCommands(Assembly.GetExecutingAssembly());

                TextCommandProcessor textCommandProcessor = new(new()
                {
                    PrefixResolver = new DefaultPrefixResolver(true, prefix.Value).ResolvePrefixAsync,
                });

                extension.AddProcessor(textCommandProcessor);
            }, new CommandsConfiguration()
            {
                RegisterDefaultCommandProcessors = true,
                DebugGuildId = 0,

            });

            #region EVENT HANDLERS

            dBuilder.ConfigureEventHandlers(
                m => m.HandleMessageCreated(async (s, e) =>
                    {
                        if (e.Message.Author!.IsBot) return;
                    })
                    .HandleChannelCreated(async (s, e) =>
                    {

                    })
                    .HandleGuildDownloadCompleted(async (e, s) =>
                    {
                        Console.WriteLine(
                            $"{Chalk.Yellow($"[{DateTimeOffset.UtcNow}]")} {Chalk.Yellow($"[Gameday Tracker]")} {Chalk.DarkBlue("[INFO]")} {Chalk.DarkGray("Guild Download Complete.")}");
                        Console.WriteLine(
                            $"{Chalk.Yellow($"[{DateTimeOffset.UtcNow}]")} {Chalk.Yellow($"[Gameday Tracker]")} {Chalk.DarkBlue("[INFO]")} {Chalk.DarkGray("Connection Success, Listening for events...")}");
                    })
                    .HandleSessionCreated(async (s, e) =>
                    {
                        Console.WriteLine(
                            $"{Chalk.Yellow($"[{DateTimeOffset.UtcNow}]")} {Chalk.Yellow($"[Gameday Tracker]")} {Chalk.DarkBlue("[INFO]")} {Chalk.DarkGray("shaking hands with discord...")}");
                        Console.WriteLine(
                            $"{Chalk.Yellow($"[{DateTimeOffset.UtcNow}]")} {Chalk.Yellow($"[Gameday Tracker]")} {Chalk.DarkBlue("[INFO]")} {Chalk.DarkGray("Session Started!")}");
                    })
                    .HandleComponentInteractionCreated(async (client, args) =>
                    {
                        await args.Interaction.DeferAsync();
                        var buttonContent = "";
                        if (args.Interaction.Data.CustomId == "scoreboardBtn")
                            buttonContent = "Scoreboard";

                        await args.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                            .WithContent($"Button {buttonContent} Clicked"));
                        // await args.Interaction.Channel.SendMessageAsync("button clicked");
                    }));

            #endregion

            var status = new DiscordActivity("Game-Day", DiscordActivityType.Watching);
            var client = dBuilder.Build();
            
            dBuilder.SetReconnectOnFatalGatewayErrors();
            await client.ConnectAsync(status, DiscordUserStatus.Online, DateTimeOffset.UtcNow);
           
            await Task.Delay(-1);
        }
    }
}
