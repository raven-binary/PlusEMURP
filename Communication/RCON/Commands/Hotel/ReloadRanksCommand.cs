﻿using System.Linq;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Rcon.Commands.Hotel
{
    internal class ReloadRanksCommand : IRconCommand
    {
        public string Description => "This command is used to reload user permissions.";

        public string Parameters => "";

        public bool TryExecute(string[] parameters)
        {
            PlusEnvironment.GetGame().GetPermissionManager().Init();

            foreach (GameClient client in PlusEnvironment.GetGame().GetClientManager().GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null || client.GetHabbo().GetPermissions() == null)
                    continue;

                client.GetHabbo().GetPermissions().Init(client.GetHabbo());
            }

            return true;
        }
    }
}