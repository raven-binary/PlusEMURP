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
    class SkinsWebEvent : IWebEvent
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
                #region Chat Bubbles
                case "chat-bubble":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().Rank < 2)
                            return;

                        if (Client.GetHabbo().getCooldown("bubble_skin") == true)
                        {
                            Client.SendWhisper("You must wait between changing bubble skins");
                            return;
                        }

                        string[] ReceivedData = Data.Split(',');
                        string Bubble = ReceivedData[1];

                        int BubbleId;
                        if (Bubble == "default")
                        {
                            BubbleId = 0;
                        }
                        else if (Bubble == "puppy")
                        {
                            BubbleId = 20;
                        }
                        else if (Bubble == "piglet")
                        {
                            BubbleId = 19;
                        }
                        else if (Bubble == "hearts")
                        {
                            BubbleId = 16;
                        }
                        else if (Bubble == "stars")
                        {
                            BubbleId = 17;
                        }
                        else if (Bubble == "duck")
                        {
                            BubbleId = 21;
                        }
                        else if (Bubble == "iconic")
                        {
                            BubbleId = 24;
                        }
                        else if (Bubble == "storm")
                        {
                            BubbleId = 27;
                        }
                        else if (Bubble == "galaxy")
                        {
                            BubbleId = 12;
                        }
                        else
                        {
                            Client.SendWhisper("Bubble id could not be found");
                            return;
                        }

                        if (BubbleId == Client.GetHabbo().CustomBubbleId)
                        {
                            char[] b = Bubble.ToCharArray();
                            b[0] = char.ToUpper(b[0]);
                            Client.SendWhisper("You already selected the " + new string(b) + " bubble");
                            return;
                        }

                        User.LastBubble = BubbleId;
                        Client.GetHabbo().CustomBubbleId = BubbleId;
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "chat-bubble;" + Bubble);
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE users SET bubble = @bubble WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("bubble", BubbleId);
                            dbClient.AddParameter("id", Client.GetHabbo().Id);
                            dbClient.RunQuery();
                        }

                        char[] a = Bubble.ToCharArray();
                        a[0] = char.ToUpper(a[0]);
                        Client.SendWhisper("Chat bubble changed to " + new string(a));
                        //Client.GetHabbo().addCooldown("bubble_skin", 1000);
                    }
                    break;
                #endregion
                #region Prefix Names
                case "prefix-name":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().Rank < 2)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Prefix = ReceivedData[1];

                        string Title;
                        if (Prefix == "default")
                        {
                            Title = "null";
                        }
                        else if (Prefix == "celebrity")
                        {
                            Title = "Celebrity";
                        }
                        else if (Prefix == "princess")
                        {
                            Title = "Princess";
                        }
                        else if (Prefix == "prince")
                        {
                            Title = "Prince";
                        }
                        else if (Prefix == "king")
                        {
                            Title = "King";
                        }
                        else if (Prefix == "queen")
                        {
                            Title = "Queen";
                        }
                        else if (Prefix == "heart")
                        {
                            Title = "";
                        }
                        else if (Prefix == "bomb")
                        {
                            Title = "†";
                        }
                        else if (Prefix == "star")
                        {
                            Title = "¥";
                        }
                        else if (Prefix == "lightning")
                        {
                            Title = "º";
                        }
                        else if (Prefix == "music")
                        {
                            Title = "—";
                        }
                        else if (Prefix == "collector")
                        {
                            Title = "Collector";
                        }
                        else if (Prefix == "boss")
                        {
                            Title = "Boss";
                        }
                        else if (Prefix == "don")
                        {
                            Title = "Don";
                        }
                        else if (Prefix == "lgbtq")
                        {
                            Title = "LGBTQ";
                        }
                        else if (Prefix == "baddie")
                        {
                            Title = "Baddie";
                        }
                        else if (Prefix == "imposter")
                        {
                            Title = "Imposter";
                        }
                        else if (Prefix == "royalty")
                        {
                            Title = "Royalty";
                        }
                        else
                        {
                            Client.SendWhisper("Title name could not be found");
                            return;
                        }

                        if (Title == Client.GetHabbo().PrefixName)
                        {
                            char[] t = Title.ToCharArray();
                            t[0] = char.ToUpper(t[0]);
                            Client.SendWhisper("You already selected the " + new string(t) + " title");
                            return;
                        }

                        Client.GetHabbo().PrefixName = Title;
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "prefix-name;" + Prefix);

                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE users SET prefix_name = @title WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("title", Title);
                            dbClient.AddParameter("id", Client.GetHabbo().Id);
                            dbClient.RunQuery();
                        }

                        char[] t2 = Title.ToCharArray();
                        t2[0] = char.ToUpper(t2[0]);
                        Client.SendWhisper("Title name changed to " + new string(t2));
                    }
                    break;
                #endregion
                #region Name Color's
                case "name-color":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().Rank < 2)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Hex = ReceivedData[1];

                        string Color;
                        if (Hex == "000000")
                        {
                            Color = "Default";
                        }
                        else if (Hex == "BBDEFB")
                        {
                            Color = "Sail";
                        }
                        else if (Hex == "90CAF9")
                        {
                            Color = "Light Malibu";
                        }
                        else
                        {
                            Client.SendWhisper("Name color could not be found");
                            return;
                        }

                        Client.GetHabbo().NameColor = "#" + Hex;

                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE users SET color_name = @color WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("color", "#" + Hex);
                            dbClient.AddParameter("id", Client.GetHabbo().Id);
                            dbClient.RunQuery();
                        }

                        char[] t2 = Color.ToCharArray();
                        t2[0] = char.ToUpper(t2[0]);
                        Client.SendWhisper("Name color changed to " + new string(t2));
                    }
                    break;
                    #endregion
            }
        }
    }
}
