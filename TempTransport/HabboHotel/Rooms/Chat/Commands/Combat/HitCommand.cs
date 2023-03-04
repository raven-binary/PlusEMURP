using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;
using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.HabboHotel.Items;
using System.Drawing;
using Plus.HabboHotel.Pathfinding;
using Plus.HabboRoleplay.Misc;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class HitCommand : IChatCommand
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
            get { return "Hits a player"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please specific a target");
                return;
            }

            if (Session.GetHabbo().getCooldown("hit"))
            {
                Session.SendWhisper("You must wait before you can swing again");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper("Player not found in this room");
                return;
            }

            if (Session.GetRoleplay().Energy < 3)
            {
                User.GetClient().SendWhisper("You need at least 2 energy to perform this action");
                return;
            }

            if (Session.GetHabbo().CheckSafezone(Username) || Session.GetHabbo().CheckWarnings())
                return;

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

            Session.GetHabbo().addCooldown("hit", 3500);
            int Minimum;
            int Maximum;
            int Damage = 0;
            int HandDamage = 2;
            #region Damage
            if (Session.GetRoleplay().Inventory.WeaponEquipped == "bat")
            {
                Damage = 2;
                if (Session.GetRoleplay().CombatLevel == 1)
                {
                    Minimum = HandDamage + Damage + 1;
                    Maximum = HandDamage + Damage + 2;
                }
                else if (Session.GetRoleplay().CombatLevel == 2)
                {
                    Minimum = HandDamage + Damage + 1;
                    Maximum = HandDamage + Damage + 3;
                }
                else if (Session.GetRoleplay().CombatLevel == 3)
                {
                    Minimum = HandDamage + Damage + 2;
                    Maximum = HandDamage + Damage + 3;
                }
                else if (Session.GetRoleplay().CombatLevel == 4)
                {
                    Minimum = HandDamage + Damage + 2;
                    Maximum = HandDamage + Damage + 4;
                }
                else if (Session.GetRoleplay().CombatLevel == 5)
                {
                    Minimum = HandDamage + Damage + 3;
                    Maximum = HandDamage + Damage + 5;
                }
                else if (Session.GetRoleplay().CombatLevel == 6)
                {
                    Minimum = HandDamage + Damage + 4;
                    Maximum = HandDamage + Damage + 5;
                }
                else if (Session.GetRoleplay().CombatLevel == 7)
                {
                    Minimum = HandDamage + Damage + 4;
                    Maximum = HandDamage + Damage + 6;
                }
                else if (Session.GetRoleplay().CombatLevel == 8)
                {
                    Minimum = HandDamage + Damage + 5;
                    Maximum = HandDamage + Damage + 6;
                }
                else if (Session.GetRoleplay().CombatLevel == 9)
                {
                    Minimum = HandDamage + Damage + 5;
                    Maximum = HandDamage + Damage + 7;
                }
                else if (Session.GetRoleplay().CombatLevel == 10)
                {
                    Minimum = HandDamage + Damage + 6;
                    Maximum = HandDamage + Damage + 7;
                }
                else if (Session.GetRoleplay().CombatLevel == 11)
                {
                    Minimum = HandDamage + Damage + 6;
                    Maximum = HandDamage + Damage + 8;
                }
                else if (Session.GetRoleplay().CombatLevel == 12)
                {
                    Minimum = HandDamage + Damage + 7;
                    Maximum = HandDamage + Damage + 8;
                }
                else if (Session.GetRoleplay().CombatLevel == 13)
                {
                    Minimum = HandDamage + Damage + 7;
                    Maximum = HandDamage + Damage + 9;
                }
                else if (Session.GetRoleplay().CombatLevel == 14)
                {
                    Minimum = HandDamage + Damage + 8;
                    Maximum = HandDamage + Damage + 9;
                }
                else if (Session.GetRoleplay().CombatLevel == 15)
                {
                    Minimum = HandDamage + Damage + 8;
                    Maximum = HandDamage + Damage + 10;
                }
                else if (Session.GetRoleplay().CombatLevel == 16)
                {
                    Minimum = HandDamage + Damage + 9;
                    Maximum = HandDamage + Damage + 10;
                }
                else if (Session.GetRoleplay().CombatLevel == 17)
                {
                    Minimum = HandDamage + Damage + 9;
                    Maximum = HandDamage + Damage + 11;
                }
                else if (Session.GetRoleplay().CombatLevel == 18)
                {
                    Minimum = HandDamage + Damage + 10;
                    Maximum = HandDamage + Damage + 11;
                }
                else if (Session.GetRoleplay().CombatLevel == 19)
                {
                    Minimum = HandDamage + Damage + 10;
                    Maximum = HandDamage + Damage + 12;
                }
                else if (Session.GetRoleplay().CombatLevel == 20)
                {
                    Minimum = HandDamage + Damage + 11;
                    Maximum = HandDamage + Damage + 12;
                }
                else if (Session.GetRoleplay().CombatLevel == 21)
                {
                    Minimum = HandDamage + Damage + 11;
                    Maximum = HandDamage + Damage + 13;
                }
                else if (Session.GetRoleplay().CombatLevel == 22)
                {
                    Minimum = HandDamage + Damage + 12;
                    Maximum = HandDamage + Damage + 13;
                }
                else if (Session.GetRoleplay().CombatLevel == 23)
                {
                    Minimum = HandDamage + Damage + 12;
                    Maximum = HandDamage + Damage + 14;
                }
                else if (Session.GetRoleplay().CombatLevel == 24)
                {
                    Minimum = HandDamage + Damage + 13;
                    Maximum = HandDamage + Damage + 14;
                }
                else if (Session.GetRoleplay().CombatLevel == 25)
                {
                    Minimum = HandDamage + Damage + 14;
                    Maximum = HandDamage + Damage + 15;
                }
                else
                {
                    Minimum = HandDamage + Damage;
                    Maximum = HandDamage + Damage;
                }
            }
            else if (Session.GetRoleplay().Inventory.WeaponEquipped == "sword")
            {
                Damage = 5;
                if (Session.GetRoleplay().CombatLevel == 1)
                {
                    Minimum = HandDamage + Damage + 1;
                    Maximum = HandDamage + Damage + 2;
                }
                else if (Session.GetRoleplay().CombatLevel == 2)
                {
                    Minimum = HandDamage + Damage + 1;
                    Maximum = HandDamage + Damage + 3;
                }
                else if (Session.GetRoleplay().CombatLevel == 3)
                {
                    Minimum = HandDamage + Damage + 2;
                    Maximum = HandDamage + Damage + 3;
                }
                else if (Session.GetRoleplay().CombatLevel == 4)
                {
                    Minimum = HandDamage + Damage + 2;
                    Maximum = HandDamage + Damage + 4;
                }
                else if (Session.GetRoleplay().CombatLevel == 5)
                {
                    Minimum = HandDamage + Damage + 3;
                    Maximum = HandDamage + Damage + 5;
                }
                else if (Session.GetRoleplay().CombatLevel == 6)
                {
                    Minimum = HandDamage + Damage + 4;
                    Maximum = HandDamage + Damage + 5;
                }
                else if (Session.GetRoleplay().CombatLevel == 7)
                {
                    Minimum = HandDamage + Damage + 4;
                    Maximum = HandDamage + Damage + 6;
                }
                else if (Session.GetRoleplay().CombatLevel == 8)
                {
                    Minimum = HandDamage + Damage + 5;
                    Maximum = HandDamage + Damage + 6;
                }
                else if (Session.GetRoleplay().CombatLevel == 9)
                {
                    Minimum = HandDamage + Damage + 5;
                    Maximum = HandDamage + Damage + 7;
                }
                else if (Session.GetRoleplay().CombatLevel == 10)
                {
                    Minimum = HandDamage + Damage + 6;
                    Maximum = HandDamage + Damage + 7;
                }
                else if (Session.GetRoleplay().CombatLevel == 11)
                {
                    Minimum = HandDamage + Damage + 6;
                    Maximum = HandDamage + Damage + 8;
                }
                else if (Session.GetRoleplay().CombatLevel == 12)
                {
                    Minimum = HandDamage + Damage + 7;
                    Maximum = HandDamage + Damage + 8;
                }
                else if (Session.GetRoleplay().CombatLevel == 13)
                {
                    Minimum = HandDamage + Damage + 7;
                    Maximum = HandDamage + Damage + 9;
                }
                else if (Session.GetRoleplay().CombatLevel == 14)
                {
                    Minimum = HandDamage + Damage + 8;
                    Maximum = HandDamage + Damage + 9;
                }
                else if (Session.GetRoleplay().CombatLevel == 15)
                {
                    Minimum = HandDamage + Damage + 8;
                    Maximum = HandDamage + Damage + 10;
                }
                else if (Session.GetRoleplay().CombatLevel == 16)
                {
                    Minimum = HandDamage + Damage + 9;
                    Maximum = HandDamage + Damage + 10;
                }
                else if (Session.GetRoleplay().CombatLevel == 17)
                {
                    Minimum = HandDamage + Damage + 9;
                    Maximum = HandDamage + Damage + 11;
                }
                else if (Session.GetRoleplay().CombatLevel == 18)
                {
                    Minimum = HandDamage + Damage + 10;
                    Maximum = HandDamage + Damage + 11;
                }
                else if (Session.GetRoleplay().CombatLevel == 19)
                {
                    Minimum = HandDamage + Damage + 10;
                    Maximum = HandDamage + Damage + 12;
                }
                else if (Session.GetRoleplay().CombatLevel == 20)
                {
                    Minimum = HandDamage + Damage + 11;
                    Maximum = HandDamage + Damage + 12;
                }
                else if (Session.GetRoleplay().CombatLevel == 21)
                {
                    Minimum = HandDamage + Damage + 11;
                    Maximum = HandDamage + Damage + 13;
                }
                else if (Session.GetRoleplay().CombatLevel == 22)
                {
                    Minimum = HandDamage + Damage + 12;
                    Maximum = HandDamage + Damage + 13;
                }
                else if (Session.GetRoleplay().CombatLevel == 23)
                {
                    Minimum = HandDamage + Damage + 12;
                    Maximum = HandDamage + Damage + 14;
                }
                else if (Session.GetRoleplay().CombatLevel == 24)
                {
                    Minimum = HandDamage + Damage + 13;
                    Maximum = HandDamage + Damage + 14;
                }
                else if (Session.GetRoleplay().CombatLevel == 25)
                {
                    Minimum = HandDamage + Damage + 14;
                    Maximum = HandDamage + Damage + 15;
                }
                else
                {
                    Minimum = HandDamage + Damage;
                    Maximum = HandDamage + Damage;
                }
            }
            else if (Session.GetRoleplay().Inventory.WeaponEquipped == "axe")
            {
                Damage = 3;
                if (Session.GetRoleplay().CombatLevel == 1)
                {
                    Minimum = HandDamage + Damage + 1;
                    Maximum = HandDamage + Damage + 2;
                }
                else if (Session.GetRoleplay().CombatLevel == 2)
                {
                    Minimum = HandDamage + Damage + 1;
                    Maximum = HandDamage + Damage + 3;
                }
                else if (Session.GetRoleplay().CombatLevel == 3)
                {
                    Minimum = HandDamage + Damage + 2;
                    Maximum = HandDamage + Damage + 3;
                }
                else if (Session.GetRoleplay().CombatLevel == 4)
                {
                    Minimum = HandDamage + Damage + 2;
                    Maximum = HandDamage + Damage + 4;
                }
                else if (Session.GetRoleplay().CombatLevel == 5)
                {
                    Minimum = HandDamage + Damage + 3;
                    Maximum = HandDamage + Damage + 5;
                }
                else if (Session.GetRoleplay().CombatLevel == 6)
                {
                    Minimum = HandDamage + Damage + 4;
                    Maximum = HandDamage + Damage + 5;
                }
                else if (Session.GetRoleplay().CombatLevel == 7)
                {
                    Minimum = HandDamage + Damage + 4;
                    Maximum = HandDamage + Damage + 6;
                }
                else if (Session.GetRoleplay().CombatLevel == 8)
                {
                    Minimum = HandDamage + Damage + 5;
                    Maximum = HandDamage + Damage + 6;
                }
                else if (Session.GetRoleplay().CombatLevel == 9)
                {
                    Minimum = HandDamage + Damage + 5;
                    Maximum = HandDamage + Damage + 7;
                }
                else if (Session.GetRoleplay().CombatLevel == 10)
                {
                    Minimum = HandDamage + Damage + 6;
                    Maximum = HandDamage + Damage + 7;
                }
                else if (Session.GetRoleplay().CombatLevel == 11)
                {
                    Minimum = HandDamage + Damage + 6;
                    Maximum = HandDamage + Damage + 8;
                }
                else if (Session.GetRoleplay().CombatLevel == 12)
                {
                    Minimum = HandDamage + Damage + 7;
                    Maximum = HandDamage + Damage + 8;
                }
                else if (Session.GetRoleplay().CombatLevel == 13)
                {
                    Minimum = HandDamage + Damage + 7;
                    Maximum = HandDamage + Damage + 9;
                }
                else if (Session.GetRoleplay().CombatLevel == 14)
                {
                    Minimum = HandDamage + Damage + 8;
                    Maximum = HandDamage + Damage + 9;
                }
                else if (Session.GetRoleplay().CombatLevel == 15)
                {
                    Minimum = HandDamage + Damage + 8;
                    Maximum = HandDamage + Damage + 10;
                }
                else if (Session.GetRoleplay().CombatLevel == 16)
                {
                    Minimum = HandDamage + Damage + 9;
                    Maximum = HandDamage + Damage + 10;
                }
                else if (Session.GetRoleplay().CombatLevel == 17)
                {
                    Minimum = HandDamage + Damage + 9;
                    Maximum = HandDamage + Damage + 11;
                }
                else if (Session.GetRoleplay().CombatLevel == 18)
                {
                    Minimum = HandDamage + Damage + 10;
                    Maximum = HandDamage + Damage + 11;
                }
                else if (Session.GetRoleplay().CombatLevel == 19)
                {
                    Minimum = HandDamage + Damage + 10;
                    Maximum = HandDamage + Damage + 12;
                }
                else if (Session.GetRoleplay().CombatLevel == 20)
                {
                    Minimum = HandDamage + Damage + 11;
                    Maximum = HandDamage + Damage + 12;
                }
                else if (Session.GetRoleplay().CombatLevel == 21)
                {
                    Minimum = HandDamage + Damage + 11;
                    Maximum = HandDamage + Damage + 13;
                }
                else if (Session.GetRoleplay().CombatLevel == 22)
                {
                    Minimum = HandDamage + Damage + 12;
                    Maximum = HandDamage + Damage + 13;
                }
                else if (Session.GetRoleplay().CombatLevel == 23)
                {
                    Minimum = HandDamage + Damage + 12;
                    Maximum = HandDamage + Damage + 14;
                }
                else if (Session.GetRoleplay().CombatLevel == 24)
                {
                    Minimum = HandDamage + Damage + 13;
                    Maximum = HandDamage + Damage + 14;
                }
                else if (Session.GetRoleplay().CombatLevel == 25)
                {
                    Minimum = HandDamage + Damage + 14;
                    Maximum = HandDamage + Damage + 15;
                }
                else
                {
                    Minimum = HandDamage + Damage;
                    Maximum = HandDamage + Damage;
                }
            }
            if (Session.GetRoleplay().Inventory.WeaponEquipped == "stungun")
            {
                Damage = 0;
                if (Session.GetRoleplay().CombatLevel == 1)
                {
                    Minimum = Damage + 1;
                    Maximum = Damage + 2;
                }
                else if (Session.GetRoleplay().CombatLevel == 2)
                {
                    Minimum = Damage + 1;
                    Maximum = Damage + 3;
                }
                else if (Session.GetRoleplay().CombatLevel == 3)
                {
                    Minimum = Damage + 2;
                    Maximum = Damage + 3;
                }
                else if (Session.GetRoleplay().CombatLevel == 4)
                {
                    Minimum = Damage + 2;
                    Maximum = Damage + 4;
                }
                else if (Session.GetRoleplay().CombatLevel == 5)
                {
                    Minimum = Damage + 3;
                    Maximum = Damage + 5;
                }
                else if (Session.GetRoleplay().CombatLevel == 6)
                {
                    Minimum = Damage + 4;
                    Maximum = Damage + 5;
                }
                else if (Session.GetRoleplay().CombatLevel == 7)
                {
                    Minimum = Damage + 4;
                    Maximum = Damage + 6;
                }
                else if (Session.GetRoleplay().CombatLevel == 8)
                {
                    Minimum = Damage + 5;
                    Maximum = Damage + 6;
                }
                else if (Session.GetRoleplay().CombatLevel == 9)
                {
                    Minimum = Damage + 5;
                    Maximum = Damage + 7;
                }
                else if (Session.GetRoleplay().CombatLevel == 10)
                {
                    Minimum = Damage + 6;
                    Maximum = Damage + 7;
                }
                else if (Session.GetRoleplay().CombatLevel == 11)
                {
                    Minimum = Damage + 6;
                    Maximum = Damage + 8;
                }
                else if (Session.GetRoleplay().CombatLevel == 12)
                {
                    Minimum = Damage + 7;
                    Maximum = Damage + 8;
                }
                else if (Session.GetRoleplay().CombatLevel == 13)
                {
                    Minimum = Damage + 7;
                    Maximum = Damage + 9;
                }
                else if (Session.GetRoleplay().CombatLevel == 14)
                {
                    Minimum = Damage + 8;
                    Maximum = Damage + 9;
                }
                else if (Session.GetRoleplay().CombatLevel == 15)
                {
                    Minimum = Damage + 8;
                    Maximum = Damage + 10;
                }
                else if (Session.GetRoleplay().CombatLevel == 16)
                {
                    Minimum = Damage + 9;
                    Maximum = Damage + 10;
                }
                else if (Session.GetRoleplay().CombatLevel == 17)
                {
                    Minimum = Damage + 9;
                    Maximum = Damage + 11;
                }
                else if (Session.GetRoleplay().CombatLevel == 18)
                {
                    Minimum = Damage + 10;
                    Maximum = Damage + 11;
                }
                else if (Session.GetRoleplay().CombatLevel == 19)
                {
                    Minimum = Damage + 10;
                    Maximum = Damage + 12;
                }
                else if (Session.GetRoleplay().CombatLevel == 20)
                {
                    Minimum = Damage + 11;
                    Maximum = Damage + 12;
                }
                else if (Session.GetRoleplay().CombatLevel == 21)
                {
                    Minimum = Damage + 11;
                    Maximum = Damage + 13;
                }
                else if (Session.GetRoleplay().CombatLevel == 22)
                {
                    Minimum = Damage + 12;
                    Maximum = Damage + 13;
                }
                else if (Session.GetRoleplay().CombatLevel == 23)
                {
                    Minimum = Damage + 12;
                    Maximum = Damage + 14;
                }
                else if (Session.GetRoleplay().CombatLevel == 24)
                {
                    Minimum = Damage + 13;
                    Maximum = Damage + 14;
                }
                else if (Session.GetRoleplay().CombatLevel == 25)
                {
                    Minimum = Damage + 14;
                    Maximum = Damage + 15;
                }
                else
                {
                    Minimum = 1;
                    Maximum = 2;
                }
            }
            else
            {//hand
                if (Session.GetRoleplay().CombatLevel == 1)
                {
                    Minimum = HandDamage + 1;
                    Maximum = HandDamage + 2;
                }
                else if (Session.GetRoleplay().CombatLevel == 2)
                {
                    Minimum = HandDamage + 1;
                    Maximum = HandDamage + 3;
                }
                else if (Session.GetRoleplay().CombatLevel == 3)
                {
                    Minimum = HandDamage + 2;
                    Maximum = HandDamage + 3;
                }
                else if (Session.GetRoleplay().CombatLevel == 4)
                {
                    Minimum = HandDamage + 2;
                    Maximum = HandDamage + 4;
                }
                else if (Session.GetRoleplay().CombatLevel == 5)
                {
                    Minimum = HandDamage + 3;
                    Maximum = HandDamage + 5;
                }
                else if (Session.GetRoleplay().CombatLevel == 6)
                {
                    Minimum = HandDamage + 4;
                    Maximum = HandDamage + 5;
                }
                else if (Session.GetRoleplay().CombatLevel == 7)
                {
                    Minimum = HandDamage + 4;
                    Maximum = HandDamage + 6;
                }
                else if (Session.GetRoleplay().CombatLevel == 8)
                {
                    Minimum = HandDamage + 5;
                    Maximum = HandDamage + 6;
                }
                else if (Session.GetRoleplay().CombatLevel == 9)
                {
                    Minimum = HandDamage + 5;
                    Maximum = HandDamage + 7;
                }
                else if (Session.GetRoleplay().CombatLevel == 10)
                {
                    Minimum = HandDamage + 6;
                    Maximum = HandDamage + 7;
                }
                else if (Session.GetRoleplay().CombatLevel == 11)
                {
                    Minimum = HandDamage + 6;
                    Maximum = HandDamage + 8;
                }
                else if (Session.GetRoleplay().CombatLevel == 12)
                {
                    Minimum = HandDamage + 7;
                    Maximum = HandDamage + 8;
                }
                else if (Session.GetRoleplay().CombatLevel == 13)
                {
                    Minimum = HandDamage + 7;
                    Maximum = HandDamage + 9;
                }
                else if (Session.GetRoleplay().CombatLevel == 14)
                {
                    Minimum = HandDamage + 8;
                    Maximum = HandDamage + 9;
                }
                else if (Session.GetRoleplay().CombatLevel == 15)
                {
                    Minimum = HandDamage + 8;
                    Maximum = HandDamage + 10;
                }
                else if (Session.GetRoleplay().CombatLevel == 16)
                {
                    Minimum = HandDamage + 9;
                    Maximum = HandDamage + 10;
                }
                else if (Session.GetRoleplay().CombatLevel == 17)
                {
                    Minimum = HandDamage + 9;
                    Maximum = HandDamage + 11;
                }
                else if (Session.GetRoleplay().CombatLevel == 18)
                {
                    Minimum = HandDamage + 10;
                    Maximum = HandDamage + 11;
                }
                else if (Session.GetRoleplay().CombatLevel == 19)
                {
                    Minimum = HandDamage + 10;
                    Maximum = HandDamage + 12;
                }
                else if (Session.GetRoleplay().CombatLevel == 20)
                {
                    Minimum = HandDamage + 11;
                    Maximum = HandDamage + 12;
                }
                else if (Session.GetRoleplay().CombatLevel == 21)
                {
                    Minimum = HandDamage + 11;
                    Maximum = HandDamage + 13;
                }
                else if (Session.GetRoleplay().CombatLevel == 22)
                {
                    Minimum = HandDamage + 12;
                    Maximum = HandDamage + 13;
                }
                else if (Session.GetRoleplay().CombatLevel == 23)
                {
                    Minimum = HandDamage + 12;
                    Maximum = HandDamage + 14;
                }
                else if (Session.GetRoleplay().CombatLevel == 24)
                {
                    Minimum = HandDamage + 13;
                    Maximum = HandDamage + 14;
                }
                else if (Session.GetRoleplay().CombatLevel == 25)
                {
                    Minimum = HandDamage + 14;
                    Maximum = HandDamage + 15;
                }
                else
                {
                    Minimum = 2;
                    Maximum = 2;
                }
            }
            #endregion


            if (!Session.GetHabbo().usingArena)
            {
                if (RoleplayManager.Hit(User, TargetUser))
                {
              

                    var random = new Random();
                    var list = new List<int> { Damage + Minimum, Damage + Maximum };
                    int index = random.Next(list.Count);
                    int result = list[index];

                    if (TargetClient.GetRoleplay().Health > 0)
                    {
                        if (Session.GetRoleplay().Inventory.WeaponEquipped == null)
                        {
                            User.Say("punches " + TargetClient.GetHabbo().Username + ", dealing " + result + " damage", 0, true);
                            TargetClient.GetRoleplay().Health -= result;
                        }
                        else if (Session.GetRoleplay().Inventory.WeaponEquipped == "bat")
                        {
                            User.Say("swings their bat at " + TargetClient.GetHabbo().Username + ", dealing " + result + " damage", 0, true);
                            TargetClient.GetRoleplay().Health -= result;
                        }
                        else if (Session.GetRoleplay().Inventory.WeaponEquipped == "sword")
                        {
                            User.Say("stabs " + TargetClient.GetHabbo().Username + " with their sword, dealing " + result + " damage", 0, true);
                            TargetClient.GetRoleplay().Health -= result;
                        }
                        else if (Session.GetRoleplay().Inventory.WeaponEquipped == "axe")
                        {
                            Random rand = new Random();
                            int chance = rand.Next(100);

                            if (chance >= 50)
                            {
                                User.Say("swings their axe at " + TargetClient.GetHabbo().Username + ", dealing " + result + " damage, stunning them", 0, true);
                                TargetClient.GetRoleplay().Stun("axe");
                            }
                            else
                                User.Say("swings their axe at " + TargetClient.GetHabbo().Username + ", dealing " + result + " damage", 0, true);

                            TargetClient.GetRoleplay().Health -= result;
                        }
                        else if (Session.GetRoleplay().Inventory.WeaponEquipped == "stungun")
                        {
                            User.Say("whacks " + TargetClient.GetHabbo().Username + " with a stun gun, dealing " + result + " damage", 0, true);
                            TargetClient.GetRoleplay().Health -= result;
                        }
                        
                        if (Session.GetRoleplay().Inventory.WeaponEquipped != null)
                            Session.GetRoleplay().Inventory.UpdateDurability(Session.GetRoleplay().Inventory.WeaponEquipped, 1);

                        Session.GetRoleplay().Energy -= 2;
                        Session.GetRoleplay().RPCache(1);
                        Session.GetRoleplay().CreateAggression();

                        TargetClient.GetHabbo().LastHitFrom = Session.GetHabbo().Username;
                        TargetClient.GetRoleplay().RPCache(1);
                        TargetClient.GetHabbo().HPBarley();

                        if (TargetClient.GetHabbo().UsingMedkit)
                        {
                            TargetClient.GetHabbo().UsingMedkit = false;
                        }

                        if (TargetClient.GetHabbo().UsingHospitalHeal)
                        {
                            TargetClient.GetHabbo().UsingHospitalHeal = false;
                        }
                    }
                }
                else
                {
                    User.Say("swings at " + TargetClient.GetHabbo().Username + ", but misses", 0, true);
                    Session.GetRoleplay().Energy -= 2;
                    Session.GetRoleplay().RPCache(1);
                }
            }
            else
            {
                if (RoleplayManager.Hit(User, TargetUser))
                {
                    if (TargetClient.GetHabbo().ArenaHealth > 10)
                    {
                        User.GetClient().Shout("*punches " + TargetClient.GetHabbo().Username + ", dealing 10 damage*");
                        Session.GetHabbo().ArenaEnergy -= 2;
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "user-stats;" + Session.GetHabbo().Id + ";" + Session.GetHabbo().Username + ";" + Session.GetHabbo().Look + ";" + Session.GetRoleplay().Passive + ";" + Session.GetHabbo().ArenaHealth + ";" + Session.GetHabbo().ArenaHealthMax + ";" + Session.GetHabbo().ArenaEnergy);

                        TargetClient.GetHabbo().ArenaHealth -= 10;
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "user-stats;" + TargetClient.GetHabbo().Id + ";" + TargetClient.GetHabbo().Username + ";" + TargetClient.GetHabbo().Look + ";" + TargetClient.GetRoleplay().Passive + ";" + TargetClient.GetHabbo().ArenaHealth + ";" + TargetClient.GetHabbo().ArenaHealthMax + ";" + TargetClient.GetHabbo().ArenaEnergy);

                        if (TargetClient.GetHabbo().ArenaHealth <= 80 && TargetClient.GetHabbo().ArenaHealth > 60)
                        {
                            TargetUser.ApplyEffect(704);
                        }
                        else if (TargetClient.GetHabbo().ArenaHealth <= 60 && TargetClient.GetHabbo().ArenaHealth > 40)
                        {
                            TargetUser.ApplyEffect(703);
                        }
                        else if (TargetClient.GetHabbo().ArenaHealth <= 40 && TargetClient.GetHabbo().ArenaHealth > 20)
                        {
                            TargetUser.ApplyEffect(702);
                        }
                        else if (TargetClient.GetHabbo().ArenaHealth <= 20)
                        {
                            TargetUser.ApplyEffect(701);
                        }
                    }
                    else
                    {
                        User.GetClient().Shout("*punches " + TargetClient.GetHabbo().Username + ", dealing 10 damage*");
                        Session.GetHabbo().ArenaEnergy -= 2;
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "user-stats;" + Session.GetHabbo().Id + ";" + Session.GetHabbo().Username + ";" + Session.GetHabbo().Look + ";" + Session.GetRoleplay().Passive + ";" + Session.GetRoleplay().Health + ";" + Session.GetRoleplay().HealthMax + ";" + Session.GetRoleplay().Energy);

                        TargetClient.GetHabbo().ArenaHealth -= 10;
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "user-stats;" + TargetClient.GetHabbo().Id + ";" + TargetClient.GetHabbo().Username + ";" + TargetClient.GetHabbo().Look + ";" + TargetClient.GetRoleplay().Passive + ";" + TargetClient.GetRoleplay().Health + ";" + TargetClient.GetRoleplay().HealthMax + ";" + TargetClient.GetRoleplay().Energy);
                        PlusEnvironment.GetGame().GetClientManager().LiveFeed("<span class=\"blue\">" + Session.GetHabbo().Username + "</span> beat <span class=\"red\">" + TargetClient.GetHabbo().Username + "</span> in the arena");

                        Session.GetHabbo().usingArena = false;
                        TargetClient.GetHabbo().usingArena = false;
                        User.DuelToken = null;
                        TargetUser.DuelToken = null;
                        User.DuelUser = null;
                        TargetUser.DuelUser = null;
                        Session.GetHabbo().resetEffectEvent();
                        Session.GetHabbo().resetAvatarEvent();
                        TargetClient.GetHabbo().resetAvatarEvent();
                        TargetClient.GetHabbo().resetEffectEvent();
                        PlusEnvironment.ArenaUsing = false;

                        foreach (Item item in Room.GetRoomItemHandler().GetFloor)
                        {
                            if (item.GetBaseItem().SpriteId == 2536)
                            {
                                Room.GetGameMap().TeleportToItem(User, item);
                                Room.GetGameMap().TeleportToItem(TargetUser, item);
                                Room.GetRoomUserManager().UpdateUserStatusses();
                            }
                        }
                    }
                }
                else
                {
                    User.Say("swings at " + TargetClient.GetHabbo().Username + ", but misses", 0, true);
                    Session.GetRoleplay().Energy = Session.GetRoleplay().Energy - 2;
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "user-stats;" + Session.GetHabbo().Id + ";" + Session.GetHabbo().Username + ";" + Session.GetHabbo().Look + ";" + Session.GetRoleplay().Passive + ";" + Session.GetHabbo().ArenaHealth + ";" + Session.GetHabbo().ArenaHealthMax + ";" + Session.GetHabbo().ArenaEnergy);
                }
            }
        }
    }
}