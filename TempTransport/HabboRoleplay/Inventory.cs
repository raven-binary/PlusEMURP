using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plus.HabboRoleplay.RoleplayUsers;

namespace Plus.HabboRoleplay.Inventory
{
    public class Inv
    {
        /// <summary>
        /// The users roleplay instance
        /// </summary>
        RoleplayUser RoleplayUser;

        #region  Values
        //equip slots area
        public string WeaponEquipped = null;
        public int WeaponEquippedSlot = 0;

        public string EquipSlot1 = null;
        public string EquipSlot1Type = null;
        public string EquipSlot2Type = null;
        public string EquipSlot2 = null;
        public string EquipSlot1Item = null;
        public string EquipSlot2Item = null;
        public int EquipSlot1Durability = 0;
        public int EquipSlot2Durability = 0;
        //from slot for HasItem();
        public string FromSlot = null;
        //slots area
        public string Slot1;
        public string Slot2;
        public string Slot3;
        public string Slot4;
        public string Slot5;
        public string Slot6;
        public string Slot7;
        public string Slot8;
        public string Slot9;
        public string Slot10;
        public string Slot1Type;
        public string Slot2Type;
        public string Slot3Type;
        public string Slot4Type;
        public string Slot5Type;
        public string Slot6Type;
        public string Slot7Type;
        public string Slot8Type;
        public string Slot9Type;
        public string Slot10Type;
        public int Slot1Quantity;
        public int Slot2Quantity;
        public int Slot3Quantity;
        public int Slot4Quantity;
        public int Slot5Quantity;
        public int Slot6Quantity;
        public int Slot7Quantity;
        public int Slot8Quantity;
        public int Slot9Quantity;
        public int Slot10Quantity;
        public int Slot1Durability;
        public int Slot2Durability;
        public int Slot3Durability;
        public int Slot4Durability;
        public int Slot5Durability;
        public int Slot6Durability;
        public int Slot7Durability;
        public int Slot8Durability;
        public int Slot9Durability;
        public int Slot10Durability;
        #endregion

        public Inv(RoleplayUser RoleplayUser, DataRow Inventory)
        {
            this.RoleplayUser = RoleplayUser;
            this.Slot1 = Convert.ToString(Inventory["slot1"]);
            this.Slot1Type = Convert.ToString(Inventory["slot1_type"]);
            this.Slot1Quantity = Convert.ToInt32(Inventory["slot1_quantity"]);
            this.Slot1Durability = Convert.ToInt32(Inventory["slot1_durability"]);
            this.Slot2 = Convert.ToString(Inventory["slot2"]);
            this.Slot2Type = Convert.ToString(Inventory["slot2_type"]);
            this.Slot2Quantity = Convert.ToInt32(Inventory["slot2_quantity"]);
            this.Slot2Durability = Convert.ToInt32(Inventory["slot2_durability"]);
            this.Slot3 = Convert.ToString(Inventory["slot3"]);
            this.Slot3Type = Convert.ToString(Inventory["slot3_type"]);
            this.Slot3Quantity = Convert.ToInt32(Inventory["slot3_quantity"]);
            this.Slot3Durability = Convert.ToInt32(Inventory["slot3_durability"]);
            this.Slot4 = Convert.ToString(Inventory["slot4"]);
            this.Slot4Type = Convert.ToString(Inventory["slot4_type"]);
            this.Slot4Quantity = Convert.ToInt32(Inventory["slot4_quantity"]);
            this.Slot4Durability = Convert.ToInt32(Inventory["slot4_durability"]);
            this.Slot5 = Convert.ToString(Inventory["slot5"]);
            this.Slot5Type = Convert.ToString(Inventory["slot5_type"]);
            this.Slot5Quantity = Convert.ToInt32(Inventory["slot5_quantity"]);
            this.Slot5Durability = Convert.ToInt32(Inventory["slot5_durability"]);
            this.Slot6 = Convert.ToString(Inventory["slot6"]);
            this.Slot6Type = Convert.ToString(Inventory["slot6_type"]);
            this.Slot6Quantity = Convert.ToInt32(Inventory["slot6_quantity"]);
            this.Slot6Durability = Convert.ToInt32(Inventory["slot6_durability"]);
            this.Slot7 = Convert.ToString(Inventory["slot7"]);
            this.Slot7Type = Convert.ToString(Inventory["slot7_type"]);
            this.Slot7Quantity = Convert.ToInt32(Inventory["slot7_quantity"]);
            this.Slot7Durability = Convert.ToInt32(Inventory["slot7_durability"]);
            this.Slot8 = Convert.ToString(Inventory["slot8"]);
            this.Slot8Type = Convert.ToString(Inventory["slot8_type"]);
            this.Slot8Quantity = Convert.ToInt32(Inventory["slot8_quantity"]);
            this.Slot8Durability = Convert.ToInt32(Inventory["slot8_durability"]);
            this.Slot9 = Convert.ToString(Inventory["slot9"]);
            this.Slot9Type = Convert.ToString(Inventory["slot9_type"]);
            this.Slot9Quantity = Convert.ToInt32(Inventory["slot9_quantity"]);
            this.Slot9Durability = Convert.ToInt32(Inventory["slot9_durability"]);
            this.Slot10 = Convert.ToString(Inventory["slot10"]);
            this.Slot10Type = Convert.ToString(Inventory["slot10_type"]);
            this.Slot10Quantity = Convert.ToInt32(Inventory["slot10_quantity"]);
            this.Slot10Durability = Convert.ToInt32(Inventory["slot10_durability"]);
            Load();
        }

        public void Load()
        {
            #region Load
            if (!String.IsNullOrEmpty(Slot1))
            {
                RoleplayUser.SendWeb("inventory-add;slot1;" + Slot1 + ";" + Slot1Quantity + ";" + Slot1Durability);
            }
            else
            {
                Slot1 = null;
                Slot1Type = null;
                Slot1Quantity = 0;
                Slot1Durability = 0;
            }
            if (!String.IsNullOrEmpty(Slot2))
            {
                RoleplayUser.SendWeb("inventory-add;slot2;" + Slot2 + ";" + Slot2Quantity + ";" + Slot2Durability);
            }
            else
            {
                Slot2 = null;
                Slot2Type = null;
                Slot2Quantity = 0;
                Slot2Durability = 0;
            }
            if (!String.IsNullOrEmpty(Slot3))
            {
                RoleplayUser.SendWeb("inventory-add;slot3;" + Slot3 + ";" + Slot3Quantity + ";" + Slot3Durability);
            }
            else
            {
                Slot3 = null;
                Slot3Type = null;
                Slot3Quantity = 0;
                Slot3Durability = 0;
            }
            if (!String.IsNullOrEmpty(Slot4))
            {
                RoleplayUser.SendWeb("inventory-add;slot4;" + Slot4 + ";" + Slot4Quantity + ";" + Slot4Durability);
            }
            else
            {
                Slot4 = null;
                Slot4Type = null;
                Slot4Quantity = 0;
                Slot4Durability = 0;
            }
            if (!String.IsNullOrEmpty(Slot5))
            {
                RoleplayUser.SendWeb("inventory-add;slot5;" + Slot5 + ";" + Slot5Quantity + ";" + Slot5Durability);
            }
            else
            {
                Slot5 = null;
                Slot5Type = null;
                Slot5Quantity = 0;
                Slot5Durability = 0;
            }
            if (!String.IsNullOrEmpty(Slot6))
            {
                RoleplayUser.SendWeb("inventory-add;slot6;" + Slot6 + ";" + Slot6Quantity + ";" + Slot6Durability);
            }
            else
            {
                Slot6 = null;
                Slot6Type = null;
                Slot6Quantity = 0;
                Slot6Durability = 0;
            }
            if (!String.IsNullOrEmpty(Slot7))
            {
                RoleplayUser.SendWeb("inventory-add;slot7;" + Slot7 + ";" + Slot7Quantity + ";" + Slot7Durability);
            }
            else
            {
                Slot7 = null;
                Slot7Type = null;
                Slot7Quantity = 0;
                Slot7Durability = 0;
            }
            if (!String.IsNullOrEmpty(Slot8))
            {
                RoleplayUser.SendWeb("inventory-add;slot8;" + Slot8 + ";" + Slot8Quantity + ";" + Slot8Durability);
            }
            else
            {
                Slot8 = null;
                Slot8Type = null;
                Slot8Quantity = 0;
                Slot8Durability = 0;
            }
            if (!String.IsNullOrEmpty(Slot9))
            {
                RoleplayUser.SendWeb("inventory-add;slot9;" + Slot9 + ";" + Slot9Quantity + ";" + Slot9Durability);
            }
            else
            {
                Slot9 = null;
                Slot9Type = null;
                Slot9Quantity = 0;
                Slot9Durability = 0;
            }
            if (!String.IsNullOrEmpty(Slot10))
            {
                RoleplayUser.SendWeb("inventory-add;slot10;" + Slot10 + ";" + Slot10Quantity + ";" + Slot10Durability);
            }
            else
            {
                Slot10 = null;
                Slot10Type = null;
                Slot10Quantity = 0;
                Slot10Durability = 0;
            }
            #endregion
        }
        public void Add(string item, string type, int quantity, int durability, bool isQuantity, bool isDurability)
        {
            if (isQuantity)
            {
                if (Slot1 == item && Slot1Quantity < 5)
                {
                    if (quantity == 5)
                    {
                        if (Slot1Quantity == 4)
                        {
                            Slot1Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot1Quantity == 3)
                        {
                            Slot1Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else if (Slot1Quantity == 2)
                        {
                            Slot1Quantity += 3;
                            Add(item, type, quantity - 3, 0, true, false);
                        }
                        else if (Slot1Quantity == 1)
                        {
                            Slot1Quantity += 4;
                            Add(item, type, quantity - 4, 0, true, false);
                        }
                    }
                    else if (quantity == 4)
                    {
                        if (Slot1Quantity == 4)
                        {
                            Slot1Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot1Quantity == 3)
                        {
                            Slot1Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else if (Slot1Quantity == 2)
                        {
                            Slot1Quantity += 3;
                            Add(item, type, quantity - 3, 0, true, false);
                        }
                        else
                        {
                            Slot1Quantity += quantity;
                        }
                    }
                    else if (quantity == 3)
                    {
                        if (Slot1Quantity == 4)
                        {
                            Slot1Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot1Quantity == 3)
                        {
                            Slot1Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else
                        {
                            Slot1Quantity += quantity;
                        }
                    }
                    else if (quantity == 2)
                    {
                        if (Slot1Quantity == 4)
                        {
                            Slot1Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else
                        {
                            Slot1Quantity += quantity;
                        }
                    }
                    else
                    {
                        Slot1Quantity += quantity;
                    }
                    RoleplayUser.SendWeb("inventory-quantity-update;slot1;" + Slot1Quantity);
                }
                else if (Slot2 == item && Slot2Quantity < 5)
                {
                    if (quantity == 5)
                    {
                        if (Slot2Quantity == 4)
                        {
                            Slot2Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot2Quantity == 3)
                        {
                            Slot2Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else if (Slot2Quantity == 2)
                        {
                            Slot2Quantity += 3;
                            Add(item, type, quantity - 3, 0, true, false);
                        }
                        else if (Slot2Quantity == 1)
                        {
                            Slot2Quantity += 4;
                            Add(item, type, quantity - 4, 0, true, false);
                        }
                    }
                    else if (quantity == 4)
                    {
                        if (Slot2Quantity == 4)
                        {
                            Slot2Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot2Quantity == 3)
                        {
                            Slot2Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else if (Slot2Quantity == 2)
                        {
                            Slot2Quantity += 3;
                            Add(item, type, quantity - 3, 0, true, false);
                        }
                        else
                        {
                            Slot2Quantity += quantity;
                        }
                    }
                    else if (quantity == 3)
                    {
                        if (Slot2Quantity == 4)
                        {
                            Slot2Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot2Quantity == 3)
                        {
                            Slot2Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else
                        {
                            Slot2Quantity += quantity;
                        }
                    }
                    else if (quantity == 2)
                    {
                        if (Slot2Quantity == 4)
                        {
                            Slot2Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else
                        {
                            Slot2Quantity += quantity;
                        }
                    }
                    else
                    {
                        Slot2Quantity += quantity;
                    }
                    RoleplayUser.SendWeb("inventory-quantity-update;slot2;" + Slot2Quantity);
                }
                else if (Slot3 == item && Slot3Quantity < 5)
                {
                    if (quantity == 5)
                    {
                        if (Slot3Quantity == 4)
                        {
                            Slot3Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot3Quantity == 3)
                        {
                            Slot3Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else if (Slot3Quantity == 2)
                        {
                            Slot3Quantity += 3;
                            Add(item, type, quantity - 3, 0, true, false);
                        }
                        else if (Slot3Quantity == 1)
                        {
                            Slot3Quantity += 4;
                            Add(item, type, quantity - 4, 0, true, false);
                        }
                    }
                    else if (quantity == 4)
                    {
                        if (Slot3Quantity == 4)
                        {
                            Slot3Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot3Quantity == 3)
                        {
                            Slot3Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else if (Slot3Quantity == 2)
                        {
                            Slot3Quantity += 3;
                            Add(item, type, quantity - 3, 0, true, false);
                        }
                        else
                        {
                            Slot3Quantity += quantity;
                        }
                    }
                    else if (quantity == 3)
                    {
                        if (Slot3Quantity == 4)
                        {
                            Slot3Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot3Quantity == 3)
                        {
                            Slot3Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else
                        {
                            Slot3Quantity += quantity;
                        }
                    }
                    else if (quantity == 2)
                    {
                        if (Slot3Quantity == 4)
                        {
                            Slot3Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else
                        {
                            Slot3Quantity += quantity;
                        }
                    }
                    else
                    {
                        Slot3Quantity += quantity;
                    }
                    RoleplayUser.SendWeb("inventory-quantity-update;slot3;" + Slot3Quantity);
                }
                else if (Slot4 == item && Slot4Quantity < 5)
                {
                    if (quantity == 5)
                    {
                        if (Slot4Quantity == 4)
                        {
                            Slot4Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot4Quantity == 3)
                        {
                            Slot4Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else if (Slot4Quantity == 2)
                        {
                            Slot4Quantity += 3;
                            Add(item, type, quantity - 3, 0, true, false);
                        }
                        else if (Slot4Quantity == 1)
                        {
                            Slot4Quantity += 4;
                            Add(item, type, quantity - 4, 0, true, false);
                        }
                    }
                    else if (quantity == 4)
                    {
                        if (Slot4Quantity == 4)
                        {
                            Slot4Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot4Quantity == 3)
                        {
                            Slot4Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else if (Slot4Quantity == 2)
                        {
                            Slot4Quantity += 3;
                            Add(item, type, quantity - 3, 0, true, false);
                        }
                        else
                        {
                            Slot4Quantity += quantity;
                        }
                    }
                    else if (quantity == 3)
                    {
                        if (Slot4Quantity == 4)
                        {
                            Slot4Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot4Quantity == 3)
                        {
                            Slot4Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else
                        {
                            Slot4Quantity += quantity;
                        }
                    }
                    else if (quantity == 2)
                    {
                        if (Slot4Quantity == 4)
                        {
                            Slot4Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else
                        {
                            Slot4Quantity += quantity;
                        }
                    }
                    else
                    {
                        Slot4Quantity += quantity;
                    }
                    RoleplayUser.SendWeb("inventory-quantity-update;slot4;" + Slot4Quantity);
                }
                else if (Slot5 == item && Slot5Quantity < 5)
                {
                    if (quantity == 5)
                    {
                        if (Slot5Quantity == 4)
                        {
                            Slot5Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot5Quantity == 3)
                        {
                            Slot5Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else if (Slot5Quantity == 2)
                        {
                            Slot5Quantity += 3;
                            Add(item, type, quantity - 3, 0, true, false);
                        }
                        else if (Slot5Quantity == 1)
                        {
                            Slot5Quantity += 4;
                            Add(item, type, quantity - 4, 0, true, false);
                        }
                    }
                    else if (quantity == 4)
                    {
                        if (Slot5Quantity == 4)
                        {
                            Slot5Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot5Quantity == 3)
                        {
                            Slot5Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else if (Slot5Quantity == 2)
                        {
                            Slot5Quantity += 3;
                            Add(item, type, quantity - 3, 0, true, false);
                        }
                        else
                        {
                            Slot5Quantity += quantity;
                        }
                    }
                    else if (quantity == 3)
                    {
                        if (Slot5Quantity == 4)
                        {
                            Slot5Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot5Quantity == 3)
                        {
                            Slot5Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else
                        {
                            Slot5Quantity += quantity;
                        }
                    }
                    else if (quantity == 2)
                    {
                        if (Slot5Quantity == 4)
                        {
                            Slot5Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else
                        {
                            Slot5Quantity += quantity;
                        }
                    }
                    else
                    {
                        Slot5Quantity += quantity;
                    }
                    RoleplayUser.SendWeb("inventory-quantity-update;slot5;" + Slot5Quantity);
                }
                else if (Slot6 == item && Slot6Quantity < 5)
                {
                    if (quantity == 5)
                    {
                        if (Slot6Quantity == 4)
                        {
                            Slot6Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot6Quantity == 3)
                        {
                            Slot6Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else if (Slot6Quantity == 2)
                        {
                            Slot6Quantity += 3;
                            Add(item, type, quantity - 3, 0, true, false);
                        }
                        else if (Slot6Quantity == 1)
                        {
                            Slot6Quantity += 4;
                            Add(item, type, quantity - 4, 0, true, false);
                        }
                    }
                    else if (quantity == 4)
                    {
                        if (Slot6Quantity == 4)
                        {
                            Slot6Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot6Quantity == 3)
                        {
                            Slot6Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else if (Slot6Quantity == 2)
                        {
                            Slot6Quantity += 3;
                            Add(item, type, quantity - 3, 0, true, false);
                        }
                        else
                        {
                            Slot6Quantity += quantity;
                        }
                    }
                    else if (quantity == 3)
                    {
                        if (Slot6Quantity == 4)
                        {
                            Slot6Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot6Quantity == 3)
                        {
                            Slot6Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else
                        {
                            Slot6Quantity += quantity;
                        }
                    }
                    else if (quantity == 2)
                    {
                        if (Slot6Quantity == 4)
                        {
                            Slot6Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else
                        {
                            Slot6Quantity += quantity;
                        }
                    }
                    else
                    {
                        Slot6Quantity += quantity;
                    }
                    RoleplayUser.SendWeb("inventory-quantity-update;slot6;" + Slot6Quantity);
                }
                else if (Slot7 == item && Slot7Quantity < 5)
                {
                    if (quantity == 5)
                    {
                        if (Slot7Quantity == 4)
                        {
                            Slot7Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot7Quantity == 3)
                        {
                            Slot7Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else if (Slot7Quantity == 2)
                        {
                            Slot7Quantity += 3;
                            Add(item, type, quantity - 3, 0, true, false);
                        }
                        else if (Slot7Quantity == 1)
                        {
                            Slot7Quantity += 4;
                            Add(item, type, quantity - 4, 0, true, false);
                        }
                    }
                    else if (quantity == 4)
                    {
                        if (Slot7Quantity == 4)
                        {
                            Slot7Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot7Quantity == 3)
                        {
                            Slot7Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else if (Slot7Quantity == 2)
                        {
                            Slot7Quantity += 3;
                            Add(item, type, quantity - 3, 0, true, false);
                        }
                        else
                        {
                            Slot7Quantity += quantity;
                        }
                    }
                    else if (quantity == 3)
                    {
                        if (Slot7Quantity == 4)
                        {
                            Slot7Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot7Quantity == 3)
                        {
                            Slot7Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else
                        {
                            Slot7Quantity += quantity;
                        }
                    }
                    else if (quantity == 2)
                    {
                        if (Slot7Quantity == 4)
                        {
                            Slot7Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else
                        {
                            Slot7Quantity += quantity;
                        }
                    }
                    else
                    {
                        Slot7Quantity += quantity;
                    }
                    RoleplayUser.SendWeb("inventory-quantity-update;slot7;" + Slot7Quantity);
                }
                else if (Slot8 == item && Slot8Quantity < 5)
                {
                    if (quantity == 5)
                    {
                        if (Slot8Quantity == 4)
                        {
                            Slot8Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot8Quantity == 3)
                        {
                            Slot8Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else if (Slot8Quantity == 2)
                        {
                            Slot8Quantity += 3;
                            Add(item, type, quantity - 3, 0, true, false);
                        }
                        else if (Slot8Quantity == 1)
                        {
                            Slot8Quantity += 4;
                            Add(item, type, quantity - 4, 0, true, false);
                        }
                    }
                    else if (quantity == 4)
                    {
                        if (Slot8Quantity == 4)
                        {
                            Slot8Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot8Quantity == 3)
                        {
                            Slot8Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else if (Slot8Quantity == 2)
                        {
                            Slot8Quantity += 3;
                            Add(item, type, quantity - 3, 0, true, false);
                        }
                        else
                        {
                            Slot8Quantity += quantity;
                        }
                    }
                    else if (quantity == 3)
                    {
                        if (Slot8Quantity == 4)
                        {
                            Slot8Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot8Quantity == 3)
                        {
                            Slot8Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else
                        {
                            Slot8Quantity += quantity;
                        }
                    }
                    else if (quantity == 2)
                    {
                        if (Slot8Quantity == 4)
                        {
                            Slot8Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else
                        {
                            Slot8Quantity += quantity;
                        }
                    }
                    else
                    {
                        Slot8Quantity += quantity;
                    }
                    RoleplayUser.SendWeb("inventory-quantity-update;slot8;" + Slot8Quantity);
                }
                else if (Slot9 == item && Slot9Quantity < 5)
                {
                    if (quantity == 5)
                    {
                        if (Slot9Quantity == 4)
                        {
                            Slot9Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot9Quantity == 3)
                        {
                            Slot9Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else if (Slot9Quantity == 2)
                        {
                            Slot9Quantity += 3;
                            Add(item, type, quantity - 3, 0, true, false);
                        }
                        else if (Slot9Quantity == 1)
                        {
                            Slot9Quantity += 4;
                            Add(item, type, quantity - 4, 0, true, false);
                        }
                    }
                    else if (quantity == 4)
                    {
                        if (Slot9Quantity == 4)
                        {
                            Slot9Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot9Quantity == 3)
                        {
                            Slot9Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else if (Slot9Quantity == 2)
                        {
                            Slot9Quantity += 3;
                            Add(item, type, quantity - 3, 0, true, false);
                        }
                        else
                        {
                            Slot9Quantity += quantity;
                        }
                    }
                    else if (quantity == 3)
                    {
                        if (Slot9Quantity == 4)
                        {
                            Slot9Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot9Quantity == 3)
                        {
                            Slot9Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else
                        {
                            Slot9Quantity += quantity;
                        }
                    }
                    else if (quantity == 2)
                    {
                        if (Slot9Quantity == 4)
                        {
                            Slot9Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else
                        {
                            Slot9Quantity += quantity;
                        }
                    }
                    else
                    {
                        Slot9Quantity += quantity;
                    }
                    RoleplayUser.SendWeb("inventory-quantity-update;slot9;" + Slot9Quantity);
                }
                else if (Slot10 == item && Slot10Quantity < 5)
                {
                    if (quantity == 5)
                    {
                        if (Slot10Quantity == 4)
                        {
                            Slot10Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot10Quantity == 3)
                        {
                            Slot10Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else if (Slot10Quantity == 2)
                        {
                            Slot10Quantity += 3;
                            Add(item, type, quantity - 3, 0, true, false);
                        }
                        else if (Slot10Quantity == 1)
                        {
                            Slot10Quantity += 4;
                            Add(item, type, quantity - 4, 0, true, false);
                        }
                    }
                    else if (quantity == 4)
                    {
                        if (Slot10Quantity == 4)
                        {
                            Slot10Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot10Quantity == 3)
                        {
                            Slot10Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else if (Slot10Quantity == 2)
                        {
                            Slot10Quantity += 3;
                            Add(item, type, quantity - 3, 0, true, false);
                        }
                        else
                        {
                            Slot10Quantity += quantity;
                        }
                    }
                    else if (quantity == 3)
                    {
                        if (Slot10Quantity == 4)
                        {
                            Slot10Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else if (Slot10Quantity == 3)
                        {
                            Slot10Quantity += 2;
                            Add(item, type, quantity - 2, 0, true, false);
                        }
                        else
                        {
                            Slot10Quantity += quantity;
                        }
                    }
                    else if (quantity == 2)
                    {
                        if (Slot10Quantity == 4)
                        {
                            Slot10Quantity += 1;
                            Add(item, type, quantity - 1, 0, true, false);
                        }
                        else
                        {
                            Slot10Quantity += quantity;
                        }
                    }
                    else
                    {
                        Slot10Quantity += quantity;
                    }
                    RoleplayUser.SendWeb("inventory-quantity-update;slot10;" + Slot10Quantity);
                }
                else
                {
                    if (Slot1 == null)
                    {
                        Slot1 = item;
                        Slot1Type = type;
                        Slot1Quantity = quantity;
                        RoleplayUser.SendWeb("inventory-add;slot1;" + Slot1 + ";" + Slot1Quantity + ";" + Slot1Durability);
                    }
                    else if (Slot2 == null)
                    {
                        Slot2 = item;
                        Slot2Type = type;
                        Slot2Quantity = quantity;
                        RoleplayUser.SendWeb("inventory-add;slot2;" + Slot2 + ";" + Slot2Quantity + ";" + Slot2Durability);
                    }
                    else if (Slot3 == null)
                    {
                        Slot3 = item;
                        Slot3Type = type;
                        Slot3Quantity = quantity;
                        RoleplayUser.SendWeb("inventory-add;slot3;" + Slot3 + ";" + Slot3Quantity + ";" + Slot3Durability);
                    }
                    else if (Slot4 == null)
                    {
                        Slot4 = item;
                        Slot4Type = type;
                        Slot4Quantity = quantity;
                        RoleplayUser.SendWeb("inventory-add;slot4;" + Slot4 + ";" + Slot4Quantity + ";" + Slot4Durability);
                    }
                    else if (Slot5 == null)
                    {
                        Slot5 = item;
                        Slot5Type = type;
                        Slot5Quantity = quantity;
                        RoleplayUser.SendWeb("inventory-add;slot5;" + Slot5 + ";" + Slot5Quantity + ";" + Slot5Durability);
                    }
                    else if (Slot6 == null)
                    {
                        Slot6 = item;
                        Slot6Type = type;
                        Slot6Quantity = quantity;
                        RoleplayUser.SendWeb("inventory-add;slot6;" + Slot6 + ";" + Slot6Quantity + ";" + Slot6Durability);
                    }
                    else if (Slot7 == null)
                    {
                        Slot7 = item;
                        Slot7Type = type;
                        Slot7Quantity = quantity;
                        RoleplayUser.SendWeb("inventory-add;slot7;" + Slot7 + ";" + Slot7Quantity + ";" + Slot7Durability);
                    }
                    else if (Slot8 == null)
                    {
                        Slot8 = item;
                        Slot8Type = type;
                        Slot8Quantity = quantity;
                        RoleplayUser.SendWeb("inventory-add;slot8;" + Slot8 + ";" + Slot8Quantity + ";" + Slot8Durability);
                    }
                    else if (Slot9 == null)
                    {
                        Slot9 = item;
                        Slot9Type = type;
                        Slot9Quantity = quantity;
                        RoleplayUser.SendWeb("inventory-add;slot9;" + Slot9 + ";" + Slot9Quantity + ";" + Slot9Durability);
                    }
                    else if (Slot10 == null)
                    {
                        Slot10 = item;
                        Slot10Type = type;
                        Slot10Quantity = quantity;
                        RoleplayUser.SendWeb("inventory-add;slot10;" + Slot10 + ";" + Slot10Quantity + ";" + Slot10Durability);
                    }
                }
            }
            else if (isDurability)
            {
                if (Slot1 == null)
                {
                    Slot1 = item;
                    Slot1Type = type;
                    Slot1Durability = durability;
                    RoleplayUser.SendWeb("inventory-add;slot1;" + Slot1 + ";0;" + Slot1Durability);
                }
                else if (Slot2 == null)
                {
                    Slot2 = item;
                    Slot2Type = type;
                    Slot2Durability = durability;
                    RoleplayUser.SendWeb("inventory-add;slot2;" + Slot2 + ";0;" + Slot2Durability);
                }
                else if (Slot3 == null)
                {
                    Slot3 = item;
                    Slot3Type = type;
                    Slot3Durability = durability;
                    RoleplayUser.SendWeb("inventory-add;slot3;" + Slot3 + ";0;" + Slot3Durability);
                }
                else if (Slot4 == null)
                {
                    Slot4 = item;
                    Slot4Type = type;
                    Slot4Durability = durability;
                    RoleplayUser.SendWeb("inventory-add;slot4;" + Slot4 + ";0;" + Slot4Durability);
                }
                else if (Slot5 == null)
                {
                    Slot5 = item;
                    Slot5Type = type;
                    Slot5Durability = durability;
                    RoleplayUser.SendWeb("inventory-add;slot5;" + Slot5 + ";0;" + Slot5Durability);
                }
                else if (Slot6 == null)
                {
                    Slot6 = item;
                    Slot6Type = type;
                    Slot6Durability = durability;
                    RoleplayUser.SendWeb("inventory-add;slot6;" + Slot6 + ";0;" + Slot6Durability);
                }
                else if (Slot7 == null)
                {
                    Slot7 = item;
                    Slot7Type = type;
                    Slot7Durability = durability;
                    RoleplayUser.SendWeb("inventory-add;slot7;" + Slot7 + ";0;" + Slot7Durability);
                }
                else if (Slot8 == null)
                {
                    Slot8 = item;
                    Slot8Type = type;
                    Slot8Durability = durability;
                    RoleplayUser.SendWeb("inventory-add;slot8;" + Slot8 + ";0;" + Slot8Durability);
                }
                else if (Slot9 == null)
                {
                    Slot9 = item;
                    Slot9Type = type;
                    Slot9Durability = durability;
                    RoleplayUser.SendWeb("inventory-add;slot9;" + Slot9 + ";0;" + Slot9Durability);
                }
                else if (Slot10 == null)
                {
                    Slot10 = item;
                    Slot10Type = type;
                    Slot10Durability = durability;
                    RoleplayUser.SendWeb("inventory-add;slot10;" + Slot10 + ";0;" + Slot10Durability);
                }
            }
        }
        public void Remove(string slot)
        {
            if (slot == "slot1")
            {
                Slot1 = null;
                Slot1Type = null;
                Slot1Quantity = 0;
                Slot1Durability = 0;
            }
            else if (slot == "slot2")
            {
                Slot2 = null;
                Slot2Type = null;
                Slot2Quantity = 0;
                Slot2Durability = 0;
            }
            else if (slot == "slot3")
            {
                Slot3 = null;
                Slot3Type = null;
                Slot3Quantity = 0;
                Slot3Durability = 0;
            }
            else if (slot == "slot4")
            {
                Slot4 = null;
                Slot4Type = null;
                Slot4Quantity = 0;
                Slot4Durability = 0;
            }
            else if (slot == "slot5")
            {
                Slot5 = null;
                Slot5Type = null;
                Slot5Quantity = 0;
                Slot6Durability = 0;
            }
            else if (slot == "slot6")
            {
                Slot6 = null;
                Slot6Type = null;
                Slot6Quantity = 0;
                Slot6Durability = 0;
            }
            else if (slot == "slot7")
            {
                Slot7 = null;
                Slot7Type = null;
                Slot7Quantity = 0;
                Slot7Durability = 0;
            }
            else if (slot == "slot8")
            {
                Slot8 = null;
                Slot8Type = null;
                Slot8Quantity = 0;
                Slot8Durability = 0;
            }
            else if (slot == "slot9")
            {
                Slot9 = null;
                Slot9Type = null;
                Slot9Quantity = 0;
                Slot9Durability = 0;
            }
            else if (slot == "slot10")
            {
                Slot10 = null;
                Slot10Type = null;
                Slot10Quantity = 0;
                Slot10Durability = 0;
            }
            RoleplayUser.SendWeb("inventory-remove;" + slot);
        }
        public void Equip(string slot, string weapon, string type, int durability)
        {
            if (RoleplayUser.Habbo.getCooldown("equip"))
            {
                RoleplayUser.GetClient().SendWhisper("Please wait before equipping your weapon again");
                return;
            }
            RoleplayUser.Habbo.addCooldown("equip", 3000);

            if (WeaponEquipped != null)
            {
                ReplaceEquip(slot, weapon, type, durability);
                return;
            }
            Remove(slot);

            if (EquipSlot1 == null)
            {
                EquipSlot1 = weapon;
                EquipSlot1Type = type;
                EquipSlot1Durability = durability;
                WeaponEquippedSlot = 1;
                RoleplayUser.SendWeb("inventory-equip;add;1;" + weapon + ";" + durability);
            }
            else if (EquipSlot2 == null)
            {
                EquipSlot2 = weapon;
                EquipSlot2Type = type;
                EquipSlot2Durability = durability;
                WeaponEquippedSlot = 2;
                RoleplayUser.SendWeb("inventory-equip;add;2;" + weapon + ";" + durability);
            }
            WeaponEquipped = weapon;
            RoleplayUser.ResetEffect();
        }
        public void Unequip(int slot)
        {
            if (slot == 1)
            {
                Add(EquipSlot1, EquipSlot1Type, 0, EquipSlot1Durability, false, true);
                EquipSlot1 = null;
                EquipSlot1Type = null;
                EquipSlot1Durability = 0;
            }
            else if (slot == 2)
            {
                Add(EquipSlot2, EquipSlot2Type, 0, EquipSlot2Durability, false, true);
                EquipSlot2 = null;
                EquipSlot2Type = null;
                EquipSlot2Durability = 0;
            }
            WeaponEquippedSlot = 0;
            WeaponEquipped = null;
            RoleplayUser.ResetEffect();
            RoleplayUser.SendWeb("inventory-equip;remove;" + slot);
        }
        public void ReplaceEquip(string slot, string weapon, string type, int durability)
        {
            if (EquipSlot1 == WeaponEquipped)
            {
                #region From Slot
                if (slot == "slot1")
                {
                    Slot1 = EquipSlot1;
                    Slot1Type = EquipSlot1Type;
                    Slot1Durability = EquipSlot1Durability;
                }
                else if (slot == "slot2")
                {
                    Slot2 = EquipSlot1;
                    Slot2Type = EquipSlot1Type;
                    Slot2Durability = EquipSlot1Durability;
                }
                else if (slot == "slot3")
                {
                    Slot3 = EquipSlot1;
                    Slot3Type = EquipSlot1Type;
                    Slot3Durability = EquipSlot1Durability;
                }
                else if (slot == "slot4")
                {
                    Slot4 = EquipSlot1;
                    Slot4Type = EquipSlot1Type;
                    Slot4Durability = EquipSlot1Durability;
                }
                else if (slot == "slot5")
                {
                    Slot5 = EquipSlot1;
                    Slot5Type = EquipSlot1Type;
                    Slot6Durability = EquipSlot1Durability;
                }
                else if (slot == "slot6")
                {
                    Slot6 = EquipSlot1;
                    Slot6Type = EquipSlot1Type;
                    Slot6Durability = EquipSlot1Durability;
                }
                else if (slot == "slot7")
                {
                    Slot7 = EquipSlot1;
                    Slot7Type = EquipSlot1Type;
                    Slot7Durability = EquipSlot1Durability;
                }
                else if (slot == "slot8")
                {
                    Slot8 = EquipSlot1;
                    Slot8Type = EquipSlot1Type;
                    Slot8Durability = EquipSlot1Durability;
                }
                else if (slot == "slot9")
                {
                    Slot9 = EquipSlot1;
                    Slot9Type = EquipSlot1Type;
                    Slot9Durability = EquipSlot1Durability;
                }
                else if (slot == "slot10")
                {
                    Slot10 = EquipSlot1;
                    Slot10Type = EquipSlot1Type;
                    Slot10Durability = EquipSlot1Durability;
                }
                RoleplayUser.SendWeb("inventory-add;" + slot + ";" + EquipSlot1 + ";0;" + EquipSlot1Durability);
                #endregion

                EquipSlot1 = weapon;
                EquipSlot1Type = type;
                EquipSlot1Durability = durability;
                WeaponEquippedSlot = 1;
                RoleplayUser.SendWeb("inventory-equip;add;1;" + EquipSlot1 + ";" + EquipSlot1Durability);
            }
            else if (EquipSlot2 == WeaponEquipped)
            {
                #region From Slot
                if (slot == "slot1")
                {
                    Slot1 = EquipSlot1;
                    Slot1Type = EquipSlot1Type;
                    Slot1Durability = EquipSlot1Durability;
                }
                else if (slot == "slot2")
                {
                    Slot2 = EquipSlot1;
                    Slot2Type = EquipSlot1Type;
                    Slot2Durability = EquipSlot1Durability;
                }
                else if (slot == "slot3")
                {
                    Slot3 = EquipSlot1;
                    Slot3Type = EquipSlot1Type;
                    Slot3Durability = EquipSlot1Durability;
                }
                else if (slot == "slot4")
                {
                    Slot4 = EquipSlot1;
                    Slot4Type = EquipSlot1Type;
                    Slot4Durability = EquipSlot1Durability;
                }
                else if (slot == "slot5")
                {
                    Slot5 = EquipSlot1;
                    Slot5Type = EquipSlot1Type;
                    Slot6Durability = EquipSlot1Durability;
                }
                else if (slot == "slot6")
                {
                    Slot6 = EquipSlot1;
                    Slot6Type = EquipSlot1Type;
                    Slot6Durability = EquipSlot1Durability;
                }
                else if (slot == "slot7")
                {
                    Slot7 = EquipSlot1;
                    Slot7Type = EquipSlot1Type;
                    Slot7Durability = EquipSlot1Durability;
                }
                else if (slot == "slot8")
                {
                    Slot8 = EquipSlot2;
                    Slot8Type = EquipSlot2Type;
                    Slot8Durability = EquipSlot2Durability;
                }
                else if (slot == "slot9")
                {
                    Slot9 = EquipSlot1;
                    Slot9Type = EquipSlot1Type;
                    Slot9Durability = EquipSlot1Durability;
                }
                else if (slot == "slot10")
                {
                    Slot10 = EquipSlot1;
                    Slot10Type = EquipSlot1Type;
                    Slot10Durability = EquipSlot1Durability;
                }
                RoleplayUser.SendWeb("inventory-add;" + slot + ";" + EquipSlot2 + ";" + EquipSlot2Type + ";" + EquipSlot2Durability);
                #endregion

                EquipSlot2 = weapon;
                EquipSlot2Type = type;
                EquipSlot2Durability = durability;
                WeaponEquippedSlot = 1;
                RoleplayUser.SendWeb("inventory-equip;add;1;" + EquipSlot2 + ";" + EquipSlot2Durability);
            }
            WeaponEquipped = weapon;
            RoleplayUser.ResetEffect();
        }
        public void UpdateDurability(string item, int durability)
        {
            if (EquipSlot1 == item)
            {
                if (EquipSlot1Durability > 0)
                {
                    EquipSlot1Durability -= durability;
                    if (EquipSlot1 == "stungun" && EquipSlot1Durability < 14)
                    {
                        EquipSlot1Durability = 1;
                    }
                    RoleplayUser.ResetEffect();
                }
                else
                {
                    EquipSlot1Durability = 0;
                }
                RoleplayUser.SendWeb("inventory-durability-update;slot1;" + EquipSlot1Durability);
            }
            else if (EquipSlot2 == item)
            {
                if (EquipSlot2Durability > 0)
                {
                    EquipSlot2Durability -= durability;
                }
                else
                {
                    EquipSlot2Durability = 0;
                }
                RoleplayUser.SendWeb("inventory-durability-update;slot2;" + EquipSlot2Durability);
            }
        }

        public void UpdateQuantity(string slot, int quantity)
        {
            if (slot == "slot1")
            {
                if (Slot1 != null)
                {
                    Slot1Quantity -= quantity;
                    if (Slot1Quantity == 0)
                        Remove("slot1");
                    else
                        RoleplayUser.SendWeb("inventory-quantity-update;slot1;" + Slot1Quantity);
                }
            }
            else if (slot == "slot2")
            {
                if (Slot2 != null)
                {
                    Slot2Quantity -= quantity;
                    if (Slot2Quantity == 0)
                        Remove("slot2");
                    else
                        RoleplayUser.SendWeb("inventory-quantity-update;slot2;" + Slot2Quantity);
                }
            }
            else if (slot == "slot3")
            {
                if (Slot3 != null)
                {
                    Slot3Quantity -= quantity;
                    if (Slot3Quantity == 0)
                        Remove("slot3");
                    else
                        RoleplayUser.SendWeb("inventory-quantity-update;slot3;" + Slot3Quantity);
                }
            }
            else if (slot == "slot4")
            {
                if (Slot4 != null)
                {
                    Slot4Quantity -= quantity;
                    if (Slot4Quantity == 0)
                        Remove("slot4");
                    else
                        RoleplayUser.SendWeb("inventory-quantity-update;slot4;" + Slot4Quantity);
                }
            }
            else if (slot == "slot5")
            {
                if (Slot5 != null)
                {
                    Slot5Quantity -= quantity;
                    if (Slot5Quantity == 0)
                        Remove("slot5");
                    else
                        RoleplayUser.SendWeb("inventory-quantity-update;slot5;" + Slot5Quantity);
                }
            }
            else if (slot == "slot6")
            {
                if (Slot6 != null)
                {
                    Slot6Quantity -= quantity;
                    if (Slot6Quantity == 0)
                        Remove("slot6");
                    else
                        RoleplayUser.SendWeb("inventory-quantity-update;slot6;" + Slot6Quantity);
                }
            }
            else if (slot == "slot7")
            {
                if (Slot7 != null)
                {
                    Slot7Quantity -= quantity;
                    if (Slot7Quantity == 0)
                        Remove("slot7");
                    else
                        RoleplayUser.SendWeb("inventory-quantity-update;slot7;" + Slot7Quantity);
                }
            }
            else if (slot == "slot8")
            {
                if (Slot8 != null)
                {
                    Slot8Quantity -= quantity;
                    if (Slot8Quantity == 0)
                        Remove("slot8");
                    else
                        RoleplayUser.SendWeb("inventory-quantity-update;slot8;" + Slot8Quantity);
                }
            }
            else if (slot == "slot9")
            {
                if (Slot9 != null)
                {
                    Slot9Quantity -= quantity;
                    if (Slot9Quantity == 0)
                        Remove("slot9");
                    else
                        RoleplayUser.SendWeb("inventory-quantity-update;slot9;" + Slot9Quantity);
                }
            }
            else if (slot == "slot10")
            {
                if (Slot10 != null)
                {
                    Slot10Quantity -= quantity;
                    if (Slot10Quantity == 0)
                        Remove("slot10");
                    else
                        RoleplayUser.SendWeb("inventory-quantity-update;slot10;" + Slot10Quantity);
                }
            }
        }

        public bool HasItem(string item, bool fromSlotAdd)
        {
            if (Slot1 == item)
            {
                if (fromSlotAdd)
                    FromSlot = "slot1";

                return true;
            }
            else if (Slot2 == item)
            {
                if (fromSlotAdd)
                    FromSlot = "slot2";

                return true;
            }
            else if (Slot3 == item)
            {
                if (fromSlotAdd)
                    FromSlot = "slot3";

                return true;
            }
            else if (Slot4 == item)
            {
                if (fromSlotAdd)
                    FromSlot = "slot4";

                return true;
            }
            else if (Slot5 == item)
            {
                if (fromSlotAdd)
                    FromSlot = "slot5";

                return true;
            }
            else if (Slot6 == item)
            {
                if (fromSlotAdd)
                    FromSlot = "slot6";

                return true;
            }
            else if (Slot7 == item)
            {
                FromSlot = "slot7";
                return true;
            }
            else if (Slot8 == item)
            {
                if (fromSlotAdd)
                    FromSlot = "slot8";

                return true;
            }
            else if (Slot9 == item)
            {
                if (fromSlotAdd)
                    FromSlot = "slot9";

                return true;
            }
            else if (Slot10 == item)
            {
                if (fromSlotAdd)
                    FromSlot = "slot10";

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool EquipHasItem(string item)
        {
            if (EquipSlot1 == item)
                return true;
            else if (EquipSlot2 == item)
                return true;
            else
                return false;
        }

        public bool EquipHasDurability(string item, int durability)
        {
            if (EquipSlot1 == item)
            {
                if (EquipSlot1Durability >= durability)
                    return true;
                else
                    return false;
            }
            else if (EquipSlot2 == item)
            {
                if (EquipSlot2Durability >= durability)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
    }
}
