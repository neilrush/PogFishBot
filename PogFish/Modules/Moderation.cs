using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PogFishInfrastructure;
// ReSharper disable UnusedMember.Global

namespace PogFish.Modules
{
    public class Moderation : ModuleBase<ICommandContext>
    {
        private readonly ILogger<Moderation> _logger;
        private readonly Servers _servers;
        private readonly IConfiguration _config;


        public Moderation(ILogger<Moderation> logger, Servers servers, IConfiguration config)
        {
            _logger = logger;
            _servers = servers;
            _config = config;
        }

        [Command("prefix")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Prefix(string prefix = null)
        {
            if (prefix == null)
            {
                prefix = await _servers.GetGuildPrefix(Context.Guild.Id) ?? _config["prefix"];
                var message = await Context.Channel.SendMessageAsync("Current command prefix is: " + prefix);
                DeleteMessageAfterTimeoutAsync(2500,message);
            }
            else
            {
                await _servers.ModifyGuildPrefix(Context.Guild.Id, prefix);
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

        #region utilities
        private async void DeleteMessageAfterTimeoutAsync(int millis, IMessage message)
        {
            await Task.Delay(millis);
            await (Context.Channel as ITextChannel).DeleteMessageAsync(message);
        }
        #endregion
        
    }
}
