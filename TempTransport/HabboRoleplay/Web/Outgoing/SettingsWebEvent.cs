using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Plus;
using Fleck;
using Plus.HabboHotel.GameClients;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Rooms;

namespace Bobba.HabboRoleplay.Web.Outgoing
{
    class SettingsWebEvent : IWebEvent
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
                #region Toggle LiveFeed
                case "livefeed":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Toggle = ReceivedData[1];

                        if (Toggle == "enabled")
                        {
                            Client.GetRoleplay().Livefeed = true;

                        }
                        else if (Toggle == "disabled")
                        {
                            Client.GetRoleplay().Livefeed = false;
                        }
                    }
                    break;
                #endregion
            }
        }
    }
}
