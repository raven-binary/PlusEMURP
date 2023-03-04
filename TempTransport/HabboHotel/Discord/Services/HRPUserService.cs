using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;

using System.Data;
using Plus.Database.Interfaces;
using DSharpPlus.CommandsNext;

namespace Plus.HabboHotel.Discord.Services
{
    public class HRPUserService
    {

        private readonly DiscordClient _client;

        // We need to save the server and roles so we arent constantly getting them from discords api.
        private DiscordRole _onlineRole;
        private DiscordRole _offlineRole;
        private DiscordGuild _communityGuild;

        public HRPUserService(DiscordClient client)
        {
            // First we get our guild and the offline/online roles, they are saved in the config.ini file.
            _client = client;
            _communityGuild = client.GetGuildAsync(ulong.Parse(Plus.PlusEnvironment.GetConfig().data["discord.bot.communityguild"].ToString())).GetAwaiter().GetResult();
            _onlineRole = _communityGuild.GetRole(ulong.Parse(Plus.PlusEnvironment.GetConfig().data["discord.bot.onlinerole"].ToString()));
            _offlineRole = _communityGuild.GetRole(ulong.Parse(Plus.PlusEnvironment.GetConfig().data["discord.bot.offlinerole"].ToString()));
               
            // Next we create a periodic task that will check user's online status and update their roles/nicknames accordingly.
            // We dont want this to be too frequest or we can get "rate limited" which means discord wont accept requests from us for a cetrain ammount of time
            _ = client.CreatePeriodicTask(FetchOnlineUsers, TimeSpan.FromSeconds(10));

            // Success! The service is loaded.
            Plus.Core.Logging.WriteLine("[DISCORD] Loaded RP User Service");
        }

        public async Task FetchOnlineUsers()
        {

            try
            {
                // For all the online clients
                foreach (GameClients.GameClient client in Plus.PlusEnvironment.GetGame().GetClientManager().GetClients)
                {

                    // Get the habborp user.
                    Users.Habbo userhabbo = client.GetHabbo();
                    DiscordMember user;

                    if (userhabbo.DiscordId == 0)
                    {
                        continue;
                    }

                    // If the user ID is already in the cache we can fetch their discord account from there, if not we need to request it from the discord api.
                    user = await _communityGuild.GetMemberAsync(((ulong)client.GetHabbo().DiscordId));
           
                    
                    // Give the user the online role and remove the offline role.
                    await user.GrantRoleAsync(_onlineRole);
                    await user.RevokeRoleAsync(_offlineRole);
                    await user.ModifyAsync(x =>
                    {
                        x.Nickname = userhabbo.Username;
                    });

                }

            } catch (Exception ex)
            {
                // debug stuff uncomment if things arent working.
                //Plus.Core.Logging.WriteLine(ex.Message);
                //Plus.Core.Logging.WriteLine(ex.StackTrace);
            }

        }

        // This method will be used to remove the roles from a user whenever they log out.
        public async Task DisconnectUser(Users.Habbo userhabbo)
        {
            DiscordMember user;

            // Get the user
            user = await _communityGuild.GetMemberAsync(((ulong)userhabbo.DiscordId));
        
                
            // Remove the online role and give the offline role.
            await user.RevokeRoleAsync(_onlineRole);
            await user.GrantRoleAsync(_offlineRole);

        }
    
    }

}
