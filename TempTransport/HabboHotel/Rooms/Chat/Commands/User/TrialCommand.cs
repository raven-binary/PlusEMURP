using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Plus.HabboHotel.Rooms;
using Plus.HabboRoleplay.Misc;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class TrialCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetRoleplay().Prison && Session.GetRoleplay().Timer >= 15)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "user"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Requests for a trial at court"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (RoleplayManager.GetCooldown("trial" + Session.GetHabbo().Id))
            {
                Session.SendWhisper("You have to wait one hour before requesting a new trial");
                return;
            }

            if (RoleplayManager.CourtUsing)
            {
                Session.SendWhisper("Hold on, there is someone in trial right now");
                return;
            }

            Session.SendWhisper("Your trial will start in 2 minutes");
            Session.GetRoleplay().Trial = true;
            RoleplayManager.CourtUsing = true;
            RoleplayManager.AddCooldown("trial" + Session.GetHabbo().Id, Convert.ToInt32(PlusEnvironment.ConvertHoursToMilliseconds(1)));

            List<GameClients.GameClient> RandomUsers = (from Client in PlusEnvironment.GetGame().GetClientManager().GetClients.ToList() where Client != null && Client.GetHabbo() != null orderby new Utilities.CryptoRandom().Next() select Client).ToList();
            lock (RandomUsers)
            {
                foreach (var Client in RandomUsers.Take(15))
                {
                    if (Client == null || Client.GetHabbo() == null || Client.GetRoleplay() == null || Client.GetHabbo().Id == Session.GetHabbo().Id || Client.GetRoleplay().Prison || Client.GetRoleplay().Hospital)
                        continue;

                    RoleplayManager.InvitedUsersToJuryDuty.Add(Client);
                    Client.GetRoleplay().SendWeb("jury;show");
                    Client.SendWhisper("You have been requested at Las Vegas Justice Court to take part in jury duty. You have 2 minutes to get there.", 5);
                    Client.SendWhisper("Click the jury button in the top right or walk to the court", 5);
                }
            }

            System.Timers.Timer StartTimer = new System.Timers.Timer(PlusEnvironment.ConvertMinutesToMilliseconds(1));
            StartTimer.Interval = PlusEnvironment.ConvertMinutesToMilliseconds(1);
            StartTimer.Elapsed += delegate
            {
                if (RoleplayManager.CourtStarting)
                {
                    if (Session.GetRoleplay().Prison)
                    {
                        Session.GetRoleplay().Prison = false;
                        Session.GetRoleplay().ResetAvatar();
                        Session.GetRoleplay().IsInCourt = true;
                        RoleplayManager.CourtStarted = true;
                        RoleplayManager.Defendant = Session;
                        RoleplayManager.InstantRL(Session, 113);

                        lock (RoleplayManager.InvitedUsersToJuryDuty)
                        {
                            foreach (GameClient Client in RoleplayManager.InvitedUsersToJuryDuty)
                            {
                                if (Client == null || Client.GetHabbo() == null || Client.GetRoleplay() == null || RoleplayManager.InvitedUsersToRemove.Contains(Client))
                                    continue;

                                Client.SendWhisper("Your jury duty request has expired", 5);
                                Client.GetRoleplay().SendWeb("jury;hide");
                            }
                        }
                    }
                    else
                    {
                        RoleplayManager.CourtStarting = false;
                    }
                    StartTimer.Stop();
                }
                else
                {
                    Session.SendWhisper("Your trial will start in 1 minutes");
                    RoleplayManager.CourtStarting = true;

                    lock (RoleplayManager.InvitedUsersToJuryDuty)
                    {
                        foreach (GameClient client in RoleplayManager.InvitedUsersToJuryDuty)
                        {
                            if (client == null || client.GetHabbo() == null)
                                continue;

                            client.SendWhisper("You have been requested at Las Vegas Justice Court to take part in jury duty. You have 1 minutes to get there.", 5);
                            client.SendWhisper("Click the jury button in the top right or walk to the court", 5);
                        }
                    }
                }
            };
            StartTimer.Start();
        }
    }
}
