using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using GamedayTracker.AutoCompleteProvider;
using GamedayTracker.Enums;
using GamedayTracker.Helpers;
using GamedayTracker.Models.Tickets;
using GamedayTracker.Records;
using System.ComponentModel;
using System.Net.Sockets;
using static System.Net.WebRequestMethods;


namespace GamedayTracker.SlashCommands.Tickets
{
    public class TicketSlashCommands(TicketCoordinator ticketCoordinator)
    {
        // ticket command
        // This command will create a new ticket.
        // first we need the ticket type ie. support, bug report, etc.
        // Then we need the ticket description.
        // Finally we need to create the ticket and send a confirmation message to the user.
        [Command("ticket-create")]
        [Description("Creates a new ticket for support.")]
        public async ValueTask CreateTicketAsync(SlashCommandContext ctx, [SlashAutoCompleteProvider<TicketTypeAutoCompleteProvider>] TicketType ticketType,
            [Parameter("description")] string description)
        {
            await ctx.DeferResponseAsync(ephemeral: true);
            var ticketId = TicketIdGenerator.NextId();
            var ticketChannel = await ctx.Client.GetChannelAsync(1395133917840937190);
            var ticketTypeName = ticketType.ToString() ?? "support";
            var supportGuild = await ctx.Client.GetGuildAsync(1384428811805921301);
            var modChannel = await ctx.Client.GetChannelAsync(1398020926695538750);
            var threadMsg = await ticketChannel.SendMessageAsync($"ticket-{ticketId}-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}");
            var thread = await ticketChannel.CreateThreadAsync(threadMsg, $"{ticketTypeName}-{ctx.Member.Id}", DiscordAutoArchiveDuration.ThreeDays);
            var inviteLink = "https://discord.gg/7mKtap5p";
            var ticket = new Ticket
            {
                TicketId = ticketId,
                UserId = ctx.User.Id,
                GuildId = ctx.Guild!.Id,
                ThreadId = thread.Id,
                Type = ticketType,
                Description = description,
                Status = TicketStatus.Open,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var member = await supportGuild.GetMemberAsync(ctx.User.Id);

                await thread.SendMessageAsync($"New ticket created by {ctx.Member.DisplayName} ({ctx.User.Id})\nType: {ticketTypeName}\nDescription: {description}");
                await thread.SendMessageAsync($"a moderator will address this ticket in the order it was received, please be patient.");
                await Task.Delay(300);
                await modChannel.SendMessageAsync($"a new ticket has been created with ID: {ticket.TicketId}. <#{ticketChannel.Id}>");

                DiscordComponent[] comps =
                [
                    new DiscordTextDisplayComponent($"Your ticket has been created with ID: {ticket.TicketId}. <#{ticketChannel.Id}> A staff member will assist you shortly.")
                ];
                var container = new DiscordContainerComponent(comps, false, DiscordColor.Goldenrod);
                var msg = new DiscordFollowupMessageBuilder()
                            .EnableV2Components()
                            .AddContainerComponent(container)
                            .AsEphemeral(true);

                await ctx.FollowupAsync(msg);
            }
            catch (NotFoundException)
            {
                ticketCoordinator.AddPendingJoin(new PendingJoin(
                   UserId: ctx.User.Id,
                   ThreadId: thread.Id,
                   TicketId: ticket.TicketId,
                   CreatedAt: DateTimeOffset.UtcNow
               ));

                var linkBtn = new DiscordLinkButtonComponent(inviteLink, "support");
                await ctx.FollowupAsync(new DiscordFollowupMessageBuilder()
                    .WithContent("You must be a member of the support server to view tickets.")
                    .AddActionRowComponent(linkBtn)
                    .AsEphemeral(true));
                return;
            }
            
        }
    }
}
