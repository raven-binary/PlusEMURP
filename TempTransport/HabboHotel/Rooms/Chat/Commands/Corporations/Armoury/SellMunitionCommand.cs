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
    class SellMunitionCommand : IChatCommand
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
            get { return "<username> <weapon>"; }
        }

        public string Description
        {
            get { return "Sell ​​ammo"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length < 3)
            {
                Session.SendWhisper("Invalid syntax :sellmunition <username> <weaponname>");
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
            string Message = CommandManager.MergeParams(Params, 2);
            if (!PlusEnvironment.checkIfItemExist(Message, "munitions"))
            {
                Session.SendWhisper("This ammunition does not exist.");
                return;
            }

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (TargetUser.Transaction != null || TargetUser.isTradingItems)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " already has a transaction in progress.");
                return;
            }

            User.GetClient().Shout("*Sells 20 cartridges "+ PlusEnvironment.getNameOfItem(Message) + " to " + TargetClient.GetHabbo().Username + "*");
            TargetUser.Transaction = "Ammo:" + PlusEnvironment.getNameOfItem(Message) + ":" + PlusEnvironment.getPriceOfItem(Message) + ":" + PlusEnvironment.getTaxeOfItem(Message);
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction;<b>" + Session.GetHabbo().Username + "</b> would like <b>20 cartridges for " + PlusEnvironment.getNameOfItem(Message) + "</b> for <b>" + PlusEnvironment.getPriceOfItem(Message) + " $</b> excl. <b>" + PlusEnvironment.getTaxeOfItem(Message) + "</b> Sell taxes.;" + PlusEnvironment.getPriceOfItem(Message));
        }
    }
}