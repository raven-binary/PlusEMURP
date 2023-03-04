using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plus.HabboRoleplay.RoleplayUsers;

namespace Plus.HabboRoleplay.Bin
{
    public class Bins
    {
        /// <summary>
        /// The users roleplay instance
        /// </summary>
        RoleplayUser RoleplayUser;

        #region  Values
        public bool Using = false;
        //Bin Coords
        public int X = 0;
        public int Y = 0;

        public string Slot1 = null;
        public string Slot2 = null;
        public string Slot3 = null;
        public string Slot4 = null;
        public string Slot5 = null;
        public string Slot6 = null;
        public string Slot7 = null;
        public string Slot8 = null;
        public string Slot9 = null;
        public string Slot10 = null;
        public string Slot1Type = null;
        public string Slot2Type = null;
        public string Slot3Type = null;
        public string Slot4Type = null;
        public string Slot5Type = null;
        public string Slot6Type = null;
        public string Slot7Type = null;
        public string Slot8Type = null;
        public string Slot9Type = null;
        public string Slot10Type = null;
        public int Slot1Quantity = 0;
        public int Slot2Quantity = 0;
        public int Slot3Quantity = 0;
        public int Slot4Quantity = 0;
        public int Slot5Quantity = 0;
        public int Slot6Quantity = 0;
        public int Slot7Quantity = 0;
        public int Slot8Quantity = 0;
        public int Slot9Quantity = 0;
        public int Slot10Quantity = 0;
        public int Slot1Durability = 0;
        public int Slot2Durability = 0;
        public int Slot3Durability = 0;
        public int Slot4Durability = 0;
        public int Slot5Durability = 0;
        public int Slot6Durability = 0;
        public int Slot7Durability = 0;
        public int Slot8Durability = 0;
        public int Slot9Durability = 0;
        public int Slot10Durability = 0;
        #endregion

        public Bins(RoleplayUser RoleplayUser)
        {
            this.RoleplayUser = RoleplayUser;
        }

        public void Add(string fromSlot)
        {
            string item = null;
            string type = null;
            int quantity = 0;
            int durability = 0;

            #region From Slot
            if (fromSlot == "slot1")
            {
                if (RoleplayUser.Inventory.Slot1 != null)
                {
                    item = RoleplayUser.Inventory.Slot1;
                    type = RoleplayUser.Inventory.Slot1Type;
                    quantity = RoleplayUser.Inventory.Slot1Quantity;
                    durability = RoleplayUser.Inventory.Slot1Durability;
                }
            }
            else if (fromSlot == "slot2")
            {
                if (RoleplayUser.Inventory.Slot2 != null)
                {
                    item = RoleplayUser.Inventory.Slot2;
                    type = RoleplayUser.Inventory.Slot2Type;
                    quantity = RoleplayUser.Inventory.Slot2Quantity;
                    durability = RoleplayUser.Inventory.Slot2Durability;
                }
            }
            else if (fromSlot == "slot3")
            {
                if (RoleplayUser.Inventory.Slot3 != null)
                {
                    item = RoleplayUser.Inventory.Slot3;
                    type = RoleplayUser.Inventory.Slot3Type;
                    quantity = RoleplayUser.Inventory.Slot3Quantity;
                    durability = RoleplayUser.Inventory.Slot3Durability;
                }
            }
            else if (fromSlot == "slot4")
            {
                if (RoleplayUser.Inventory.Slot4 != null)
                {
                    item = RoleplayUser.Inventory.Slot4;
                    type = RoleplayUser.Inventory.Slot4Type;
                    quantity = RoleplayUser.Inventory.Slot4Quantity;
                    durability = RoleplayUser.Inventory.Slot4Durability;
                }
            }
            else if (fromSlot == "slot5")
            {
                if (RoleplayUser.Inventory.Slot5 != null)
                {
                    item = RoleplayUser.Inventory.Slot5;
                    type = RoleplayUser.Inventory.Slot5Type;
                    quantity = RoleplayUser.Inventory.Slot5Quantity;
                    durability = RoleplayUser.Inventory.Slot5Durability;
                }
            }
            else if (fromSlot == "slot6")
            {
                if (RoleplayUser.Inventory.Slot6 != null)
                {
                    item = RoleplayUser.Inventory.Slot6;
                    type = RoleplayUser.Inventory.Slot6Type;
                    quantity = RoleplayUser.Inventory.Slot6Quantity;
                    durability = RoleplayUser.Inventory.Slot6Durability;
                }
            }
            else if (fromSlot == "slot7")
            {
                if (RoleplayUser.Inventory.Slot7 != null)
                {
                    item = RoleplayUser.Inventory.Slot7;
                    type = RoleplayUser.Inventory.Slot7Type;
                    quantity = RoleplayUser.Inventory.Slot7Quantity;
                    durability = RoleplayUser.Inventory.Slot7Durability;
                }
            }
            else if (fromSlot == "slot8")
            {
                if (RoleplayUser.Inventory.Slot8 != null)
                {
                    item = RoleplayUser.Inventory.Slot8;
                    type = RoleplayUser.Inventory.Slot8Type;
                    quantity = RoleplayUser.Inventory.Slot8Quantity;
                    durability = RoleplayUser.Inventory.Slot8Durability;
                }
            }
            else if (fromSlot == "slot9")
            {
                if (RoleplayUser.Inventory.Slot9 != null)
                {
                    item = RoleplayUser.Inventory.Slot9;
                    type = RoleplayUser.Inventory.Slot9Type;
                    quantity = RoleplayUser.Inventory.Slot9Quantity;
                    durability = RoleplayUser.Inventory.Slot9Durability;
                }
            }
            else if (fromSlot == "slot10")
            {
                if (RoleplayUser.Inventory.Slot10 != null)
                {
                    item = RoleplayUser.Inventory.Slot10;
                    type = RoleplayUser.Inventory.Slot10Type;
                    quantity = RoleplayUser.Inventory.Slot10Quantity;
                    durability = RoleplayUser.Inventory.Slot10Durability;
                }
            }
            else
                return;
            #endregion

            if (quantity > 0)
            {
                if (Slot1 == item && Slot1Type == type)
                {
                    Slot1Quantity += 1;
                    RoleplayUser.SendWeb("bin-add;slot1;" + Slot1 + ";" + Slot1Quantity);
                }
                else if (Slot2 == item && Slot2Type == type)
                {
                    Slot2Quantity += 1;
                    RoleplayUser.SendWeb("bin-add;slot2;" + Slot2 + ";" + Slot2Quantity);
                }
                else if (Slot3 == item && Slot3Type == type)
                {
                    Slot3Quantity += 1;
                    RoleplayUser.SendWeb("bin-add;slot3;" + Slot3 + ";" + Slot3Quantity);
                }
                else if (Slot4 == item && Slot4Type == type)
                {
                    Slot4Quantity += 1;
                    RoleplayUser.SendWeb("bin-add;slot4;" + Slot4 + ";" + Slot4Quantity);
                }
                else if (Slot5 == item && Slot5Type == type)
                {
                    Slot5Quantity += 1;
                    RoleplayUser.SendWeb("bin-add;slot5;" + Slot5 + ";" + Slot5Quantity);
                }
                else if (Slot6 == item && Slot6Type == type)
                {
                    Slot6Quantity += 1;
                    RoleplayUser.SendWeb("bin-add;slot6;" + Slot6 + ";" + Slot6Quantity);
                }
                else if (Slot7 == item && Slot7Type == type)
                {
                    Slot7Quantity += 1;
                    RoleplayUser.SendWeb("bin-add;slot7;" + Slot7 + ";" + Slot7Quantity);
                }
                else if (Slot8 == item && Slot8Type == type)
                {
                    Slot8Quantity += 1;
                    RoleplayUser.SendWeb("bin-add;slot8;" + Slot8 + ";" + Slot8Quantity);
                }
                else if (Slot9 == item && Slot9Type == type)
                {
                    Slot9Quantity += 1;
                    RoleplayUser.SendWeb("bin-add;slot9;" + Slot9 + ";" + Slot9Quantity);
                }
                else if (Slot10 == item && Slot10Type == type)
                {
                    Slot10Quantity += 1;
                    RoleplayUser.SendWeb("bin-add;slot10;" + Slot10 + ";" + Slot10Quantity);
                }
                else
                {
                    if (IsFull())
                    {
                        RoleplayUser.GetClient().SendWhisper("Your bin is full!");
                        return;
                    }

                    if (Slot1 == null)
                    {
                        Slot1 = item;
                        Slot1Type = type;
                        Slot1Quantity = 1;
                        RoleplayUser.SendWeb("bin-add;slot1;" + Slot1 + ";" + Slot1Quantity + ";0");
                    }
                    else if (Slot2 == null)
                    {
                        Slot2 = item;
                        Slot2Type = type;
                        Slot2Quantity = 1;
                        RoleplayUser.SendWeb("bin-add;slot2;" + Slot2 + ";" + Slot2Quantity + ";0");
                    }
                    else if (Slot3 == null)
                    {
                        Slot3 = item;
                        Slot3Type = type;
                        Slot3Quantity = 1;
                        RoleplayUser.SendWeb("bin-add;slot3;" + Slot3 + ";" + Slot3Quantity + ";0");
                    }
                    else if (Slot4 == null)
                    {
                        Slot4 = item;
                        Slot4Type = type;
                        Slot4Quantity = 1;
                        RoleplayUser.SendWeb("bin-add;slot4;" + Slot4 + ";" + Slot4Quantity + ";0");
                    }
                    else if (Slot5 == null)
                    {
                        Slot5 = item;
                        Slot5Type = type;
                        Slot5Quantity = 1;
                        RoleplayUser.SendWeb("bin-add;slot5;" + Slot5 + ";" + Slot5Quantity + ";0");
                    }
                    else if (Slot6 == null)
                    {
                        Slot6 = item;
                        Slot6Type = type;
                        Slot6Quantity = 1;
                        RoleplayUser.SendWeb("bin-add;slot6;" + Slot6 + ";" + Slot6Quantity + ";0");
                    }
                    else if (Slot7 == null)
                    {
                        Slot7 = item;
                        Slot7Type = type;
                        Slot7Quantity = 1;
                        RoleplayUser.SendWeb("bin-add;slot7;" + Slot7 + ";" + Slot7Quantity + ";0");
                    }
                    else if (Slot8 == null)
                    {
                        Slot8 = item;
                        Slot8Type = type;
                        Slot8Quantity = 1;
                        RoleplayUser.SendWeb("bin-add;slot8;" + Slot8 + ";" + Slot8Quantity + ";0");
                    }
                    else if (Slot9 == null)
                    {
                        Slot9 = item;
                        Slot9Type = type;
                        Slot9Quantity = 1;
                        RoleplayUser.SendWeb("bin-add;slot9;" + Slot9 + ";" + Slot9Quantity + ";0");
                    }
                    else if (Slot10 == null)
                    {
                        Slot10 = item;
                        Slot10Type = type;
                        Slot10Quantity = 1;
                        RoleplayUser.SendWeb("bin-add;slot10;" + Slot10 + ";" + Slot10Quantity + ";0");
                    }
                }
                RoleplayUser.Inventory.UpdateQuantity(fromSlot, 1);
            }
            else if (durability > 0)
            {
                if (IsFull())
                {
                    RoleplayUser.GetClient().SendWhisper("Your bin is full!");
                    return;
                }

                if (Slot1 == null)
                {
                    Slot1 = item;
                    Slot1Type = type;
                    Slot1Durability = durability;
                    RoleplayUser.SendWeb("bin-add;slot1;" + Slot1 + ";0;" + Slot1Durability);
                }
                else if (Slot2 == null)
                {
                    Slot2 = item;
                    Slot2Type = type;
                    Slot2Durability = durability;
                    RoleplayUser.SendWeb("bin-add;slot2;" + Slot2 + ";0;" + Slot2Durability);
                }
                else if (Slot3 == null)
                {
                    Slot3 = item;
                    Slot3Type = type;
                    Slot3Durability = durability;
                    RoleplayUser.SendWeb("bin-add;slot3;" + Slot3 + ";0;" + Slot3Durability);
                }
                else if (Slot4 == null)
                {
                    Slot4 = item;
                    Slot4Type = type;
                    Slot4Durability = durability;
                    RoleplayUser.SendWeb("bin-add;slot4;" + Slot4 + ";0;" + Slot4Durability);
                }
                else if (Slot5 == null)
                {
                    Slot5 = item;
                    Slot5Type = type;
                    Slot5Durability = durability;
                    RoleplayUser.SendWeb("bin-add;slot5;" + Slot5 + ";0;" + Slot5Durability);
                }
                else if (Slot6 == null)
                {
                    Slot6 = item;
                    Slot6Type = type;
                    Slot6Durability = durability;
                    RoleplayUser.SendWeb("bin-add;slot6;" + Slot6 + ";0;" + Slot6Durability);
                }
                else if (Slot7 == null)
                {
                    Slot7 = item;
                    Slot7Type = type;
                    Slot7Durability = durability;
                    RoleplayUser.SendWeb("bin-add;slot7;" + Slot7 + ";0;" + Slot7Durability);
                }
                else if (Slot8 == null)
                {
                    Slot8 = item;
                    Slot8Type = type;
                    Slot8Durability = durability;
                    RoleplayUser.SendWeb("bin-add;slot8;" + Slot8 + ";0;" + Slot8Durability);
                }
                else if (Slot9 == null)
                {
                    Slot9 = item;
                    Slot9Type = type;
                    Slot9Durability = durability;
                    RoleplayUser.SendWeb("bin-add;slot9;" + Slot9 + ";0;" + Slot9Durability);
                }
                else if (Slot10 == null)
                {
                    Slot10 = item;
                    Slot10Type = type;
                    Slot10Durability = durability;
                    RoleplayUser.SendWeb("bin-add;slot10;" + Slot10 + ";0;" + Slot10Durability);
                }
                RoleplayUser.Inventory.Remove(fromSlot);
            }
        }

        public void Remove(string slot)
        {
            if (slot == "slot1")
            {
                if (Slot1 != null)
                {
                    if (Slot1Quantity > 0)
                    {
                        Slot1Quantity -= 1;
                        RoleplayUser.Inventory.Add(Slot1, Slot1Type, 1, 0, true, false);
                        if (Slot1Quantity == 0)
                        {
                            Slot1 = null;
                            Slot1Type = null;
                            Slot1Quantity = 0;
                            Slot1Durability = 0;
                            RoleplayUser.SendWeb("bin-remove;" + slot);
                        }
                        else
                            RoleplayUser.SendWeb("bin-add;" + slot + ";" + Slot1 + ";" + Slot1Quantity + ";0");
                    }
                    else if (Slot1Durability > 0)
                    {
                        RoleplayUser.Inventory.Add(Slot1, Slot1Type, 0, Slot1Durability, false, true);
                        Slot1 = null;
                        Slot1Type = null;
                        Slot1Quantity = 0;
                        Slot1Durability = 0;
                        RoleplayUser.SendWeb("bin-remove;" + slot);
                    }
                }
            }
            else if (slot == "slot2")
            {
                if (Slot2 != null)
                {
                    if (Slot2Quantity > 0)
                    {
                        Slot2Quantity -= 1;
                        RoleplayUser.Inventory.Add(Slot2, Slot2Type, 1, 0, true, false);
                        if (Slot2Quantity == 0)
                        {
                            Slot2 = null;
                            Slot2Type = null;
                            Slot2Quantity = 0;
                            Slot2Durability = 0;
                            RoleplayUser.SendWeb("bin-remove;" + slot);
                        }
                        else
                            RoleplayUser.SendWeb("bin-add;" + slot + ";" + Slot2 + ";" + Slot2Quantity + ";0");
                    }
                    else if (Slot2Durability > 0)
                    {
                        RoleplayUser.Inventory.Add(Slot2, Slot2Type, 0, Slot2Durability, false, true);
                        Slot2 = null;
                        Slot2Type = null;
                        Slot2Quantity = 0;
                        Slot2Durability = 0;
                        RoleplayUser.SendWeb("bin-remove;" + slot);
                    }
                }
            }
            else if (slot == "slot3")
            {
                if (Slot3 != null)
                {
                    if (Slot3Quantity > 0)
                    {
                        Slot3Quantity -= 1;
                        RoleplayUser.Inventory.Add(Slot3, Slot3Type, 1, 0, true, false);
                        if (Slot3Quantity == 0)
                        {
                            Slot3 = null;
                            Slot3Type = null;
                            Slot3Quantity = 0;
                            Slot3Durability = 0;
                            RoleplayUser.SendWeb("bin-remove;" + slot);
                        }
                        else
                            RoleplayUser.SendWeb("bin-add;" + slot + ";" + Slot3 + ";" + Slot3Quantity + ";0");
                    }
                    else if (Slot3Durability > 0)
                    {
                        RoleplayUser.Inventory.Add(Slot3, Slot3Type, 0, Slot3Durability, false, true);
                        Slot3 = null;
                        Slot3Type = null;
                        Slot3Quantity = 0;
                        Slot3Durability = 0;
                        RoleplayUser.SendWeb("bin-remove;" + slot);
                    }
                }
            }
            else if (slot == "slot4")
            {
                if (Slot4 != null)
                {
                    if (Slot4Quantity > 0)
                    {
                        Slot4Quantity -= 1;
                        RoleplayUser.Inventory.Add(Slot4, Slot4Type, 1, 0, true, false);
                        if (Slot4Quantity == 0)
                        {
                            Slot4 = null;
                            Slot4Type = null;
                            Slot4Quantity = 0;
                            Slot4Durability = 0;
                            RoleplayUser.SendWeb("bin-remove;" + slot);
                        }
                        else
                            RoleplayUser.SendWeb("bin-add;" + slot + ";" + Slot4 + ";" + Slot4Quantity + ";0");
                    }
                    else if (Slot4Durability > 0)
                    {
                        RoleplayUser.Inventory.Add(Slot4, Slot4Type, 0, Slot4Durability, false, true);
                        Slot4 = null;
                        Slot4Type = null;
                        Slot4Quantity = 0;
                        Slot4Durability = 0;
                        RoleplayUser.SendWeb("bin-remove;" + slot);
                    }
                }
            }
            else if (slot == "slot5")
            {
                if (Slot5 != null)
                {
                    if (Slot5Quantity > 0)
                    {
                        Slot5Quantity -= 1;
                        RoleplayUser.Inventory.Add(Slot5, Slot5Type, 1, 0, true, false);
                        if (Slot5Quantity == 0)
                        {
                            Slot5 = null;
                            Slot5Type = null;
                            Slot5Quantity = 0;
                            Slot5Durability = 0;
                            RoleplayUser.SendWeb("bin-remove;" + slot);
                        }
                        else
                            RoleplayUser.SendWeb("bin-add;" + slot + ";" + Slot5 + ";" + Slot5Quantity + ";0");
                    }
                    else if (Slot5Durability > 0)
                    {
                        RoleplayUser.Inventory.Add(Slot5, Slot5Type, 0, Slot5Durability, false, true);
                        Slot5 = null;
                        Slot5Type = null;
                        Slot5Quantity = 0;
                        Slot5Durability = 0;
                        RoleplayUser.SendWeb("bin-remove;" + slot);
                    }
                }
            }
            else if (slot == "slot6")
            {
                if (Slot6 != null)
                {
                    if (Slot6Quantity > 0)
                    {
                        Slot6Quantity -= 1;
                        RoleplayUser.Inventory.Add(Slot6, Slot6Type, 1, 0, true, false);
                        if (Slot6Quantity == 0)
                        {
                            Slot6 = null;
                            Slot6Type = null;
                            Slot6Quantity = 0;
                            Slot6Durability = 0;
                            RoleplayUser.SendWeb("bin-remove;" + slot);
                        }
                        else
                            RoleplayUser.SendWeb("bin-add;" + slot + ";" + Slot6 + ";" + Slot6Quantity + ";0");
                    }
                    else if (Slot6Durability > 0)
                    {
                        RoleplayUser.Inventory.Add(Slot6, Slot6Type, 0, Slot6Durability, false, true);
                        Slot6 = null;
                        Slot6Type = null;
                        Slot6Quantity = 0;
                        Slot6Durability = 0;
                        RoleplayUser.SendWeb("bin-remove;" + slot);
                    }
                }
            }
            else if(slot == "slot7")
            {
                if (Slot7 != null)
                {
                    if (Slot7Quantity > 0)
                    {
                        Slot7Quantity -= 1;
                        RoleplayUser.Inventory.Add(Slot7, Slot7Type, 1, 0, true, false);
                        if (Slot7Quantity == 0)
                        {
                            Slot7 = null;
                            Slot7Type = null;
                            Slot7Quantity = 0;
                            Slot7Durability = 0;
                            RoleplayUser.SendWeb("bin-remove;" + slot);
                        }
                        else
                            RoleplayUser.SendWeb("bin-add;" + slot + ";" + Slot7 + ";" + Slot7Quantity + ";0");
                    }
                    else if (Slot7Durability > 0)
                    {
                        RoleplayUser.Inventory.Add(Slot7, Slot7Type, 0, Slot7Durability, false, true);
                        Slot7 = null;
                        Slot7Type = null;
                        Slot7Quantity = 0;
                        Slot7Durability = 0;
                        RoleplayUser.SendWeb("bin-remove;" + slot);
                    }
                }
            }
            else if (slot == "slot8")
            {
                if (Slot8 != null)
                {
                    if (Slot8Quantity > 0)
                    {
                        Slot8Quantity -= 1;
                        RoleplayUser.Inventory.Add(Slot8, Slot8Type, 1, 0, true, false);
                        if (Slot8Quantity == 0)
                        {
                            Slot8 = null;
                            Slot8Type = null;
                            Slot8Quantity = 0;
                            Slot8Durability = 0;
                            RoleplayUser.SendWeb("bin-remove;" + slot);
                        }
                        else
                            RoleplayUser.SendWeb("bin-add;" + slot + ";" + Slot8 + ";" + Slot8Quantity + ";0");
                    }
                    else if (Slot8Durability > 0)
                    {
                        RoleplayUser.Inventory.Add(Slot8, Slot8Type, 0, Slot8Durability, false, true);
                        Slot8 = null;
                        Slot8Type = null;
                        Slot8Quantity = 0;
                        Slot8Durability = 0;
                        RoleplayUser.SendWeb("bin-remove;" + slot);
                    }
                }
            }
        }

        public void End()
        {
            Using = false;
            RoleplayUser.SendWeb("bin;end");

            #region Reset Bin Slots
            Slot1 = null;
            Slot2 = null;
            Slot3 = null;
            Slot4 = null;
            Slot5 = null;
            Slot6 = null;
            Slot7 = null;
            Slot8 = null;
            Slot9 = null;
            Slot10 = null;
            Slot1Type = null;
            Slot2Type = null;
            Slot3Type = null;
            Slot4Type = null;
            Slot5Type = null;
            Slot6Type = null;
            Slot7Type = null;
            Slot8Type = null;
            Slot9Type = null;
            Slot10Type = null;
            Slot1Quantity = 0;
            Slot2Quantity = 0;
            Slot3Quantity = 0;
            Slot4Quantity = 0;
            Slot5Quantity = 0;
            Slot6Quantity = 0;
            Slot7Quantity = 0;
            Slot8Quantity = 0;
            Slot9Quantity = 0;
            Slot10Quantity = 0;
            Slot1Durability = 0;
            Slot2Durability = 0;
            Slot3Durability = 0;
            Slot4Durability = 0;
            Slot5Durability = 0;
            Slot6Durability = 0;
            Slot7Durability = 0;
            Slot8Durability = 0;
            Slot9Durability = 0;
            Slot10Durability = 0;
            #endregion
        }

        public bool IsFull()
        {
            if (Slot1 == null)
                return false;
            else if (Slot2 == null)
                return false;
            else if (Slot3 == null)
                return false;
            else if (Slot4 == null)
                return false;
            else if (Slot5 == null)
                return false;
            else if (Slot6 == null)
                return false;
            else if (Slot7 == null)
                return false;
            else if (Slot8 == null)
                return false;
            else if (Slot9 == null)
                return false;
            else if (Slot10 == null)
                return false;
            else
                return true;
        }
    }
}
