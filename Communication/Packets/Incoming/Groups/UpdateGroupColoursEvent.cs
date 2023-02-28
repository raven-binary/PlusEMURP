﻿using System;
using System.Linq;
using Plus.Communication.Packets.Outgoing.Groups;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.Items;

namespace Plus.Communication.Packets.Incoming.Groups
{
    internal class UpdateGroupColoursEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int groupId = packet.PopInt();
            int mainColour = packet.PopInt();
            int secondaryColour = packet.PopInt();

            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out Group group))
                return;

            if (group.CreatorId != session.GetHabbo().Id)
                return;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `groups` SET `colour1` = @colour1, `colour2` = @colour2 WHERE `id` = @groupId LIMIT 1");
                dbClient.AddParameter("colour1", mainColour);
                dbClient.AddParameter("colour2", secondaryColour);
                dbClient.AddParameter("groupId", group.Id);
                dbClient.RunQuery();
            }

            group.Colour1 = mainColour;
            group.Colour2 = secondaryColour;

            session.SendPacket(new GroupInfoComposer(group, session));
            if (session.GetHabbo().CurrentRoom != null)
            {
                foreach (Item item in session.GetHabbo().CurrentRoom.GetRoomItemHandler().GetFloor.ToList())
                {
                    if (item == null || item.GetBaseItem() == null)
                        continue;

                    if (item.GetBaseItem().InteractionType != InteractionType.GuildItem && item.GetBaseItem().InteractionType != InteractionType.GuildGate || item.GetBaseItem().InteractionType != InteractionType.GuildForum)
                        continue;

                    session.GetHabbo().CurrentRoom.SendPacket(new ObjectUpdateComposer(item, Convert.ToInt32(item.UserId)));
                }
            }
        }
    }
}