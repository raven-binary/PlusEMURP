using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using WebHook;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class StunCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().JobId == 1 && Session.GetHabbo().Working)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "job"; }
        }

        public string Parameters
        {
            get { return "<username>"; }
        }

        public string Description
        {
            get { return "Stuns the target player"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please specific a target");
                return;
            }

            if (Session.GetHabbo().getCooldown("stun"))
            {
                Session.SendWhisper("You must wait before you can stun someone again");
                return;
            }

            if (Session.GetRoleplay().Inventory.WeaponEquipped != "stungun")
            {
                Session.SendWhisper("You have to equip your stun gun to stun someone");
                return;
            }

            if (!Session.GetRoleplay().Inventory.EquipHasDurability("stungun", 14))
            {
                Session.SendWhisper("You are out of stuns head back to LVPD to replenish");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper("Player not found in this room");
                return;
            }

            if (TargetClient.GetHabbo().Id == Session.GetHabbo().Id)
                return;

            if (Session.GetRoleplay().Escort)
            {
                Session.SendWhisper("You cannot perform this action while being escorted");
                return;
            }

            if (TargetClient.GetRoleplay().Duty)
            {
                Session.SendWhisper("You cannot perform this action to a staff member while they're on duty");
                return;
            }

            if (TargetClient.GetHabbo().Prison != 0 || TargetClient.GetHabbo().Hospital == 1)
            {
                Session.SendWhisper("You cannot stun a player in a hospital or prison");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

            Session.GetRoleplay().Inventory.UpdateDurability(Session.GetRoleplay().Inventory.WeaponEquipped, 14);
            Session.GetHabbo().addCooldown("stun", 2000);

            if (Math.Abs(User.Y - TargetUser.Y) > 2 || Math.Abs(User.X - TargetUser.X) > 2)
            {
                User.Say("fires their stungun at " + TargetClient.GetHabbo().Username + ", but misses", 5);
                return;
            }

            User.Say("fires their stun gun at " + TargetClient.GetHabbo().Username, 5);
            Session.GetRoleplay().CreateAggression(100);
            TargetClient.GetRoleplay().Stun("stungun");

            //Webhook.SendCopFeed(Session.GetHabbo().RankInfo.Name + " **" + Session.GetHabbo().Username + "** stunned **" + TargetClient.GetHabbo().Username + "**");
        }
    }
}