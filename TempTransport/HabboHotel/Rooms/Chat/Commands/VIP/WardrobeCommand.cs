using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Avatar;
using Plus.HabboHotel.Items;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class WardrobeCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank > 1)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "vip"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Opens Wardrobe for VIP players"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Session.GetHabbo().CurrentRoomId == 70)
            {
                Session.SendWhisper("You can't summon a wardrobe to Forever21");
                return;
            }

            if (Session.GetHabbo().getCooldown("wardrobe_command") == true)
            {
                Session.SendWhisper("You must wait before you can summon a wardrobe again");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            User.Say("summons a wardrobe to change their looks");
            Session.GetHabbo().addCooldown("wardrobe_command", 5000);

            int randomId = PlusEnvironment.GetRandomNumber(123456789, 987654321);

            Item Wardrobe = new Item(randomId, Session.GetHabbo().CurrentRoomId, 5749, "", User.X, User.Y, 0, 0, 0, 0, 0, 0, string.Empty, Room);
            if (Room.GetRoomItemHandler().SetFloorItemByForce(null, Wardrobe, User.X, User.Y, 0, true, false, true))
            {
                Room.SendMessage(new ObjectUpdateComposer(Wardrobe, Session.GetHabbo().Id));
            }

            System.Timers.Timer WardrobeTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(5));
            WardrobeTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(5);
            WardrobeTimer.Elapsed += delegate
            {
                Session.GetHabbo().CurrentRoom.GetRoomItemHandler().RemoveFurniture(null, Wardrobe.Id);
                WardrobeTimer.Stop();
            };
            WardrobeTimer.Start();
        }
    }
}