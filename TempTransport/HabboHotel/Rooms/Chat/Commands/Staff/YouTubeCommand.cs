using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Newtonsoft.Json;
using System.Net;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class YouTubeCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank == 8)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "staff"; }
        }

        public string Parameters
        {
            get { return "<YouTube ID>"; }
        }

        public string Description
        {
            get { return "Play music in a room"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :play <YouTube video ID>");
                return;
            }

            /*if(Params[1] == "stop")
            {
                foreach (RoomUser UserInRoom in Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetUserList().ToList())
                {
                    if (UserInRoom == null || UserInRoom.IsBot || UserInRoom.GetClient() == null)
                        continue;

                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(UserInRoom.GetClient(), "youtube;stop");
                }
                
                Session.SendWhisper("YouTube Video ist gestoppt!");
                return;
            }*/

            if (Params[1].Contains(";"))
            {
                Session.SendWhisper("The video ID is invalid.");
                return;
            }

            System.Net.WebClient wc = new System.Net.WebClient();

            var json = wc.DownloadString("https://www.youtube.com/oembed?format=json&url=https://www.youtube.com/watch?v=" + Params[1]);
            string jsonResponse = wc.DownloadString("https://www.googleapis.com/youtube/v3/videos?id=" + Params[1] + "&key=AIzaSyBEYBwbKWC9yceFn4p7PHrgruhMzLeYqAg&part=contentDetails");
            dynamic myClass = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            Session.SendWhisper(Convert.ToString(myClass.title));

            dynamic dynamicObject = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);
            string tmp = dynamicObject.items[0].contentDetails.duration;
            var Duration = Convert.ToInt32(System.Xml.XmlConvert.ToTimeSpan(tmp).TotalSeconds);
            var test = PlusEnvironment.ConvertSecondsToMilliseconds(Convert.ToInt32(Duration));
            Session.SendWhisper(Convert.ToString(test));
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "youtube;start;" + Params[1] + ";" + Convert.ToString(myClass.title));

            System.Timers.Timer timer1 = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(Convert.ToInt32(Duration)));
            timer1.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(Convert.ToInt32(Duration));
            timer1.Elapsed += delegate
            {
                Session.SendWhisper("end");
                timer1.Stop();
            };
            timer1.Start();

            /*System.Net.WebClient wc = new System.Net.WebClient();
            try
            {
                byte[] raw = wc.DownloadData("https://www.youtube.com/oembed?format=json&url=https://www.youtube.com/watch?v=" + Params[1]);
                foreach (RoomUser UserInRoom in Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetUserList().ToList())
                {
                    if (UserInRoom == null || UserInRoom.IsBot || UserInRoom.GetClient() == null)
                        continue;

                    var intArray = raw.Select(b => (int)b).ToArray();

                    Session.SendWhisper(Convert.ToString(intArray[1]));
                    Session.SendWhisper(Convert.ToString(intArray));

                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(UserInRoom.GetClient(), "youtube;start;" + Params[1]);
                }

                Session.SendWhisper("YouTube Video ist gestartet!");
                return;
            }
            catch
            {
                Session.SendWhisper("Das Video-ID ist ungültig.");
                return;
            }*/
        }
    }
}