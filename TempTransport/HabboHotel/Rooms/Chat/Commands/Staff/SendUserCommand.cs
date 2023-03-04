using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.HabboRoleplay.Misc;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class SendUserCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank == 8 || Session.GetHabbo().Rank == 7)
                return true;

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
            get { return "Send a citizen into a room."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {

            if (Params.Length == 2)
            {
                Session.SendWhisper("Invalid syntax :senduser <username> <roomid>");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);

            if (TargetClient.GetHabbo() == null)
            {
                Session.SendWhisper(Username + " could not be found.");
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("You cannot send yourself into a room.");
                return;
            }

            int RoomID;
            if (!int.TryParse(Params[2], out RoomID))
            {
                Session.SendWhisper("Invalid RoomID: [" + Params[2] + "]");
                return;
            }

            if (TargetClient.GetHabbo().Rank == 8 || TargetClient.GetHabbo().Rank == 7 || TargetClient.GetHabbo().Rank == 6)
            {
                Session.SendWhisper("You can use this command for staff members.");
                return;
            }

            RoomData RoomData = PlusEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomID);
            TargetClient.GetHabbo().CanChangeRoom = true;
            
            RoleplayManager.InstantRL(TargetClient, RoomID);
        }
    }
}