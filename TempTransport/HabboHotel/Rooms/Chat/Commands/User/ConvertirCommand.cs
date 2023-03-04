using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.HabboHotel.Groups;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class ConvertirCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().JobId == 16 && Session.GetHabbo().RankId == 1 && Session.GetHabbo().Working == true || Session.GetHabbo().JobId == 16 && Session.GetHabbo().RankId == 3 && Session.GetHabbo().Working == true)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "job"; }
        }

        public string Parameters
        {
            get { return "<username> <chip amount>"; }
        }

        public string Description
        {
            get { return "Cash out a citizen's chips in money."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length < 3)
            {
                Session.SendWhisper("Invalid syntax. :convertchips <username> <chip amount>");
                return;
            }

            if (Session.GetHabbo().getCooldown("convertir_command"))
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
                Session.SendWhisper("Go closer to " + TargetClient.GetHabbo().Username + " to convert the chips.");
                return;
            }

            if(TargetUser.participateRoulette == true)
            {
                Session.SendWhisper("You cannot swap chips from " + TargetClient.GetHabbo().Username + " because he is playing roulette.");
                return;
            }

            if (TargetUser.isTradingItems)
            {
                Session.SendWhisper("You can't swap the chips of "+ TargetClient.GetHabbo().Username + " because he's currently in a swap.");
                return;
            }

            int Amount;
            string Montant = Params[2];
            if (!int.TryParse(Montant, out Amount) || Convert.ToInt32(Params[2]) <= 0 || Montant.StartsWith("0"))
            {
                Session.SendWhisper("Chip count is invalid.");
                return;
            }

            if (Convert.ToInt32(Montant) > TargetClient.GetHabbo().Casino_Jetons)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username +  " has only " + TargetClient.GetHabbo().Casino_Jetons + " chips.");
                return;
            }

            Session.GetHabbo().addCooldown("convertir_command", 3000);
            TargetClient.GetHabbo().Casino_Jetons -= Convert.ToInt32(Montant);
            TargetClient.GetHabbo().updateCasinoJetons();
            Group Casino = null;
            if (PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(16, out Casino))
            {
                Casino.ChiffreAffaire -= Convert.ToInt32(Montant) * 10;
                Casino.updateChiffre();
            }
            TargetClient.GetHabbo().Credits += Convert.ToInt32(Montant) * 10;
            TargetClient.SendMessage(new CreditBalanceComposer(TargetClient.GetHabbo().Credits));
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "my_stats;" + TargetClient.GetHabbo().Credits + ";" + TargetClient.GetHabbo().Duckets + ";" + TargetClient.GetHabbo().EventPoints);
            User.GetClient().Shout("*Takes " + Convert.ToInt32(Montant) + " chips from " + TargetClient.GetHabbo().Username + " and passes " + Convert.ToInt32(Montant) * 10 + " $*");
        }
    }
}