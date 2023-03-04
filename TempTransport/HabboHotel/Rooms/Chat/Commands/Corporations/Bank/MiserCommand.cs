using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.HabboHotel.Groups;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class MiserCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().CurrentRoomId == 46)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "user"; }
        }

        public string Parameters
        {
            get { return "<number> <amount>"; }
        }

        public string Description
        {
            get { return "Bet on roulette"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length < 3)
            {
                Session.SendWhisper("Invalid syntax. :bet <number> <amount>.");
                return;
            }

            if (PlusEnvironment.RouletteEtat != 0 || PlusEnvironment.RouletteTurning == true)
            {
                Session.SendWhisper("You cannot place any bets at the moment");
                return;
            }

            if (Session.GetHabbo().getCooldown("miser_command"))
            {
                Session.SendWhisper("Wait a minute...");
                return;
            }

            if (Session.GetHabbo().JobId == 16 && Session.GetHabbo().Working == true)
            {
                Session.SendWhisper("You can't make bets while you're working.");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User.participateRoulette == true)
            {
                Session.SendWhisper("You have already placed a bet.");
                return;
            }

            if (PlusEnvironment.GetGame().GetClientManager().userCroupierWorking() == 0)
            {
                Session.SendWhisper("Unfortunately you can't bet as no server is working.");
                return;
            }

            if (User.Item_On != 3800)
            {
                Session.SendWhisper("You must be at a table to place a bet.");
                return;
            }

            int num;
            if (!Int32.TryParse(Params[1], out num) || Convert.ToInt32(Params[1]) < 0 || Convert.ToInt32(Params[1]) > 36 || Params[1].StartsWith("0") && Params[1].Length > 1)
            {
                Session.SendWhisper("The number is not valid.");
                return;
            }

            if (!Int32.TryParse(Params[2], out num) || Params[2].StartsWith("0") || Convert.ToInt32(Params[2]) <= 0)
            {
                Session.SendWhisper("The amount of money is not valid.");
                return;
            }

            if (Convert.ToInt32(Params[2]) > 100)
            {
                Session.SendWhisper("You cannot bet more than 100 chips.");
                return;
            }

            if (Session.GetHabbo().Casino_Jetons == 0)
            {
                Session.SendWhisper("You're out of chips.");
                return;
            }

            if (Convert.ToInt32(Params[2]) > Session.GetHabbo().Casino_Jetons)
            {
                Session.SendWhisper("You only have " + Session.GetHabbo().Casino_Jetons + " chips.");
                return;
            }

            Session.GetHabbo().addCooldown("miser_command", 3000);
            User.participateRoulette = true;
            Session.GetHabbo().Casino_Jetons -= Convert.ToInt32(Params[2]);
            Session.GetHabbo().updateCasinoJetons();
            User.numberRoulette = Convert.ToInt32(Params[1]);
            User.miseRoulette = Convert.ToInt32(Params[2]);
            User.GetClient().Shout("*Placed " + Params[2] + " chips on the number " + Params[1] + "*");
        }
    }
}