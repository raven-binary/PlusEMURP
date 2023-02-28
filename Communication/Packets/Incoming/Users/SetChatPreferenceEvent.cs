﻿using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Users
{
    internal class SetChatPreferenceEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            bool preference = packet.PopBoolean();

            session.GetHabbo().ChatPreference = preference;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `chat_preference` = @chatPreference WHERE `id` = '" + session.GetHabbo().Id + "' LIMIT 1");
                dbClient.AddParameter("chatPreference", PlusEnvironment.BoolToEnum(preference));
                dbClient.RunQuery();
            }
        }
    }
}