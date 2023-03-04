using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using log4net;
using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Global
{
    public class ServerStatusUpdater : IDisposable
    {
        private static ILog log = LogManager.GetLogger("Mango.Global.ServerUpdater");

        private const int UPDATE_IN_SECS = 30;

        private Timer _timer;
        
        public ServerStatusUpdater()
        {
        }
        public void Init()
        {
            this._timer = new Timer(new TimerCallback(this.OnTick), null, TimeSpan.FromSeconds(UPDATE_IN_SECS), TimeSpan.FromSeconds(UPDATE_IN_SECS));

            Console.Title = PlusEnvironment.Hotelname + " Emulator - 0 Users Online - 0 Rooms Loaded - 0 Day(s), 0 Hour(s), 0 Minute(s)";

            log.Info("Server Status Updater has been started.");
        }
        public void OnTick(object Obj)
        {
            this.UpdateOnlineUsers();
        }
        private void UpdateOnlineUsers()
        {
            TimeSpan Uptime = DateTime.Now - PlusEnvironment.ServerStarted;

            int UsersOnline = Convert.ToInt32(PlusEnvironment.GetGame().GetClientManager().Count);
            int RoomCount = PlusEnvironment.GetGame().GetRoomManager().Count;

            Console.Title = PlusEnvironment.Hotelname + " Emulator - " + UsersOnline + " Users Online - " + RoomCount + " Rooms Loaded - " + Uptime.Days + " Day(s), " + Uptime.Hours + " Hour(s), " + Uptime.Minutes + " Minute(s)";

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `server_status` SET `users_online` = @usersOnline, `loaded_rooms` = @loadedRoomsInt LIMIT 1;");
                dbClient.AddParameter("usersOnline", UsersOnline);
                dbClient.AddParameter("loadedRoomsInt", RoomCount);
                dbClient.RunQuery();
            }
        }
        public void Dispose()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `server_status` SET `users_online` = '0', `loaded_rooms` = '0'");
            }

            this._timer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
