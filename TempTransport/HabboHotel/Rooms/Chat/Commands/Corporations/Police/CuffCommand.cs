using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections.Generic;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.HabboRoleplay.Misc;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class CuffCommand : IChatCommand
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
            get { return "Cuffs a player and starts escorting them"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please specific a target");
                return;
            }

            if (Session.GetHabbo().getCooldown("cuff"))
            {
                Session.SendWhisper("You must wait before you can cuff again");
                return;
            }

            if (!Session.GetRoleplay().Inventory.HasItem("handcuffs", true))
            {
                Session.SendWhisper("You need cuffs to perform this action");
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

            if (TargetClient.GetHabbo().Prison != 0 || TargetClient.GetHabbo().Hospital == 1)
                return;

            if (Session.GetRoleplay().Cuffed)
            {
                Session.SendWhisper("You cannot perform this action while cuffed");
                return;
            }

            if (Session.GetRoleplay().Escorting && Session.GetRoleplay().EscortUsername != null)
            {
                Session.SendWhisper("You cannot perform this action while escorting");
                return;
            }

            if (TargetClient.GetRoleplay().EscortBy != null)
            {
                Session.SendWhisper("You cannot perform this action while escorting someone");
                return;
            }

            if (TargetClient.GetRoleplay().StunType == "stungun" || TargetClient.GetRoleplay().StunType == "flashbang")
            {
                RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

                #region Distance
                Point ClientPos = new Point(User.SetX, User.SetY);
                Point TargetClientPos = new Point(TargetUser.SetX, TargetUser.SetY);
                double Distance = RoleplayManager.GetDistanceBetweenPoints2D(ClientPos, TargetClientPos);

                if (Distance > 1)
                {
                    Session.SendWhisper(TargetClient.GetHabbo().Username + " is too far away");
                    return;
                }
                #endregion

                Session.GetHabbo().addCooldown("cuff", 2500);
                TargetClient.GetRoleplay().Cuffed = true;
                Session.GetRoleplay().StartEscorting(TargetClient, TargetUser);
                Session.GetRoleplay().Inventory.Remove(Session.GetRoleplay().Inventory.FromSlot);

                User.Say("wraps their cuffs around " + TargetClient.GetHabbo().Username + "'s wrist");
                User.Say("grabs " + TargetClient.GetHabbo().Username + " by the cuffs and starts escorting them");
            }
            else
            {
                Session.SendWhisper("You must stun your target first");
            }
        }
    }
}