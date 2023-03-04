using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class BubbleCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank == 8)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "staff"; }
        }

        public string Parameters
        {
            get { return "<id>"; }
        }

        public string Description
        {
            get { return "Changes your chat bubble"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :bubble <id>");
                return;
            }

            int Bubble = 0;
            if (!int.TryParse(Params[1].ToString(), out Bubble))
            {
                Session.SendWhisper("Please enter a valid bubble id");
                return;
            }

            User.LastBubble = Bubble;
            Session.GetHabbo().CustomBubbleId = Bubble;
            Session.SendWhisper("Chat Bubble changed to: " + Bubble);
        }
    }
}
