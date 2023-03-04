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

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class EuroRPCommand : IChatCommand
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
            get { return "Register a citizen for the HabboRP draw."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :eurorp <username>");
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
                Session.SendWhisper("You can't register " + TargetClient.GetHabbo().Username + " to the HabboRP draw because it's too far.");
                return;
            }

            if (TargetUser.Transaction != null || TargetUser.isTradingItems)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " already has a transaction, wait a minute.");
                return;
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT COUNT(0) FROM `eurorp_participants` WHERE `participant_id` = @userid");
                dbClient.AddParameter("userid", TargetClient.GetHabbo().Id);
                int getLot = dbClient.getInteger();

                if (getLot > 0)
                {
                    if (Session.GetHabbo().Id == TargetClient.GetHabbo().Id)
                    {
                        Session.SendWhisper("You have already entered the HabboRP draw.");
                    }
                    else
                    {
                        Session.SendWhisper(TargetClient.GetHabbo().Username + " is already entered in the HabboRP raffle.");
                    }
                    User.makeAction = false;
                    return;
                }
            }
            
            User.OnChat(User.LastBubble, "* Offers " + TargetClient.GetHabbo().Username + " to enter the EuroRP raffle *", true);
            TargetUser.Transaction = "HabboRP:" + PlusEnvironment.getPriceOfItem("Tirage EuroRP") + ":" + PlusEnvironment.getTaxeOfItem("Tirage EuroRP");
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction;<b>" + Session.GetHabbo().Username + "</b> would like to register for the <b>HabboRP</b> draw for <b>" + PlusEnvironment.getPriceOfItem("Tirage EuroRP") + " $</b> including <b>" + PlusEnvironment.getTaxeOfItem("Tirage EuroRP") + "</b> which will go to the State.;" + PlusEnvironment.getPriceOfItem("Tirage EuroRP"));
        }
    }
}