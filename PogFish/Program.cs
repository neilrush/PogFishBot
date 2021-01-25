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
                try
                {
                    await host.RunAsync();
                }
                catch (Microsoft.Extensions.Options.OptionsValidationException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine("Exception: " + ex.Message);
                    Console.WriteLine("Caused by invalid discord token. Are you sure your token was correct?");
                    Console.WriteLine("Deleting " + ConfigPath + "!");
                    //File.Delete(ConfigPath);
                    Console.ResetColor();
                }


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
                Console.WriteLine("Warning: Config file missing or invalid! Creating Default.");
                Console.Write("Please input your discord bot token: ");
                string token = Console.ReadLine();
                Console.Write("Please input the sql server address: ");
                string sqlServer = Console.ReadLine();
                Console.Write("Database name: ");
                string databaseName = Console.ReadLine();
                Console.Write("Username: ");
                string username = Console.ReadLine();
                Console.Write("Password: ");
                string password = Console.ReadLine();
                string defaultConfig = "{" +
                                       "\n  \"prefix\": \"!\"," +
                                       "\n  \"token\": \"" + token + "\"," +
                                       "\n  \"mysql\":" +
                                       "\n  {" +
                                       "\n    \"username\": \"" + username + "\"," +
                                       "\n    \"password\": \"" + password + "\"," +
                                       "\n    \"server\": \"" + sqlServer + "\"," +
                                       "\n    \"database\": \"" + databaseName + "\"" +
                                       "\n  }" +
                                       "\n}";
                File.WriteAllText(ConfigPath, defaultConfig);
                Path.GetFullPath(ConfigPath);
                Console.WriteLine("New config file at:\n " + Path.GetFullPath(ConfigPath));
                Console.ResetColor();
                GenerateConfig(config);
            }
        }
    }
}