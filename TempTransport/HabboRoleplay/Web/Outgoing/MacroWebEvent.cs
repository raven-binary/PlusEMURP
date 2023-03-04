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

using Plus.HabboHotel.Rooms.AI;
using Plus.HabboHotel.Rooms.AI.Speech;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.HabboHotel.Rooms.AI.Responses;

namespace Bobba.HabboRoleplay.Web.Outgoing
{
    class MacroWebEvent : IWebEvent
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
                #region Macro
                case "macro":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;
                        var This = Client.GetRoleplay();

                        string[] ReceivedData = Data.Split(',');
                        string Key = ReceivedData[1];
                        string Message = "null0";
                        if (Key == User.GetClient().GetHabbo().Macro1_1Key)
                        {
                            Message = User.GetClient().GetHabbo().Macro1_1;
                        }
                        else if (Key == User.GetClient().GetHabbo().Macro1_2Key)
                        {
                            Message = User.GetClient().GetHabbo().Macro1_2;
                        }
                        else if (Key == User.GetClient().GetHabbo().Macro1_3Key)
                        {
                            Message = User.GetClient().GetHabbo().Macro1_3;
                        }

                        if (Message.StartsWith(":", StringComparison.CurrentCulture) && Message.Contains(" x"))
                        {
                            var user = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(This.TargetId);
                            if (user != null && user.GetHabbo().CurrentRoomId == Client.GetHabbo().CurrentRoomId)
                                Message = Regex.Replace(Message, " x", " " + user.GetHabbo().Username + "").Trim();
                        }
                        if (Message.StartsWith(":", StringComparison.CurrentCulture) && PlusEnvironment.GetGame().GetChatManager().GetCommands().Parse(User.GetClient(), Message))
                            return;

                        //User.GetClient().SendMessage(new ChatComposer(User.VirtualId, Message, 0, 0));
                        User.OnChat(User.LastBubble, Message, false);
                    }
                    break;
                #endregion
                #region Load
                case "load":
                    {
                        DataRow Key = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `macros` WHERE `user_id` = @id LIMIT 1");
                            dbClient.AddParameter("id", Client.GetHabbo().Id);
                            Key = dbClient.getRow();
                        }

                        if (Convert.ToString(Key["key1"]) != "null")
                        {
                            Client.GetHabbo().Macro1_1 = Convert.ToString(Key["key1_1"]);
                            Client.GetHabbo().Macro1_1Key = Convert.ToString(Key["key1"]);
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "macro-add;1_1;" + Client.GetHabbo().Macro1_1Key + ";" + Client.GetHabbo().Macro1_1);
                        }

                        if (Convert.ToString(Key["key2"]) != "null")
                        {
                            Client.GetHabbo().Macro1_2 = Convert.ToString(Key["key1_2"]);
                            Client.GetHabbo().Macro1_2Key = Convert.ToString(Key["key2"]);
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "macro-add;1_2;" + Client.GetHabbo().Macro1_2Key + ";" + Client.GetHabbo().Macro1_2);
                        }

                        if (Convert.ToString(Key["key3"]) != "null")
                        {
                            Client.GetHabbo().Macro1_3 = Convert.ToString(Key["key1_3"]);
                            Client.GetHabbo().Macro1_3Key = Convert.ToString(Key["key3"]);
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "macro-add;1_3;" + Client.GetHabbo().Macro1_3Key + ";" + Client.GetHabbo().Macro1_3);
                        }
                    }
                    break;
                #endregion
                #region Add
                case "add":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Key = ReceivedData[1];
                        string Aktion = ReceivedData[2];

                        if (User.GetClient().GetHabbo().Macro1_1 == "null")
                        {
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `macros` SET `key1` = '" + Key + "', `key1_1` = '" + Aktion + "' WHERE `user_id` = '" + User.GetClient().GetHabbo().Id + "' LIMIT 1; ");
                                dbClient.RunQuery();
                            }
                            User.GetClient().GetHabbo().Macro1_1 = Aktion;
                            User.GetClient().GetHabbo().Macro1_1Key = Key;
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "macro-add;1_1;" + User.GetClient().GetHabbo().Macro1_1Key + ";" + User.GetClient().GetHabbo().Macro1_1);
                        }
                        else if (User.GetClient().GetHabbo().Macro1_2 == "null")
                        {
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `macros` SET `key2` = '" + Key + "', `key1_2` = '" + Aktion + "' WHERE `user_id` = '" + User.GetClient().GetHabbo().Id + "' LIMIT 1; ");
                                dbClient.RunQuery();
                            }
                            User.GetClient().GetHabbo().Macro1_2 = Aktion;
                            User.GetClient().GetHabbo().Macro1_2Key = Key;
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "macro-add;1_2;" + User.GetClient().GetHabbo().Macro1_2Key + ";" + User.GetClient().GetHabbo().Macro1_2);
                        }
                        else if(User.GetClient().GetHabbo().Macro1_3 == "null")
                        {
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `macros` SET `key3` = '" + Key + "', `key1_3` = '" + Aktion + "' WHERE `user_id` = '" + User.GetClient().GetHabbo().Id + "' LIMIT 1; ");
                                dbClient.RunQuery();
                            }
                            User.GetClient().GetHabbo().Macro1_3 = Aktion;
                            User.GetClient().GetHabbo().Macro1_3Key = Key;
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "macro-add;1_3;" + User.GetClient().GetHabbo().Macro1_3Key + ";" + User.GetClient().GetHabbo().Macro1_3);
                        }
                    }
                    break;
                #endregion
                #region Delete
                case "delete":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Slot = ReceivedData[1];

                        if (Slot == "1_1")
                        {
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `macros` SET `key1` = 'null', `key1_1` = 'null' WHERE `user_id` = '" + User.GetClient().GetHabbo().Id + "' LIMIT 1; ");
                                dbClient.RunQuery();
                            }
                            User.GetClient().GetHabbo().Macro1_1 = "null";
                            User.GetClient().GetHabbo().Macro1_1Key = "null";
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "macro-delete;1_1");
                        }
                        if (Slot == "1_2")
                        {
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `macros` SET `key2` = 'null', `key1_2` = 'null' WHERE `user_id` = '" + User.GetClient().GetHabbo().Id + "' LIMIT 1; ");
                                dbClient.RunQuery();
                            }
                            User.GetClient().GetHabbo().Macro1_2 = "null";
                            User.GetClient().GetHabbo().Macro1_2Key = "null";
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "macro-delete;1_2");
                        }
                        if (Slot == "1_3")
                        {
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `macros` SET `key3` = 'null', `key1_3` = 'null' WHERE `user_id` = '" + User.GetClient().GetHabbo().Id + "' LIMIT 1; ");
                                dbClient.RunQuery();
                            }
                            User.GetClient().GetHabbo().Macro1_3 = "null";
                            User.GetClient().GetHabbo().Macro1_3Key = "null";
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "macro-delete;1_3");
                        }
                    }
                    break;
                #endregion
                #region Walk Keys
                case "walk":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Key = ReceivedData[1];

                        if (Key == "up")
                        {
                            User.MoveTo(User.X, User.Y - 1);
                        }
                        else if (Key == "down")
                        {
                            User.MoveTo(User.X, User.Y + 1);
                        }
                        else if (Key == "left")
                        {
                            User.MoveTo(User.X - 1, User.Y);
                        }
                        else if (Key == "right")
                        {
                            User.MoveTo(User.X + 1, User.Y);
                        }
                    }
                    break;
                    #endregion
            }
        }
    }
}