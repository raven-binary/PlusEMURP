using System;
using System.Data;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Rooms;
using Plus.HabboRoleplay.Misc;
using Plus;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Pathfinding;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.Communication.Packets.Outgoing.Navigator;
using Plus.Communication.Packets.Outgoing.Rooms.Session;
using Plus.Communication.Packets.Outgoing.Rooms.Avatar;
using log4net;
using Plus.HabboHotel.Items;

namespace Plus.HabboRoleplay.Misc
{
    public static class Taxi
    {
        private static readonly ILog log = LogManager.GetLogger("Plus.HabboRoleplay.Misc");

        public static int CurrentTaxiRides = 0;

        /// <summary>
        /// Taxi Table (Corps, Streets etc)
        /// </summary>
        public static void Load(GameClient Client)
        {
            Client.GetRoleplay().usingTaxiPole = true;

            DataRow RoomData = null;
            DataRow OutRoomData = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `groups` LIMIT 6;");
                DataTable Corps = dbClient.getTable();

                foreach (DataRow Corp in Corps.Rows)
                {
                    dbClient.SetQuery("SELECT * FROM `rooms` WHERE `id` = @roomId LIMIT 1;");
                    dbClient.AddParameter("roomId", Convert.ToInt32(Corp["room_id"]));
                    RoomData = dbClient.getRow();

                    dbClient.SetQuery("SELECT * FROM `rooms` WHERE `id` = @roomId LIMIT 1;");
                    dbClient.AddParameter("roomId", Convert.ToInt32(Corp["room_outside_id"]));
                    OutRoomData = dbClient.getRow();

                    Client.GetRoleplay().SendWeb("taxi-corp;" + Corp["badge"] + ";" + Corp["name"] + ";" + RoomData["caption"] + ";" + RoomData["id"] + ";0;" + OutRoomData["caption"] + ";" + OutRoomData["id"] + ";0");
                }
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `rooms` WHERE `taxi` = '1'");
                DataTable taxis = dbClient.getTable();

                foreach (DataRow taxi in taxis.Rows)
                {
                    Client.GetRoleplay().SendWeb("taxi-add;" + taxi["taxi_type"] + ";" + taxi["caption"] + ";" + taxi["id"] + ";0");
                }
            }
            Client.GetRoleplay().SendWeb("taxi;show");
        }

        /// <summary>
        /// Taxi Routes
        /// </summary>
        public static void Route(GameClient Client)
        {
            if (Client.GetRoleplay().TaxiRideId > 0)
            {
                Client.GetRoleplay().roomUser.CanWalk = false;
                Client.GetRoleplay().roomUser.SuperFastWalking = true;
                if (Client.GetHabbo().CurrentRoomId == Client.GetRoleplay().TaxiRideId)
                {
                    if (Client.GetRoleplay().TaxiRideId == 66)
                    {
                        Client.GetRoleplay().roomUser.MoveTo(18, 14);
                    }
                    else if (Client.GetRoleplay().TaxiRideId == 62)
                    {
                        Client.GetRoleplay().roomUser.MoveTo(19, 11);
                    }
                    else if (Client.GetRoleplay().TaxiRideId == 94)
                    {
                        Client.GetRoleplay().roomUser.MoveTo(21, 15);
                    }
                    else if (Client.GetRoleplay().TaxiRideId == 97)
                    {
                        Client.GetRoleplay().roomUser.MoveTo(17, 19);
                    }
                    else if (Client.GetRoleplay().TaxiRideId == 75)
                    {
                        Client.GetRoleplay().roomUser.MoveTo(13, 8);
                    }
                    else if (Client.GetRoleplay().TaxiRideId == 70)
                    {
                        Client.GetRoleplay().roomUser.MoveTo(12, 19);
                    }
                    else
                    {
                        foreach (Item item in Client.GetHabbo().CurrentRoom.GetRoomItemHandler().GetFloor)
                        {
                            if (item.GetBaseItem().SpriteId == 3090)
                            {
                                Client.GetRoleplay().roomUser.MoveTo(item.GetX, item.GetY);
                            }
                        }
                    }
                }
                else
                {
                    #region Hospital
                    if (Client.GetRoleplay().TaxiRideId == 62 | Client.GetRoleplay().TaxiRideId == 160)
                    {
                        if (Client.GetRoleplay().TaxiRideId == 160 && Client.GetHabbo().CurrentRoomId == 62)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(17, 22);
                            #region Hospital - LVPD Inside
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 160)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(1, 17);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 253)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(9, 25);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(9, 14);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 66)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 14);
                        }
                        #region Hospital - LVPD Outside
                    }
                    else if (Client.GetHabbo().CurrentRoomId == 160)
                    {
                        Client.GetRoleplay().roomUser.MoveTo(1, 17);
                    }
                    else if (Client.GetHabbo().CurrentRoomId == 253)
                    {
                        Client.GetRoleplay().roomUser.MoveTo(9, 25);
                    }
                    else if (Client.GetHabbo().CurrentRoomId == 162)
                    {
                        Client.GetRoleplay().roomUser.MoveTo(16, 0);
                    }
                    else if (Client.GetHabbo().CurrentRoomId == 163)
                    {
                        Client.GetRoleplay().roomUser.MoveTo(5, 19);
                    }


                    #region LVPD
                    if (Client.GetRoleplay().TaxiRideId == 66 | Client.GetRoleplay().TaxiRideId == 163)
                    {
                        if (Client.GetRoleplay().TaxiRideId == 163 && Client.GetHabbo().CurrentRoomId == 66)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(13, 22);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 62)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(21, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 160)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(9, 5);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(9, 14);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 152)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(1, 21);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 157)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(1, 19);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 63)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(6, 6);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 94)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 12);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 214)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(17, 24);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 211)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(1, 14);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 210)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(4, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 97)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 16);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 75)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 8);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(15, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(17, 0);
                        }
                    }
                    #endregion

                    #region Starbucks
                    if (Client.GetRoleplay().TaxiRideId == 94 | Client.GetRoleplay().TaxiRideId == 214)
                    {
                        if (Client.GetRoleplay().TaxiRideId == 214 && Client.GetHabbo().CurrentRoomId == 94)
                        {
                           Client.GetRoleplay().roomUser.MoveTo(25, 12);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 66)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(13, 22);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 20);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 152)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 19);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 157)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(20, 24);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 63)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 19);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 62)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 22);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 160)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 13);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 210)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(17, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(24, 14);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 214)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(10, 7);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 211)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 97)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 16);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 75)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 8);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 16);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 253)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(32, 24);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(23, 8);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 127)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 255)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                    }
                    #endregion

                    #region Bank
                    if (Client.GetRoleplay().TaxiRideId == 97 | Client.GetRoleplay().TaxiRideId == 156)
                    {
                        if (Client.GetRoleplay().TaxiRideId == 156 && Client.GetHabbo().CurrentRoomId == 97)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 16);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 66)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(13, 22);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 94)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 12);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(12, 8);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 214)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(17, 24);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 211)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(1, 14);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 210)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(17, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 157)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(20, 24);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 152)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 19);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 20);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(17, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 253)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(32, 26);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 160)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 13);
                        }
                    }
                    #endregion

                    #region Armoury
                    if (Client.GetRoleplay().TaxiRideId == 75 | Client.GetRoleplay().TaxiRideId == 162)
                    {
                        if (Client.GetRoleplay().TaxiRideId == 162 && Client.GetHabbo().CurrentRoomId == 75)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 8);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 66)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(13, 22);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 160)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(1, 16);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 253)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(9, 24);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(8, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 94)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 12);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 214)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(17, 24);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 211)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(1, 14);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 210)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(1, 12);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 24);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 62)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 22);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(17, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 152)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(1, 20);
                        }
                    }
                    #endregion

                    #region Forever21
                    if (Client.GetRoleplay().TaxiRideId == 70 | Client.GetRoleplay().TaxiRideId == 25)
                    {
                        if (Client.GetRoleplay().TaxiRideId == 25 && Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 66)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(13, 22);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 75)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 8);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 24);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(15, 24);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 94)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 12);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 214)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(17, 24);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 211)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(1, 14);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(1, 19);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 255 | Client.GetHabbo().CurrentRoomId == 127)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(1, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(17, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 62)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 22);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 160)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(1, 16);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 253)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(9, 24);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 97)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 16);
                        }
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// Taxi Reset
        /// </summary>
        public static void Reset(GameClient Client)
        {
            Client.GetRoleplay().TaxiRideId = 0;
            Client.GetRoleplay().TaxiRideTimer = 0;
            Client.GetRoleplay().usingTaxiRide = false;
            Client.GetRoleplay().ResetEffect();
            Client.GetRoleplay().roomUser.SuperFastWalking = false;
            Client.GetRoleplay().roomUser.CanWalk = true;
        }
    }
}
#endregion

#endregion

#endregion