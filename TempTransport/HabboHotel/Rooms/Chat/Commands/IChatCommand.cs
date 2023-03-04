using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands
{
    public interface IChatCommand
    {
        bool getPermission(GameClient Session);
        string TypeCommand { get; }
        string Parameters { get; }
        string Description { get; }
        void Execute(GameClient Session, Room Room, string[] Params);
    }
}
