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
    class SellCarCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().JobId == 11 && Session.GetHabbo().Working == true)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "job"; }
        }

        public string Parameters
        {
            get { return "<username> <car name>"; }
        }

        public string Description
        {
            get { return "Sell ​​a car to a citizen."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length < 3)
            {
                Session.SendWhisper("Invalid syntax. :sellcar <username> <car name>");
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
            string Message = CommandManager.MergeParams(Params, 2);
            if (!PlusEnvironment.checkIfItemExist(Message, "voiture"))
            {
                Session.SendWhisper("This car doesn't exist.");
                return;
            }

            if (Message.ToLower() == "audi a8" && TargetClient.GetHabbo().AudiA8 == 1)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " already has an A8 car.");
                return;
            }
            else if (Message.ToLower() == "porsche 911" && TargetClient.GetHabbo().Porsche911 == 1)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " already has a Porsche 911.");
                return;
            }
            else if (Message.ToLower() == "fiat punto" && TargetClient.GetHabbo().FiatPunto == 1)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " already has a Fiat Punto.");
                return;
            }
            else if (Message.ToLower() == "volkswagen jetta" && TargetClient.GetHabbo().VolkswagenJetta == 1)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " already has a Volkswagen Jetta.");
                return;
            }
            else if (Message.ToLower() == "bmw i8" && TargetClient.GetHabbo().BmwI8 == 1)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " already has a BMW i8.");
                return;
            }

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (TargetUser.Transaction != null || TargetUser.isTradingItems)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " , you already have a transaction in progress, please wait...");
                return;
            }

            User.OnChat(User.LastBubble, "* Sells one "+ PlusEnvironment.getNameOfItem(Message) + " an " + TargetClient.GetHabbo().Username + " *", true);
            TargetUser.Transaction = "car:" + PlusEnvironment.getNameOfItem(Message) + ":" + PlusEnvironment.getPriceOfItem(Message) + ":" + PlusEnvironment.getTaxeOfItem(Message);
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction;<b>" + Session.GetHabbo().Username + "</b> want you a <b>" + PlusEnvironment.getNameOfItem(Message) + "</b> for <b>" + PlusEnvironment.getPriceOfItem(Message) + " $</b> excl. <b>" + PlusEnvironment.getTaxeOfItem(Message) + "</b> Sell taxes;" + PlusEnvironment.getPriceOfItem(Message));
        }
    }
}