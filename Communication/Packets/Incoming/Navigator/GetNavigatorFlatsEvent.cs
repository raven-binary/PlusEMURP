﻿using System.Collections.Generic;
using Plus.Communication.Packets.Outgoing.Navigator;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Navigator;

namespace Plus.Communication.Packets.Incoming.Navigator
{
    internal class GetNavigatorFlatsEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            ICollection<SearchResultList> categories = PlusEnvironment.GetGame().GetNavigator().GetEventCategories();

            session.SendPacket(new NavigatorFlatCatsComposer(categories));
        }
    }
}