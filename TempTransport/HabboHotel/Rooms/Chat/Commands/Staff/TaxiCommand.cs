using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Session;
using Plus.HabboRoleplay.Misc;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class TaxiCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank > 5 && Session.GetHabbo().isLoggedIn)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "staff"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Supers calls a Taxi"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax: taxi <room id>");
                return;
            }

            int num;
            if (!Int32.TryParse(Params[1], out num) || Params[1].StartsWith("0"))
            {
                Session.SendWhisper("Invalid room ID");
                return;
            }

            Room TargetRoom = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(Convert.ToInt32(Params[1]));
            if (TargetRoom == null)
            {
                Session.SendWhisper("Invalid room ID: [" + Params[1] + "]");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (Convert.ToInt32(Params[1]) == Session.GetHabbo().CurrentRoomId)
            {
                Session.SendWhisper("You are already in this room");
                return;
            }

            User.ApplyEffect(596);
            User.Say("super calls for a taxi to " + TargetRoom.Name + " [" + TargetRoom.Id + "]", 5);
            System.Timers.Timer TeleportTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(2));
            TeleportTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(2);
            TeleportTimer.Elapsed += delegate
            {
                RoleplayManager.InstantRL(Session, TargetRoom.Id);
                TeleportTimer.Stop();
            };
            TeleportTimer.Start();
        }
    }
}