using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;

using Plus.HabboHotel.Discord.Modules;

namespace Plus.HabboHotel.Discord.Services
{
    
    public class CommandHandler
    {

        private readonly DiscordClient _client;
        private readonly IServiceProvider _services;

        public CommandHandler(DiscordClient client, IServiceProvider services)
        {
            
            // Fetch our client and the services provider to use in commandsnext
            _client = client;
            _services = services;

            // configure commandsNext
            var commandsNextConfig = new CommandsNextConfiguration()
            {
                Services = services,
                StringPrefixes = new String[] { Plus.PlusEnvironment.GetConfig().data["discord.bot.prefix"] },
                EnableMentionPrefix = true

            };

            var cmdsNext = _client.UseCommandsNext(commandsNextConfig);

            // Register our modules
            cmdsNext.RegisterCommands<Admin>();

            // ADD NEW MODULES HERE
            //  cmdsNext.RegisterCommands<ModuleClass>();
            // LIKE SO ^^ 

            Plus.Core.Logging.WriteLine("[DISCORD] Command Handler Loaded!");

        }

    }
}
