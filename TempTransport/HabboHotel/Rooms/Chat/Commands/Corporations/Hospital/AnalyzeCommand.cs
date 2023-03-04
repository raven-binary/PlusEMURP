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
    class AnalyzeCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().JobId == 3 && Session.GetHabbo().Working == true)
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
            get { return "A citizen analyze"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :analyze <username>");
                return;
            }

            if(Session.GetHabbo().getCooldown("analyser_command"))
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

            if (Math.Abs(User.Y - TargetUser.Y) > 1 || Math.Abs(User.X - TargetUser.X) > 1)
            {
                Session.SendWhisper("You can't parse " + TargetClient.GetHabbo().Username + " because it's too far away.");
                return;
            }

            if (TargetClient.GetHabbo().Hospital == 0)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " is not in the hospital.");
                return;
            }

            if(TargetUser.wantSoin == false && TargetClient.GetHabbo().Mutuelle != 4)
            {
                if (TargetUser.Transaction != null)
                {
                    Session.SendWhisper(TargetClient.GetHabbo().Username + " already has a proposal in progress.", 1);
                    return;
                }

                Session.GetHabbo().addCooldown("analyser_command", 2000);
                User.GetClient().Shout("*Suggests that " + TargetClient.GetHabbo().Username + " be billed*");
                int PrixSoin = Convert.ToInt32(PlusEnvironment.GetConfig().data["price.soin"]);

                if (TargetClient.GetHabbo().Mutuelle == 1)
                {
                    PrixSoin  = (PrixSoin * 75) / 100;
                }
                else if (TargetClient.GetHabbo().Mutuelle == 2)
                {
                    PrixSoin = (PrixSoin * 50) / 100;
                }
                else if(TargetClient.GetHabbo().Mutuelle == 3)
                {
                    PrixSoin = (PrixSoin * 25) / 100;
                }

                TargetUser.Transaction = "soins";
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction;<b>" + Session.GetHabbo().Username + "</b> want to treat you with  <b>" + PrixSoin + " handle $</b>.;0");
                return;
            }

            if (User.AnalyseUser == TargetClient.GetHabbo().Username)
            {
                Session.SendWhisper("You have already analyzed this citizen.");
                return;
            }

            Session.GetHabbo().addCooldown("analyser_command", 2000);
            User.AnalyseUser = TargetClient.GetHabbo().Username;
            User.GetClient().Shout("*Analyze " + TargetClient.GetHabbo().Username + "*");
            if (TargetClient.GetRoleplay().Energy == 0)
            {
                User.GetClient().Shout("*Finds that " + TargetClient.GetHabbo().Username + "'s Energy is missing*");
            }
            else if (TargetClient.GetHabbo().Health != 100)
            {
                User.GetClient().Shout("*Finds " + TargetClient.GetHabbo().Username + "'s violations*");
            }
            else
            {
                User.GetClient().Shout("*Notice the voltage drop " + TargetClient.GetHabbo().Username + "*");
            }
        }
    }
}