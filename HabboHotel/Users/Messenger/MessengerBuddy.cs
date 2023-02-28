using System;
using System.Linq;
using Plus.Communication.Packets.Outgoing;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users.Relationships;

namespace Plus.HabboHotel.Users.Messenger
{
    public class MessengerBuddy
    {
        #region Fields

        public int UserId;
        public bool MAppearOffline;
        public bool MHideInRoom;
        public int MLastOnline;
        public string MLook;
        public string MMotto;

        public GameClient Client;
        public string MUsername;

        #endregion

        #region Return values

        public int Id => UserId;

        public bool IsOnline =>
            (Client != null && Client.GetHabbo() != null && Client.GetHabbo().GetMessenger() != null &&
             !Client.GetHabbo().GetMessenger().AppearOffline);

        public bool InRoom => (CurrentRoom != null);

        public Room CurrentRoom { get; set; }

        #endregion

        #region Constructor

        public MessengerBuddy(int userId, string pUsername, string pLook, string pMotto, int pLastOnline,
            bool pAppearOffline, bool pHideInRoom)
        {
            UserId = userId;
            MUsername = pUsername;
            MLook = pLook;
            MMotto = pMotto;
            MLastOnline = pLastOnline;
            MAppearOffline = pAppearOffline;
            MHideInRoom = pHideInRoom;
        }

        #endregion

        #region Methods

        public void UpdateUser(GameClient client)
        {
            Client = client;
            if (client != null && client.GetHabbo() != null)
                CurrentRoom = client.GetHabbo().CurrentRoom;
        }

        public void Serialize(ServerPacket message, Habbo habbo)
        {
            Relationship relationship = null;

            if (habbo != null && habbo.Relationships != null)
                relationship = habbo.Relationships.FirstOrDefault(x => x.Value.UserId == Convert.ToInt32(UserId)).Value;

            int y = relationship == null ? 0 : relationship.Type;

            message.WriteInteger(UserId);
            message.WriteString(MUsername);
            message.WriteInteger(1);
            message.WriteBoolean(!MAppearOffline || habbo.GetPermissions().HasRight("mod_tool") ? IsOnline : false);
            message.WriteBoolean(!MHideInRoom || habbo.GetPermissions().HasRight("mod_tool") ? InRoom : false);
            message.WriteString(IsOnline ? MLook : "");
            message.WriteInteger(0); // categoryid
            message.WriteString(MMotto);
            message.WriteString(string.Empty); // Facebook username
            message.WriteString(string.Empty);
            message.WriteBoolean(true); // Allows offline messaging
            message.WriteBoolean(false); // ?
            message.WriteBoolean(false); // Uses phone
            message.WriteShort(y);
        }

        #endregion
    }
}