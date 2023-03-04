using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plus;
using Fleck;
using Plus.HabboHotel.GameClients;
using System.Text.RegularExpressions;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using System.Data;
using System.Threading;

namespace Bobba.HabboRoleplay.Web.Outgoing
{
    class TargetWebEvent : IWebEvent
    {
        /// <summary>
        /// Executes socket data.
        /// </summary>
        /// <param name="Client"></param>
        /// <param name="Data"></param>
        /// <param name="Socket"></param>
        public void Execute(GameClient Client, string Data, IWebSocketConnection Socket)
        {

            if (!PlusEnvironment.GetGame().GetWebEventManager().SocketReady(Client, true) || !PlusEnvironment.GetGame().GetWebEventManager().SocketReady(Socket))
                return;

            string Action = (Data.Contains(',') ? Data.Split(',')[0] : Data);

            switch (Action)
            {

                #region Lock Target
                case "lock":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string isBot = ReceivedData[1];

                        if (isBot == "true")
                        {
                            RoomUser bot = null;
                            if (!Room.GetRoomUserManager().TryGetBot(User.GetClient().GetRoleplay().TargetId, out bot))
                                return;
                            User.GetClient().GetRoleplay().LockBot = User.GetClient().GetRoleplay().TargetId;
                            if (User.GetClient().GetRoleplay().TargetLockId > 0)
                                User.GetClient().GetRoleplay().TargetLockId = 0;
                        }
                        else
                        {
                            var client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(User.GetClient().GetRoleplay().TargetId);
                            if (client == null)
                                return;
                            User.GetClient().GetRoleplay().TargetLockId = User.GetClient().GetRoleplay().TargetId;
                            if (User.GetClient().GetRoleplay().LockBot > 0)
                                User.GetClient().GetRoleplay().LockBot = 0;
                        }
                    }
                    break;
                #endregion

                #region Unlock Target
                case "unlock":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string isBot = ReceivedData[1];

                        if (isBot == "true")
                        {
                            RoomUser bot = null;
                            if (Room.GetRoomUserManager().TryGetBot(User.GetClient().GetRoleplay().TargetId, out bot))
                            if (User.GetClient().GetRoleplay().LockBot == User.GetClient().GetRoleplay().TargetId)
                                User.GetClient().GetRoleplay().LockBot = 0;

                        }
                        else
                        {
                            var client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(User.GetClient().GetRoleplay().TargetId);
                            if (client != null)
                            if (User.GetClient().GetRoleplay().TargetLockId == User.GetClient().GetRoleplay().TargetId)
                                User.GetClient().GetRoleplay().TargetLockId = 0;
                        }
                        User.GetClient().GetRoleplay().TargetId = 0;
                    }
                    break;
                #endregion

                #region Close Target
                case "close":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string isBot = ReceivedData[1];

                        if (isBot == "true")
                        {
                            RoomUser bot = null;
                            if (Room.GetRoomUserManager().TryGetBot(User.GetClient().GetRoleplay().TargetId, out bot))
                                if (User.GetClient().GetRoleplay().LockBot == User.GetClient().GetRoleplay().TargetId)
                                    User.GetClient().GetRoleplay().LockBot = 0;

                        }
                        else
                        {
                            var client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(User.GetClient().GetRoleplay().TargetId);
                            if (client != null)
                                if (User.GetClient().GetRoleplay().TargetLockId == User.GetClient().GetRoleplay().TargetId)
                                    User.GetClient().GetRoleplay().TargetLockId = 0;
                        }
                        User.GetClient().GetRoleplay().TargetId = 0;
                    }
                    break;
                #endregion

                #region Update Target
                case "update":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        var Target = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(Client.GetRoleplay().TargetId);
                        if (Target != null)
                        {
                            if (Target.GetHabbo().usingArena)
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "target-stats-update;" + Target.GetRoleplay().Passive + ";" + Target.GetHabbo().ArenaHealth + ";" + Target.GetHabbo().ArenaHealthMax + ";" + Target.GetHabbo().ArenaEnergy);
                            }
                            else
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "target-stats-update;" + Target.GetRoleplay().Passive + ";" + Target.GetRoleplay().Health + ";" + Target.GetRoleplay().HealthMax + ";" + Target.GetRoleplay().Energy + ";" + Target.GetRoleplay().Aggression);
                            }
                        }
                    }
                    break;
                    #endregion
            }
        }
    }
}