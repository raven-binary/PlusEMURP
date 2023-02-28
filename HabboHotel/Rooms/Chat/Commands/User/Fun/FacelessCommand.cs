﻿using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    internal class FacelessCommand : IChatCommand
    {
        public string PermissionRequired => "command_faceless";

        public string Parameters => "";

        public string Description => "Allows you to go faceless!";

        public void Execute(GameClient session, Room room, string[] @params)
        {
            RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
            if (user == null || user.GetClient() == null)
                return;

            string[] headParts;
            string[] figureParts = session.GetHabbo().Look.Split('.');
            foreach (string part in figureParts)
            {
                if (part.StartsWith("hd"))
                {
                    headParts = part.Split('-');
                    if (!headParts[1].Equals("99999"))
                        headParts[1] = "99999";
                    else
                        return;

                    session.GetHabbo().Look = session.GetHabbo().Look.Replace(part, "hd-" + headParts[1] + "-" + headParts[2]);
                    break;
                }
            }

            session.GetHabbo().Look = PlusEnvironment.GetFigureManager().ProcessFigure(session.GetHabbo().Look, session.GetHabbo().Gender, session.GetHabbo().GetClothing().GetClothingParts, true);

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `users` SET `look` = '" + session.GetHabbo().Look + "' WHERE `id` = '" + session.GetHabbo().Id + "' LIMIT 1");
            }

            session.SendPacket(new UserChangeComposer(user, true));
            session.GetHabbo().CurrentRoom.SendPacket(new UserChangeComposer(user, false));
        }
    }
}