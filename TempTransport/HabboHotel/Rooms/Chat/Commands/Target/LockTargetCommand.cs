using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class LockTargetCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
        }

        public string TypeCommand
        {
            get { return "target"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Locks/Unlocks the current target"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Session.GetRoleplay().TargetId == 0)
            {
                Session.SendWhisper("Please specific a target");
                return;
            }

            if (Session.GetRoleplay().TargetLockId > 0)
            {
                Session.SendWhisper("You have unlocked your target");
                Session.GetRoleplay().TargetLockId = 0;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "lock-target;");
                return;
            }

            Session.SendWhisper("You have locked on to your target");
            Session.GetRoleplay().TargetLockId = Session.GetRoleplay().TargetId;
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "lock-target;");
        }
    }
}
