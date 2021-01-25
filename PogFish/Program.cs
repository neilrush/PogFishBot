using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PogFish.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using PogFishInfrastructure;

namespace PogFish
{
    public static class Program
    {
        private const string ConfigPath = "appsettings.json";

        public static async Task Main()
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration(GenerateConfig)
                .ConfigureLogging(console =>
                {
                    console.AddConsole();
                    console.SetMinimumLevel(LogLevel.Debug); // Defines what kind of information should be logged (e.g. Debug, Information, Warning, Critical) adjust this to your liking
                })
                .ConfigureDiscordHost<DiscordSocketClient>((context, config) =>
                {
                    config.SocketConfig = new DiscordSocketConfig
                    {
                        LogLevel = LogSeverity.Verbose, // Defines what kind of information should be logged from the API (e.g. Verbose, Info, Warning, Critical) adjust this to your liking
                        AlwaysDownloadUsers = true,
                        MessageCacheSize = 200,
                    };
                    ValidateApiToken(context, config);
                }
                )
                .UseCommandService((context, config) =>
                {
                    config.CaseSensitiveCommands = false;
                    config.LogLevel = LogSeverity.Verbose;
                    config.DefaultRunMode = RunMode.Sync;
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddHostedService<CommandHandler>()
                        .AddDbContext<PogFishContext>()
                        .AddSingleton<Servers>();
                })
                .UseConsoleLifetime();

            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();


            }
        }

        private static void ValidateApiToken(HostBuilderContext context, DiscordHostConfiguration config)
        {
            config.Token = context.Configuration["token"];

        }

        private static void GenerateConfig(IConfigurationBuilder config)
        {
            try
            {
                var configuration = new ConfigurationBuilder()
                                        .SetBasePath(Directory.GetCurrentDirectory())
                                        .AddJsonFile(ConfigPath, false, true)
                                        .Build();

                config.AddConfiguration(configuration);
            }
            catch (FileNotFoundException)
            {
                Console.BackgroundColor = ConsoleColor.DarkYellow;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("Warning: Config file missing! Creating Default.");
                string defaultConfig = "{\n  \"prefix\": \"!\",\n  \"token\": \"yourToken\"\n}";
                File.WriteAllText(ConfigPath, defaultConfig);
                Path.GetFullPath(ConfigPath);
                Console.WriteLine("New config file at:\n " + Path.GetFullPath(ConfigPath));
                Console.ResetColor();
            }
        }
    }
}