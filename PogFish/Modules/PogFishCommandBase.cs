using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using PogFishInfrastructure;

namespace PogFish.Modules
{
    public class PogFishCommandBase: ModuleBase<ICommandContext>
    {
        protected readonly ILogger<PogFishCommandBase> Logger;
        protected readonly Servers Servers;

        protected PogFishCommandBase(ILogger<PogFishCommandBase> logger, Servers servers)
        {
            Logger = logger;
            Servers = servers;
        }
        protected async void DeleteMessageAfterTimeoutAsync(int millis, IMessage message)
        {
            await Task.Delay(millis);
            await (Context.Channel as ITextChannel).DeleteMessageAsync(message);
        }
    }
}
