using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class RenouvelerCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().CurrentRoom.OwnerId == Session.GetHabbo().Id && Session.GetHabbo().CurrentRoom.Loyer == 1)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "appart"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Renew your rent"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (Session.GetHabbo().getCooldown("renouveller_appart") || User.Transaction != null || User.isTradingItems)
            {
                Session.SendWhisper("Wait a minute...");
                return;
            }

            DateTime newDate = Session.GetHabbo().CurrentRoom.Loyer_Date.AddDays(-1);
            if (newDate > DateTime.Now)
            {
                Session.SendWhisper("You can extend the rental periods from " + newDate.Day + "/" + newDate.Month + " to " + newDate.Hour + ":" + newDate.Minute + " extend.");
                return;
            }
            

            int TargetRoomPrice = Session.GetHabbo().CurrentRoom.Prix_Vente;
            decimal TargetRoomPriceDecimal = Convert.ToDecimal(TargetRoomPrice);
            int TargetRoomTaxe = Convert.ToInt32((TargetRoomPriceDecimal / 100m) * 15m);
            User.GetClient().Shout("*Renews lease*");
            User.Transaction = "renouveller_appart:" + Session.GetHabbo().CurrentRoom.Id + ":" + TargetRoomPrice + ":" + TargetRoomTaxe;
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "transaction;Do you really want <b>your lease</b> for the property <b>[" + Session.GetHabbo().CurrentRoom.Id + "] " + Session.GetHabbo().CurrentRoom.Name + "</b> for <b>" + TargetRoomPrice + " $</b> plus <b>" + TargetRoomTaxe + "</b> extend taxes?;" + TargetRoomPrice);
        }
    }
}