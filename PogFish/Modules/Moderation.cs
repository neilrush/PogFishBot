using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PogFishInfrastructure;
// ReSharper disable UnusedMember.Global

namespace PogFish.Modules
{
    public class Moderation : PogFishCommandBase
    {
        [Command("prefix")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Prefix(string prefix = null)
        {
            if (prefix == null)
            {
                prefix = await Servers.GetGuildPrefix(Context.Guild.Id);
                var message = await Context.Channel.SendMessageAsync("Current command prefix is: " + prefix);
                DeleteMessageAfterTimeoutAsync(2500,message);
            }
            else
            {

            }

        }
        [Command("purge")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Purge(int amount)
        {
            var messages = (await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync()).ToList();
            await ((SocketTextChannel) Context.Channel).DeleteMessagesAsync(messages);

            var message = await Context.Channel.SendMessageAsync($"{messages.Count()} messages deleted successfully!");
            
        }

        public Moderation(ILogger<PogFishCommandBase> logger, Servers servers) : base(logger, servers)
        {
        }
    }
}
