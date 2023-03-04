using System;
using System.Data;
using log4net;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
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
                        Client.GetRoleplay().roomUser.MoveTo(13, 19);
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

                    // New Taxi by Enzo                  
                    //Vintage - Taxi Routes
                    //Petco
// New Taxi by Enzo                  
                    //Vintage - Taxi Routes
                    //Petco
                    #region Petco
                    if (Client.GetRoleplay().TaxiRideId == 30)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(9, 1);
                        }
                    }
                    #endregion
                    //Lake Mead Blvd
                    #region Lake Mead Blvd
                    if (Client.GetRoleplay().TaxiRideId == 32)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                    }
                    #endregion
                    //Casino ST
                    #region Casino
                    if (Client.GetRoleplay().TaxiRideId == 29)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(9, 1);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 30)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                    }
                    #endregion
                    //Cortex St //events
                    #region Cortex St
                    if (Client.GetRoleplay().TaxiRideId == 27)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(9, 1);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 30)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 29)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                    }
                    #endregion
                    //Oxy St //job center heist
                    #region Oxy St
                    if (Client.GetRoleplay().TaxiRideId == 165)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(9, 1);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 30)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 29)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 27)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(5, 15);
                        }
                    }
                    #endregion
                    //Lean st //Outside F21
                    #region Lean St
                    if (Client.GetRoleplay().TaxiRideId == 25)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 0);
                        }
                    }
                    #endregion
                    //Durango Dr //Outside Armoury
                    #region Durango Dr
                    if (Client.GetRoleplay().TaxiRideId == 162)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                    }
                    #endregion
                    //LV National park
                    #region LV national park
                    if (Client.GetRoleplay().TaxiRideId == 2)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        
                        }
                    }
                    #endregion
                    //Dreva St //Outside police
                    #region Dreva St
                    if (Client.GetRoleplay().TaxiRideId == 163)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 0);
                        }
                    }
                    #endregion
                    //Maryland Pkwy //Outside hosp
                    #region Maryland Pkwy
                    if (Client.GetRoleplay().TaxiRideId == 160)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(9, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 5)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 17);
                        }
                    }
                    #endregion
                    //Fremont St //Outside bank
                    #region Fremont St
                    if (Client.GetRoleplay().TaxiRideId == 156)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                    }
                    #endregion
                    //Neil St //Library HEIST
                    #region Neil St
                    if (Client.GetRoleplay().TaxiRideId == 10)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(17, 25);
                        }
                    }
                    #endregion
                    //Spencer St //Grocery store
                    #region Spencer St
                    if (Client.GetRoleplay().TaxiRideId == 12)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 11)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(19, 23);
                        }
                    }
                    #endregion
                    //Highland Dr //LUX Nightclub
                    #region Highland Dr
                    if (Client.GetRoleplay().TaxiRideId == 13)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 11)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(19, 23);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 12)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(19, 19);
                       
                        }
                    }
                    #endregion
                    //City Pkway //City Hall
                    #region City Pkway
                    if (Client.GetRoleplay().TaxiRideId == 14)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 11)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(19, 23);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 12)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(24, 6);
                        }
                    }
                    #endregion
                    //Reno St //Prison
                    #region Prison
                    if (Client.GetRoleplay().TaxiRideId == 15)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 11)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(19, 23);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 12)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(24, 6);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 14)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 16);
                        }
                    }
                    #endregion
                    //Odgen Av. //Outside Starbucks
                    #region Odgen Av.
                    if (Client.GetRoleplay().TaxiRideId == 214)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 11)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(19, 0);
                        }
                    }
                    #endregion
                    //Poplar Av. //Versace HEIST
                    #region  Poplar Av.
                    if (Client.GetRoleplay().TaxiRideId == 19)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 11)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(19, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 214)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                    }
                    #endregion
                    //F21 //CORP
                    #region F21
                    if (Client.GetRoleplay().TaxiRideId == 70)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 11);
                        }
                    }
                    #endregion
                    //ARMOURY //CORP
                    #region Armoury
                    if (Client.GetRoleplay().TaxiRideId == 75)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(8, 10);
                        }
                    }
                    #endregion
                    //POLICE //CORP
                    #region Police
                    if (Client.GetRoleplay().TaxiRideId == 66)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(9, 14);
                        }
                    }
                    #endregion
                    //HOSPITAL //CORP
                    #region Hospital
                    if (Client.GetRoleplay().TaxiRideId == 62)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(9, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 5)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 17);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 160)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(8, 5);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 62)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(19, 11);
                        }
                    }
                    #endregion
                    //BANK //CORP
                    #region Bank
                    if (Client.GetRoleplay().TaxiRideId == 97)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(12, 9);
                        }
                    }
                    #endregion
                    //STARBUCKS //CORP
                    #region Starbucks
                    if (Client.GetRoleplay().TaxiRideId == 94)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 11)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(19, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 214)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(10, 7);
                        }
                    }
                    #endregion
                    //Elkhorn Rd. //Marketplace
                    #region Elkhorn Rd.
                    if (Client.GetRoleplay().TaxiRideId == 142)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
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
                            Client.GetRoleplay().roomUser.MoveTo(20, 0);
                        }
                    }
                    #endregion
                    //Russell Rd. //GYM
                    #region Russell Rd.
                    if (Client.GetRoleplay().TaxiRideId == 24)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 20);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 152)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(15, 0);
                        }
                    }
                    #endregion
                    //Jones Blvd. //Court
                    #region Jones Blvd.
                    if (Client.GetRoleplay().TaxiRideId == 152)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 20);
                        }
                    }
                    #endregion
                    //Sahara Av. //Apartments
                    #region Sahara Av.
                    if (Client.GetRoleplay().TaxiRideId == 23)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 20);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 152)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 19);
                        }
                    }
                    #endregion
                    //Spring Mountain //Farming
                    #region Spring Mountain
                    if (Client.GetRoleplay().TaxiRideId == 22)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 20);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 152)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(15, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 24)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 16);
                        }
                    }
                    #endregion
                    //Duela Rd
                    #region Duela Rd.
                    if (Client.GetRoleplay().TaxiRideId == 8)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(9, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 160)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 12);
                        }
                    }
                    #endregion
                    // Mining
                    #region Mining
                    if (Client.GetRoleplay().TaxiRideId == 125)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(22, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
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
                            Client.GetRoleplay().roomUser.MoveTo(20, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 142)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(23, 23);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 182)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 126)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(17, 6);
                        }
                    }
                    #endregion
                    //FOREVER 21 INSIDE
                    #region Lean St
                    if (Client.GetRoleplay().TaxiRideId == 25)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                    }
                    #endregion
                    #region Park
                    if (Client.GetRoleplay().TaxiRideId == 2)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                    }
                    #endregion
                    #region Hospital Inside
                    if (Client.GetRoleplay().TaxiRideId == 62)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 3);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 160)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(8, 5);
                        }
                    }
                    #endregion
                    #region Maryland Rd.
                    if (Client.GetRoleplay().TaxiRideId == 160)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 3);
                        }
                    }
                    #endregion
                    #region Duela 
                    if (Client.GetRoleplay().TaxiRideId == 8)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 3);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 160)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 12);
                        }
                    }
                    #endregion
                    #region Fremont St
                    if (Client.GetRoleplay().TaxiRideId == 156)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                    }
                    #endregion
                    #region Bank Inside
                    if (Client.GetRoleplay().TaxiRideId == 97)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(12, 8);
                        }
                    }
                    #endregion
                    #region Neil St.
                    if (Client.GetRoleplay().TaxiRideId == 10)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(17, 25);
                        }
                    }
                    #endregion
                    #region Odgen Av
                    if (Client.GetRoleplay().TaxiRideId == 214)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 11)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(19, 0);
                        }
                    }
                    #endregion
                    #region Starbucks Inside
                    if (Client.GetRoleplay().TaxiRideId == 94)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 11)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(19, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 214)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(10, 6);
                        }
                    }
                    #endregion
                    #region Spencer St.
                    if (Client.GetRoleplay().TaxiRideId == 12)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 11)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(19, 23);
                        }
                    }
                    #endregion
                    #region Highland Dr
                    if (Client.GetRoleplay().TaxiRideId == 13)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 11)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(19, 23);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 12)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(19, 19);
                        }
                    }
                    #endregion
                    #region City Pkwy
                    if (Client.GetRoleplay().TaxiRideId == 14)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 11)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(19, 23);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 12)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(24, 6);
                        }
                    }
                    #endregion
                    #region Reno St
                    if (Client.GetRoleplay().TaxiRideId == 15)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 11)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(19, 23);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 12)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(24, 6);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 14)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 16);
                        }
                    }
                    #endregion
                    #region Lake Mead
                    if (Client.GetRoleplay().TaxiRideId == 32)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 25);
                        }
                    }
                    #endregion
                    #region Vintage St
                    if (Client.GetRoleplay().TaxiRideId == 31)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 25);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(1, 18);
                        }
                    }
                    #endregion
                    #region Hound St
                    if (Client.GetRoleplay().TaxiRideId == 30)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 25);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(1, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(9, 1);
                        }
                    }
                    #endregion
                    #region Durango Dr.
                    if (Client.GetRoleplay().TaxiRideId == 162)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                    }
                    #endregion
                    #region Armoury Inside
                    if (Client.GetRoleplay().TaxiRideId == 75)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(8, 10);
                        }
                    }
                    #endregion
                    #region Dreva St
                    if (Client.GetRoleplay().TaxiRideId == 163)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 0);
                        }
                    }
                    #endregion
                    #region LVPD
                    if (Client.GetRoleplay().TaxiRideId == 66)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(9, 14);
                        }
                    }
                    #endregion
                    #region Cortex St
                    if (Client.GetRoleplay().TaxiRideId == 27)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(1, 20);
                        }
                    }
                    #endregion
                    #region Paradise St
                    if (Client.GetRoleplay().TaxiRideId == 29)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(1, 20);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 27)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(12, 25);
                        }
                    }
                    #endregion
                    #region Oxy St
                    if (Client.GetRoleplay().TaxiRideId == 165)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(1, 20);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 27)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(5, 15);
                        }
                    }
                    #endregion
                    #region Mall
                    if (Client.GetRoleplay().TaxiRideId == 142)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
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
                            Client.GetRoleplay().roomUser.MoveTo(20, 0);
                        }
                    }
                    #endregion
                    #region Mining Sewer
                    if (Client.GetRoleplay().TaxiRideId == 125)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
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
                            Client.GetRoleplay().roomUser.MoveTo(20, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 142)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(23, 23);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 182)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 126)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 6);
                        }
                    }
                    #endregion
                    #region Jones Blvd.
                    if (Client.GetRoleplay().TaxiRideId == 152)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 20);
                        }
                    }
                    #endregion
                    #region Russell Rd.
                    if (Client.GetRoleplay().TaxiRideId == 24)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 20);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 152)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(15, 0);
                        }
                    }
                    #endregion
                    #region Sahara Av
                    if (Client.GetRoleplay().TaxiRideId == 23)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 20);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 152)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 19);
                        }
                    }
                    #endregion
                    #region Poplar Av
                    if (Client.GetRoleplay().TaxiRideId == 19)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 70)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 20);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 152)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 19);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 23)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(21, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 22)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 21);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 21)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(23, 21);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 20)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(19, 25);
                        }
                    }
                    #endregion

                    //LEAN ST
                    #region Park
                    if (Client.GetRoleplay().TaxiRideId == 2)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 15);
                        }
                    }
                    #endregion
                    #region Hospital Inside
                    if (Client.GetRoleplay().TaxiRideId == 62)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 3);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 160)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(8, 5);
                        }
                    }
                    #endregion
                    #region Maryland Rd.
                    if (Client.GetRoleplay().TaxiRideId == 160)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 3);
                        }
                    }
                    #endregion
                    #region Duela 
                    if (Client.GetRoleplay().TaxiRideId == 8)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(11, 3);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 160)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 12);
                        }
                    }
                    #endregion
                    #region Fremont St
                    if (Client.GetRoleplay().TaxiRideId == 156)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                    }
                    #endregion
                    #region Bank Inside
                    if (Client.GetRoleplay().TaxiRideId == 97)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(12, 8);
                        }
                    }
                    #endregion
                    #region Neil St.
                    if (Client.GetRoleplay().TaxiRideId == 10)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(17, 25);
                        }
                    }
                    #endregion
                    #region Odgen Av
                    if (Client.GetRoleplay().TaxiRideId == 214)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 11)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(19, 0);
                        }
                    }
                    #endregion
                    #region Starbucks Inside
                    if (Client.GetRoleplay().TaxiRideId == 94)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 11)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(19, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 214)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(10, 6);
                        }
                    }
                    #endregion
                    #region Spencer St.
                    if (Client.GetRoleplay().TaxiRideId == 12)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 11)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(19, 23);
                        }
                    }
                    #endregion
                    #region Highland Dr
                    if (Client.GetRoleplay().TaxiRideId == 13)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 11)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(19, 23);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 12)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(19, 19);
                        }
                    }
                    #endregion
                    #region City Pkwy
                    if (Client.GetRoleplay().TaxiRideId == 14)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 11)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(19, 23);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 12)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(24, 6);
                        }
                    }
                    #endregion
                    #region Reno St
                    if (Client.GetRoleplay().TaxiRideId == 15)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 2)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 10);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 7)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 11);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 156)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(26, 15);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 11)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(19, 23);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 12)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(24, 6);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 14)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 16);
                        }
                    }
                    #endregion
                    #region Lake Mead
                    if (Client.GetRoleplay().TaxiRideId == 32)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 25);
                        }
                    }
                    #endregion
                    #region Vintage St
                    if (Client.GetRoleplay().TaxiRideId == 31)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 25);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(1, 18);
                        }
                    }
                    #endregion
                    #region Hound St
                    if (Client.GetRoleplay().TaxiRideId == 30)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 25);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 32)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(1, 18);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 31)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(9, 1);
                        }
                    }
                    #endregion
                    #region Durango Dr.
                    if (Client.GetRoleplay().TaxiRideId == 162)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                    }
                    #endregion
                    #region Armoury Inside
                    if (Client.GetRoleplay().TaxiRideId == 75)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(8, 10);
                        }
                    }
                    #endregion
                    #region Dreva St
                    if (Client.GetRoleplay().TaxiRideId == 163)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 0);
                        }
                    }
                    #endregion
                    #region LVPD
                    if (Client.GetRoleplay().TaxiRideId == 66)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(9, 14);
                        }
                    }
                    #endregion
                    #region Cortex St
                    if (Client.GetRoleplay().TaxiRideId == 27)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(1, 20);
                        }
                    }
                    #endregion
                    #region Paradise St
                    if (Client.GetRoleplay().TaxiRideId == 29)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(1, 20);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 27)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(12, 25);
                        }
                    }
                    #endregion
                    #region Oxy St
                    if (Client.GetRoleplay().TaxiRideId == 165)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(1, 20);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 27)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(5, 15);
                        }
                    }
                    #endregion
                    #region Mall
                    if (Client.GetRoleplay().TaxiRideId == 142)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
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
                            Client.GetRoleplay().roomUser.MoveTo(20, 0);
                        }
                    }
                    #endregion
                    #region Mining Sewer
                    if (Client.GetRoleplay().TaxiRideId == 125)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
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
                            Client.GetRoleplay().roomUser.MoveTo(20, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 142)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(23, 23);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 182)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 9);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 126)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 6);
                        }
                    }
                    #endregion
                    #region Jones Blvd.
                    if (Client.GetRoleplay().TaxiRideId == 152)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 20);
                        }
                    }
                    #endregion
                    #region Russell Rd.
                    if (Client.GetRoleplay().TaxiRideId == 24)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 20);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 152)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(15, 0);
                        }
                    }
                    #endregion
                    #region Sahara Av
                    if (Client.GetRoleplay().TaxiRideId == 23)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 20);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 152)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 19);
                        }
                    }
                    #endregion
                    #region Poplar Av
                    if (Client.GetRoleplay().TaxiRideId == 19)
                    {
                        if (Client.GetHabbo().CurrentRoomId == 25)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(18, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 162)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(16, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 163)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 20);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 152)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 19);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 23)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(21, 0);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 22)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(25, 21);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 21)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(23, 21);
                        }
                        else if (Client.GetHabbo().CurrentRoomId == 20)
                        {
                            Client.GetRoleplay().roomUser.MoveTo(19, 25);
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