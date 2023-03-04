using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fleck;
using log4net;
using Newtonsoft.Json;

using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Cache;
using System.Collections.Concurrent;
using Plus.Core;
using Plus;
using Bobba.HabboRoleplay.Web.Outgoing;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;

namespace Bobba.HabboRoleplay.Web
{
    public class WebSocketUser
    {
        /// <summary>
        /// The username string.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The ID int.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The Closing bool.
        /// </summary>
        public bool Closing { get; set; }

        /// <summary>
        /// The IWebSocketConnection void.
        /// </summary>
        public IWebSocketConnection Connection { get; set; }

        /// <summary>
        /// WebSocketUser construct.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Username"></param>
        /// <param name="Connection"></param>
        public WebSocketUser(int ID, string Username, IWebSocketConnection Connection)
        {
            this.ID = ID;
            this.Username = Username;
            this.Closing = false;
            this.Connection = Connection;
        }

        /// <summary>
        /// Dispose void to dispose socket connections.
        /// </summary>
        public void Dispose()
        {
            this.ID = 0;
            this.Username = null;
            this.Closing = true;
        }
    }

    /// <summary>
    /// WebEventManager class.
    /// </summary>
    public sealed class WebEventManager
    {
        /// <summary>
        /// log4net.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger("Plus.HabboHotel.Roleplayer.Web.WebEventManager");

        /// <summary>
        /// _webSocketServer.
        /// </summary>
        public WebSocketServer _webSocketServer;

        /// <summary>
        /// Concurrent dictionary containing websocket connections.
        /// </summary>
        public ConcurrentDictionary<IWebSocketConnection, WebSocketUser> _webSockets;

        /// <summary>
        /// Concurrent dictionary containing web events.
        /// </summary>
        private ConcurrentDictionary<string, IWebEvent> _webEvents;

        /// <summary>
        /// WebEventManager function.
        /// </summary>
        public WebEventManager()
        {

            string IP = PlusEnvironment.GetConfig().data["ws.tcp.bindip"];
            int Port = int.Parse(PlusEnvironment.GetConfig().data["ws.tcp.port"]);
            this._webSocketServer = new WebSocketServer("wss://" + IP + ":" + Port);
            _webSocketServer.Certificate = new X509Certificate2(@"C:\habbo.pfx", "lol123lol");
            _webSocketServer.EnabledSslProtocols = SslProtocols.Ssl3 | SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
            this._webSockets = new ConcurrentDictionary<IWebSocketConnection, WebSocketUser>();
            this._webEvents = new ConcurrentDictionary<string, IWebEvent>();
            this.RegisterOutgoing();
        }

        /// <summary>
        /// Initializes the websocket connection.
        /// </summary>
        public void Init()
        {
            this._webSocketServer.ListenerSocket.NoDelay = true;
            this._webSocketServer.Start(ConnectingSocket =>
            {
                ConnectingSocket.OnOpen = () => this.OnSocketAdd(ConnectingSocket);
                ConnectingSocket.OnClose = () => this.OnSocketRemove(ConnectingSocket);
                ConnectingSocket.OnMessage = SocketData => this.OnSocketMessage(ConnectingSocket, SocketData);
                ConnectingSocket.OnError = SocketError => this.OnSocketError(SocketError.ToString());
            });
        }

        /// <summary>
        /// Registers the outgoing web events.
        /// </summary>
        public void RegisterOutgoing()
        {
            this._webEvents.TryAdd("login", new LoginWebEvent());
            this._webEvents.TryAdd("fontaine", new FoutainWebEvent());
            this._webEvents.TryAdd("transaction", new TransactionWebEvent());
            this._webEvents.TryAdd("atm", new ATMWebEvent());
            this._webEvents.TryAdd("panier", new PanierWebEvent());
            this._webEvents.TryAdd("item", new ItemWebEvent());
            this._webEvents.TryAdd("gang", new GangWebEvent());
            this._webEvents.TryAdd("look", new LookWebEvent());
            this._webEvents.TryAdd("appart", new AppartWebEvent());
            this._webEvents.TryAdd("casino", new SlotWebEvent());
            this._webEvents.TryAdd("trade", new TradeWebEvent());
            this._webEvents.TryAdd("taxi", new TaxiWebEvent());
            this._webEvents.TryAdd("shop", new ShopWebEvent());
            this._webEvents.TryAdd("jobcenter", new JobCenterWebEvent());
            //this._webEvents.TryAdd("target", new TargetUserWebEvent());
            this._webEvents.TryAdd("clothing", new ClothingWebEvent());
            this._webEvents.TryAdd("inventory", new InventoryWebEvent());
            this._webEvents.TryAdd("bin", new BinWebEvent());
            this._webEvents.TryAdd("depositbox", new DepositBoxWebEvent());
            this._webEvents.TryAdd("profile", new ProfileWebEvent());
            this._webEvents.TryAdd("worldevent", new WorldEventWebEvent());
            this._webEvents.TryAdd("target", new TargetWebEvent());
            this._webEvents.TryAdd("macro", new MacroWebEvent());
            this._webEvents.TryAdd("hotelalert", new HotelAlertWebEvent());
            this._webEvents.TryAdd("skins", new SkinsWebEvent());
            this._webEvents.TryAdd("settings", new SettingsWebEvent());
            this._webEvents.TryAdd("turf", new TurfWebEvent());
            this._webEvents.TryAdd("police", new PoliceWebEvent());
            this._webEvents.TryAdd("paramedic", new ParamedicWebEvent());
            this._webEvents.TryAdd("cards", new CardsWebEvent());
            this._webEvents.TryAdd("jukebox", new JukeboxWebEvent());
            this._webEvents.TryAdd("bounty", new BountyWebEvent());
            this._webEvents.TryAdd("marketplace", new MarketplaceWebEvent());
            this._webEvents.TryAdd("court", new JuryDutyWebEvent());
        }

        /// <summary>
        /// On socket data received
        /// </summary>
        /// <param name="InteractingSocket"></param>
        /// <param name="SentData"></param>
        private void OnSocketMessage(IWebSocketConnection InteractingSocket, string SentData)
        {
            try
            {
                var ReceivedData = JsonConvert.DeserializeObject<WebEvent>(SentData);

                if (string.IsNullOrEmpty(ReceivedData.EventName))
                    return;

                GameClient InteractingClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(ReceivedData.UserId);

                if (InteractingClient == null)
                    return;

                if (InteractingClient.LoggingOut)
                    return;

                if (!this._webSockets.ContainsKey(InteractingSocket))
                    return;

                IWebEvent webEvent = null;

                if (_webEvents.TryGetValue(ReceivedData.EventName, out webEvent))
                {
                    webEvent.Execute(InteractingClient, ReceivedData.ExtraData, InteractingSocket);
                    return;
                }

                log.Debug("Unrecognized Web Event: '" + ReceivedData.EventName + "'");

            }
            catch (Exception ex)
            {
                if (ex is System.IO.IOException)
                    return;

                this.OnSocketError(ex.Message);
            }
        }

        /// <summary>
        /// OnSocketAdd.
        /// </summary>
        /// <param name="NewUser"></param>
        public void OnSocketAdd(IWebSocketConnection ConnectingSocket)
        {
            if (ConnectingSocket == null)
                return;

            if (!ConnectingSocket.IsAvailable)
                return;

            if (ConnectingSocket.ConnectionInfo == null)
                return;

            if (string.IsNullOrEmpty(ConnectingSocket.ConnectionInfo.Path))
                return;

            if (!ConnectingSocket.ConnectionInfo.Path.Trim().Contains("/"))
                return;

            if ((string.IsNullOrEmpty(ConnectingSocket.ConnectionInfo.Path.Trim().Split('/')[1])))
                return;

            try
            {
                WebSocketUser outUser;

                int ConnectingUsersID = this.GetSocketsUserID(ConnectingSocket);
                List<GameClient> PotentialConnectedClients = this.GetSocketsClient(ConnectingSocket);

                if (PotentialConnectedClients.Count > 0)
                {
                    this.DeactivateSocket(ConnectingSocket);
                    return;
                }

                WebSocketUser User = null;
                if (this._webSockets.ContainsKey(ConnectingSocket))
                    this._webSockets.TryRemove(ConnectingSocket, out User);
                
                this._webSockets.TryAdd(ConnectingSocket, new WebSocketUser(ConnectingUsersID, "", ConnectingSocket));


            }
            catch (Exception ex)
            {
                if (ex is System.IO.IOException)
                    return;

                this.OnSocketError(ex.Message);
            }
        }

        /// <summary>
        /// OnSocketRemove.
        /// </summary>
        /// <param name="User"></param>
        public void OnSocketRemove(IWebSocketConnection User)
        {
            //WebSocketUser outUser;

            if (!this._webSockets.ContainsKey(User))
                return;

            try
            {
                this.CloseSimilarSockets(GetSocketsUserID(User));
            }
            catch (Exception ex)
            {
                if (ex is System.IO.IOException)
                    return;

                this.OnSocketError(ex.Message);
            }
        }

        /// <summary>
        /// Closes any similar sockets with the target user ID
        /// </summary>
        /// <param name="Id"></param>
        public void CloseSimilarSockets(int Id)
        {
            List<IWebSocketConnection> SocketsToClose = GetSimilarSockets(Id);

            foreach (IWebSocketConnection Socket in SocketsToClose)
            {
                this.DeactivateSocket(Socket);
            }
        }

        public List<IWebSocketConnection> GetSimilarSockets(int Id)
        {
            List<IWebSocketConnection> SimilarSockets = new List<IWebSocketConnection>();

            foreach (KeyValuePair<IWebSocketConnection, WebSocketUser> AvailableSockets in this._webSockets)
            {
                if (AvailableSockets.Value == null)
                    continue;

                if (AvailableSockets.Key == null)
                    continue;

                if (this.GetSocketsUserID(AvailableSockets.Key) == Id)
                    SimilarSockets.Add(AvailableSockets.Key);
            }

            return SimilarSockets;
        }

        /// <summary>
        /// OnSocketError.
        /// </summary>
        /// <param name="Error"></param>
        public void OnSocketError(string Error)
        {
            Logging.LogWebSocketError(Error);
        }

        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="Client"></param>
        /// <param name="EventName"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public bool ExecuteWebEvent(GameClient Client, string EventName, string ReceivedData)
        {
            try
            {
                #region User client null checks

                if (ReceivedData.StartsWith("{"))
                {
                    // JSON
                }
                else
                {
                    if (!ReceivedData.Contains("bypass"))
                    {
                        if (Client == null)
                            return false;

                        if (Client.LoggingOut)
                            return false;

                        if (!this.SocketReady(Client))
                            return false;
                    }
                }

                if (string.IsNullOrEmpty(EventName))
                    return false;

                #endregion

                #region Socket null checks

                IWebSocketConnection InteractingSocket = Client.GetHabbo().WebSocketConnection;

                if (!this.SocketReady(InteractingSocket))
                    return false;

                #endregion

                #region Execute event
                IWebEvent webEvent = null;
                if (this._webEvents.TryGetValue(EventName, out webEvent))
                {
                    if (!this._webSockets[InteractingSocket].Closing || !InteractingSocket.IsAvailable)
                        webEvent.Execute(Client, ReceivedData, InteractingSocket);
                    return true;
                }
                #endregion

            }
            catch (Exception ex)
            {
                if (ex is System.IO.IOException)
                    return false;

                this.OnSocketError(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Broadcasts the web event.
        /// </summary>
        /// <param name="EventName"></param>
        /// <param name="Data"></param>
        public void BroadCastWebEvent(string EventName, string Data)
        {
            foreach (GameClient User in PlusEnvironment.GetGame().GetClientManager().GetClients)
            {
                if (User == null)
                    continue;

                if (User.LoggingOut)
                    continue;

                this.ExecuteWebEvent(User, EventName, Data);
            }
        }

        /// <summary>
        /// Sends the given user web data.
        /// </summary>
        /// <param name="User"></param>
        /// <param name="Data"></param>
        public void SendDataDirect(GameClient User, string Data)
        {
            if (!this.SocketReady(User, true))
                return;

            if (User.GetHabbo().WebSocketConnection == null)
                return;

            if (!this.SocketReady(User.GetHabbo().WebSocketConnection))
                return;

            try
            {
                User.GetHabbo().WebSocketConnection.Send(Data);
            }
            catch (Exception ex)
            {
                if (ex is System.IO.IOException)
                    return;

                this.OnSocketError(ex.Message);
            }
        }

        /// <summary>
        /// Broadcasts the web data.
        /// </summary>
        /// <param name="Data"></param>
        public void BroadCastWebData(string Data)
        {
            foreach (GameClient User in PlusEnvironment.GetGame().GetClientManager().GetClients)
            {
                if (!this.SocketReady(User, true))
                    continue;

                if (!this.SocketReady(User.GetHabbo().WebSocketConnection))
                    continue;

                try
                {
                    User.GetHabbo().WebSocketConnection.Send(Data);
                }
                catch (Exception ex)
                {
                    if (ex is System.IO.IOException)
                        return;

                    this.OnSocketError(ex.Message);
                }
            }
        }

        /// <summary>
        /// SocketReady.
        /// </summary>
        /// <param name="Socket"></param>
        /// <returns></returns>
        public bool SocketReady(IWebSocketConnection Socket)
        {
            if (Socket == null)
                return false;

            if (!this._webSockets.ContainsKey(Socket))
                return false;

            if (this._webSockets[Socket].Closing)
                return false;

            if (!Socket.IsAvailable)
                return false;


            return true;
        }

        /// <summary>
        /// Checks if the socket is ready to be interacted with
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public bool SocketReady(GameClient User, bool Logout = false)
        {
            if (User == null)
                return false;

            if (User.GetHabbo() == null)
                return false;

            if (!this.SocketReady(User.GetHabbo().WebSocketConnection))
                return false;

            if (Logout)
            {
                if (User.LoggingOut)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the user connection.
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public IWebSocketConnection GetUsersConnection(GameClient User)
        {

            IWebSocketConnection UsersSocket = null;

            /*if (User == null)
                return null;

            if (User.GetHabbo() == null)
                return null;

            if (User.LoggingOut)
                return null;*/

            UsersSocket = _webSockets
                            .Where(MySockets => this.GetSocketsUserID(MySockets.Key) == User.GetHabbo().Id)
                            .Where(MySockets => SocketReady(MySockets.Key)).FirstOrDefault().Key;


            if (!this.SocketReady(UsersSocket))
                return null;

            return UsersSocket;
        }

        /// <summary>
        /// Retrieves the sockets userID from its ConnectionInformation
        /// </summary>
        /// <param name="Socket"></param>
        /// <returns></returns>
        public int GetSocketsUserID(IWebSocketConnection Socket)
        {
            if (Socket == null)
                return 0;

            if (Socket.ConnectionInfo == null)
                return 0;

            if (Socket.ConnectionInfo.Path == null)
                return 0;

            if (String.IsNullOrEmpty(Socket.ConnectionInfo.Path))
                return 0;

            return Convert.ToInt32(Socket.ConnectionInfo.Path.Trim().Split('/')[1]);
        }

        /// <summary>
        /// Gets the client assosciated with the targeted socket
        /// </summary>
        /// <param name="Socket"></param>
        /// <returns></returns>
        public List<GameClient> GetSocketsClient(IWebSocketConnection Socket)
        {

            int SocketsUserId = this.GetSocketsUserID(Socket);

            List<GameClient> RunningClients = PlusEnvironment.GetGame().GetClientManager().GetClients.ToList().
               Where(Client => Client != null).
               Where(Client => Client.LoggingOut != true).
               Where(Client => Client.GetHabbo() != null).
               Where(Client => Client.GetHabbo().Id == SocketsUserId).
               Where(Client => this.SocketReady(Client)).ToList();

            return RunningClients;
        }

        /// <summary>
        /// Gets _websockets dictionary
        /// </summary>
        /// <returns></returns>
        public ConcurrentDictionary<IWebSocketConnection, WebSocketUser> GetConnectedUsers()
        {
            return this._webSockets;
        }

        /// <summary>
        /// Completely shuts down a targeted Socket
        /// </summary>
        /// <param name="Socket"></param>
        public void DeactivateSocket(IWebSocketConnection Socket)
        {

            if (this._webSockets.ContainsKey(Socket))
            {
                WebSocketUser user = null;
                this._webSockets[Socket].Closing = true;
                this._webSockets[Socket].Dispose();
                this._webSockets.TryRemove(Socket, out user);
            }

            Socket.Close();
        }

        /// <summary>
        /// Closes any sockets assosciated with the character/userID 
        /// </summary>
        /// <param name="SocketUserID"></param>
        public void CloseSocketByGameClient(int SocketUserID)
        {
            if (SocketUserID == 0)
                return;

            try
            {
                this.CloseSimilarSockets(SocketUserID);
            }
            catch (Exception e)
            {

            }
        }
    }
}
