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
    class PoliceWebEvent : IWebEvent
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

            if (Client.GetHabbo().JobId != 1 || !Client.GetHabbo().Working)
                return;

            switch (Action)
            {
                #region Police Call Id View
                case "viewing":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        int CallId = Convert.ToInt32(ReceivedData[1]);

                        Client.GetHabbo().PoliceCallViewing = CallId;
                    }
                    break;
                #endregion
                #region Teleport
                case "teleport":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        int RoomId = Convert.ToInt32(ReceivedData[1]);
                        int CallId = Convert.ToInt32(ReceivedData[2]);

                        if (Client.GetHabbo().isUsingPoliceCar)
                            return;

                        if (PlusEnvironment.ExpiredPoliceCalls.ContainsKey(CallId))
                        {
                            Client.SendWhisper("This police call has expired");
                            return;
                        }

                        Random TokenRand = new Random();
                        int tokenNumber = TokenRand.Next(1600, 2894354);
                        Client.GetHabbo().PlayToken = tokenNumber;

                        Room RoomName = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(RoomId);
                        User.Say("jumps in to their squad car and heads towards " + RoomName.Name + " [" + RoomId + "]");
                        Client.GetHabbo().isUsingPoliceCar = true;

                        System.Timers.Timer TeleportTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(3));
                        TeleportTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(3);
                        TeleportTimer.Elapsed += delegate
                        {
                            if (Client.GetHabbo().PlayToken == tokenNumber && Client.GetHabbo().PlayToken == tokenNumber)
                            {
                                Client.GetHabbo().isUsingPoliceCar = false;
                                Client.SendMessage(new RoomForwardComposer(RoomId));
                            }
                            TeleportTimer.Stop();
                        };
                        TeleportTimer.Start();
                    }
                    break;
                #endregion
                #region Helpful
                case "helpful":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Username = ReceivedData[1];
                        string CallId = ReceivedData[2];

                        GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
                        if (TargetClient == null)
                            return;

                        DataRow Calls = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `police_calls` WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("id", Convert.ToInt32(CallId));
                            Calls = dbClient.getRow();
                        }

                        if (Convert.ToInt32(Calls["responded_cop_id"]) == 0)
                        {
                            TargetClient.SendWhisper("You have been given a $15 tip for your helpful call");
                            TargetClient.GetHabbo().Credits += 15;
                            TargetClient.SendMessage(new CreditBalanceComposer(TargetClient.GetHabbo().Credits));
                            TargetClient.GetHabbo().RPCache(3);

                            PlusEnvironment.GetGame().GetClientManager().PoliceRespond(Convert.ToInt32(CallId), Client.GetHabbo().Username);

                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `police_calls` SET `responded_cop_id` = '" + Client.GetHabbo().Id + "'  WHERE `id` = '" + Convert.ToInt32(CallId) + "' LIMIT 1;");
                                dbClient.RunQuery();
                            }
                        }
                    }
                    break;
                #endregion
                #region Abuse
                case "abuse":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Username = ReceivedData[1];
                        string CallId = ReceivedData[2];

                        GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
                        if (TargetClient == null)
                            return;

                        DataRow Calls = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `police_calls` WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("id", Convert.ToInt32(CallId));
                            Calls = dbClient.getRow();
                        }

                        if (Convert.ToInt32(Calls["responded_cop_id"]) == 0)
                        {
                            PlusEnvironment.GetGame().GetChatManager().GetCommands().Parse(Client, ":charge " + Username + " 911abuse");
                            PlusEnvironment.GetGame().GetClientManager().PoliceRespond(Convert.ToInt32(CallId), Client.GetHabbo().Username);

                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `police_calls` SET `responded_cop_id` = '" + Client.GetHabbo().Id + "'  WHERE `id` = '" + Convert.ToInt32(CallId) + "' LIMIT 1;");
                                dbClient.RunQuery();
                            }
                        }
                    }
                    break;
                #endregion
            }
        }
    }
}