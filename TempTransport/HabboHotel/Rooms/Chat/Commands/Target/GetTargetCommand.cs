using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;
using System.Data;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class GetTargetCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
        }

        public string TypeCommand
        {
            get { return "target"; }
        }

        public string Parameters
        {
            get { return "<username>"; }
        }

        public string Description
        {
            get { return "Gets a player target"; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please specific a target");
                return;
            }

            GameClient Target = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Convert.ToString(Params[1]));

            if (Target == null || Target.GetHabbo().HideOnline == 1)
            {
                Session.SendWhisper("Player not found");
                return;
            }

            if (Target.GetHabbo().Username == Session.GetHabbo().Username)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            var This = Session.GetRoleplay();

            if (This.TargetId > 0 && (This.TargetId == This.TargetLockId || This.TargetId == This.LockBot))
                return;

            This.TargetId = Target.GetHabbo().Id;

            var TargetUser = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(This.TargetId);

            Session.SendWhisper("You have targetted " + Target.GetHabbo().Username);

            DataRow GetProfile = null;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT * FROM `users` WHERE `id` = @id LIMIT 1");
                    dbClient.AddParameter("id", Target.GetHabbo().Id);
                    GetProfile = dbClient.getRow();
                }

                if (Convert.ToInt32(GetProfile["gang"]) > 0)
                {
                    DataRow GetGang = null;
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("SELECT * FROM `gang` WHERE `id` = @user_gang LIMIT 1");
                        dbClient.AddParameter("user_gang", GetProfile["gang"]);
                        GetGang = dbClient.getRow();
                    }
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "target-stats;" + Target.GetHabbo().Id + ";" + Target.GetHabbo().Username + ";" + Target.GetHabbo().Look + ";" + Target.GetRoleplay().Passive + ";" + Target.GetHabbo().GetClient().GetRoleplay().Health + ";" + Target.GetHabbo().GetClient().GetRoleplay().HealthMax + ";" + Target.GetHabbo().GetClient().GetRoleplay().Energy + ";false;1;" + GetGang["color_1"] + ";" + GetGang["color_2"] + ";" + Target.GetRoleplay().Aggression);
                }
                else
                {
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "target-stats;" + Target.GetHabbo().Id + ";" + Target.GetHabbo().Username + ";" + Target.GetHabbo().Look + ";" + Target.GetRoleplay().Passive + ";" + Target.GetHabbo().GetClient().GetRoleplay().Health + ";" + Target.GetHabbo().GetClient().GetRoleplay().HealthMax + ";" + Target.GetHabbo().GetClient().GetRoleplay().Energy + ";false;0;0;0;" + Target.GetRoleplay().Aggression);
            }
        }
    }
}
