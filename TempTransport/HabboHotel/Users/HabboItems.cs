using Fleck;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using Plus.HabboHotel.Rooms;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Users
{
    public class RPItems
    {
        public GameClient client;
        public string Taser { get; set; }
        public string Bat { get; set; }
        public int BatDamage { get; set; }
        public string Sword { get; set; }
        public int SwordDamage { get; set; }
        public string Axe { get; set; }
        public int AxeDamage { get; set; }
        public int Knifes { get; set; }
        public int BodyArmour { get; set; }
        public int Medkits { get; set; }
        public int Snacks { get; set; }
        public int Carrots { get; set; }
        public int NFCHacker { get; set; }
        public int Sprays { get; set; }
        public int Lockpicks { get; set; }

        public RPItems(string Taser, string Bat, int BatDamage, string Sword, int SwordDamage, string Axe, int AxeDamage, int Knifes, int BodyArmour, int Medkits, int Snacks, int Carrots, int NFCHacker, int Sprays, int Lockpicks)
        {
            this.Taser = Taser;
            this.Bat = Bat;
            this.BatDamage = BatDamage;
            this.Sword = Sword;
            this.SwordDamage = SwordDamage;
            this.Axe = Axe;
            this.AxeDamage = AxeDamage;
            this.Knifes = Knifes;
            this.BodyArmour = BodyArmour;
            this.Medkits = Medkits;
            this.Snacks = Snacks;
            this.Carrots = Carrots;
            this.NFCHacker = NFCHacker;
            this.Sprays = Sprays;
            this.Lockpicks = Lockpicks;
        }

        public GameClient GetClient()
        {
            return this.client;
        }

        public Habbo habbo
        {
            get
            {
                return this.GetClient().GetHabbo();
            }
        }

        public void updateLockpicks()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE user_items SET lockpicks = @lockpicks WHERE `user_id` = @id LIMIT 1");
                dbClient.AddParameter("lockpicks", this.Lockpicks);
                dbClient.AddParameter("id", habbo.Id);
                dbClient.RunQuery();
            }
        }
    }
}
