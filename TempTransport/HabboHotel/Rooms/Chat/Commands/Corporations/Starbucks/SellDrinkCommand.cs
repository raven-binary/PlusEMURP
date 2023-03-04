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
    class SellDrinkCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().JobId == 6 && Session.GetHabbo().Working == true)
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
            get { return "Sell"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :sells <username>");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper(Username + " could not be found");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

            if (User.boissonPrepared == null)
            {
                Session.SendWhisper("You have nothing on your hand to sell");
                return;
            }
                

            if (Math.Abs(User.Y - TargetUser.Y) > 2 || Math.Abs(User.X - TargetUser.X) > 2)
            {
                Session.SendWhisper("You must next to the target");
                return;
            }

            string Name = "";
            int Price = 0;
            int Taxe = 0;

            if (User.boissonPrepared == "coffee")
            {
                Name = PlusEnvironment.getNameOfItem("Coffee");
                Price = PlusEnvironment.getPriceOfItem("Coffee");
                Taxe = PlusEnvironment.getTaxeOfItem("Coffee");
            }
            else if (User.boissonPrepared == "snack")
            {
                Name = PlusEnvironment.getNameOfItem("Snack");
                Price = PlusEnvironment.getPriceOfItem("Snack");
                Taxe = PlusEnvironment.getTaxeOfItem("Snack");
            }

            if (TargetUser.Transaction != null || TargetUser.isTradingItems)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " already has a transaction in progress");
                return;
            }

            Session.GetHabbo().addCooldown("sell_starbucks", 5000);
            User.boissonPrepared = null;
            User.CarryItem(0);
            User.Say("offers " + TargetClient.GetHabbo().Username + " 1x " + Name + " for $" + Price);
            TargetUser.Transaction = "starbucks:" + Name + ":" + Price + ":" + Taxe;
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction;<b>" + Session.GetHabbo().Username + "</b> wants to sell you a <b>" + Name + "</b> for <b>$" + Price);
        }
    }
} 