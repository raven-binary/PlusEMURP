using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.Database;
using Plus.HabboHotel.Groups;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class GagnantCommand : IChatCommand
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
            get { return "Give the winner their prize from HabboRP drawing"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :winner <username>");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient.GetHabbo() == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper(Username + " could not be found in this room.");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

            if (Math.Abs(User.Y - TargetUser.Y) > 2 || Math.Abs(User.X - TargetUser.X) > 2)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " is too far.");
                return;
            }

            if (Session.GetHabbo().getCooldown("gagnant_command"))
            {
                Session.SendWhisper("Wait a minute...");
                return;
            }

            Session.GetHabbo().addCooldown("gagnant_command", 5000);
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT COUNT(0) FROM `eurorp` WHERE `last_winner` = @userid AND lot = 0");
                dbClient.AddParameter("userid", TargetClient.GetHabbo().Id);
                int getLot = dbClient.getInteger();

                if (getLot == 0)
                {
                    if (Session.GetHabbo().Id == TargetClient.GetHabbo().Id)
                    {
                        Session.SendWhisper("You did not win the HabboRP draw or you have already received your prize.");
                        return;
                    }
                    else
                    {
                        Session.SendWhisper(TargetClient.GetHabbo().Username + " did not win the HabboRP draw or already received their prize.");
                        return;
                    }
                }

                dbClient.SetQuery("SELECT `last_montant` FROM `eurorp`");
                int EuroRPWin = dbClient.getInteger();
                dbClient.RunQuery("UPDATE `eurorp` SET lot = '1'");
                Group Cafe = null;
                if (PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(5, out Cafe))
                {
                    Cafe.ChiffreAffaire -= EuroRPWin;
                    Cafe.updateChiffre();
                }
                User.OnChat(User.LastBubble, "* Returns " + TargetClient.GetHabbo().Username + "'s profit from the HabboRP draw *", true);
                TargetUser.OnChat(TargetUser.LastBubble, "* Receives [+" + EuroRPWin + " $] *", true);
                TargetClient.GetHabbo().Credits += EuroRPWin;
                TargetClient.SendMessage(new CreditBalanceComposer(TargetClient.GetHabbo().Credits));
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "my_stats;" + TargetClient.GetHabbo().Credits + ";" + TargetClient.GetHabbo().Duckets + ";" + TargetClient.GetHabbo().EventPoints);
                return;
            }
        }
    }
}