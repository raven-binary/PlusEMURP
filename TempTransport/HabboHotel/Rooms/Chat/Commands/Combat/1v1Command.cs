using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class OneVsOneCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
        }

        public string TypeCommand
        {
            get { return "combat"; }
        }

        public string Parameters
        {
            get { return "<username>"; }
        }

        public string Description
        {
            get { return "Start a 1v1 in Armoury Arena"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Session.GetHabbo().CurrentRoomId != 102)
                return;

            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :1v1 <username>");
                return;
            }

            if (Session.GetHabbo().getCooldown("duel_command"))
            {
                Session.SendWhisper("Wait before you can use :1v1 again");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper(Username + " could not be found in " + Session.GetHabbo().CurrentRoom.Name + " room");
                return;
            }

            if (TargetClient.GetHabbo().Id == Session.GetHabbo().Id)
            {
                return;
            }

            if (Session.GetRoleplay().Passive)
            {
                Session.SendWhisper("You cannot challenge a 1v1 while in passive mode");
                return;
            }

            if (TargetClient.GetRoleplay().Passive)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " can't challenge to a 1v1 while in passive mode");
                return;
            }

           

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

            if (TargetUser.DuelUser != null)
            {
                return;
            }

            if (TargetUser.Transaction != null || TargetUser.isTradingItems)
            {
                //Session.SendWhisper(TargetClient.GetHabbo().Username + " hat bereits einen Vorschlag, bitte warten.");
                return;
            }

            Session.GetHabbo().addCooldown("duel_command", 20000);
            User.Say("challenges " + TargetClient.GetHabbo().Username + " to 1v1");
            TargetClient.GetHabbo().TransactionFrom = Session.GetHabbo().Username;
            Session.GetHabbo().TransactionTo = TargetClient.GetHabbo().Username;

            Random TokenRand = new Random();
            int tokenNumber = TokenRand.Next(1600, 2894354);
            TargetClient.GetHabbo().OfferToken = tokenNumber;

            TargetUser.Transaction = "1v1:" + Session.GetHabbo().Username;
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction;<b>" + Session.GetHabbo().Username + "</b> is challenging you to 1v1;0");
        }
    }
}