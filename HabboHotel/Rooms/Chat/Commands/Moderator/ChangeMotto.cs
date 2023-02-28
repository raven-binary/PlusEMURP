using Org.BouncyCastle.Bcpg;
using Plus.Communication.Packets.Outgoing.Handshake;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Quests;
using Plus.Utilities;
using System;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class ChangeMotto : IChatCommand
    {
        public string PermissionRequired => "command_change_motto";

        public string Parameters => "%username% %motto%";

        public string Description => "Changes specified users motto.";

        public void Execute(GameClient session, Room room, string[] @params)
        {
            if (@params.Length == 1)
            {
                session.SendWhisper("Please enter the target username and future motto.");
                return;
            }

            GameClient targetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(@params[1]);

            if (targetClient == null)
            {
                session.SendWhisper("An error occoured whilst finding that user, maybe they're not online.");
                return;
            }

            string newMotto = StringCharFilter.Escape(@params[2]);

            if (newMotto.Length > 38)
                newMotto = newMotto.Substring(0, 38);

            if (newMotto == targetClient.GetHabbo().Motto)
                return;

            newMotto = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(newMotto);

            targetClient.GetHabbo().Motto = newMotto;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `motto` = @motto WHERE `id` = @userId LIMIT 1");
                dbClient.AddParameter("userId", targetClient.GetHabbo().Id);
                dbClient.AddParameter("motto", newMotto);
                dbClient.RunQuery();
            }

            if (targetClient.GetHabbo().InRoom)
            {
                RoomUser user = room?.GetRoomUserManager().GetRoomUserByHabbo(targetClient.GetHabbo().Id);
                if (user == null || user.GetClient() == null)
                    return;

                room.SendPacket(new UserChangeComposer(user, false));
            }
            
            session.SendWhisper("Motto changed to " + newMotto);
        }
    }
}