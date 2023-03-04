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
    class StartWorkCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
        }

        public string TypeCommand
        {
            get { return "job"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Starts your shift"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            
            if (Session.GetHabbo().Working)
                return;

            if (Session.GetHabbo().Prison > 0 || Session.GetHabbo().Hospital == 1)
                return;

            if (Session.GetHabbo().Cuffed)
            {
                Session.SendWhisper("You cannot perform this action while cuffed");
                return;
            }

            if (Session.GetHabbo().getCooldown("startwork"))
            {
                Session.SendWhisper("You must wait before you can use startwork comamnd again");
                return;
            }
            
            if (Session.GetHabbo().JobId == 8)
            {
                Session.SendWhisper("You don't have any job. Go to the job center to find one");
                return;
            }

            if (Session.GetHabbo().RankInfo == null || Session.GetHabbo().TravailInfo == null)
            {
                Session.SendWhisper("There is a problem with your corp");
                return;
            }

            if (Session.GetHabbo().CurrentRoomId == Session.GetHabbo().TravailInfo.RoomId || Session.GetHabbo().JobId == 2 && Session.GetHabbo().CurrentRoomId == 63 || Session.GetHabbo().RankId == 1 && Session.GetHabbo().CurrentRoom.CorpOffice
                || (Session.GetHabbo().JobId == 1 && Session.GetHabbo().RankId >= 1 && Session.GetHabbo().CurrentRoomId == 98 || Session.GetHabbo().CurrentRoomId == 67 || Session.GetHabbo().CurrentRoomId == 68 || Session.GetHabbo().CurrentRoomId == 239 || Session.GetHabbo().CurrentRoomId == 99)
                || (Session.GetHabbo().JobId == 1 && Session.GetHabbo().RankId < 4 && Session.GetHabbo().CurrentRoomId == 69 || Session.GetHabbo().JobId == 4 && Session.GetHabbo().RankId == 4 && Session.GetHabbo().CurrentRoomId == 75 || Session.GetHabbo().JobId == 4 && Session.GetHabbo().RankId == 5 && Session.GetHabbo().CurrentRoomId == 75 || Session.GetHabbo().JobId == 4 && Session.GetHabbo().RankId == 6 && Session.GetHabbo().CurrentRoomId == 75 
                || Session.GetHabbo().JobId == 4 && Session.GetHabbo().RankId == 7 && Session.GetHabbo().CurrentRoomId == 75 || Session.GetHabbo().JobId == 5 && Session.GetHabbo().RankId <= 1 && Session.GetHabbo().CurrentRoomId == 71)

                )
            {
                RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

                if (User.isTradingItems)
                {
                    //Session.SendWhisper("You cannot work while trading.");
                    return;
                }
                
                if (Session.GetRoleplay().Aggression > 0)
                {
                    Session.SendWhisper("You cannot start work with aggression");
                    return;
                }

                if (PlusEnvironment.usersSuspendus.ContainsKey(Session.GetHabbo().Username))
                {
                    Session.SendWhisper("You have been sent home, you don't have permission to start your shift at this time");
                    return;
                }

                Session.GetHabbo().StartedWork = DateTime.Now;
                Session.GetHabbo().addCooldown("startwork", 5000);
                Session.GetHabbo().updateAvatarEvent(Session.GetHabbo().RankInfo.Look_F, Session.GetHabbo().RankInfo.Look_H, "[WORKING] " + Session.GetHabbo().RankInfo.Name + ", " + Session.GetHabbo().TravailInfo.Name);
                if (Session.GetHabbo().Timer < 1)
                {
                    Session.GetHabbo().Timer = 10;
                    Session.GetHabbo().updateTimer();
                }
                Session.GetHabbo().Working = true;
                Session.SendWhisper("You have started your shift");

                if (Session.GetHabbo().JobId == 1)
                {
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "police-calls;show");
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "police-calls-icon;show");
                }

            }
            else
            {
                Session.SendWhisper("You don't work here");
            }
        }
    }
}