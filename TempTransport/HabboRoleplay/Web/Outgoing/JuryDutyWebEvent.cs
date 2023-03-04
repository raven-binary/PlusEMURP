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
using Plus.HabboRoleplay.Misc;
using Plus.Communication.Packets.Outgoing.Rooms.Session;

namespace Bobba.HabboRoleplay.Web.Outgoing
{
    class JuryDutyWebEvent : IWebEvent
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
                #region Teleport
                case "teleport":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (RoleplayManager.CourtStarted || !RoleplayManager.InvitedUsersToJuryDuty.Contains(Client) || RoleplayManager.InvitedUsersToRemove.Contains(Client) || Client.GetRoleplay().IsTeleportingToCourt)
                            return;

                        RoleplayManager.InvitedUsersToRemove.Add(Client);
                        if (Client.GetHabbo().CurrentRoomId == 113)
                        {
                            Client.GetRoleplay().SendWeb("jury;hide");
                            return;
                        }

                        User.Say("starts teleporting to Las Vegas Jury for jury duty");
                        Client.GetRoleplay().IsTeleportingToCourt = true;

                        System.Timers.Timer TeleportTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(5));
                        TeleportTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(5);
                        TeleportTimer.Elapsed += delegate
                        {
                            if (Client.GetRoleplay().IsTeleportingToCourt && Client.GetHabbo().CurrentRoomId != 113)
                            {
                                Client.GetRoleplay().IsTeleportingToCourt = false;
                                RoleplayManager.InstantRL(User.GetClient(), 113);
                            }
                            TeleportTimer.Stop();
                        };
                        TeleportTimer.Start();
                    }
                    break;
                #endregion
                #region Vote Guilty
                case "guilty":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (!RoleplayManager.InvitedUsersToRemove.Contains(Client) || Client.GetRoleplay().CourtVoted || !RoleplayManager.CourtStarted || !RoleplayManager.CourtVoteStarted)
                            return;

                        User.Say("places their vote");
                        Client.GetRoleplay().CourtVoted = true;
                        RoleplayManager.CourtGuiltyVotes += 1;
                        Client.GetRoleplay().SendWeb("jury-vote;hide");
                    }
                break;
                #endregion
                #region Vote Innocent
                case "innocent":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (!RoleplayManager.InvitedUsersToRemove.Contains(Client) || Client.GetRoleplay().CourtVoted || !RoleplayManager.CourtStarted || !RoleplayManager.CourtVoteStarted)
                            return;

                        User.Say("places their vote");
                        Client.GetRoleplay().CourtVoted = true;
                        RoleplayManager.CourtInnocentVotes += 1;
                        Client.GetRoleplay().SendWeb("jury-vote;hide");
                    }
                break;
                #endregion
            }
        }
    }
}