using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class ShCommand : IChatCommand
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
            get { return "<number>"; }
        }

        public string Description
        {
            get { return "Element for individual stacking heights."; }
        }
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :set <number>");
                return;
            }

            if (!double.TryParse(Params[1], out Session.GetHabbo().StackHeight) || Convert.ToInt32(Params[1]) > 100 || Convert.ToInt32(Params[2]) < 0 || Params[2].StartsWith("0") && Params[2].Length > 1)
                Session.SendWhisper("The parameter must be between 0 and 100");
            else
            {
                Session.SendWhisper("Saved! Now choose a piece of furniture and place it anywhere in the room where you want");
                return;
            }
        }
    }
}
