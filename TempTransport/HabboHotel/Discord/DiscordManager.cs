using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Extensions.Logging;

using Microsoft.Extensions.DependencyInjection;

using System.Threading.Tasks;
using DSharpPlus;
using Plus.HabboHotel.Discord.Services;

namespace Plus.HabboHotel.Discord
{
    public class DiscordManager
    {

        private static IServiceProvider _services;

        // Allows the rest of the emulator to access the discord bot and it's services.
        public IServiceProvider GetDiscordServices()
        {
            return _services;
        }

       public DiscordManager()
        {
            BotClient().GetAwaiter().GetResult();
        }

        static async Task BotClient()
        {  

            // Configure the discord client first
            _services = ConfigureClient(PlusEnvironment.GetConfig().data["discord.bot.token"]);

            // In order to enable a service we have to use the GetRequiredService method
            _services.GetRequiredService<CommandHandler>();
            _services.GetRequiredService<HRPUserService>();

            // ENABLE NEW SERVICES HERE
            //  _services.GetRequiredService<ServiceClass>();
            // LIKE SO ^%^

            // These services need to be put into variables so we can use further commands
            var discordClient = _services.GetRequiredService<DiscordClient>();
            
            // Connect to discord
            await discordClient.ConnectAsync();

            // setup our ready event
            discordClient.Ready += (async (c, e) =>
            {

                Plus.Core.Logging.WriteLine($"[DISCORD] Logged in as {discordClient.CurrentUser.Username}");
                Plus.Core.Logging.WriteLine("[DISCORD] Discord Bot is Ready!");

            });

            // Infinite delay ensures the bot runs for the programs lifecycle.
            //await Task.Delay(-1);
        }

        // Method to configure all the services and modules that we will add to our discord client
        static IServiceProvider ConfigureClient(String token)
        {
            // Setting up the discord config
            var discordConfig = new DiscordConfiguration()
            {
                Token = token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All,
                // Change to debug if any errors occur!!
                // MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.None
            };

            // Here is where we create our discord bot client
            var discordClient = new DiscordClient(discordConfig);

            // Here is where we create our service collection
            //
            // Everything here can be accessed by the bot and it's modules
            var services = new ServiceCollection()
                .AddSingleton<DiscordClient>(discordClient)
                // ADD NEW SERVICES HERE!!
                //  .AddSingleton<ServiceClass>()
                // LIKE SO ^^ You also need to start the services above in the BotClient Method
                .AddSingleton<CommandHandler>()
                .AddSingleton<HRPUserService>()
                .BuildServiceProvider();

            return services;

        }

   
        
    }
}
