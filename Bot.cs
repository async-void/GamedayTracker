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

namespace GamedayTracker
{
    public class Bot
    { 
        private DiscordClient Client;
        public CommandsNextExtension? Commands { get; set; }
        public SlashCommandsExtension? SlashCommands { get; set; }
        public static InteractivityExtension? Interactivity { get; set; }

        public async Task RunAsync()
        {
            
            var configService = new ConfigurationDataService();
            var token = configService.GetBotToken();
            var prefix = configService.GetBotPrefix();

            var clientConfig = new DiscordConfiguration
            {
                Token = token.Value,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                AlwaysCacheMembers = true,
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
                Intents = DiscordIntents.All
            };

            Client = new DiscordClient(clientConfig);
            Client.Ready += OnClientReady;
            Client.MessageCreated += OnMessageCreated;

            var iteractivityConfig = new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(1),
                PollBehaviour = PollBehaviour.KeepEmojis,
                PaginationEmojis = new PaginationEmojis(),
                PaginationBehaviour = PaginationBehaviour.WrapAround,
                PaginationDeletion = PaginationDeletion.KeepEmojis
            };

            Client.UseInteractivity(iteractivityConfig);

            //add services here.
            var services = new ServiceCollection()
                .AddSingleton<IGameData, GameDataService>()
                .BuildServiceProvider();

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = [ prefix.Value ] ,
                EnableDms = true,
                EnableMentionPrefix = true,
                Services = services
            };

            this.Commands = Client.UseCommandsNext(commandsConfig);
            this.SlashCommands = Client.UseSlashCommands();
            RegisterCommands();
            RegisterSlashCommands();

            await Client.ConnectAsync(new DiscordActivity("Game-Day", ActivityType.Watching)).ConfigureAwait(false);
            await Task.Delay(-1).ConfigureAwait(false);
        }

        

        private void RegisterCommands() => Client.GetCommandsNext().RegisterCommands(Assembly.GetExecutingAssembly());
        private void RegisterSlashCommands() => Client.GetSlashCommands().RegisterCommands(Assembly.GetExecutingAssembly());


        #region GATEWAY EVENTS

        #region CLIENT READY
        private Task OnClientReady(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs args)
        {
            Console.WriteLine(Chalk.Yellow(
                $"{Chalk.DarkGray($"[{DateTime.UtcNow}]")} {Chalk.Yellow("Client Ready...")}\r\n{Chalk.DarkGray($"[{DateTime.UtcNow}]")} {Chalk.Yellow("Listening for events...")}"));
            return Task.CompletedTask;
        }
        #endregion

        #region MESSAGE CREATED
        private async Task OnMessageCreated(DiscordClient sender, MessageCreateEventArgs args)
        {
            if (!args.Message.Author.IsBot)
            {

            }
        }
        #endregion

        #endregion
    }
}
