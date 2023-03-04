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
    class SacCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().JobId == 14 && Session.GetHabbo().Working == true)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "job"; }
        }

        public string Parameters
        {
            get { return "<username> <name of the bag>"; }
        }

        public string Description
        {
            get { return "Sell ​​a bag"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length < 3)
            {
                Session.SendWhisper("Invalid syntax. :sellbag <username> <name of bag>");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper("The user " + Username + " could not be found.");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            string Message = CommandManager.MergeParams(Params, 2);
            if (!PlusEnvironment.checkIfItemExist(Message, "sac"))
            {
                Session.SendWhisper("This bag does not exist.");
                return;
            }

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (TargetUser.Transaction != null || TargetUser.isTradingItems)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " , there is already a transaction in progress, wait a minute...");
                return;
            }

            User.OnChat(User.LastBubble, "* Sells a "+ PlusEnvironment.getNameOfItem(Message) + " to " + TargetClient.GetHabbo().Username + " *", true);
            TargetUser.Transaction = "sac:" + PlusEnvironment.getNameOfItem(Message) + ":" + PlusEnvironment.getTypeOfItem(Message) + ":" + PlusEnvironment.getPriceOfItem(Message) + ":" + PlusEnvironment.getTaxeOfItem(Message);
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction;<b>" + Session.GetHabbo().Username + "</b> would like to send you a <b>" + PlusEnvironment.getNameOfItem(Message) + "</b> for <b>" + PlusEnvironment.getPriceOfItem(Message) + " $</b> excl. <b>" + PlusEnvironment.getTaxeOfItem(Message) + "</b> Sell taxes.;" + PlusEnvironment.getPriceOfItem(Message));
        }
    }
}