using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus;
using Plus.Database.Interfaces;
using Plus.Database.Adapter;
using System.Data;
using Plus.HabboHotel.Discord.Services;

namespace Plus.HabboHotel.Discord.Modules
{
    // Command module for administrators.
    class Admin : BaseCommandModule
    {

        // Stop Command
        [Command("stop"),
         Description("Stops the habborp emulator (Must be restarted manually!!)"),
         // Permission requirement can be changed to anything you want.
         RequirePermissions(DSharpPlus.Permissions.Administrator)]
        public async Task StopCommand(CommandContext ctx)
        {
            // Sends a confirmation message and then shuts down the emulator
            await ctx.RespondAsync("Stopping emulator...");
            Plus.PlusEnvironment.PerformShutDown();
        }


        // Restart command
        [Command("restart"),
         Description("Restarts the habborp emulator"),
         // Permission requirement can be changed to anything you want.
         RequirePermissions(DSharpPlus.Permissions.Administrator)]
        public async Task RestartCommand(CommandContext ctx)
        {
            // Sends a confirmation message and then restarts the emulator
            await ctx.RespondAsync("Restart emulator...");
            Plus.PlusEnvironment.RestartServer();
        }

    }

}
