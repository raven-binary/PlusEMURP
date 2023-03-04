using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Session;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class SummonCommand : IChatCommand
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
            get { return "<username>"; }
        }

        public string Description
        {
            get { return "Teleport a player to your current room"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax: :summon <username>");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null)
            {
                Session.SendWhisper(Username + " could not be found");
                return;
            }

            if (TargetClient.GetHabbo().Id == Session.GetHabbo().Id)
            {
                return;
            }

            if (TargetClient.GetHabbo().CurrentRoomId == Session.GetHabbo().CurrentRoomId)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " is already in this room");
                return;
            }

            if (TargetClient.GetHabbo().Prison != 0 || TargetClient.GetHabbo().Hospital != 0)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " cannot be summoned because " + TargetClient.GetHabbo().Username + " is jailed or dead");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            User.Say("teleports " + TargetClient.GetHabbo().Username + " to this room");
            TargetClient.SendMessage(new RoomForwardComposer(Session.GetHabbo().CurrentRoomId));
        }
    }
}