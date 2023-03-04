using System;
using System.Collections.Generic;
using System.Data;

using Plus.HabboHotel.Groups;
using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Rooms
{
    public class RoomData
    {
        public int Id;
        public int AllowPets;
        public int AllowPetsEating;
        public int RoomBlockingEnabled;
        public int Category;
        public string Description;
        public string Floor;
        public int FloorThickness;
        public int GroupId;
        public Group Group;
        public int Hidewall;
        public string Landscape;
        public string ModelName;
        public string Name;
        public string OwnerName;
        public int OwnerId;
        public string Password;
        public int Score;
        public RoomAccess Access;
        public List<string> Tags;
        public string Type;
        public int UsersMax;
        public int UsersNow;
        public int WallThickness;
        public string Wallpaper;
        public int WhoCanBan;
        public int WhoCanKick;
        public int WhoCanMute;
        private RoomModel mModel;
        public int chatMode;
        public int chatSpeed;
        public int chatSize;
        public int extraFlood;
        public int chatDistance;

        public int TradeSettings;//Default = 2;

        public RoomPromotion _promotion;

        public bool PushEnabled;
        public bool PullEnabled;
        public bool SPushEnabled;
        public bool SPullEnabled;
        public bool EnablesEnabled;
        public bool RespectNotificationsEnabled;
        public bool PetMorphsAllowed;
        public int Offi;
        public int Reseau;
        public int Turf;
        public int Capture;
        public DateTime CaptureDate;
        public int Loyer;
        public int Prix_Vente;
        public DateTime Loyer_Date;
        public int Taxi;
        public int TaxiCost;
        public int Assault;
        public int ClaimUserId;
        public double ClaimPrecent;
        public bool Fightable;
        public bool CorpOffice;

        public void Fill(DataRow Row)
        {
            Id = Convert.ToInt32(Row["id"]);
            Name = Convert.ToString(Row["caption"]);
            Description = Convert.ToString(Row["description"]);
            Type = Convert.ToString(Row["roomtype"]);
            OwnerId = Convert.ToInt32(Row["owner"]);

            OwnerName = "HabboRP";
            /*using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `username` FROM `users` WHERE `id` = @owner LIMIT 1");
                dbClient.AddParameter("owner", OwnerId);
                string result = dbClient.getString();
                if (!String.IsNullOrEmpty(result))
                    OwnerName = result;
            }*/

            this.Access = RoomAccessUtility.ToRoomAccess(Row["state"].ToString().ToLower());

            Category = Convert.ToInt32(Row["category"]);
            if (!string.IsNullOrEmpty(Row["users_now"].ToString()))
                UsersNow = Convert.ToInt32(Row["users_now"]);
            else
                UsersNow = 0;
            UsersMax = Convert.ToInt32(Row["users_max"]);
            ModelName = Convert.ToString(Row["model_name"]);
            Score = Convert.ToInt32(Row["score"]);
            Tags = new List<string>();
            AllowPets = Convert.ToInt32(Row["allow_pets"].ToString());
            AllowPetsEating = Convert.ToInt32(Row["allow_pets_eat"].ToString());
            RoomBlockingEnabled = 1; //RoomBlockingEnabled = Convert.ToInt32(Row["room_blocking_disabled"].ToString());
            Hidewall = Convert.ToInt32(Row["allow_hidewall"].ToString());
            Password = Convert.ToString(Row["password"]);
            Wallpaper = Convert.ToString(Row["wallpaper"]);
            Floor = "110"; //Floor = Convert.ToString(Row["floor"]);
            Landscape = Convert.ToString(Row["landscape"]);
            FloorThickness = Convert.ToInt32(Row["floorthick"]);
            GroupId = Convert.ToInt32(Row["group_id"]);
            WallThickness = Convert.ToInt32(Row["wallthick"]);
            WhoCanMute = Convert.ToInt32(Row["mute_settings"]);
            WhoCanKick = Convert.ToInt32(Row["kick_settings"]);
            WhoCanBan = Convert.ToInt32(Row["ban_settings"]);
            chatMode = Convert.ToInt32(Row["chat_mode"]);
            chatSpeed = Convert.ToInt32(Row["chat_speed"]);
            chatSize = Convert.ToInt32(Row["chat_size"]);
            TradeSettings = Convert.ToInt32(Row["trade_settings"]);

            Group G = null;
            if (PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(Convert.ToInt32(Row["group_id"]), out G))
                Group = G;
            else
                Group = null;

            foreach (string Tag in Row["tags"].ToString().Split(','))
            {
                Tags.Add(Tag);
            }

            mModel = PlusEnvironment.GetGame().GetRoomManager().GetModel(ModelName);

            this.PushEnabled = PlusEnvironment.EnumToBool(Row["push_enabled"].ToString());
            this.PullEnabled = PlusEnvironment.EnumToBool(Row["pull_enabled"].ToString());
            this.SPushEnabled = PlusEnvironment.EnumToBool(Row["spush_enabled"].ToString());
            this.SPullEnabled = PlusEnvironment.EnumToBool(Row["spull_enabled"].ToString());
            this.EnablesEnabled = PlusEnvironment.EnumToBool(Row["enables_enabled"].ToString());
            this.RespectNotificationsEnabled = PlusEnvironment.EnumToBool(Row["respect_notifications_enabled"].ToString());
            this.PetMorphsAllowed = PlusEnvironment.EnumToBool(Row["pet_morphs_allowed"].ToString());
            Offi = Convert.ToInt32(Row["offi"]);
            Reseau = Convert.ToInt32(Row["reseau"]);
            Turf = Convert.ToInt32(Row["is_turf"]);
            Capture = Convert.ToInt32(Row["capture"]);
            CaptureDate = Convert.ToDateTime(Row["capture_date"]);
            Loyer = Convert.ToInt32(Row["loyer"]);
            Prix_Vente = Convert.ToInt32(Row["prix_vente"]);
            Loyer_Date = Convert.ToDateTime(Row["loyer_date"]);
            Taxi = Convert.ToInt32(Row["taxi"]);
            TaxiCost = Convert.ToInt32(Row["taxiCost"]);
            Assault = 0;
            ClaimUserId = 0;
            Fightable = Convert.ToBoolean(Row["fightable"]);
            CorpOffice = Convert.ToBoolean(Row["corp_office"]);
        }

        public RoomPromotion Promotion
        {
            get { return this._promotion; }
            set { this._promotion = value; }
        }

        public bool HasActivePromotion
        {
            get { return this.Promotion != null; }
        }

        public void EndPromotion()
        {
            if (!this.HasActivePromotion)
                return;

            this.Promotion = null;
        }

        public RoomModel Model
        {
            get
            {
                if (mModel == null)
                    mModel = PlusEnvironment.GetGame().GetRoomManager().GetModel(ModelName);
                return mModel;
            }
        }
    }
}