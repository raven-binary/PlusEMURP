using System;
using System.Collections.Concurrent;
using Plus.Communication.Packets.Incoming;
using Plus.Communication.Packets.Outgoing;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Rooms;

namespace Plus.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class BotChangesClothesBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.EffectBotChangesClothesBox;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public BotChangesClothesBox(Room instance, Item item)
        {
            Instance = instance;
            Item = item;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket packet)
        {
            int unknown = packet.PopInt();
            string botConfiguration = packet.PopString();

            if (SetItems.Count > 0)
                SetItems.Clear();

            StringData = botConfiguration;
        }

        public bool Execute(params object[] @params)
        {
            if (@params == null || @params.Length == 0)
                return false;

            if (string.IsNullOrEmpty(StringData))
                return false;


            string[] stuff = StringData.Split('\t');
            if (stuff.Length != 2)
                return false; //This is important, incase a cunt scripts.

            string username = stuff[0];

            RoomUser user = Instance.GetRoomUserManager().GetBotByName(username);
            if (user == null)
                return false;

            string figure = stuff[1];

            user.BotData.Look = figure;
            user.BotData.Gender = "M";

            MessageComposer userChangeComposer = new UserChangeComposer(user.VirtualId, user.BotData);
            Instance.SendPacket(userChangeComposer);


            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `bots` SET `look` = @look, `gender` = @gender WHERE `id` = '" + user.BotData.Id + "' LIMIT 1");
                dbClient.AddParameter("look", user.BotData.Look);
                dbClient.AddParameter("gender", user.BotData.Gender);
                dbClient.RunQuery();
            }

            return true;
        }
    }
}