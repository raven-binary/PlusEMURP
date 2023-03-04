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
    class BingoCommand : IChatCommand
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
            get { return "Sell ​​a bingo to a citizen."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :bingo <username>");
                return;
            }

            if(Session.GetHabbo().getCooldown("bingo_command"))
            {
                Session.SendWhisper("Wait a minute...");
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

            if (Math.Abs(User.Y - TargetUser.Y) > 2 || Math.Abs(User.X - TargetUser.X) > 2)
            {
                Session.SendWhisper("You can't " + TargetClient.GetHabbo().Username + " sell a bingo ticket because it's too far.");
                return;
            }

            if (TargetUser.Transaction != null || TargetUser.isTradingItems)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " already has a transaction in progress, Wait a minute...");
                return;
            }

            Session.GetHabbo().addCooldown("bingo_command", 5000);
            User.OnChat(User.LastBubble, "* Sells a bingo ticket " + TargetClient.GetHabbo().Username + " *", true);
            TargetUser.Transaction = "bingo:" + PlusEnvironment.getPriceOfItem("Ticket de Bingo") + ":" + PlusEnvironment.getTaxeOfItem("Ticket de Bingo");
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction;<b>" + Session.GetHabbo().Username + "</b> möchte dir ein <b>Bingo-Ticket</b> für <b>" + PlusEnvironment.getPriceOfItem("Ticket de Bingo") + " Euro</b> verkaufen <b>" + PlusEnvironment.getTaxeOfItem("Ticket de Bingo") + "</b> qui iront à l'État.;" + PlusEnvironment.getPriceOfItem("Ticket de Bingo"));
        }
    }
}