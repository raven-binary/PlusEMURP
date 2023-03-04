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
    class SummonAllCommand : IChatCommand
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
            get { return "Summons all online players to the room you are in"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            foreach (GameClient Client in PlusEnvironment.GetGame().GetClientManager().GetClients.ToList())
            {
                if (Client != null && Client.GetHabbo() != null && !(Client.GetHabbo().Username == Session.GetHabbo().Username))
                {
                    if (!Client.GetHabbo().InRoom)
                    {
                        Client.SendMessage(new RoomForwardComposer(Session.GetHabbo().CurrentRoomId));
                    }
                    else if (Client.GetHabbo().InRoom)
                    {
                        Client.SendMessage(new RoomForwardComposer(Session.GetHabbo().CurrentRoomId));
                    }
                }
            }
        }
    }
}