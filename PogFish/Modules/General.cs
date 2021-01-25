using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Audio;
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
    public class General : ModuleBase<ICommandContext>
    {
        private readonly ILogger<General> _logger;
        private readonly Servers _servers;
        private readonly IConfiguration _config;


        public General(ILogger<General> logger, Servers servers, IConfiguration config)
        {
            _logger = logger;
            _servers = servers;
            _config = config;
        }
        [Command("ping")]
        public async Task Ping()
        {
            await Context.Channel.SendMessageAsync("Pong!");
        }

        [Command("info")]
        public async Task Info(SocketGuildUser user = null)
        {
            if (user == null)
            {
                user = Context.User as SocketGuildUser;
            }

            var embed = new EmbedBuilder()
                .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                .WithDescription("In this message you can see some information about yourself!")
                .WithColor(new Color(33, 176, 252))
                .AddField("User ID", user.Id, true)
                .AddField("Created at", user.CreatedAt.ToString("MM/dd/yyyy"), true)
                .AddField("Joined at", user.JoinedAt?.ToString("MM/dd/yyyy"), true)
                .AddField("Roles", string.Join(" ", user.Roles.Select(x => x.Mention)))
                .WithCurrentTimestamp()
                .Build();
            await Context.Channel.SendMessageAsync(null, false, embed);
        }



        [Command("server")]
        public async Task Server()
        {
            var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .WithDescription("In this message you can find some nice information about the current server.")
                .WithTitle($"{Context.Guild.Name} Information")
                .WithColor(new Color(33, 176, 252))
                .AddField("Created at", Context.Guild.CreatedAt.ToString("MM/dd/yyyy"), true)
                .AddField("Member Count", ((SocketGuild) Context.Guild).MemberCount + " members", true)
                .AddField("Online users", ((SocketGuild) Context.Guild).Users.Count(x => x.Status != UserStatus.Offline) + " members", true);
            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, embed);
        }

    }
}
