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
        private Embed CreateInfoEmbed(ulong id, DateTimeOffset createdAt, DateTimeOffset joinedAt, string avatarUrl, IEnumerable<SocketRole> roles)
        {
            return new EmbedBuilder()
                .WithThumbnailUrl(avatarUrl)
                .WithDescription("In this message you can see some information about yourself!")
                .WithColor(new Color(33, 176, 252))
                .AddField("User ID", id, true)
                .AddField("Created at", createdAt.ToString("MM/dd/yyyy"), true)
                .AddField("Joined at", joinedAt.ToString("MM/dd/yyyy"), true)
                .AddField("Roles", string.Join(" ", roles.Select(x => x.Mention)))
                .WithCurrentTimestamp()
                .Build();
        }

        [Command("ping")]
        public async Task Ping()
        {
            await Context.Channel.SendMessageAsync("Pong!");
        }

        [Command("info")]
        public async Task Info(SocketGuildUser user = null)
        {
            Embed embed;

            if (user == null)
            {
                user = Context.User as SocketGuildUser;
            }

            embed = CreateInfoEmbed(user.Id, user.CreatedAt, user.JoinedAt.Value, user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl(), user.Roles);
            await Context.Channel.SendMessageAsync(null, false, embed);
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
        public async Task Roulette() {
            await Context.Guild.DownloadUsersAsync();
            var message = await Context.Channel.SendMessageAsync("Kicking random user (who will it be :O)...");
            await Task.Delay(1000);

            await message.DeleteAsync();
            var voiceChannels = await Context.Guild.GetVoiceChannelsAsync();

            var userList = voiceChannels
                .Select(async channel => await channel.GetUsersAsync().FlattenAsync())
                .Select(t => t.Result)
                .Aggregate(new List<IGuildUser>(), (acc, rhs) => acc.Concat(rhs).ToList());

            Random randomNumber = new Random();
            IGuildUser unluckyUser = userList[randomNumber.Next(userList.Count)];
            await unluckyUser.ModifyAsync(x => x.Channel = null);
            message = await Context.Channel.SendMessageAsync($"{unluckyUser.Nickname} has been chosen...");
            await Task.Delay(1000);
            await message.DeleteAsync();
        }
    }
}
