using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class HealCommand : IChatCommand
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
            get { return "Heal a citizen"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :heal <username>");
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
                Session.SendWhisper("You can't heal " + TargetClient.GetHabbo().Username + " because he's too far away.");
                return;
            }

            if (TargetClient.GetHabbo().Hospital == 0)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " is not in bed .");
                return;
            }

            if (User.AnalyseUser != TargetClient.GetHabbo().Username)
            {
                Session.SendWhisper("You must first parse " + TargetClient.GetHabbo().Username + " before you can heal him.");
                return;
            }

            if (!User.mainPropre)
            {
                Session.SendWhisper("You must wash your hands before you can heal a citizen.");
                return;
            }

            if (TargetClient.GetRoleplay().Energy == 0)
            {
                User.GetClient().Shout("*Returns " + TargetClient.GetHabbo().Username + " a bar of chocolate*");
                User.GetClient().Shout("*Eat a bar of chocolate [+50% Energy]*");
                TargetClient.GetRoleplay().Energy = 50;
                TargetClient.GetHabbo().updateEnergy();
                TargetClient.SendMessage(new WhisperComposer(TargetUser.VirtualId, "Energy : " + TargetClient.GetRoleplay().Energy + "/100", 0, 34));
                TargetClient.GetHabbo().endHospital(TargetClient, 1);
            }
            else if (TargetClient.GetHabbo().Health != Session.GetHabbo().HealthMax)
            {
                User.GetClient().Shout("*Finds " + TargetClient.GetHabbo().Username + "'s violations*");
                if (TargetClient.GetHabbo().Health > 49)
                {
                    User.GetClient().Shout("*Stings on the wounds of " + TargetClient.GetHabbo().Username + "*");
                    User.GetClient().Shout("*Receives care [+50% HEALTH]*");
                    TargetClient.GetHabbo().Health = Session.GetHabbo().HealthMax;
                    TargetClient.GetHabbo().updateSante();
                    TargetClient.GetHabbo().endHospital(TargetClient, 1);
                }
                else
                {
                    User.GetClient().Shout("*Gives " + TargetClient.GetHabbo().Username + " antibiotics and treats the deepest wounds*");
                    TargetUser.GetClient().Shout("*Receives care [+50% HEALTH]*");
                    TargetClient.GetHabbo().Health = TargetClient.GetHabbo().Health + 50;
                    TargetClient.GetHabbo().updateSante();
                }
            }
            else
            {
                User.GetClient().Shout("*Whole " + TargetClient.GetHabbo().Username + "*");
                TargetClient.GetHabbo().endHospital(TargetClient, 1);
            }
        }
    }
}