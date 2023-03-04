using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class EquipCommand : IChatCommand
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
            get { return "<weapon>"; }
        }

        public string Description
        {
            get { return "Equip a weapon"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :eq <weapon>");
                return;
            }

            string Arme = Params[1];
            string Slot = Params[2];
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (Session.GetHabbo().getCooldown("eq_command") == true)
            {
                Session.SendWhisper("Please wait before you equip your weapon again");
                return;
            }

            if (Session.GetHabbo().ArmeEquiped != null)
            {
                if (Session.GetHabbo().ArmeEquiped == "bat")
                {
                    Session.GetHabbo().InventoryEquip("bat", Session.GetHabbo().RPItems().BatDamage);
                }
                else if (Session.GetHabbo().ArmeEquiped == "sword")
                {
                    Session.GetHabbo().InventoryEquip("sword", Session.GetHabbo().RPItems().SwordDamage);
                }
                else if (Session.GetHabbo().ArmeEquiped == "axe")
                {
                    Session.GetHabbo().InventoryEquip("axe", Session.GetHabbo().RPItems().AxeDamage);
                }
            }

            if (User.isTradingItems)
            {
                Session.SendWhisper("You cannot equip a weapon while trading.");
                return;
            }
            int Enable;

            Session.GetHabbo().addCooldown("eq_command", 3000);
            if (Arme == "bat")
            {
                if (Session.GetHabbo().RPItems().Bat == "")
                {
                    Session.SendWhisper("You don't have any Bat!");
                    return;
                }

                if (Session.GetHabbo().RPItems().Bat == "Mat")
                {
                    Enable = 709;
                }
                else if (Session.GetHabbo().RPItems().Bat == "Fat")
                {
                    Enable = 710;
                }
                else if (Session.GetHabbo().RPItems().Bat == "Wat")
                {
                    Enable = 711;
                }
                else if (Session.GetHabbo().RPItems().Bat == "Bat")
                {
                    Enable = 591;
                }
                else if (Session.GetHabbo().RPItems().Bat == "Zat")
                {
                    Enable = 683;
                }
                else
                {
                    return;
                }
            }
            else if (Arme == "sword")
            {
                if (Session.GetHabbo().RPItems().Sword == "")
                {
                    Session.SendWhisper("You don't have any Sword!");
                    return;
                }

                if (Session.GetHabbo().RPItems().Sword == "PirateSworg")
                {
                    Enable = 708;
                }
                else if (Session.GetHabbo().RPItems().Sword == "PirateSword")
                {
                    Enable = 162;
                }
                else if (Session.GetHabbo().RPItems().Sword == "PirateSwort")
                {
                    Enable = 714;
                }
                else if (Session.GetHabbo().RPItems().Sword == "PirateSworp")
                {
                    Enable = 719;
                }
                else if (Session.GetHabbo().RPItems().Sword == "ButterKnife")
                {
                    Enable = 672;
                }
                else if (Session.GetHabbo().RPItems().Sword == "SamuraiSwor")
                {
                    Enable = 673;
                }
                else if (Session.GetHabbo().RPItems().Sword == "SamuraiSwoo")
                {
                    Enable = 674;
                }
                else if (Session.GetHabbo().RPItems().Sword == "SwordAlmigh")
                {
                    Enable = 678;
                }
                else if (Session.GetHabbo().RPItems().Sword == "SwordAlmigo")
                {
                    Enable = 679;

                }
                else if (Session.GetHabbo().RPItems().Sword == "SwordAlmigf")
                {
                    Enable = 682;
                }
                else
                {
                    return;
                }
            }
            else if (Arme == "axe")
            {
                if (Session.GetHabbo().RPItems().Axe == "")
                {
                    Session.SendWhisper("You don't have any Axe!");
                    return;
                }

                if (Session.GetHabbo().RPItems().Axe == "Executioneb")
                {
                    Enable = 707;
                }
                else if (Session.GetHabbo().RPItems().Axe == "Executionew")
                {
                    Enable = 712;
                }
                else if (Session.GetHabbo().RPItems().Axe == "Executionbg")
                {
                    Enable = 713;
                }
                else if (Session.GetHabbo().RPItems().Axe == "Executionep")
                {
                    Enable = 675;
                }
                else if (Session.GetHabbo().RPItems().Axe == "Executioneo")
                {
                    Enable = 676;
                }
                else
                {
                    return;
                }
            }
            else
            {
                Session.SendWhisper("The specified weapon is invalid.");
                return;
            }

            if (Session.GetHabbo().Conduit == null)
            {
                Session.GetHabbo().ArmeEquiped = Arme;
                Session.GetHabbo().resetEffectEvent();
            }
            if (Arme == "bat")
            {
                Session.GetHabbo().InventoryEquip("bat", Session.GetHabbo().RPItems().BatDamage);
            }
            else if (Arme == "sword")
            {
                Session.GetHabbo().InventoryEquip("sword", Session.GetHabbo().RPItems().SwordDamage);
            }
            else if (Arme == "axe")
            {
                Session.GetHabbo().InventoryEquip("axe", Session.GetHabbo().RPItems().AxeDamage);
            }
        }
    }
}