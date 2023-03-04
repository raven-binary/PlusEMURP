using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using System.Collections.Generic;
using Plus.HabboHotel.Items;
using Plus.Communication.Packets.Outgoing.Inventory.Furni;
using System.Threading.Tasks;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class PickAllCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
        }

        public string TypeCommand
        {
            get { return "appart"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Picks up all of the furniture from your room"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
                return;

            if (!Session.GetHabbo().isLoggedIn && Session.GetHabbo().Rank > 4)
            {
                PlusEnvironment.GetGame().GetClientManager().StaffWhisper(Session.GetHabbo().Username + " kicked out from the client! Reason: tried to :pickle without verifying themselves");
                using (IQueryAdapter queryAdapter = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryAdapter.SetQuery(("UPDATE `users` SET rank = '1' WHERE `id` = " + Session.GetHabbo().Id) ?? "");
                }
                Task t = Task.Run(async delegate
                {
                    await Task.Delay(1000);
                    Session.Disconnect();
                });
                return;
            }

            Room.GetRoomItemHandler().RemoveItems(Session);

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `items` SET `room_id` = '0' WHERE `room_id` = @RoomId AND `user_id` = @UserId");
                dbClient.AddParameter("RoomId", Room.Id);
                dbClient.AddParameter("UserId", Session.GetHabbo().Id);
                dbClient.RunQuery();
            }

            List<Item> Items = Room.GetRoomItemHandler().GetWallAndFloor.ToList();
            if (Items.Count > 0)
                Session.SendWhisper("There are still more items in this room, manually remove them or use :ejectall to eject them!");

            Session.SendMessage(new FurniListUpdateComposer());
        }
    }
}