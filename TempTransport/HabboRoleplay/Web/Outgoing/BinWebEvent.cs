using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plus;
using Fleck;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using System.Data;
using Plus.HabboHotel.Rooms;

namespace Bobba.HabboRoleplay.Web.Outgoing
{
    class BinWebEvent : IWebEvent
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

            if (!Client.GetRoleplay().Bin.Using)
                return;

            string Action = (Data.Contains(',') ? Data.Split(',')[0] : Data);

            switch (Action)
            {
                #region Remove
                case "remove":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Slot = ReceivedData[1];

                        Client.GetRoleplay().Bin.Remove(Slot);
                    }
                break;
                #endregion

                #region Close
                case "close":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        Client.GetRoleplay().Bin.End();
                    }
                break;
                #endregion
            }
        }
    }
}
