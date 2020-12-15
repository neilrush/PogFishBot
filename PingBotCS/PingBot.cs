using System;
using System.Threading.Tasks;

namespace PingBotCS
{
    class PingBot
    {
        public static async Task Main(string[] args)
            => await Startup.RunAsync(args);
    }
}
