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
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms.AI;
using Plus.HabboHotel.Rooms.AI.Speech;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.HabboHotel.Rooms.AI.Responses;
using Plus.Communication.Packets.Outgoing.Rooms.Session;
using Plus.HabboRoleplay.Misc;

namespace Bobba.HabboRoleplay.Web.Outgoing
{
    class TaxiWebEvent : IWebEvent
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
                #region Taxi
                case "taxi":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetRoleplay().CallingTaxi)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string TargetRoomID = ReceivedData[1];
                        int TargetRoom;
                        TargetRoom = Convert.ToInt32(TargetRoomID);
                        DataRow GetRow = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `rooms` WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("id", TargetRoom);
                            GetRow = dbClient.getRow();
                        }

                        if (TargetRoom == Client.GetHabbo().CurrentRoomId)
                        {
                            Client.SendWhisper("You are already in this room");
                            return;
                        }

                        if (Convert.ToString(GetRow["taxi"]) == "0")
                        {
                            Client.SendWhisper("You can't call a taxi there");
                            return;
                        }

                        if (Client.GetHabbo().Credits < Convert.ToInt32(GetRow["taxiCost"]))
                        {
                            Client.SendWhisper("You need $" + GetRow["taxiCost"] + " for this taxi ride");
                            return;
                        }

                        User.ApplyEffect(596);
                        User.Say("calls for a taxi to " + GetRow["caption"], 5);
                        Client.GetRoleplay().SendWeb("taxi;hide;");
                        Client.GetRoleplay().ActionTimer("Calling a taxi", 5);
                        Client.GetRoleplay().CallingTaxi = true;
                        Client.GetRoleplay().TaxiRideId = TargetRoom;
                    }
                    break;
                    #endregion
            }
        }
    }
}