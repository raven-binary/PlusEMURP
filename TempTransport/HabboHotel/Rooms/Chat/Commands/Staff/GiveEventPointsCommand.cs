using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class GiveEventPointsCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank == 8)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "staff"; }
        }

        public string Parameters
        {
            get { return "<username> <amount>"; }
        }

        public string Description
        {
            get { return "Give points to a citizen event"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length < 3)
            {
                Session.SendWhisper("Invalid syntax :gep <citizen name> <amount>", 1);
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null)
            {
                Session.SendWhisper(Username + " could not be found.", 1);
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

            int Amount;
            string Montant = Params[2];
            if (!int.TryParse(Montant, out Amount) || Convert.ToInt32(Params[2]) <= 0 || Montant.StartsWith("0"))
            {
                Session.SendWhisper("The amount is invalid.", 1);
                return;
            }

            if (Session.GetHabbo() != null)
            {
                User.GetClient().Shout("*Returns  " + TargetClient.GetHabbo().Username + " " + Montant + " event point(s)*");
                TargetClient.GetHabbo().EventPoints += Convert.ToInt32(Montant);
                TargetClient.GetHabbo().updateEventPoints();
                TargetClient.SendWhisper(TargetClient.GetHabbo().Username + " gave you " + Montant + " in total you have " + TargetClient.GetHabbo().EventPoints + " event point(s).");
            }
        }
    }
}