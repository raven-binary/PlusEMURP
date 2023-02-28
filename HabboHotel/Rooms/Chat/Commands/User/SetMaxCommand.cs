﻿using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    internal class SetMaxCommand : IChatCommand
    {
        public string PermissionRequired => "command_setmax";

        public string Parameters => "%value%";

        public string Description => "Set the visitor limit to the room.";

        public void Execute(GameClient session, Room room, string[] @params)
        {
            if (!room.CheckRights(session, true))
                return;

            if (@params.Length == 1)
            {
                session.SendWhisper("Please enter a value for the room visitor limit.");
                return;
            }

            if (int.TryParse(@params[1], out int maxAmount))
            {
                if (maxAmount == 0)
                {
                    maxAmount = 10;
                    session.SendWhisper("visitor amount too low, visitor amount has been set to 10.");
                }
                else if (maxAmount > 200 && !session.GetHabbo().GetPermissions().HasRight("override_command_setmax_limit"))
                {
                    maxAmount = 200;
                    session.SendWhisper("visitor amount too high for your rank, visitor amount has been set to 200.");
                }
                else
                    session.SendWhisper("visitor amount set to " + maxAmount + ".");

                room.UsersMax = maxAmount;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `rooms` SET `users_max` = " + maxAmount + " WHERE `id` = '" + room.Id + "' LIMIT 1");
                }
            }
            else
                session.SendWhisper("Invalid amount, please enter a valid number.");
        }
    }
}