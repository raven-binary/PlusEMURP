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
    class JukeboxWebEvent : IWebEvent
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
                #region request
                case "request":
                    {
                        if (Client.GetHabbo().Rank < 8)
                            return;


                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string VideoUrl = ReceivedData[1];

                        if (VideoUrl.Contains(";") || VideoUrl == "")
                        {
                            Client.SendWhisper("The video ID is invalid");
                            return;
                        }

                        System.Net.WebClient wc = new System.Net.WebClient();

                        var json = wc.DownloadString("https://www.youtube.com/oembed?format=json&url=https://www.youtube.com/watch?v=" + VideoUrl);
                        string jsonResponse = wc.DownloadString("https://www.googleapis.com/youtube/v3/videos?id=" + VideoUrl + "&key=AIzaSyBEYBwbKWC9yceFn4p7PHrgruhMzLeYqAg&part=contentDetails");
                        dynamic myClass = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                        dynamic dynamicObject = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);
                        bool Music = dynamicObject.items[0].contentDetails.licensedContent;

                        if (!Music)
                        {
                            Client.SendWhisper("Please choose a music video");
                            return;
                        }

                        string tmp = dynamicObject.items[0].contentDetails.duration;
                        var Duration = Convert.ToInt32(System.Xml.XmlConvert.ToTimeSpan(tmp).TotalSeconds);

                        if (Duration >= 300)
                        {
                            Client.SendWhisper("Please choose a music video less than 5 minutes");
                            return;
                        }
                        else if (Duration < 60)
                        {
                            Client.SendWhisper("Please choose a music video more than 1 minute");
                            return;
                        }

                        if (!PlusEnvironment.JukeboxPlaying)
                        {
                            PlusEnvironment.JukeboxPlaying = true;
                            PlusEnvironment.JukeboxNowPlaying = VideoUrl;

                            foreach (RoomUser UserInRoom in Client.GetHabbo().CurrentRoom.GetRoomUserManager().GetUserList().ToList())
                            {
                                if (UserInRoom == null || UserInRoom.IsBot || UserInRoom.GetClient() == null)
                                    continue;

                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(UserInRoom.GetClient(), "jukebox-play;start;" + VideoUrl + ";" + Convert.ToString(myClass.title));
                            }

                            System.Timers.Timer Timer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(Convert.ToInt32(Duration + 1)));
                            Timer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(Convert.ToInt32(Duration + 1));
                            Timer.Elapsed += delegate
                            {
                                PlusEnvironment.JukeboxPlaying = false;
                                PlusEnvironment.JukeboxNowPlaying = null;

                                DataRow Next = null;
                                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT * FROM `jukebox_requests` WHERE `id` = @id LIMIT 1");
                                    dbClient.AddParameter("id", PlusEnvironment.JukeboxNow + 1);
                                    Next = dbClient.getRow();
                                }
                                if (Next != null)
                                {
                                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.SetQuery("DELETE FROM `jukebox_requests` WHERE `id` = @id LIMIT 1");
                                        dbClient.AddParameter("id", PlusEnvironment.JukeboxNow + 1);
                                        dbClient.RunQuery();
                                    }

                                    DataRow UpNext = null;
                                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.SetQuery("SELECT * FROM `jukebox_requests` WHERE `id` = @id LIMIT 1");
                                        dbClient.AddParameter("id", PlusEnvironment.JukeboxNow + 2);
                                        UpNext = dbClient.getRow();
                                    }

                                    PlusEnvironment.JukeboxNow += 1;

                                    foreach (RoomUser UserInRoom in Client.GetHabbo().CurrentRoom.GetRoomUserManager().GetUserList().ToList())
                                    {
                                        if (UserInRoom == null || UserInRoom.IsBot || UserInRoom.GetClient() == null)
                                            continue;

                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "jukebox", "request," + Next["video_id"]);

                                        if (UpNext != null)
                                        {
                                            PlusEnvironment.JukeboxUpNext = 1;
                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(UserInRoom.GetClient(), "jukebox-next;" + PlusEnvironment.UpNextReq(Convert.ToString(UpNext["video_id"])));
                                        }
                                        else
                                        {
                                            PlusEnvironment.JukeboxUpNext = 0;
                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(UserInRoom.GetClient(), "jukebox-next;" + "");
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (RoomUser UserInRoom in Client.GetHabbo().CurrentRoom.GetRoomUserManager().GetUserList().ToList())
                                    {
                                        if (UserInRoom == null || UserInRoom.IsBot || UserInRoom.GetClient() == null)
                                            continue;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(UserInRoom.GetClient(), "jukebox-play;start;0;Nothing playing");
                                    }
                                }
                                Timer.Stop();
                            };
                            Timer.Start();
                        }
                        else
                        {
                            PlusEnvironment.JukeboxQueue += 1;
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("INSERT INTO `jukebox_requests` (`id`,`video_id`) VALUES (@Id, @videoId)");
                                dbClient.AddParameter("Id", PlusEnvironment.JukeboxQueue);
                                dbClient.AddParameter("videoId", VideoUrl);
                                dbClient.RunQuery();
                            }

                            Client.SendWhisper("Your requested song [" + PlusEnvironment.UpNextReq(Convert.ToString(VideoUrl)) + "] is on queue", 6);

                            if (PlusEnvironment.JukeboxUpNext == 0)
                            {
                                PlusEnvironment.JukeboxUpNext = 1;
                                foreach (RoomUser UserInRoom in Client.GetHabbo().CurrentRoom.GetRoomUserManager().GetUserList().ToList())
                                {
                                    if (UserInRoom == null || UserInRoom.IsBot || UserInRoom.GetClient() == null)
                                        continue;

                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(UserInRoom.GetClient(), "jukebox-next;" + PlusEnvironment.UpNextReq(Convert.ToString(VideoUrl)));
                                }
                            }
                        }
                    }
                    break;
                #endregion
            }
        }
    }
}