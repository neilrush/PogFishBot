using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PogFishInfrastructure;
// ReSharper disable UnusedMember.Global

namespace PogFish.Modules
{
    public class Fun : PogFishCommandBase
    {
        [Command("meme")]
        [Alias("reddit")]
        public async Task Meme(string subreddit = null)
        {
            var client = new HttpClient();
            var result = await client.GetStringAsync($"https://reddit.com/r/{subreddit ?? "dankmemes"}/random.json?limit=1?obey_over18=true");
            if (!result.StartsWith("["))
            {
                await Context.Channel.SendMessageAsync("This subreddit doesn't exist!");
                return;
            }
            JArray array = JArray.Parse(result);
            JObject post = JObject.Parse(array[0]["data"]["children"][0]["data"].ToString()); //strips off metadata from json data

            var embedBuilder = new EmbedBuilder()
                .WithImageUrl(post["url"].ToString())
                .WithColor(new Color(33, 176, 252))
                .WithTitle(post["title"].ToString())
                .WithUrl("https://reddit.com" + post["permalink"].ToString())
                .WithFooter($"🗨 {post["num_comments"]} ⬆ {post["ups"]}");
            var embed = embedBuilder.Build();
            await Context.Channel.SendMessageAsync(null, false, embed);

        }

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

        protected Fun(ILogger<PogFishCommandBase> logger, Servers servers) : base(logger, servers)
        {
        }
    }
}
