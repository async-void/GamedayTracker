using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using GamedayTracker.Helpers;
using GamedayTracker.Models.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Handlers
{
    public class GuildMemberAddedEventHandler(TicketCoordinator ticketCoordinator) : IEventHandler<GuildMemberAddedEventArgs>
    {
        public async Task HandleEventAsync(DiscordClient sender, GuildMemberAddedEventArgs eventArgs)
        {
            //get support guild
            var supportGuild = await sender.GetGuildAsync(1384428811805921301);
            if (eventArgs.Guild.Id != supportGuild.Id)
                return;

            // Check if this user has a pending ticket join
            if (!ticketCoordinator.TryGetPendingJoin(eventArgs.Member.Id, out var pending) || pending is null)
                return;

            //assign role
            var role = await eventArgs.Guild.GetRoleAsync(1484981623316414514);
            if (role is not null)
            {
                await eventArgs.Member.GrantRoleAsync(role);
            }

           
            //add member to the thread
            var thread = await sender.GetChannelAsync(pending.ThreadId);
            if (thread is DiscordThreadChannel threadChannel)
            {
                // build thread variables
                await threadChannel.AddThreadMemberAsync(eventArgs.Member);
            }

            //cleanup
            ticketCoordinator.RemovePendingJoin(eventArgs.Member.Id);

        }
    }
}
