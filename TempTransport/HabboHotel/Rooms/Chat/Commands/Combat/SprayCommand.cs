using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class SprayCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
        }

        public string TypeCommand
        {
            get { return "combat"; }
        }

        public string Parameters
        {
            get { return "<username>"; }
        }

        public string Description
        {
            get { return "Sprays a player and blinds them"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please specific a target");
                return;
            }

            if (Session.GetHabbo().getCooldown("spray_command"))
            {
                Session.SendWhisper("Wait a bit before you can spray someone again");
                return;
            }

            #region Slot
            string Slot = string.Empty;
            if (Session.GetHabbo().InventorySlot1 == "stolenpepperspray")
            {
                Slot = "slot1";
            }
            else if (Session.GetHabbo().InventorySlot2 == "stolenpepperspray")
            {
                Slot = "slot2";
            }
            else if (Session.GetHabbo().InventorySlot3 == "stolenpepperspray")
            {
                Slot = "slot3";
            }
            else if (Session.GetHabbo().InventorySlot4 == "stolenpepperspray")
            {
                Slot = "slot4";
            }
            else if (Session.GetHabbo().InventorySlot5 == "stolenpepperspray")
            {
                Slot = "slot5";
            }
            else if (Session.GetHabbo().InventorySlot6 == "stolenpepperspray")
            {
                Slot = "slot6";
            }
            else if (Session.GetHabbo().InventorySlot7 == "stolenpepperspray")
            {
                Slot = "slot7";
            }
            else if (Session.GetHabbo().InventorySlot8 == "stolenpepperspray")
            {
                Slot = "slot8";
            }
            else if (Session.GetHabbo().InventorySlot9 == "stolenpepperspray")
            {
                Slot = "slot9";
            }
            else if (Session.GetHabbo().InventorySlot10 == "stolenpepperspray")
            {
                Slot = "slot10";
            }
            else
            {
                Session.SendWhisper("You don't have any medkit");
                return;
            }
            #endregion

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper("Player not found in this room");
                return;
            }

            if (TargetClient.GetHabbo().Id == Session.GetHabbo().Id)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

            if (Session.GetHabbo().CheckSafezone(TargetUser.GetUsername()))
                return;

            if (Math.Abs(User.Y - TargetUser.Y) > 2 || Math.Abs(User.X - TargetUser.X) > 2 || User.X != TargetUser.X && User.Y != TargetUser.Y)
            {
                User.Say("sprays Pepper Spray at " + TargetClient.GetHabbo().Username + ", but misses");
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "inventory", "update_quantity," + Slot + ",-,1");
                return;
            }

            User.Say("sprays Pepper Spray at " + TargetClient.GetHabbo().Username + " blinding them");
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "blind;show;");
            TargetClient.SendWhisper("You feel confused");
            TargetUser.CanWalk = false;

            Session.GetHabbo().addCooldown("spray_command", 3000);
            PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "inventory", "update_quantity," + Slot + ",-,1");

            TargetUser.MoveTo(TargetUser.X - 1, TargetUser.Y, true);

            System.Timers.Timer Walk1 = new System.Timers.Timer(1000);
            Walk1.Interval = 1000;
            Walk1.Elapsed += delegate
            {
                TargetUser.MoveTo(TargetUser.X + 1, TargetUser.Y, true);
                Walk1.Stop();
            };
            Walk1.Start();

            System.Timers.Timer Walk2 = new System.Timers.Timer(2000);
            Walk2.Interval = 2000;
            Walk2.Elapsed += delegate
            {
                TargetUser.MoveTo(TargetUser.X, TargetUser.Y + 1, true);
                Walk2.Stop();
            };
            Walk2.Start();

            System.Timers.Timer Walk3 = new System.Timers.Timer(3000);
            Walk3.Interval = 3000;
            Walk3.Elapsed += delegate
            {
                TargetUser.MoveTo(TargetUser.X, TargetUser.Y - 1, true);
                Walk3.Stop();
            };
            Walk3.Start();

            System.Timers.Timer Walk4 = new System.Timers.Timer(4000);
            Walk4.Interval = 4000;
            Walk4.Elapsed += delegate
            {
                TargetUser.MoveTo(TargetUser.X + 1, TargetUser.Y, true);
                Walk4.Stop();
            };
            Walk4.Start();

            System.Timers.Timer Walk5 = new System.Timers.Timer(5000);
            Walk5.Interval = 5000;
            Walk5.Elapsed += delegate
            {
                TargetUser.MoveTo(TargetUser.X - 1, TargetUser.Y, true);
                Walk5.Stop();
            };
            Walk5.Start();

            System.Timers.Timer SprayTimer = new System.Timers.Timer(10000);
            SprayTimer.Interval = 10000;
            SprayTimer.Elapsed += delegate
            {
                TargetUser.CanWalk = true;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "blind;hide;");
                SprayTimer.Stop();
            };
            SprayTimer.Start();
        }
    }
}
