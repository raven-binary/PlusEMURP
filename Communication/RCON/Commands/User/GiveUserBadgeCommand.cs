﻿using System;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Rcon.Commands.User
{
    internal class GiveUserBadgeCommand : IRconCommand
    {
        public string Description => "This command is used to give a user a badge.";

        public string Parameters => "%userId% %badgeId%";

        public bool TryExecute(string[] parameters)
        {
            if (!int.TryParse(parameters[0], out int userId))
                return false;

            GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(userId);
            if (client == null || client.GetHabbo() == null)
                return false;

            // Validate the badge
            if (string.IsNullOrEmpty(Convert.ToString(parameters[1])))
                return false;

            string badge = Convert.ToString(parameters[1]);

            if (!client.GetHabbo().GetBadgeComponent().HasBadge(badge))
            {
                client.GetHabbo().GetBadgeComponent().GiveBadge(badge, true, client);
                client.SendPacket(new BroadcastMessageAlertComposer("You have been given a new badge!"));
            }

            return true;
        }
    }
}