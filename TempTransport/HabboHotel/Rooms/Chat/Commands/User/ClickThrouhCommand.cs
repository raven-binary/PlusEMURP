using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Permissions;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class ClickThrouhCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
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
            get { return "Toggle Clickthrough"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            Session.SendWhisper("Click thru " + (Session.GetHabbo().ClickThru ? "disabled" : "enabled"));
            Session.GetHabbo().ClickThru = !Session.GetHabbo().ClickThru;
            Session.SendMessage(new YouArePlayingGameMessageComposer(Session.GetHabbo().ClickThru));
        }
    }
}