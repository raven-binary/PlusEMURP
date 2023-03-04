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
    class BetCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().CurrentRoomId == 200)
            {
                return true;
            }
            return false;
        }

        public string TypeCommand
        {
            get { return "casino"; }
        }

        public string Parameters
        {
            get { return "<usenname>"; }
        }

        public string Description
        {
            get { return "Starts a High/Low game"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length < 3)
            {
                Session.SendWhisper("Invalid syntax :bet <username> <amount>");
                return;
            }

            string Username = Params[1];
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper(Username + " could not be found");
                return;
            }

            if (TargetClient.GetHabbo().Id == Session.GetHabbo().Id)
                return;

            if (Session.GetHabbo().Credits < Convert.ToInt32(Params[2]))
            {
                Session.SendWhisper("You do not have that much money");
                return;
            }

            if (TargetClient.GetHabbo().Credits < Convert.ToInt32(Params[2]))
            {
                Session.SendWhisper("Your target don't have that much money");
                return;
            }

            if (Math.Abs(User.X - TargetUser.X) > 2 || Math.Abs(User.Y - TargetUser.Y) > 2)
            {
                Session.SendWhisper("You must next to the target");
                return;
            }

            if (Session.GetHabbo().CardsPlaying)
            {
                Session.SendWhisper("You are already in a bet");
                return;
            }

            if (TargetClient.GetHabbo().CardsPlaying)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " is already in a bet");
                return;
            }

            if (User.Item_On != 3254594)
            {
                Session.SendWhisper("You have to sit to a golden throne chair to start a bet");
                return;
            }

            if (TargetUser.Item_On != 3124)
            {
                Session.SendWhisper("Your target have to sit front of you");
                return;
            }

            if (TargetUser.Transaction != null || TargetUser.isTradingItems)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " already has a transaction in progress");
                return;
            }

            User.Say("wishes to bet " + PlusEnvironment.ConvertToPrice(Convert.ToInt32(Params[2])) + " dollars with " + TargetClient.GetHabbo().Username + " on cards");
            Session.GetHabbo().CardsOffer = Convert.ToInt32(Params[2]);
            TargetUser.Transaction = "cards:" + Session.GetHabbo().Username;
            TargetClient.GetHabbo().TransactionFrom = Session.GetHabbo().Username;
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction;<b>" + Session.GetHabbo().Username + "</b> wishes to bet <b>" + PlusEnvironment.ConvertToPrice(Convert.ToInt32(Params[2])) + "</b> dollars with you on cards;0");
        }
    }
}