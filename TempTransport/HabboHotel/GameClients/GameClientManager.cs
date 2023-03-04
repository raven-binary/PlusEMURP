using System;
using System.Collections.Generic;
using System.Text;
using ConnectionManager;

using Plus.Core;
using Plus.HabboHotel.Users.Messenger;


using System.Linq;
using System.Collections.Concurrent;
using Plus.Communication.Packets.Outgoing;
using System.Text.RegularExpressions;
using log4net;
using System.Data;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.Database.Interfaces;
using System.Collections;
using Plus.Communication.Packets.Outgoing.Handshake;
using System.Diagnostics;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using WebHook;
using Plus.HabboRoleplay.Misc;

namespace Plus.HabboHotel.GameClients
{
    public class GameClientManager
    {
        private static readonly ILog log = LogManager.GetLogger("Plus.HabboHotel.GameClients.GameClientManager");

        public ConcurrentDictionary<int, GameClient> _clients;
        private ConcurrentDictionary<int, GameClient> _userIDRegister;
        private ConcurrentDictionary<string, GameClient> _usernameRegister;
        private ConcurrentDictionary<int, int> _UserIds;

        private readonly Queue timedOutConnections;

        private readonly Stopwatch clientPingStopwatch;

        public GameClientManager()
        {
            this._clients = new ConcurrentDictionary<int, GameClient>();
            this._userIDRegister = new ConcurrentDictionary<int, GameClient>();
            this._usernameRegister = new ConcurrentDictionary<string, GameClient>();
            this._UserIds = new ConcurrentDictionary<int, int>();

            timedOutConnections = new Queue();

            clientPingStopwatch = new Stopwatch();
            clientPingStopwatch.Start();
        }

        public void OnCycle()
        {
            TestClientConnections();
            HandleTimeouts();
        }

        public GameClient GetClientByUserID(int userID)
        {
            if (_userIDRegister.ContainsKey(userID))
                return _userIDRegister[userID];
            return null;
        }

        public GameClient GetClientByUsername(string username)
        {
            if (_usernameRegister.ContainsKey(username.ToLower()))
                return _usernameRegister[username.ToLower()];
            return null;
        }

        public bool TryGetClient(int ClientId, out GameClient Client)
        {
            return this._clients.TryGetValue(ClientId, out Client);
        }

        public bool UpdateClientUsername(GameClient Client, string OldUsername, string NewUsername)
        {
            if (Client == null || !_usernameRegister.ContainsKey(OldUsername.ToLower()))
                return false;

            _usernameRegister.TryRemove(OldUsername.ToLower(), out Client);
            _usernameRegister.TryAdd(NewUsername.ToLower(), Client);
            return true;
        }

        public string GetNameById(int Id)
        {
            GameClient client = GetClientByUserID(Id);

            if (client != null)
                return client.GetHabbo().Username;

            string username;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT username FROM users WHERE id = @id LIMIT 1");
                dbClient.AddParameter("id", Id);
                username = dbClient.getString();
            }

            return username;
        }

        public IEnumerable<GameClient> GetClientsById(Dictionary<int, MessengerBuddy>.KeyCollection users)
        {
            foreach (int id in users)
            {
                GameClient client = GetClientByUserID(id);
                if (client != null)
                    yield return client;
            }
        }

        public void StaffAlert(ServerPacket Message, int Exclude = 0)
        {
            foreach (GameClient client in this.GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                    continue;

                if (client.GetHabbo().Rank < 2 || client.GetHabbo().Id == Exclude)
                    continue;

                client.SendMessage(Message);
            }
        }

        public void ModAlert(string Message)
        {
            foreach (GameClient client in this.GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                    continue;

                if (client.GetHabbo().GetPermissions().HasRight("mod_tool") && !client.GetHabbo().GetPermissions().HasRight("staff_ignore_mod_alert"))
                {
                    try { client.SendWhisper(Message, 5); }
                    catch { }
                }
            }
        }

        public void DoAdvertisingReport(GameClient Reporter, GameClient Target)
        {
            if (Reporter == null || Target == null || Reporter.GetHabbo() == null || Target.GetHabbo() == null)
                return;

            StringBuilder Builder = new StringBuilder();
            Builder.Append("New report submitted!\r\r");
            Builder.Append("Reporter: " + Reporter.GetHabbo().Username + "\r");
            Builder.Append("Reported User: " + Target.GetHabbo().Username + "\r\r");
            Builder.Append(Target.GetHabbo().Username + "s last 10 messages:\r\r");

            DataTable GetLogs = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `message` FROM `chatlogs` WHERE `user_id` = '" + Target.GetHabbo().Id + "' ORDER BY `id` DESC LIMIT 10");
                GetLogs = dbClient.getTable();

                if (GetLogs != null)
                {
                    int Number = 11;
                    foreach (DataRow Log in GetLogs.Rows)
                    {
                        Number -= 1;
                        Builder.Append(Number + ": " + Convert.ToString(Log["message"]) + "\r");
                    }
                }
            }

            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client == null || Client.GetHabbo() == null)
                    continue;

                if (Client.GetHabbo().GetPermissions().HasRight("mod_tool") && !Client.GetHabbo().GetPermissions().HasRight("staff_ignore_advertisement_reports"))
                    Client.SendMessage(new MOTDNotificationComposer(Builder.ToString()));
            }
        }


        public void SendMessage(ServerPacket Packet, string fuse = "")
        {
            foreach (GameClient Client in this._clients.Values.ToList())
            {
                if (Client == null || Client.GetHabbo() == null)
                    continue;

                if (!string.IsNullOrEmpty(fuse))
                {
                    if (!Client.GetHabbo().GetPermissions().HasRight(fuse))
                        continue;
                }

                Client.SendMessage(Packet);
            }
        }

        public bool CheckIfInUse(int ItemId)
        {
            foreach (GameClient Client in this._clients.Values.ToList())
            {
                if (Client == null || Client.GetHabbo() == null)
                    continue;

                if (Client.GetHabbo().UsingItem == ItemId)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public void PoliceRadio(string Message, int Bubble = 15)
        {
            foreach (GameClient Client in this._clients.Values.ToList())
            {
                if (Client == null || Client.GetHabbo() == null || !Client.GetHabbo().InRoom || Client.GetHabbo().JobId != 1 || !Client.GetHabbo().Working)
                    continue;

                Client.SendWhisper(Message, Bubble);
            }
        }

        public void sendAlertOffi(string msg)
        {
            foreach (GameClient Client in this._clients.Values.ToList())
            {
                if (Client == null || Client.GetHabbo() == null || !Client.GetHabbo().InRoom)
                    continue;

                Client.GetHabbo().sendMsgOffi(msg);
            }
        }

        public void checkIfWinSalade()
        {
            int Count = 0;

            GameClient Session = null;

            foreach (GameClient Client in this._clients.Values.ToList())
            {
                if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().Rank == 8 || Client.GetHabbo().JobId == 18 && Client.GetHabbo().RankId == 2)
                    continue;

                if (Client.GetHabbo().CurrentRoomId == PlusEnvironment.Salade)
                {
                    Count += 1;
                    Session = Client;
                }
            }

            if (Count == 1 && Session != null || Count == 0)
            {
                PlusEnvironment.Salade = 0;
                if (Count == 1)
                {
                    Session.GetHabbo().winSalade();
                    sendAlertOffi(Session.GetHabbo().Username + " won the salade.");
                }
            }
        }

        public string getPoliceMenotte(string Username)
        {
            foreach (GameClient Client in this._clients.Values.ToList())
            {
                if (Username == Client.GetHabbo().MenottedUsername)
                {
                    return Client.GetHabbo().Username;
                }
            }

            return null;
        }

        public void CreateAndStartClient(int clientID, ConnectionInformation connection)
        {
            GameClient Client = new GameClient(clientID, connection);
            if (this._clients.TryAdd(Client.ConnectionID, Client))
                Client.StartConnection();
            else
                connection.Dispose();
        }

        public void DisposeConnection(int clientID)
        {
            GameClient Client = null;
            if (!TryGetClient(clientID, out Client))
                return;

            if (Client != null)
                Client.Dispose(clientID);

            this._clients.TryRemove(clientID, out Client);
        }

        public void removeConnection(int clientID)
        {
            GameClient Client = null;
            this._clients.TryRemove(clientID, out Client);
        }

        public void LogClonesOut(int UserID)
        {
            GameClient client = GetClientByUserID(UserID);
            if (client != null)
                client.Disconnect();
        }

        public void RegisterClient(GameClient client, int userID, string username)
        {
            if (_usernameRegister.ContainsKey(username.ToLower()))
                _usernameRegister[username.ToLower()] = client;
            else
                _usernameRegister.TryAdd(username.ToLower(), client);

            if (_userIDRegister.ContainsKey(userID))
                _userIDRegister[userID] = client;
            else
                _userIDRegister.TryAdd(userID, client);

            _UserIds.TryAdd(userID, userID);
        }

        public void UnregisterClient(int userid, string username)
        {
            GameClient Client = null;
            _userIDRegister.TryRemove(userid, out Client);
            _usernameRegister.TryRemove(username.ToLower(), out Client);
        }

        public void CloseAll()
        {
            foreach (GameClient client in this.GetClients.ToList())
            {
                if (client == null)
                    continue;

                if (client.GetHabbo() != null)
                {
                    try
                    {
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunQuery(client.GetHabbo().GetQueryString);
                        }
                        log.Info("<<- SERVER SHUTDOWN ->> SAVING CLIENTS");
                    }
                    catch
                    {
                    }
                }
            }
            RoleplayManager.SaveData();

            log.Info("Done saving users inventory");
            log.Info("Closing server connections...");
            try
            {
                foreach (GameClient client in this.GetClients.ToList())
                {
                    if (client == null || client.GetConnection() == null)
                        continue;

                    try
                    {
                        client.GetConnection().Dispose();
                    }
                    catch { }

                    //Console.Clear();
                    //log.Info("<<- SERVER SHUTDOWN ->> CLOSING CONNECTIONS");

                }
            }
            catch (Exception e)
            {
                Logging.LogCriticalException(e.ToString());
            }

            if (this._clients.Count > 0)
                this._clients.Clear();

            log.Info("Connections closed!");
        }

        private void TestClientConnections()
        {
            if (clientPingStopwatch.ElapsedMilliseconds >= 30000)
            {
                clientPingStopwatch.Restart();

                try
                {
                    List<GameClient> ToPing = new List<GameClient>();

                    foreach (GameClient client in this._clients.Values.ToList())
                    {
                        if (client.PingCount < 6)
                        {
                            client.PingCount++;

                            ToPing.Add(client);
                        }
                        else
                        {
                            lock (timedOutConnections.SyncRoot)
                            {
                                timedOutConnections.Enqueue(client);
                            }
                        }
                    }

                    DateTime start = DateTime.Now;

                    foreach (GameClient Client in ToPing.ToList())
                    {
                        try
                        {
                            Client.SendMessage(new PongComposer());
                        }
                        catch
                        {
                            lock (timedOutConnections.SyncRoot)
                            {
                                timedOutConnections.Enqueue(Client);
                            }
                        }
                    }

                }
                catch (Exception e)
                {

                }
            }
        }

        private void HandleTimeouts()
        {
            if (timedOutConnections.Count > 0)
            {
                lock (timedOutConnections.SyncRoot)
                {
                    while (timedOutConnections.Count > 0)
                    {
                        GameClient client = null;

                        if (timedOutConnections.Count > 0)
                            client = (GameClient)timedOutConnections.Dequeue();

                        if (client != null)
                            client.Disconnect();
                    }
                }
            }
        }

        public int Count
        {
            get { return this._clients.Count; }
        }

        public ICollection<GameClient> GetClients
        {
            get
            {
                return this._clients.Values;
            }
        }

        public int userBankWorking()
        {
            int count = 0;
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client != null && Client.GetHabbo() != null && Client.GetHabbo().JobId == 4 && Client.GetHabbo().Working == true)
                {
                    count++;
                }
            }
            return count;
        }

        public int userCroupierWorking()
        {
            int count = 0;
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client != null && Client.GetHabbo() != null && Client.GetHabbo().JobId == 16 && Client.GetHabbo().Working == true)
                {
                    count++;
                }
            }
            return count;
        }

        public int UsersWorking(int CorpId)
        {
            int count = 0;
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client != null && Client.GetHabbo() != null && Client.GetHabbo().JobId == CorpId && Client.GetHabbo().Working)
                {
                    count++;
                }
            }
            return count;
        }

        public void CorpBonus(int CorpId, int Bonus)
        {
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client == null || Client.GetConnection() == null || Client.GetHabbo() == null || Client.GetHabbo().CurrentRoom == null || Client.GetHabbo().JobId != CorpId || !Client.GetHabbo().Working)
                    continue;

                Client.SendWhisper("You have received a $" + Bonus + " bonus!", 4);
                Client.GetHabbo().Credits += Bonus;
                Client.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                Client.GetHabbo().RPCache(3);
            }
        }

        public bool checkCapture(int RoomId)
        {
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client.GetHabbo().CurrentRoomId == RoomId)
                {
                    RoomUser User = Client.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Username);

                    if (User.Capture != 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void endPurge()
        {
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client == null || Client.GetConnection() == null || Client.GetHabbo() == null || Client.GetHabbo().CurrentRoom == null)
                    continue;

                if(Client.GetHabbo().ArmeEquiped != null && Client.GetHabbo().CurrentRoom.Description.Contains("INDOOR") && Client.GetHabbo().ArmeEquiped != "taser")
                {
                    Client.GetHabbo().ArmeEquiped = null;
                    Room Room = Client.GetHabbo().CurrentRoom;
                    RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                    Client.GetHabbo().resetEffectEvent();
                    User.GetClient().Shout("*Entfernt seine Waffen*");
                }
            }
        }

        public void sendServerMessage(string Msg)
        {
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client == null || Client.GetConnection() == null || Client.GetHabbo() == null)
                    continue;

                Client.SendWhisper("[HOTEL ALERT] " + Msg);
            }
        }

        public void PurgeSound(bool Pause)
        {
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client == null || Client.GetConnection() == null)
                    continue;

                try
                {
                    if (Pause)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "purge;stop");
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "purge;start");
                    }
                }
                catch { }
            }
        }

        public bool PoliceOnDuty(int Id)
        {
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client == null || Client.GetConnection() == null || Client.GetHabbo() == null || Client.GetHabbo().JobId != 1 || !Client.GetHabbo().Working || Client.GetHabbo().Id == Id)
                    continue;

                return true;
            }

            return false;
        }

        public string appelPolice(int Id)
        {
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client == null || Client.GetConnection() == null || Client.GetHabbo() == null || Client.GetHabbo().JobId != 2 || Client.GetHabbo().Working == false || Client.GetHabbo().Telephone == 0 || Client.GetHabbo().TelephoneEteint == true || Client.GetHabbo().inCallWithUsername != null || Client.GetHabbo().receiveCallUsername != null || Client.GetHabbo().isCalling == true || Client.GetHabbo().Id == Id)
                    continue;

                return Client.GetHabbo().Username;
            }

            return null;
        }

        public void sendStaffMsg(string Msg)
        {
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().Rank != 8)
                    continue;

                Client.SendWhisper("[STAFF ALERT] " + Msg);
            }
        }

        public void HotelWhisper(string Message)
        {
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client == null || Client.GetHabbo() == null)
                    continue;

                Client.SendWhisper(Message, 34);
            }
        }

        public int footballCountUserPlay(Room room, string Color)
        {
            int count = 0;
            foreach (RoomUser roomUser in room.GetRoomUserManager().GetUserList().ToList())
            {
                if (roomUser == null || roomUser.GetClient() == null || roomUser.IsPet || roomUser.IsBot || roomUser.GetClient().GetHabbo().footballTeam == null)
                    continue;

                if(roomUser.GetClient().GetHabbo().footballTeam == Color)
                {
                    count += 1;
                }
            }

            return count;
        }

        public void footballEndMatch()
        {
            PlusEnvironment.footballResetScore();

            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client == null || Client.GetConnection() == null || Client.GetHabbo().CurrentRoomId == 56 || Client.GetHabbo().footballTeam == null)
                    continue;

                Client.GetHabbo().footballTeam = null;
                Client.GetHabbo().footballSpawnChair();
            }
        }

        public void footballUpdateHtmlScore()
        {
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client == null || Client.GetConnection() == null || Client.GetHabbo().CurrentRoomId == 56)
                    continue;

                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "footballScore;show;" + PlusEnvironment.footballGoalGreen + ";" + PlusEnvironment.footballGoalBlue);
            }
        }

        public void footballSendUserInSpawn()
        {
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client == null || Client.GetConnection() == null || Client.GetHabbo().CurrentRoomId == 56)
                    continue;

                if (Client.GetHabbo().footballTeam == "green")
                {
                    Client.GetHabbo().footballSpawnInItem("green");
                }
                else if (Client.GetHabbo().footballTeam == "blue")
                {
                    Client.GetHabbo().footballSpawnInItem("blue");
                }
            }
        }

        public void LiveFeed(string Msg)
        {
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client == null || Client.GetConnection() == null || !Client.GetRoleplay().Livefeed)
                    continue;

                try
                {
                    PlusEnvironment.LastActionMessage += 1;
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "live-feed;" + PlusEnvironment.LastActionMessage + ";" + Msg);
                }
                catch (Exception E)
                {

                }
            }
        }

        public void sendLastActionMsg(string Msg)
        {
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client == null || Client.GetConnection() == null)
                    continue;

                try
                {
                    PlusEnvironment.LastActionMessage += 1;
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "last_action;" + PlusEnvironment.LastActionMessage + ";" + Msg);
                }
                catch(Exception E)
                {

                }
            }
        }

        public void sendGangMsg(int Gang, string Msg)
        {
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client == null || Client.GetConnection() == null || Client.GetHabbo() == null)
                    continue;

                if (Client.GetHabbo().Gang != Gang)
                    continue;

                Client.SendWhisper(Msg, 4);
            }
        }

        public void GangWhisper(int Gang, string Username, string Msg)
        {
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client == null || Client.GetConnection() == null || Client.GetHabbo() == null || Client.GetHabbo().Gang != Gang)
                    continue;

                Client.SendWhisper(Username + ": " + Msg, 6);
            }
        }

        public void TurfAlert(int Gang, string Action)
        {
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client == null || Client.GetConnection() == null || Client.GetHabbo() == null)
                    continue;

                if (Client.GetHabbo().Gang != Gang)
                    continue;

                if (Action == "show")
                {
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "turf-alert;show");
                }
                else if (Action == "hide")
                {
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "turf-alert;hide");
                }
            }
        }

        public void resetQuizz()
        {
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client == null || Client.GetConnection() == null || Client.GetHabbo() == null)
                    continue;

                if (Client.GetHabbo().Quizz_Points != 0)
                    Client.GetHabbo().Quizz_Points = 0;
            }
        }

        public void sendGouvMsg(string Msg)
        {
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client == null || Client.GetConnection() == null || Client.GetHabbo() == null)
                    continue;

                Client.SendWhisper("[MESSAGE FROM GOUVERNEMENT] " + Msg);
            }
        }

        public void HotelAlert(string Button, string Message, bool Sound = true)
        {
            if (Regex.IsMatch(Button, @"^\d+$"))
            {
                PlusEnvironment.HotelAlertType = "teleport";
                PlusEnvironment.HotelAlertTeleport = Convert.ToInt32(Button);
            }
            else if (Button.StartsWith("http") || Button.StartsWith("https"))
            {
                PlusEnvironment.HotelAlertType = "link";
                PlusEnvironment.HotelAlertLink = Button;
            }
            
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client == null || Client.GetHabbo() == null)
                    continue;

                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "hotel-alert;" + PlusEnvironment.Hotelname + " News;" + Message + ";" + Button + ";" + Sound);
            }
        }

        public void WorldEvent(string action, int startTime, int runningTime, string title, string description, int joinButton, int teleportButton, int teleportRoomId)
        {
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client == null || Client.GetConnection() == null || Client.GetHabbo() == null)
                    continue;

                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "world-event;" + action + ";" + startTime + ";" + runningTime + ";" + title + ";" + description + ";" + joinButton + ";" + teleportButton + ";" + teleportRoomId);
            }
        }

        public void StaffWhisper(string Message)
        {
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client != null && Client.GetHabbo() != null && (Client.GetHabbo().Rank > 5))
                {
                    Client.SendWhisper("[STAFF ALERT] " + Message, 5);
                }
            }
        }

        public void ParamedicCall(string Username, string RoomName)
        {
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client != null && Client.GetHabbo() != null && Client.GetHabbo().JobId == 2 && Client.GetHabbo().Working && Client.GetHabbo().RankInfo.RankId < 5 && Client.GetHabbo().ParamedicUsername == null)
                {
                    RoomUser User = Client.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Username);

                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "paramedic-call;show;" + Username + ";<strong>" + Username + "</strong> has died at <strong>" + RoomName + "</strong> and requires transportation");
                }
            }
        }

        public void PoliceCall(int UserId, string Username, string Look, string Room, int RoomId, string Message, int ExpireSeconds)
        {
            PlusEnvironment.PoliceGeneralCalls += 1;
            PlusEnvironment.PoliceCalls.Add(PlusEnvironment.PoliceGeneralCalls, RoomId);
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `police_calls` (`id`,`user_id`,`room_id`,`message`,`responded_cop_id`) VALUES (@Id, @userId, @roomId, @message, '0')");
                dbClient.AddParameter("Id", PlusEnvironment.PoliceGeneralCalls);
                dbClient.AddParameter("userId", UserId);
                dbClient.AddParameter("roomId", RoomId);
                dbClient.AddParameter("message", Message);
                dbClient.RunQuery();
            }

            int CallId = PlusEnvironment.PoliceGeneralCalls;

            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client != null && Client.GetHabbo() != null && (Client.GetHabbo().JobId == 1))
                {
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "police-call;" + Username + ";" + Look + ";["+  RoomId + "] " + Room + ";" + RoomId + ";" + Message + ";" + CallId);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "police-current-call;" + CallId + ";" + PlusEnvironment.PoliceGeneralCalls);

                    if (Client.GetHabbo().Working)
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "police-calls;show");
                }
            }

            System.Timers.Timer ExpireTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(ExpireSeconds));
            ExpireTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(ExpireSeconds);
            ExpireTimer.Elapsed += delegate
            {
                PlusEnvironment.ExpiredPoliceCalls.Add(CallId, CallId);
                ExpireTimer.Stop();
            };
            ExpireTimer.Start();
        }

        public void PoliceRespond(int CallId, string Username)
        {
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client != null && Client.GetHabbo() != null && (Client.GetHabbo().JobId == 1))
                {
                    RoomUser User = Client.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Username);

                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "police-call-respond;" + CallId + ";" + Username);
                }
            }
        }

        public void LoadWantedList(bool Remove = false, int Id = 0)
        {
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client != null && Client.GetHabbo() != null)
                {
                    if (Remove == true)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "wanted-list-remove;" + Id);
                        if (PlusEnvironment.WantedListColor)
                        {
                            PlusEnvironment.WantedListColor = false;
                        }
                        LoadWantedList();
                    }
                    else if (PlusEnvironment.WantedList.Count == 0)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "wanted-list-empty");
                        PlusEnvironment.WantedListColor = true;
                    }
                    else
                    {
                        foreach (var UserId in PlusEnvironment.WantedList.Keys)
                        {
                            if (PlusEnvironment.WantedListColor)
                            {
                                PlusEnvironment.WantedListColor = false;
                            }
                            else
                            {
                                PlusEnvironment.WantedListColor = true;
                            }
                            GameClient User = GetClientByUserID(UserId);
                            if (User != null)
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "wanted-list;" + PlusEnvironment.WantedList.Count + ";" + User.GetHabbo().Id + ";" + User.GetHabbo().Username + ";" + User.GetHabbo().Look + ";" + User.GetHabbo().CheckChargess() + ";" + User.GetHabbo().assault + ";" + User.GetHabbo().murder + ";" + User.GetHabbo().ganghomicide + ";" + User.GetHabbo().copassault + ";" + User.GetHabbo().copmurder + ";" + User.GetHabbo().obstruction + ";" + User.GetHabbo().hacking + ";" + User.GetHabbo().trespassing + ";" + User.GetHabbo().robbery + ";" + User.GetHabbo().illegalarea + ";" + User.GetHabbo().jailbreak + ";" + User.GetHabbo().terrorism + ";" + User.GetHabbo().drugs + ";" + User.GetHabbo().execution + ";" + User.GetHabbo().escaping + ";" + User.GetHabbo().nonCompliance + ";" + User.GetHabbo().callAbuse + ";" + User.GetHabbo().WantedPassed + ";" + (PlusEnvironment.WantedListColor ? "bg-dark-0" : "bg-dark-1"));
                            }
                        }
                    }
                }
            }
        }

        public void ATMCall(string RoomName, int ATMId)
        {
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client != null && Client.GetHabbo() != null && Client.GetHabbo().JobId == 3 && Client.GetHabbo().Working && Client.GetHabbo().RankId < 5)
                {
                    Client.SendWhisper("An ATM is low on funds at " + RoomName + " and requires refilling", 6);
                    //PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "atm-call;show;" + RoomName + ";" + ATMId);
                }
            }
        }
        public void ATMClaimedWhisper(string Username, int ATMId)
        {
            DataRow ATM = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `atms` WHERE `atm_id` = @id LIMIT 1;");
                dbClient.AddParameter("id", ATMId);
                ATM = dbClient.getRow();
            }

            DataRow Items = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `items` WHERE `id` = @id LIMIT 1;");
                dbClient.AddParameter("id", ATMId);
                Items = dbClient.getRow();
            }

            DataRow Rooms = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `rooms` WHERE `id` = @id LIMIT 1;");
                dbClient.AddParameter("id", Convert.ToInt32(Items["room_id"]));
                Rooms = dbClient.getRow();
            }

            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client != null && Client.GetHabbo() != null && Client.GetHabbo().JobId == 3 && Client.GetHabbo().Working && Client.GetHabbo().RankInfo.RankId < 5)
                {
                    Client.SendWhisper(Username + " claimed the ATM refill for " + Rooms["caption"], 6);
                }
            }
        }
        public int PickRandomUserId()
        {
            List<int> keyList = new List<int>(_UserIds.Keys);
            Random rand = new Random();
            int randomKey = keyList[rand.Next(keyList.Count)];
            return _UserIds[randomKey];
        }
    }
}