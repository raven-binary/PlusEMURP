using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class SellVoucherCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().JobId == 4 && Session.GetHabbo().Working == true)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "job"; }
        }

        public string Parameters
        {
            get { return "<username>"; }
        }

        public string Description
        {
            get { return "Sell ​​a Barber Voucher to a Citizen."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Session.GetHabbo().JobId != 4 || Session.GetHabbo().Working == false)
                return;

            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :sellvoucher <username>");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper(Username + " could not be found in this room.");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (User.ConnectedMetier == false)
            {
                Session.SendWhisper("You must connect to the barber network before you can sell a coupon.");
                return;
            }

            if (TargetUser.Transaction != null || TargetUser.isTradingItems)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " already has a transaction, wait a minute.");
                return;
            }

            if (TargetClient.GetHabbo().Confirmed == 1)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " doesn't have to buy a haircut because he's a citizen.");
                return;
            }

            if (TargetClient.GetHabbo().Coiffure > 9)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " halready has 10 hairstyles in stock.");
                return;
            }

            int Prix;
            int Taxe;
            Prix = PlusEnvironment.getPriceOfItem("Coiffure Homme");
            Taxe = PlusEnvironment.getTaxeOfItem("Coiffure Homme");
            User.OnChat(User.LastBubble, "* Sells a barber voucher " + TargetClient.GetHabbo().Username + " *", true);
            TargetUser.Transaction = "coiffure:" + Prix + ":" + Taxe;
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction;<b>" + Session.GetHabbo().Username + "</b> would like to sell you a hairdressing voucher</b> for <b>" + Prix + " $</b> excl. <b>" + Taxe + "</b> taxes.;" + Prix);
        }
    }
}