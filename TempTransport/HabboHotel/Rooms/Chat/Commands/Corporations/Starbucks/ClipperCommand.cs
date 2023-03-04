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
    class ClipperCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().JobId == 5 && Session.GetHabbo().RankId == 2 && Session.GetHabbo().Working == true || Session.GetHabbo().JobId == 5 && Session.GetHabbo().RankId == 4 && Session.GetHabbo().Working == true || Session.GetHabbo().JobId == 5 && Session.GetHabbo().RankId == 3 && Session.GetHabbo().Working == true)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "job"; }
        }

        public string Parameters
        {
            get { return "<pseudonym>"; }
        }

        public string Description
        {
            get { return "Sell ​​a hair clippers to a citizen."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :clipper <username>");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper(Username + " could not be found in this space.");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

            if (Math.Abs(User.Y - TargetUser.Y) > 2 || Math.Abs(User.X - TargetUser.X) > 2)
            {
                Session.SendWhisper("You can't " + TargetClient.GetHabbo().Username + " sell clippers because it's too far.");
                return;
            }

            if ((TargetClient.GetHabbo().Clipper + 50) > 100)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " already has 2 clippers.");
                return;
            }

            if (TargetUser.Transaction != null || TargetUser.isTradingItems)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " already has a transaction, wait a minute.");
                return;
            }

            User.GetClient().Shout("*Selling a lighter " + TargetClient.GetHabbo().Username + "*");
            TargetUser.Transaction = "clipper:" + PlusEnvironment.getPriceOfItem("Clipper") + ":" + PlusEnvironment.getTaxeOfItem("Clipper");
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction;<b>" + Session.GetHabbo().Username + "</b> want to sell you a <b>hair clipper</b> for <b>" + PlusEnvironment.getPriceOfItem("Clipper") + " $</b>, of which <b>" + PlusEnvironment.getTaxeOfItem("Clipper") + "</b> go to the state.;" + PlusEnvironment.getPriceOfItem("Clipper"));
        }
    }
}