﻿using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using Plus.Communication.Packets.Outgoing.Inventory.Furni;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items;

namespace Plus.Communication.Packets.Incoming.Inventory.Furni
{
    internal class RequestFurniInventoryEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            IEnumerable<Item> items = session.GetHabbo().GetInventoryComponent().GetWallAndFloor;

            int page = 0;
            int pages = ((items.Count() - 1) / 700) + 1;

            if (!items.Any())
            {
                session.SendPacket(new FurniListComposer(items.ToList(), 1, 0));
            }
            else
            {
                foreach (ICollection<Item> batch in items.Batch(700))
                {
                    session.SendPacket(new FurniListComposer(batch.ToList(), pages, page));

                    page++;
                }
            }
        }
    }
}