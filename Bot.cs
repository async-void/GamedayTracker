using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using GamedayTracker.Models;
using GamedayTracker.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using ChalkDotNET;
using DSharpPlus.EventArgs;
using GamedayTracker.Interfaces;
using Serilog;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Processors.TextCommands.Parsing;
using GamedayTracker.SlashCommands;
using GamedayTracker.SlashCommands.Economy;
using GamedayTracker.SlashCommands.News;
using GamedayTracker.SlashCommands.Player;

namespace GamedayTracker
{
    public class Bot
    { 
        private DiscordClient? Client;
        public CommandsNextExtension? Commands { get; set; }
        public CommandsNextExtension? SlashCommands { get; set; }
        public static InteractivityExtension? Interactivity { get; set; }

        public async Task RunAsync()
        {
            
            var configService = new ConfigurationDataService();
            var token = configService.GetBotToken();
            var prefix = configService.GetBotPrefix();

            var dBuilder = DiscordClientBuilder.CreateDefault(token.Value, TextCommandProcessor.RequiredIntents | SlashCommandProcessor.RequiredIntents);

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

            dBuilder.ConfigureEventHandlers(
                m => m.HandleMessageCreated(async (s, e) =>
                {
                    if (e.Message.Author!.IsBot) return;
                }));

            var status = new DiscordActivity("Game-Day", DiscordActivityType.Watching);
            var client = dBuilder.Build();
            dBuilder.SetReconnectOnFatalGatewayErrors();
            await client.ConnectAsync(status, DiscordUserStatus.Online, DateTimeOffset.UtcNow);
            Console.WriteLine($"{Chalk.DarkGray("Connection Success!")}");
            await Task.Delay(-1);
        }
    }
}
