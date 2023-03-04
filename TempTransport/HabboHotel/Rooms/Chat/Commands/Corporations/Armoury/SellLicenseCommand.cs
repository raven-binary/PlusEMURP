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
    class SellLicenseCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().JobId == 17 && Session.GetHabbo().Working == true)
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
            get { return "Sell ​​a gun license"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax. :selllicense <username>");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper(Username + " could not be found.");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

            if (Math.Abs(User.Y - TargetUser.Y) > 2 || Math.Abs(User.X - TargetUser.X) > 2)
            {
                Session.SendWhisper("You can't sell the license to " + TargetClient.GetHabbo().Username + " because it's too far away.");
                return;
            }

            if (TargetClient.GetHabbo().Permis_arme == 1)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " already has a transaction, wait a minute.");
                return;
            }

            if (TargetUser.Transaction != null || TargetUser.isTradingItems)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " already has a transaction in progress, wait a minute...");
                return;
            }

            User.GetClient().Shout("*Sells a gun license " + TargetClient.GetHabbo().Username + "*");
            TargetUser.Transaction = "behave:" + PlusEnvironment.getPriceOfItem("Permis port arme") + ":" + PlusEnvironment.getTaxeOfItem("Permis port arme");
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction;<b>" + Session.GetHabbo().Username + "</b> want to sell you a gun license</b> for <b>" + PlusEnvironment.getPriceOfItem("Permis port arme") + " sell $</b> excl. <b>" + PlusEnvironment.getTaxeOfItem("Permis port arme") + "</b> Steer.;" + PlusEnvironment.getPriceOfItem("Permis port arme"));
        }
    }
}