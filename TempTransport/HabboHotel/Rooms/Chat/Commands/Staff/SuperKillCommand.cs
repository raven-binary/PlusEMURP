using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.GroupsRank;
using Plus.HabboHotel.Items;


namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class SuperKillCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank == 8)
            {
                return true;
            }

            return false;
        }

        public string TypeCommand
        {
            get { return "staff"; }
        }

        public string Parameters
        {
            get { return "<username>"; }
        }

        public string Description
        {
            get { return "Kills instantly a player"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {

            if (Params.Length < 2)
            {
                Session.SendWhisper("Invalid syntax :superkill <username>");
                return;
            }

            string Username = Params[1];
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper("Player not found in this room");
                return;
            }

            TargetUser.Freezed = true;
            User.Say("super throws a bomb to " + TargetClient.GetHabbo().Username + ", killing them");

            int randomId = PlusEnvironment.GetRandomNumber(123456789, 987654321);

            Item Bomb = new Item(randomId, Session.GetHabbo().CurrentRoomId, 1371, "", TargetUser.X, TargetUser.Y, 0, 0, 0, 0, 0, 0, string.Empty, Room);
            if (Room.GetRoomItemHandler().SetFloorItemByForce(null, Bomb, TargetUser.X, TargetUser.Y, 0, true, false, true))
            {
                Room.SendMessage(new ObjectUpdateComposer(Bomb, 0));
                Bomb.ExtraData = "1";
                Bomb.UpdateState(false, true);
            }

            System.Timers.Timer FarmingTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(1));
            FarmingTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(1);
            FarmingTimer.Elapsed += delegate
            {
                Session.GetHabbo().CurrentRoom.GetRoomItemHandler().RemoveFurniture(null, Bomb.Id);
                FarmingTimer.Stop();
            };
            FarmingTimer.Start();
            
        }
    }
}