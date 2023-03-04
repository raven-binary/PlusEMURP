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
using Plus.Communication.Packets.Outgoing.Rooms.Session;

namespace Bobba.HabboRoleplay.Web.Outgoing
{
    class ParamedicWebEvent : IWebEvent
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
                #region Accept
                case "accept":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Username = ReceivedData[1];

                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "paramedic-call;hide");
                        GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
                        if (TargetClient == null)
                        {
                            Client.SendWhisper("An error occoured whilst finding that user, maybe they're not online");
                            return;
                        }

                        if (!TargetClient.GetHabbo().IsWaitingForParamedic)
                        {
                            Client.SendWhisper("The player isn't requiring paramedic assistance anymore");
                            return;
                        }

                        if (TargetClient.GetHabbo().WaitingForParamedicFrom != null)
                        {
                            Client.SendWhisper("Another paramedic is on the way for " + TargetClient.GetHabbo().Username);
                            return;
                        }

                        TargetClient.GetHabbo().WaitingForParamedicFrom = Client.GetHabbo().Username;

                        Client.GetHabbo().UsingParamedic = true;

                        User.ApplyEffect(599);
                        User.Say("accepts the paramedic call for " + TargetClient.GetHabbo().Username);
                        User.Say("jumps in their ambulance and heads towards " + TargetClient.GetHabbo().CurrentRoom.Name);
                        TargetClient.SendWhisper(Client.GetHabbo().Username + " accepted your paramedic call. The ambulance is on the way!");

                        System.Timers.Timer TeleportTimer = new System.Timers.Timer(900);
                        TeleportTimer.Interval = 900;
                        TeleportTimer.Elapsed += delegate
                        {
                            Client.SendMessage(new RoomForwardComposer(TargetClient.GetHabbo().CurrentRoomId));
                            TeleportTimer.Stop();
                        };
                        TeleportTimer.Start();
                    }
                    break;
                #endregion
            }
        }
    }
}