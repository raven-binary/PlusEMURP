using System.Collections.Generic;
using Plus.Communication.Packets.Outgoing.LandingView;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.LandingView.Promotions;

namespace Plus.Communication.Packets.Incoming.LandingView
{
    internal class GetPromoArticlesEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            ICollection<Promotion> landingPromotions = PlusEnvironment.GetGame().GetLandingManager().GetPromotionItems();

            session.SendPacket(new PromoArticlesComposer(landingPromotions));
        }
    }
}