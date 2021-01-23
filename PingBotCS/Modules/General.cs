using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingBotCS.Modules
{
    public class General : ModuleBase
    {
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
                var builder = new EmbedBuilder()
                    .WithThumbnailUrl(Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl())
                    .WithDescription("In this message you can see some information about yourself!")
                    .WithColor(new Color(33, 176, 252))
                    .AddField("User ID", Context.User.Id, true)
                    .AddField("Created at", Context.User.CreatedAt.ToString("MM/dd/yyyy"), true)
                    .AddField("Joined at", (Context.User as SocketGuildUser).JoinedAt.Value.ToString("MM/dd/yyyy"), true)
                    .AddField("Roles", string.Join(" ", (Context.User as SocketGuildUser).Roles.Select(x => x.Mention)))
                    .WithCurrentTimestamp();
                var embed = builder.Build();
                await Context.Channel.SendMessageAsync(null, false, embed);
            }
            else
            {
                var builder = new EmbedBuilder()
                    .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                    .WithDescription("In this message you can see some information about yourself!")
                    .WithColor(new Color(33, 176, 252))
                    .AddField("User ID", user.Id, true)
                    .AddField("Created at", user.CreatedAt.ToString("MM/dd/yyyy"), true)
                    .AddField("Joined at", user.JoinedAt.Value.ToString("MM/dd/yyyy"), true)
                    .AddField("Roles", string.Join(" ", user.Roles.Select(x => x.Mention)))
                    .WithCurrentTimestamp();
                var embed = builder.Build();
                await Context.Channel.SendMessageAsync(null, false, embed);
            }
        }

        [Command("purge")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Purge(int amount)
        {
            var messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);

            var message = await Context.Channel.SendMessageAsync($"{messages.Count()} messages deleted successfully!");
            await Task.Delay(2500);
            await message.DeleteAsync();
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
                .AddField("Membercount", (Context.Guild as SocketGuild).MemberCount + " members", true)
                .AddField("Online users", (Context.Guild as SocketGuild).Users.Where(x => x.Status != UserStatus.Offline).Count() + " members", true);
            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, embed);
        }
        [Command("roulette")]
        [RequireUserPermission(GuildPermission.MoveMembers)]
        public async Task Roulette()
        {
            await Context.Guild.DownloadUsersAsync();
            var message = await Context.Channel.SendMessageAsync("Kicking random user (who will it be :O)...");
            await Task.Delay(1000);
            List<IGuildUser> userList = new List<IGuildUser>();
            await message.DeleteAsync();
            var voiceChannels = await Context.Guild.GetVoiceChannelsAsync();
            foreach (IVoiceChannel channel in voiceChannels)
            {
                var users = await channel.GetUsersAsync().FlattenAsync();
                foreach (IGuildUser user in users)
                {
                    if (user != null)
                    {
                        userList.Add(user);
                    }
                }

            }

            Random randomNumber = new Random();
            IGuildUser unluckyUser = userList[randomNumber.Next(userList.Count)];
            await unluckyUser.ModifyAsync(x => x.Channel = null);
            message = await Context.Channel.SendMessageAsync($"{unluckyUser.Nickname} has been chosen...");
            await Task.Delay(1000);
            await message.DeleteAsync();
        }
    }
}
