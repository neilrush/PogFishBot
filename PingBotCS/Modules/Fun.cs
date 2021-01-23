using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingBotCS.Modules
{
    class Fun : ModuleBase
    {
        [Command("roulette")]
        [RequireUserPermission(GuildPermission.MoveMembers)]
        public async Task Roulette()
        {
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
