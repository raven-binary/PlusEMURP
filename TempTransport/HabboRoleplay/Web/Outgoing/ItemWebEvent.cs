using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plus;
using Fleck;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Rooms.Notifications;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.Items;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using System.Drawing;
using System.Data;

namespace Bobba.HabboRoleplay.Web.Outgoing
{
    class ItemWebEvent : IWebEvent
    {
        /// <summary>
        /// Executes socket data.
        /// </summary>
        /// <param name="Client"></param>
        /// <param name="Data"></param>
        /// <param name="Socket"></param>
        /// 
        public static bool AreUsersInRange(RoomUser User1, RoomUser User2, int range)
        {
            int DistanceA = Math.Abs(User1.X - User2.X) + Math.Abs(User1.Y - User2.Y);
            int DistanceB = Math.Abs(User1.SetX - User2.SetX) + Math.Abs(User1.SetY - User2.SetY);
            return DistanceA > range ? false : true;
        }

        public bool CheckDiag(int GetX, int GetY, int roomUserX, int roomUserY)
        {
            if (roomUserX - 1 == GetX && roomUserY - 1 == GetY || roomUserX - 1 == GetX && roomUserY + 1 == GetY)
                return true;
            else return false;
        }
        public void Execute(GameClient Client, string Data, IWebSocketConnection Socket)
        {

            if (!PlusEnvironment.GetGame().GetWebEventManager().SocketReady(Client, true) || !PlusEnvironment.GetGame().GetWebEventManager().SocketReady(Socket))
                return;


            string Action = (Data.Contains(',') ? Data.Split(',')[0] : Data);
            switch (Action)
            {
                #region GetTarget
                case "target":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Username = ReceivedData[1];

                        PlusEnvironment.GetGame().GetChatManager().GetCommands().Parse(Client, ":t " + Username);
                    }
                    break;
                #endregion
                #region Spray
                case "stolenpepperspray":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().getCooldown("spray_command"))
                        {
                            Client.SendWhisper("Wait a bit before you can spray someone again");
                            return;
                        }

                        string[] ReceivedData = Data.Split(',');
                        string Slot = ReceivedData[1];

                        if (Client.GetHabbo().CheckWarnings())
                            return;

                        if (Client.GetRoleplay().TargetId == 0)
                        {
                            Client.SendWhisper("Please specific a target");
                            return;
                        }

                        GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(Client.GetRoleplay().TargetId);
                        if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Client.GetHabbo().CurrentRoom)
                        {
                            Client.SendWhisper("Player not found in this room");
                            return;
                        }
                        RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetRoleplay().TargetId);

                        if (Client.GetHabbo().CheckSafezone(TargetUser.GetUsername()))
                            return;

                        if (Math.Abs(User.Y - TargetUser.Y) > 2 || Math.Abs(User.X - TargetUser.X) > 2 || User.X != TargetUser.X && User.Y != TargetUser.Y)
                        {
                            User.Say("sprays Pepper Spray at " + TargetClient.GetHabbo().Username + ", but misses");
                            PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                            return;
                        }

                        User.Say("sprays Pepper Spray at " + TargetClient.GetHabbo().Username + " blinding them");
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "blind;show;");
                        TargetClient.SendWhisper("You feel confused");
                        TargetUser.CanWalk = false;

                        Client.GetHabbo().addCooldown("spray_command", 3000);
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");

                        TargetUser.MoveTo(TargetUser.X - 1, TargetUser.Y, true);

                        System.Timers.Timer MovementTimer = new System.Timers.Timer(850);
                        MovementTimer.Interval = 850;
                        MovementTimer.Elapsed += delegate
                        {
                            if (!TargetClient.GetHabbo().SprayMov1)
                            {
                                TargetClient.GetHabbo().SprayMov1 = true;
                                TargetUser.MoveTo(TargetUser.X + 1, TargetUser.Y, true);
                            }
                            else if (!TargetClient.GetHabbo().SprayMov2)
                            {
                                TargetClient.GetHabbo().SprayMov2 = true;
                                TargetUser.MoveTo(TargetUser.X, TargetUser.Y + 1, true);
                            }
                            else if (!TargetClient.GetHabbo().SprayMov3)
                            {
                                TargetClient.GetHabbo().SprayMov3 = true;
                                TargetUser.MoveTo(TargetUser.X, TargetUser.Y - 1, true);
                            }
                            else if (!TargetClient.GetHabbo().SprayMov4)
                            {
                                TargetClient.GetHabbo().SprayMov4 = true;
                                TargetUser.MoveTo(TargetUser.X + 1, TargetUser.Y, true);
                            }
                            else if (!TargetClient.GetHabbo().SprayMov5)
                            {
                                TargetClient.GetHabbo().SprayMov5 = true;
                                TargetUser.MoveTo(TargetUser.X - 1, TargetUser.Y, true);
                            }
                            else
                            {
                                TargetClient.GetHabbo().SprayMov1 = false;
                                TargetClient.GetHabbo().SprayMov2 = false;
                                TargetClient.GetHabbo().SprayMov3 = false;
                                TargetClient.GetHabbo().SprayMov4 = false;
                                TargetClient.GetHabbo().SprayMov5 = false;
                                TargetUser.CanWalk = true;
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "blind;hide;");
                                MovementTimer.Stop();
                            }
                        };
                        MovementTimer.Start();
                    }
                    break;
                #endregion
                #region Medkit
                case "medkit":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetRoleplay().Stunned)
                        {
                            Client.SendWhisper("You can not perform this action while stunned");
                            return;
                        }

                        if (Client.GetHabbo().Hospital == 1 || Client.GetRoleplay().Dead)
                        {
                            Client.SendWhisper("You can not perform this action while dead");
                            return;
                        }

                        if (Client.GetHabbo().isBleeding)
                        {
                            Client.SendWhisper("You can not use a medkit while bleeding");
                            return;
                        }

                        if (Client.GetRoleplay().MedkitCooldown > 0)
                        {
                            Client.SendWhisper("Please wait " + Client.GetRoleplay().MedkitCooldown + " seconds before using another medkit");
                            return;
                        }

                        if (Client.GetRoleplay().Health == Client.GetRoleplay().HealthMax)
                        {
                            Client.SendWhisper("Your health is already full");
                            return;
                        }

                        string[] ReceivedData = Data.Split(',');
                        string Slot = ReceivedData[1];

                        Client.GetRoleplay().Inventory.UpdateQuantity(Slot, 1);
                        Client.GetRoleplay().MedkitCooldown = 180;
                        User.Say("uses a medkit");
                        Client.GetRoleplay().Health += 25;

                        #region In-case
                        if (Client.GetRoleplay().Health > Client.GetRoleplay().HealthMax)
                        {
                            Client.GetRoleplay().Health = Client.GetRoleplay().HealthMax;
                            Client.GetRoleplay().RPCache(1);
                            return;
                        }
                        #endregion

                        Client.GetRoleplay().RPCache(1);
                        Client.GetRoleplay().UsingMedkit = true;
                    }
                    break;
                #endregion
                #region Snack
                case "snack":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetRoleplay().Stunned)
                        {
                            Client.SendWhisper("You can not perform this action while stunned");
                            return;
                        }

                        if (Client.GetRoleplay().Hospital)
                        {
                            Client.SendWhisper("You can not perform this action while dead");
                            return;
                        }

                        if (Client.GetRoleplay().Energy == 100)
                        {
                            Client.SendWhisper("Your energy is already full");
                            return;
                        }

                        string[] ReceivedData = Data.Split(',');
                        string Slot = ReceivedData[1];

                        Client.GetRoleplay().Inventory.UpdateQuantity(Slot, 1);
                        Client.GetRoleplay().UsingSnack = true;
                        User.Say("opens a bag of cheetos and begins eating");
                        User.CarryItem(51);
                    }
                    break;
                #endregion
                #region Weapon
                case "arme":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Weapon = ReceivedData[1];
                        string FromSlot = ReceivedData[2];
                        string Durability = ReceivedData[3];

                        if (Client.GetHabbo().Hospital == 1 || Client.GetHabbo().Prison != 0)
                            return;

                        if (Client.GetHabbo().getCooldown("equip_command"))
                        {
                            Client.SendWhisper("Please wait before equipping your weapon again");
                            return;
                        }

                        User.UnIdle();

                        if (Client.GetHabbo().ArmeEquiped != null)
                        {
                            if (Client.GetHabbo().InventoryEquipSlot1Item == "stungun" || Client.GetHabbo().InventoryEquipSlot1Item == "bat" || Client.GetHabbo().InventoryEquipSlot1Item == "sword" || Client.GetHabbo().InventoryEquipSlot1Item == "axe")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "replace-equip,slot1," + FromSlot);
                            }
                            else if (Client.GetHabbo().InventoryEquipSlot2Item == "stungun" || Client.GetHabbo().InventoryEquipSlot2Item == "bat" || Client.GetHabbo().InventoryEquipSlot2Item == "sword" || Client.GetHabbo().InventoryEquipSlot2Item == "axe")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "replace-equip,slot2," + FromSlot);
                            }
                        }

                        Client.GetHabbo().addCooldown("equip_command", 3000);
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "equip," + Weapon + "," + Durability + "," + FromSlot);

                    }
                    break;
                #endregion
                #region Bodyarmour
                case "bodyarmour":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Slot = ReceivedData[1];
                        string Durability = ReceivedData[2];

                        if (Client.GetHabbo().Hospital == 1)
                        {
                            Client.SendWhisper("You can not perform this action while dead");
                            return;
                        }

                        if (Client.GetHabbo().usingArena)
                        {
                            Client.SendWhisper("You can not perform this action while fighting in the arena");
                            return;
                        }

                        if (Client.GetHabbo().BodyArmour)
                            return;

                        string Color = "62";
                        DataRow Gang = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `gang` WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("id", Client.GetHabbo().Gang);
                            Gang = dbClient.getRow();
                        }

                        if (Gang != null)
                        {
                            string[] Colors = Convert.ToString(Gang["color_1"]).Split('-');
                            Color = Colors[2];
                        }

                        if (Client.GetHabbo().InventoryEquipSlot1 == null)
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-equip;add;1;bodyarmour;" + Slot + ";" + Durability);
                            Client.GetHabbo().InventoryEquipSlot1 = Slot;
                            Client.GetHabbo().InventoryEquipSlot1Item = "bodyarmour";
                        }
                        else if (Client.GetHabbo().InventoryEquipSlot2 == null)
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-equip;add;2;bodyarmour;" + Slot + ";" + Durability);
                            Client.GetHabbo().InventoryEquipSlot2 = Slot;
                            Client.GetHabbo().InventoryEquipSlot2Item = "bodyarmour";
                        }

                        Client.GetHabbo().updateAvatarEvent(Client.GetHabbo().Look + ".cp-58481-73", Client.GetHabbo().Look + ".cp-58481-73", "Wearing Body Armour");
                        Client.GetHabbo().BodyArmour = true;
                    }
                    break;
                #endregion
                #region sell
                case "sell":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;


                        string[] ReceivedData = Data.Split(',');
                        string Id = ReceivedData[1];

                        if (Client.GetHabbo().JobId == 4)
                        {
                            if (Id == "bat" | Id == "sword" | Id == "axe" | Id == "vest" | Id == "knive" && Client.GetHabbo().RankId == 7)
                            {
                                Client.SendWhisper("You are not able to sell that item yet");
                                return;
                            }
                            else if (Id == "sword" | Id == "axe" && Client.GetHabbo().RankId == 6)
                            {
                                Client.SendWhisper("You are not able to sell that item yet");
                                return;
                            }
                            else if (Id == "sword" && Client.GetHabbo().RankId == 5)
                            {
                                Client.SendWhisper("You are not able to sell that item yet");
                                return;
                            }

                            if (User.CarryItemID != 0)
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "UserEffects_effects_2AlOo;hide;0;" + Client.GetHabbo().PlayToken);
                                Client.GetHabbo().PlayToken = 0;
                            }

                            Random TokenRand = new Random();
                            int tokenNumber = TokenRand.Next(1600, 2894354);
                            Client.GetHabbo().PlayToken = tokenNumber;
                            User.CarryItem(1004);
                            int Precent = 100;
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "UserEffects_effects_2AlOo;show;" + Precent + ";" + Client.GetHabbo().PlayToken);
                           
                            System.Timers.Timer Count = new System.Timers.Timer(500);
                            Count.Interval = 500;
                            Count.Elapsed += delegate
                            {
                                if (Client.GetHabbo().PlayToken == tokenNumber)
                                {
                                    Precent -= 5;
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "UserEffects_effects_2AlOo;count;" + Precent + ";" + Client.GetHabbo().PlayToken);

                                    if (Precent <= 0 && Client.GetHabbo().PlayToken == tokenNumber)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "UserEffects_effects_2AlOo;hide;" + Precent + ";" + Client.GetHabbo().PlayToken);
                                        Client.GetHabbo().PlayToken = 0;
                                        
                                        Client.GetHabbo().CorpSell = null;
                                        Count.Stop();
                                    }
                                }
                                else
                                {
                                    Count.Dispose();
                                }
                            };
                            Count.Start();

                            
                            Client.GetHabbo().CorpSell = Id;
                        }
                        else if (Client.GetHabbo().JobId == 3)
                        {
                            if (User.CarryItemID != 0)
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "UserEffects_effects_2AlOo;hide;0;" + Client.GetHabbo().PlayToken);
                                Client.GetHabbo().PlayToken = 0;
                            }
                            
                            Client.GetHabbo().CorpSell = Id;
                            Random TokenRand = new Random();
                            int tokenNumber = TokenRand.Next(1600, 2894354);
                            Client.GetHabbo().PlayToken = tokenNumber;

                            int Precent = 100;
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "UserEffects_effects_2AlOo;show;" + Precent + ";" + Client.GetHabbo().PlayToken);

                            System.Timers.Timer Count = new System.Timers.Timer(500);
                            Count.Interval = 500;
                            Count.Elapsed += delegate
                            {
                                if (Client.GetHabbo().PlayToken == tokenNumber)
                                {
                                    Precent -= 5;
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "UserEffects_effects_2AlOo;count;" + Precent + ";" + Client.GetHabbo().PlayToken);

                                    if (Precent <= 0 && Client.GetHabbo().PlayToken == tokenNumber)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "UserEffects_effects_2AlOo;hide;" + Precent + ";" + Client.GetHabbo().PlayToken);
                                        Client.GetHabbo().PlayToken = 0;
                                        
                                        Client.GetHabbo().CorpSell = null;
                                        Count.Stop();
                                    }
                                }
                                else
                                {
                                    Count.Dispose();
                                }
                            };
                            Count.Start();
                           
                        }
                    }
                    break;
                #endregion
                #region Manager Mail
                case "managermail":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Slot = ReceivedData[1];

                        if (Client.GetHabbo().UsingManagerMail)
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                            User.Say("delivers manager mail");
                            User.GetClient().SendWhisper("You have earned $25 for completing this task");
                            User.GetClient().GetHabbo().UpdateTasksCompleted();
                            User.GetClient().GetHabbo().Credits += 25;
                            User.GetClient().SendMessage(new CreditBalanceComposer(User.GetClient().GetHabbo().Credits));
                            User.GetClient().GetHabbo().RPCache(3);
                        }
                    }
                    break;
                #endregion
                #region Armoury Box
                case "box":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Slot = ReceivedData[1];

                        if (Client.GetHabbo().UsingArmouryMerchandise)
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                            User.Say("delivers merchandise");
                            User.GetClient().SendWhisper("You have earned $50 for completing this task");
                            User.GetClient().GetHabbo().UpdateTasksCompleted();
                            User.GetClient().GetHabbo().Credits += User.GetClient().GetHabbo().Credits + 50;
                            User.GetClient().SendMessage(new CreditBalanceComposer(User.GetClient().GetHabbo().Credits));
                            User.GetClient().GetHabbo().RPCache(3);
                        }
                    }
                    break;
                #endregion
                #region Sell Stock
                case "sell-stock":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Id = ReceivedData[1];
                        string Slot = "";
                        int Quantity = 0;

                        if (Client.GetHabbo().InventorySlot1 == Id)
                        {
                            Slot = "slot1";
                            Quantity = Client.GetHabbo().InventorySlot1Quantity;
                        }
                        else if (Client.GetHabbo().InventorySlot2 == Id)
                        {
                            Slot = "slot2";
                            Quantity = Client.GetHabbo().InventorySlot2Quantity;
                        }
                        else if (Client.GetHabbo().InventorySlot3 == Id)
                        {
                            Slot = "slot3";
                            Quantity = Client.GetHabbo().InventorySlot3Quantity;
                        }
                        else if (Client.GetHabbo().InventorySlot4 == Id)
                        {
                            Slot = "slot4";
                            Quantity = Client.GetHabbo().InventorySlot4Quantity;
                        }
                        else if (Client.GetHabbo().InventorySlot5 == Id)
                        {
                            Slot = "slot5";
                            Quantity = Client.GetHabbo().InventorySlot5Quantity;
                        }
                        else if (Client.GetHabbo().InventorySlot6 == Id)
                        {
                            Slot = "slot6";
                            Quantity = Client.GetHabbo().InventorySlot6Quantity;
                        }
                        else if (Client.GetHabbo().InventorySlot7 == Id)
                        {
                            Slot = "slot7";
                            Quantity = Client.GetHabbo().InventorySlot7Quantity;
                        }
                        else if (Client.GetHabbo().InventorySlot8 == Id)
                        {
                            Slot = "slot8";
                            Quantity = Client.GetHabbo().InventorySlot8Quantity;
                        }
                        else if (Client.GetHabbo().InventorySlot9 == Id)
                        {
                            Slot = "slot9";
                            Quantity = Client.GetHabbo().InventorySlot9Quantity;
                        }
                        else if (Client.GetHabbo().InventorySlot10 == Id)
                        {
                            Slot = "slot10";
                            Quantity = Client.GetHabbo().InventorySlot10Quantity;
                        }
                        else
                        {
                            if (Id == "coffee_bean")
                            {
                                Client.SendWhisper("You don't have any Coffee Bean to sell");
                            }
                            else if (Id == "healingcrop")
                            {
                                Client.SendWhisper("You don't have any Mending Weed to sell");
                            }
                            else if (Id == "wool")
                            {
                                Client.SendWhisper("You don't have any Wool to sell");
                            }
                            else if (Id == "ironore")
                            {
                                Client.SendWhisper("You don't have any Iron Ore to sell");
                            }
                            else if (Id == "coal")
                            {
                                Client.SendWhisper("You don't have any Coal to sell");
                            }
                            return;
                        }

                        int Credits = 0;
                        int Stock = 0;

                        if (Client.GetHabbo().usingSellingStock)
                        {
                            if (Id == "coffee_bean")
                            {
                                Group Starbucks = null;
                                if (PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(6, out Starbucks))
                                {
                                    if (Starbucks.ChiffreAffaire == 2500)
                                    {
                                        Client.SendWhisper("The corporation stock is currently full");
                                        return;
                                    }

                                    Starbucks.ChiffreAffaire += Stock;
                                    Starbucks.updateChiffre();

                                    User.Say("sells " + Quantity + "x Coffee Beans");
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-," + Quantity);
                                    User.GetClient().GetHabbo().Credits += Credits;
                                    Client.SendWhisper("You earned $" + Credits + " for selling " + Quantity + "x coffee beans");
                                    User.GetClient().SendMessage(new CreditBalanceComposer(User.GetClient().GetHabbo().Credits));
                                    User.GetClient().GetHabbo().RPCache(3);
                                }
                            }
                            else if (Id == "healingcrop")
                            {
                                Group Hospital = null;
                                if (PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(2, out Hospital))
                                {
                                    if (Hospital.ChiffreAffaire == 2500)
                                    {
                                        Client.SendWhisper("The corporation stock is currently full");
                                        return;
                                    }

                                    Hospital.ChiffreAffaire += Stock;
                                    Hospital.updateChiffre();

                                    User.Say("sells " + Quantity + "x Mending Weeds");
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-," + Quantity);
                                    User.GetClient().GetHabbo().Credits += Credits;
                                    Client.SendWhisper("You earned $" + Credits + " for selling " + Quantity + "x mending weeds");
                                    User.GetClient().SendMessage(new CreditBalanceComposer(User.GetClient().GetHabbo().Credits));
                                    User.GetClient().GetHabbo().RPCache(3);
                                }
                            }
                            else if (Id == "wool")
                            {
                                Group Forever21 = null;
                                if (PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(5, out Forever21))
                                {
                                    if (Forever21.ChiffreAffaire == 3500)
                                    {
                                        Client.SendWhisper("The corporation stock is currently full");
                                        return;
                                    }

                                    Forever21.ChiffreAffaire += Stock;
                                    Forever21.updateChiffre();

                                    User.Say("sells " + Quantity + "x Wools");
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-," + Quantity);
                                    User.GetClient().GetHabbo().Credits += Credits;
                                    Client.SendWhisper("You earned $" + Credits + " for selling " + Quantity + "x wools");
                                    User.GetClient().SendMessage(new CreditBalanceComposer(User.GetClient().GetHabbo().Credits));
                                    User.GetClient().GetHabbo().RPCache(3);
                                }
                            }
                            else if (Id == "ironore")
                            {
                                Stock = 7 * Quantity;
                                Credits = 3 * Quantity;
                                Group Armoury = null;
                                if (PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(4, out Armoury))
                                {
                                    if (Armoury.ChiffreAffaire == 3500)
                                    {
                                        Client.SendWhisper("The corporation stock is currently full");
                                        return;
                                    }

                                    Armoury.ChiffreAffaire += Stock;
                                    Armoury.updateChiffre();

                                    User.Say("sells " + Quantity + "x Iron Ore");
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-," + Quantity);
                                    User.GetClient().GetHabbo().Credits += Credits;
                                    Client.SendWhisper("You earned $" + Credits + " for selling " + Quantity + "x iron ores");
                                    User.GetClient().SendMessage(new CreditBalanceComposer(User.GetClient().GetHabbo().Credits));
                                    User.GetClient().GetHabbo().RPCache(3);
                                }
                            }
                            else if (Id == "coal")
                            {
                                Stock = 4 * Quantity;
                                Credits = 5 * Quantity;
                                Group Armoury = null;
                                if (PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(4, out Armoury))
                                {
                                    if (Armoury.ChiffreAffaire == 3500)
                                    {
                                        Client.SendWhisper("The corporation stock is currently full");
                                        return;
                                    }

                                    Armoury.ChiffreAffaire += Stock;
                                    Armoury.updateChiffre();

                                    User.Say("sells " + Quantity + "x Coal");
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-," + Quantity);
                                    User.GetClient().GetHabbo().Credits += Credits;
                                    Client.SendWhisper("You earned $" + Credits + " for selling " + Quantity + "x coals");
                                    User.GetClient().SendMessage(new CreditBalanceComposer(User.GetClient().GetHabbo().Credits));
                                    User.GetClient().GetHabbo().RPCache(3);
                                }
                            }
                        }
                    }
                    break;
                #endregion
                #region Farming Text
                case "stock-texts":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Id = ReceivedData[1];

                        if (Id == "coffee_bean")
                        {
                            Room GetRoom = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(94);
                            Client.SendWhisper("You can sell your Coffee Bean at " + GetRoom.Name);
                        }
                        else if (Id == "healingcrop")
                        {
                            Room GetRoom = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(62);
                            Client.SendWhisper("You can sell your Mending Weed at " + GetRoom.Name);
                        }
                        else if (Id == "wool")
                        {
                            Room GetRoom = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(70);
                            Client.SendWhisper("You can sell your Wool at " + GetRoom.Name);
                        }
                    }
                    break;
                #endregion
                #region Bomb
                case "bomb":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().ThrowingBomb)
                        {
                            Client.SendWhisper("You have put your bomb away");
                            Client.GetHabbo().ThrowingBomb = false;
                            User.CarryItem(0);
                            Client.GetHabbo().BombFromSlot = null;
                            Client.GetHabbo().AddToInventory2("bomb", 1);
                            return;
                        }

                        if (Client.GetHabbo().getCooldown("bomb_command") == true)
                        {
                            Client.SendWhisper("You must wait between detonating bombs");
                            return;
                        }

                        string[] ReceivedData = Data.Split(',');
                        string Id = ReceivedData[0];

                        string Slot = ReceivedData[1];
                        Client.GetHabbo().BombFromSlot = Slot;

                        User.CarryItem(1027);
                        Client.GetHabbo().ThrowingBomb = true;
                        Client.SendWhisper("Click to throw your bomb");
                        Client.GetHabbo().addCooldown("bomb_command", 3500);
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Client.GetHabbo().BombFromSlot + ",-,1");
                    }
                    break;
                #endregion
                #region Heroindrug
                case "heroindrug":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().CheckWarnings())
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Slot = ReceivedData[1];

                        Client.GetRoleplay().Inventory.UpdateQuantity(Slot, 1);
                        Client.GetRoleplay().UsingHeroinDrug = true;
                        User.Say("injects a dose of heroin");
                        User.ApplyEffect(12);

                        System.Timers.Timer RemoveHeroinTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(6));
                        RemoveHeroinTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(6);
                        RemoveHeroinTimer.Elapsed += delegate
                        {
                            Client.GetRoleplay().UsingHeroinDrug = false;
                            Client.GetRoleplay().ResetEffect();
                            RemoveHeroinTimer.Stop();
                        };
                        RemoveHeroinTimer.Start();

                    }
                    break;
                #endregion
                #region Flashbang
                case "flashbang":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Slot = ReceivedData[1];

                        if (Client.GetRoleplay().CheckWarnings())
                            return;

                        Client.GetRoleplay().CreateAggression(100);
                        Client.GetRoleplay().Inventory.UpdateQuantity(Slot, 1);
                        User.Say("throws a flashbang, stunning everyone around them", 5);

                        if (!Room.Fightable)
                        {
                            foreach (RoomUser UserInRoom in Room.GetRoomUserManager().GetUserList().ToList())
                            {
                                if (UserInRoom == null || UserInRoom.IsBot || UserInRoom.GetClient() == null || UserInRoom.GetClient().GetHabbo() == null || UserInRoom.GetClient().GetHabbo().Id == Client.GetHabbo().Id || UserInRoom.IsAsleep || UserInRoom.GetClient().GetHabbo().usingHeroinDrug || UserInRoom.GetClient().GetRoleplay().Aggression <= 0)
                                    continue;

                                if (UserInRoom.X == User.X && UserInRoom.Y == User.Y || UserInRoom.X == User.X && UserInRoom.Y == User.Y - 1 || UserInRoom.X == User.X && UserInRoom.Y == User.Y - 2 || UserInRoom.X == User.X - 1 && UserInRoom.Y == User.Y - 1 || UserInRoom.X == User.X + 1 && UserInRoom.Y == User.Y - 1 || UserInRoom.X == User.X - 1 && UserInRoom.Y == User.Y || UserInRoom.X == User.X - 2 && UserInRoom.Y == User.Y || UserInRoom.X == User.X + 1 && UserInRoom.Y == User.Y || UserInRoom.X == User.X + 2 && UserInRoom.Y == User.Y || UserInRoom.X == User.X - 1 && UserInRoom.Y == User.Y + 1 || UserInRoom.X == User.X && UserInRoom.Y == User.Y + 1 || UserInRoom.X == User.X + 1 && UserInRoom.Y == User.Y + 1 || UserInRoom.X == User.X && UserInRoom.Y == User.Y + 2
                                || UserInRoom.SetX == User.SetX && UserInRoom.SetY == User.SetY || UserInRoom.SetX == User.SetX && UserInRoom.SetY == User.SetY - 1 || UserInRoom.SetX == User.SetX && UserInRoom.SetY == User.SetY - 2 || UserInRoom.SetX == User.SetX - 1 && UserInRoom.SetY == User.SetY - 1 || UserInRoom.SetX == User.SetX + 1 && UserInRoom.SetY == User.SetY - 1 || UserInRoom.SetX == User.SetX - 1 && UserInRoom.SetY == User.SetY || UserInRoom.SetX == User.SetX - 2 && UserInRoom.SetY == User.SetY || UserInRoom.SetX == User.SetX + 1 && UserInRoom.SetY == User.SetY || UserInRoom.SetX == User.SetX + 2 && UserInRoom.SetY == User.SetY || UserInRoom.SetX == User.SetX - 1 && UserInRoom.SetY == User.SetY + 1 || UserInRoom.SetX == User.SetX && UserInRoom.SetY == User.SetY + 1 || UserInRoom.SetX == User.SetX + 1 && UserInRoom.SetY == User.SetY + 1 || UserInRoom.SetX == User.SetX && UserInRoom.SetY == User.SetY + 2)
                                {
                                    UserInRoom.GetClient().GetRoleplay().Stun("normal");
                                }
                            }
                        }
                        else
                        {
                            foreach (RoomUser UserInRoom in Room.GetRoomUserManager().GetUserList().ToList())
                            {
                                if (UserInRoom == null || UserInRoom.IsBot || UserInRoom.GetClient() == null || UserInRoom.GetClient().GetHabbo() == null || UserInRoom.GetClient().GetHabbo().Id == Client.GetHabbo().Id || UserInRoom.IsAsleep || UserInRoom.GetClient().GetHabbo().usingHeroinDrug)
                                    continue;

                                if (UserInRoom.X == User.X && UserInRoom.Y == User.Y || UserInRoom.X == User.X && UserInRoom.Y == User.Y - 1 || UserInRoom.X == User.X && UserInRoom.Y == User.Y - 2 || UserInRoom.X == User.X - 1 && UserInRoom.Y == User.Y - 1 || UserInRoom.X == User.X + 1 && UserInRoom.Y == User.Y - 1 || UserInRoom.X == User.X - 1 && UserInRoom.Y == User.Y || UserInRoom.X == User.X - 2 && UserInRoom.Y == User.Y || UserInRoom.X == User.X + 1 && UserInRoom.Y == User.Y || UserInRoom.X == User.X + 2 && UserInRoom.Y == User.Y || UserInRoom.X == User.X - 1 && UserInRoom.Y == User.Y + 1 || UserInRoom.X == User.X && UserInRoom.Y == User.Y + 1 || UserInRoom.X == User.X + 1 && UserInRoom.Y == User.Y + 1 || UserInRoom.X == User.X && UserInRoom.Y == User.Y + 2
                                || UserInRoom.SetX == User.SetX && UserInRoom.SetY == User.SetY || UserInRoom.SetX == User.SetX && UserInRoom.SetY == User.SetY - 1 || UserInRoom.SetX == User.SetX && UserInRoom.SetY == User.SetY - 2 || UserInRoom.SetX == User.SetX - 1 && UserInRoom.SetY == User.SetY - 1 || UserInRoom.SetX == User.SetX + 1 && UserInRoom.SetY == User.SetY - 1 || UserInRoom.SetX == User.SetX - 1 && UserInRoom.SetY == User.SetY || UserInRoom.SetX == User.SetX - 2 && UserInRoom.SetY == User.SetY || UserInRoom.SetX == User.SetX + 1 && UserInRoom.SetY == User.SetY || UserInRoom.SetX == User.SetX + 2 && UserInRoom.SetY == User.SetY || UserInRoom.SetX == User.SetX - 1 && UserInRoom.SetY == User.SetY + 1 || UserInRoom.SetX == User.SetX && UserInRoom.SetY == User.SetY + 1 || UserInRoom.SetX == User.SetX + 1 && UserInRoom.SetY == User.SetY + 1 || UserInRoom.SetX == User.SetX && UserInRoom.SetY == User.SetY + 2)
                                {
                                    UserInRoom.GetClient().GetRoleplay().Stun("normal");
                                }
                            }
                        }
                    }
                    break;
                #endregion
                #region Throwing Knife
                case "throwingknife":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Slot = ReceivedData[1];

                        if (Client.GetHabbo().CheckWarnings())
                            return;

                        if (Client.GetRoleplay().TargetId == 0)
                        {
                            Client.SendWhisper("Please specific a target");
                            return;
                        }

                        RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetRoleplay().TargetId);

                        if (Client.GetHabbo().CheckSafezone(TargetUser.GetUsername()))
                            return;

                        if (TargetUser.GetClient().GetHabbo().isBleeding)
                        {
                            Client.SendWhisper(TargetUser.GetUsername() + " has knive immunity");
                            return;
                        }

                        if (Client.GetHabbo().KniveCooldownTimer > 0)
                        {
                            Client.SendWhisper("Please wait " + Client.GetHabbo().KniveCooldownTimer + " seconds before throwing another Knive");
                            return;
                        }

                        Client.GetHabbo().CreateAggression(100);
                        //Client.GetHabbo().KniveCooldownTimer = 120;
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");

                        System.Timers.Timer KniveCooldownTimer = new System.Timers.Timer(500);
                        KniveCooldownTimer.Interval = 500;
                        KniveCooldownTimer.Elapsed += delegate
                        {
                            if (Client.GetHabbo().KniveCooldownTimer > 0)
                            {
                                Client.GetHabbo().KniveCooldownTimer -= 1;
                                KniveCooldownTimer.Start();
                            }
                            else
                            {
                                Client.GetHabbo().KniveCooldownTimer = 0;
                                KniveCooldownTimer.Stop();
                            }
                        };
                        KniveCooldownTimer.Start();

                        bool Throws = false;
                        Random rand = new Random();

                        if (rand.Next(0, 2) != 0)
                        {
                            Throws = true;
                        }

                        if (AreUsersInRange(User, TargetUser, 2) && Throws && !TargetUser.GetClient().GetHabbo().usingHeroinDrug || CheckDiag(TargetUser.X, TargetUser.Y, User.X, User.Y) && Throws && !TargetUser.GetClient().GetHabbo().usingHeroinDrug)
                        {
                            User.Say("throws a throwing knive at " + TargetUser.GetUsername() + ", causing them to bleed", 13);
                            TargetUser.GetClient().GetHabbo().isBleeding = true;
                            TargetUser.GetClient().GetHabbo().BleedsOut += 1;
                            TargetUser.Say("bleeds out for 3 damage", 3);
                            TargetUser.GetClient().GetRoleplay().Health -= 3;
                            TargetUser.GetClient().GetHabbo().RPCache(1);
                            TargetUser.GetClient().GetHabbo().LastHitFrom = Client.GetHabbo().Username;
                            TargetUser.GetClient().GetHabbo().HPBarley();

                            System.Timers.Timer KniveImmunityTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(20));
                            KniveImmunityTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(20);
                            KniveImmunityTimer.Elapsed += delegate
                            {
                                TargetUser.GetClient().SendWhisper("Your throwing knife immunity has worn off");
                                TargetUser.GetClient().GetHabbo().isBleeding = false;
                                KniveImmunityTimer.Stop();
                            };
                            KniveImmunityTimer.Start();

                            Item Blood = new Item(PlusEnvironment.GetRandomNumber(8888, 889000), TargetUser.GetRoom().Id, 900481, "", TargetUser.SetX, TargetUser.SetY, 0, 0, 0, 0, 0, 0, string.Empty, Room);
                            if (Room.GetRoomItemHandler().SetFloorItemByForce(null, Blood, TargetUser.SetX, TargetUser.SetY, 0, true, false, true))
                            {
                                Room.SendMessage(new ObjectUpdateComposer(Blood, 0));
                            }

                            System.Timers.Timer RemoveBloodTimer = new System.Timers.Timer(PlusEnvironment.ConvertMinutesToMilliseconds(5));
                            RemoveBloodTimer.Interval = PlusEnvironment.ConvertMinutesToMilliseconds(5);
                            RemoveBloodTimer.Elapsed += delegate
                            {
                                Room.GetRoomItemHandler().RemoveFurniture(null, Blood.Id);
                                RemoveBloodTimer.Stop();
                            };
                            RemoveBloodTimer.Start();

                            System.Timers.Timer BleedsOutTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(2));
                            BleedsOutTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(2);
                            BleedsOutTimer.Elapsed += delegate
                            {
                                if (TargetUser.GetClient().GetHabbo().isBleeding && TargetUser.GetClient().GetHabbo().BleedsOut < 5)
                                {
                                    TargetUser.Say("bleeds out for 3 damage", 3);
                                    TargetUser.GetClient().GetHabbo().BleedsOut += 1;
                                    TargetUser.GetClient().GetRoleplay().Health -= 3;
                                    TargetUser.GetClient().GetHabbo().RPCache(1);
                                    TargetUser.GetClient().GetHabbo().LastHitFrom = Client.GetHabbo().Username;
                                    TargetUser.GetClient().GetHabbo().HPBarley();

                                    Item Blood2 = new Item(PlusEnvironment.GetRandomNumber(8888, 889000), TargetUser.GetRoom().Id, 900481, "", TargetUser.SetX, TargetUser.SetY, 0, 0, 0, 0, 0, 0, string.Empty, Room);
                                    if (Room.GetRoomItemHandler().SetFloorItemByForce(null, Blood2, TargetUser.SetX, TargetUser.SetY, 0, true, false, true))
                                    {
                                        Room.SendMessage(new ObjectUpdateComposer(Blood2, 0));
                                    }

                                    System.Timers.Timer RemoveBloodTimer2 = new System.Timers.Timer(PlusEnvironment.ConvertMinutesToMilliseconds(5));
                                    RemoveBloodTimer2.Interval = PlusEnvironment.ConvertMinutesToMilliseconds(5);
                                    RemoveBloodTimer.Elapsed += delegate
                                    {
                                        Room.GetRoomItemHandler().RemoveFurniture(null, Blood2.Id);
                                        RemoveBloodTimer2.Stop();
                                    };
                                    RemoveBloodTimer2.Start();

                                    BleedsOutTimer.Start();
                                }
                                else
                                {
                                    TargetUser.GetClient().GetHabbo().BleedsOut = 0;
                                    BleedsOutTimer.Stop();
                                }
                            };
                            BleedsOutTimer.Start();
                        }
                        else
                        {
                            User.Say("throws a throwing knive at " + TargetUser.GetUsername() + ", but misses", 13);
                        }
                    }
                    break;
                #endregion
                #region tnt
                case "tnt":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Slot = ReceivedData[1];

                        User.Say("places a bomb on the floor and lights the fuse", 22, true);
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                        Item Bomb = new Item(PlusEnvironment.GetRandomNumber(590000, 590090095), Client.GetHabbo().CurrentRoomId, 900361, "", User.X, User.Y, 0, 0, 0, 0, 0, 0, string.Empty, Room);
                        if (Room.GetRoomItemHandler().SetFloorItemByForce(null, Bomb, User.X, User.Y, 0, true, false, true))
                        {
                            Room.SendMessage(new ObjectUpdateComposer(Bomb, 0));
                            Bomb.Rotation = 2;
                            Bomb.UpdateState(false, true);
                        }

                        System.Timers.Timer BombStartTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(3));
                        BombStartTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(3);
                        BombStartTimer.Elapsed += delegate
                        {
                            Client.GetHabbo().CurrentRoom.GetRoomItemHandler().RemoveFurniture(null, Bomb.Id);

                            Item Bomb1 = new Item(PlusEnvironment.GetRandomNumber(590000, 590090095), Client.GetHabbo().CurrentRoomId, 1371, "", Bomb.GetX, Bomb.GetY, 0, 0, 0, 0, 0, 0, string.Empty, Room);
                            if (Room.GetRoomItemHandler().SetFloorItemByForce(null, Bomb1, Bomb.GetX, Bomb.GetY, 0, true, false, true))
                            {
                                Room.SendMessage(new ObjectUpdateComposer(Bomb1, 0));
                                Bomb1.ExtraData = "1";
                                Bomb1.UpdateState(false, true);
                            }

                            Room CurrentRoom = Client.GetHabbo().CurrentRoom;

                            foreach (RoomUser UserInRoom in CurrentRoom.GetRoomUserManager().GetUserList().ToList())
                            {
                                if (UserInRoom == null || UserInRoom.IsBot || UserInRoom.GetClient() == null || UserInRoom.GetClient().GetHabbo() == null || UserInRoom.GetClient().GetHabbo().usingHeroinDrug)
                                    continue;

                                CurrentRoom.GetRoomUserManager().UpdateUserStatus(UserInRoom, true);

                                if (UserInRoom.X == Bomb1.GetX && UserInRoom.Y == Bomb1.GetY || UserInRoom.X == Bomb1.GetX && UserInRoom.Y == Bomb1.GetY - 1 || UserInRoom.X == Bomb1.GetX && UserInRoom.Y == Bomb1.GetY - 2 || UserInRoom.X == Bomb1.GetX - 1 && UserInRoom.Y == Bomb1.GetY - 1 || UserInRoom.X == Bomb1.GetX + 1 && UserInRoom.Y == Bomb1.GetY - 1 || UserInRoom.X == Bomb1.GetX - 1 && UserInRoom.Y == Bomb1.GetY || UserInRoom.X == Bomb1.GetX - 2 && UserInRoom.Y == Bomb1.GetY || UserInRoom.X == Bomb1.GetX + 1 && UserInRoom.Y == Bomb1.GetY || UserInRoom.X == Bomb1.GetX + 2 && UserInRoom.Y == Bomb1.GetY || UserInRoom.X == Bomb1.GetX - 1 && UserInRoom.Y == Bomb1.GetY + 1 || UserInRoom.X == Bomb1.GetX && UserInRoom.Y == Bomb1.GetY + 1 || UserInRoom.X == Bomb1.GetX + 1 && UserInRoom.Y == Bomb1.GetY + 1 || UserInRoom.X == Bomb1.GetX && UserInRoom.Y == Bomb1.GetY + 2 || UserInRoom.X == Bomb1.GetX - 2 && UserInRoom.Y == Bomb1.GetY + 1 || UserInRoom.X == Bomb1.GetX - 2 && UserInRoom.Y == Bomb1.GetY + 2 || UserInRoom.X == Bomb1.GetX - 1 && UserInRoom.Y == Bomb1.GetY + 2 || UserInRoom.X == Bomb1.GetX - 2 && UserInRoom.Y == Bomb1.GetY - 1 || UserInRoom.X == Bomb1.GetX - 2 && UserInRoom.Y == Bomb1.GetY - 2 || UserInRoom.X == Bomb1.GetX - 1 && UserInRoom.Y == Bomb1.GetY - 2 || UserInRoom.X == Bomb1.GetX + 1 && UserInRoom.Y == Bomb1.GetY - 2 || UserInRoom.X == Bomb1.GetX + 2 && UserInRoom.Y == Bomb1.GetY - 2 || UserInRoom.X == Bomb1.GetX + 2 && UserInRoom.Y == Bomb1.GetY - 1 || UserInRoom.X == Bomb1.GetX + 2 && UserInRoom.Y == Bomb1.GetY + 1 || UserInRoom.X == Bomb1.GetX + 2 && UserInRoom.Y == Bomb1.GetY + 2 || UserInRoom.X == Bomb1.GetX + 1 && UserInRoom.Y == Bomb1.GetY + 2)
                                {
                                    UserInRoom.Say("takes 36 explosion damage", 0, true);
                                    UserInRoom.GetClient().GetRoleplay().Health -= 36;
                                    UserInRoom.GetClient().GetHabbo().updateHealth();

                                    if (UserInRoom.GetClient().GetRoleplay().Health == 0 || UserInRoom.GetClient().GetRoleplay().Health < 0)
                                    {
                                        if (UserInRoom.RotBody % 2 == 0)
                                        {
                                            if (UserInRoom != null)
                                            {
                                                try
                                                {
                                                    UserInRoom.Statusses.Add("lay", "1.0 null");
                                                    UserInRoom.Z -= 0.35;
                                                    UserInRoom.isLying = true;
                                                    UserInRoom.UpdateNeeded = true;
                                                }
                                                catch
                                                {
                                                }
                                            }
                                        }
                                        else
                                        {
                                            UserInRoom.RotBody--;
                                            UserInRoom.Statusses.Add("lay", "1.0 null");
                                            UserInRoom.Z -= 0.35;
                                            UserInRoom.isLying = true;
                                            UserInRoom.UpdateNeeded = true;
                                        }

                                        PlusEnvironment.GetGame().GetClientManager().ParamedicCall(UserInRoom.GetClient().GetHabbo().Username, UserInRoom.GetClient().GetHabbo().CurrentRoom.Name);
                                        UserInRoom.GetClient().GetHabbo().IsWaitingForParamedic = true;
                                        UserInRoom.GetClient().GetRoleplay().Health = 0;
                                        UserInRoom.GetClient().GetHabbo().updateHealth();
                                        UserInRoom.GetClient().GetRoleplay().Deaths += 1;
                                        UserInRoom.GetClient().GetHabbo().updateDeaths();
                                        UserInRoom.GetClient().GetHabbo().RPCache(1);

                                        System.Timers.Timer DieTimer = new System.Timers.Timer(PlusEnvironment.ConvertMinutesToMilliseconds(5));
                                        DieTimer.Interval = PlusEnvironment.ConvertMinutesToMilliseconds(5);
                                        DieTimer.Elapsed += delegate
                                        {
                                            if (UserInRoom.GetClient().GetHabbo().IsWaitingForParamedic)
                                            {
                                                UserInRoom.GetClient().GetHabbo().Hospital = 1;
                                                UserInRoom.GetClient().GetHabbo().updateHospitalEtat(UserInRoom, 3);
                                            }
                                            DieTimer.Stop();
                                        };
                                        DieTimer.Start();
                                    }
                                    else
                                    {
                                        UserInRoom.GetClient().GetHabbo().RPCache(1);
                                    }
                                }
                            }

                            System.Timers.Timer BombTimer = new System.Timers.Timer(500);
                            BombTimer.Interval = 500;
                            BombTimer.Elapsed += delegate
                            {
                                Client.GetHabbo().CurrentRoom.GetRoomItemHandler().RemoveFurniture(null, Bomb1.Id);
                                BombTimer.Stop();
                            };
                            BombTimer.Start();

                            BombStartTimer.Stop();
                        };
                        BombStartTimer.Start();

                    }
                    break;
                #endregion
                #region Suicidevest
                case "suicidevest":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().CheckWarnings())
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Slot = ReceivedData[1];

                        User.Say("pulls the pin on their suicide vest", 3, true);
                        User.ApplyEffect(25);
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");

                        #region Aggression Timer
                        Client.GetRoleplay().Aggression = 100;
                        Random TokenRand = new Random();
                        int tokenNumber = TokenRand.Next(1600, 2894354);
                        Client.GetHabbo().AggressionToken = tokenNumber;
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "aggression;" + Client.GetRoleplay().Aggression);

                        System.Timers.Timer AggressionTimer1 = new System.Timers.Timer(500);
                        AggressionTimer1.Interval = 500;
                        AggressionTimer1.Elapsed += delegate
                        {
                            if (Client.GetHabbo().AggressionToken == tokenNumber)
                            {
                                if (Client.GetRoleplay().Aggression < 0)
                                {
                                    Client.GetRoleplay().Aggression = 0;
                                    Client.GetHabbo().AggressionToken = 0;
                                    AggressionTimer1.Stop();
                                }
                                else
                                {
                                    Client.GetRoleplay().Aggression -= 1;
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "aggression;" + Client.GetRoleplay().Aggression);
                                    AggressionTimer1.Start();
                                }
                            }
                        };
                        AggressionTimer1.Start();
                        #endregion

                        System.Timers.Timer BombStartTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(5));
                        BombStartTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(5);
                        BombStartTimer.Elapsed += delegate
                        {
                            User.Say("suicide vest detonates, killing them instantly", 22, true);
                            Client.GetHabbo().resetEffectEvent();
                            Client.GetRoleplay().Health -= Client.GetRoleplay().Health;
                            Client.GetHabbo().updateHealth();
                            Client.GetHabbo().RPCache(1);
                            Client.GetHabbo().HPBarley();

                            /* Starts */

                                    Room CurrentRoom = Client.GetHabbo().CurrentRoom;

                            if (Room.RoomData.GroupId > 0)
                            {
                                foreach (RoomUser UserInRoom in CurrentRoom.GetRoomUserManager().GetUserList().ToList())
                                {
                                    if (UserInRoom == null || UserInRoom.IsBot || UserInRoom.GetClient() == null || UserInRoom.GetClient().GetHabbo() == null || UserInRoom.IsAsleep || UserInRoom.GetClient().GetHabbo().usingHeroinDrug || UserInRoom.GetClient().GetHabbo().IsWaitingForParamedic || UserInRoom.GetClient().GetHabbo().AggressionToken == 0)
                                        continue;

                                    CurrentRoom.GetRoomUserManager().UpdateUserStatus(UserInRoom, true);

                                    if (UserInRoom.X == User.X && UserInRoom.Y == User.Y || UserInRoom.X == User.X && UserInRoom.Y == User.Y - 1 || UserInRoom.X == User.X && UserInRoom.Y == User.Y - 2 || UserInRoom.X == User.X - 1 && UserInRoom.Y == User.Y - 1 || UserInRoom.X == User.X + 1 && UserInRoom.Y == User.Y - 1 || UserInRoom.X == User.X - 1 && UserInRoom.Y == User.Y || UserInRoom.X == User.X - 2 && UserInRoom.Y == User.Y || UserInRoom.X == User.X + 1 && UserInRoom.Y == User.Y || UserInRoom.X == User.X + 2 && UserInRoom.Y == User.Y || UserInRoom.X == User.X - 1 && UserInRoom.Y == User.Y + 1 || UserInRoom.X == User.X && UserInRoom.Y == User.Y + 1 || UserInRoom.X == User.X + 1 && UserInRoom.Y == User.Y + 1 || UserInRoom.X == User.X && UserInRoom.Y == User.Y + 2 || UserInRoom.X == User.X - 2 && UserInRoom.Y == User.Y + 1 || UserInRoom.X == User.X - 2 && UserInRoom.Y == User.Y + 2 || UserInRoom.X == User.X - 1 && UserInRoom.Y == User.Y + 2 || UserInRoom.X == User.X - 2 && UserInRoom.Y == User.Y - 1 || UserInRoom.X == User.X - 2 && UserInRoom.Y == User.Y - 2 || UserInRoom.X == User.X - 1 && UserInRoom.Y == User.Y - 2 || UserInRoom.X == User.X + 1 && UserInRoom.Y == User.Y - 2 || UserInRoom.X == User.X + 2 && UserInRoom.Y == User.Y - 2 || UserInRoom.X == User.X + 2 && UserInRoom.Y == User.Y - 1 || UserInRoom.X == User.X + 2 && UserInRoom.Y == User.Y + 1 || UserInRoom.X == User.X + 2 && UserInRoom.Y == User.Y + 2 || UserInRoom.X == User.X + 1 && UserInRoom.Y == User.Y + 2)
                                    {
                                        if (UserInRoom.GetClient().GetHabbo().Id != Client.GetHabbo().Id)
                                        {
                                            UserInRoom.Say("takes 117 explosion damage", 3, true);
                                            UserInRoom.GetClient().GetRoleplay().Health -= 117;
                                            UserInRoom.GetClient().GetHabbo().updateHealth();

                                            if (UserInRoom.GetClient().GetRoleplay().Health == 0 || UserInRoom.GetClient().GetRoleplay().Health < 0)
                                            {
                                                UserInRoom.Freezed = true;
                                                if (UserInRoom.RotBody % 2 == 0)
                                                {
                                                    if (UserInRoom != null)
                                                    {
                                                        try
                                                        {
                                                            UserInRoom.Statusses.Add("lay", "1.0 null");
                                                            UserInRoom.Z -= 0.35;
                                                            UserInRoom.isLying = true;
                                                            UserInRoom.UpdateNeeded = true;
                                                        }
                                                        catch
                                                        {
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    UserInRoom.RotBody--;
                                                    UserInRoom.Statusses.Add("lay", "1.0 null");
                                                    UserInRoom.Z -= 0.35;
                                                    UserInRoom.isLying = true;
                                                    UserInRoom.UpdateNeeded = true;
                                                }

                                                PlusEnvironment.GetGame().GetClientManager().ParamedicCall(UserInRoom.GetClient().GetHabbo().Username, UserInRoom.GetClient().GetHabbo().CurrentRoom.Name);
                                                UserInRoom.GetClient().GetHabbo().IsWaitingForParamedic = true;
                                                UserInRoom.GetClient().GetRoleplay().Health = 0;
                                                UserInRoom.GetClient().GetHabbo().updateHealth();
                                                UserInRoom.GetClient().GetRoleplay().Deaths += 1;
                                                UserInRoom.GetClient().GetHabbo().updateDeaths();
                                                UserInRoom.GetClient().GetHabbo().RPCache(1);
                                                Client.GetRoleplay().Kills += 1;
                                                Client.GetHabbo().updateKills();

                                                int Credits = 0;
                                                if (UserInRoom.GetClient().GetHabbo().Rank < 3 && UserInRoom.GetClient().GetHabbo().Hospital == 0)
                                                {
                                                    decimal decimalCredits = Convert.ToInt32(UserInRoom.GetClient().GetHabbo().Credits);
                                                    Credits = Convert.ToInt32((decimalCredits / 100m) * 50m);
                                                }
                                                if (Credits > 0)
                                                {
                                                    UserInRoom.GetClient().GetHabbo().Credits -= Credits;
                                                    UserInRoom.GetClient().SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                                                    UserInRoom.GetClient().GetHabbo().RPCache(3);
                                                    Client.GetHabbo().Credits += Credits;
                                                    Client.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                                                    Client.GetHabbo().RPCache(3);
                                                    User.Say("ª lands the final blow on " + UserInRoom.GetClient().GetHabbo().Username + " and steals " + PlusEnvironment.ConvertToPrice(Credits) + " dollars", 0, true);
                                                    PlusEnvironment.GetGame().GetClientManager().LiveFeed("<span class=\"blue\">" + Client.GetHabbo().Username + "</span> knocked out <span class=\"red\">" + UserInRoom.GetClient().GetHabbo().Username + "</span>, stealing $" + Credits);
                                                }
                                                else
                                                {
                                                    User.Say("ª lands the final blow on " + UserInRoom.GetClient().GetHabbo().Username, 0, true);
                                                    PlusEnvironment.GetGame().GetClientManager().LiveFeed("<span class=\"blue\">" + Client.GetHabbo().Username + "</span> knocked out <span class=\"red\">" + UserInRoom.GetClient().GetHabbo().Username + "</span>");
                                                }

                                                System.Timers.Timer DieTimer = new System.Timers.Timer(PlusEnvironment.ConvertMinutesToMilliseconds(5));
                                                DieTimer.Interval = PlusEnvironment.ConvertMinutesToMilliseconds(5);
                                                DieTimer.Elapsed += delegate
                                                {
                                                    if (UserInRoom.GetClient().GetHabbo().IsWaitingForParamedic)
                                                    {
                                                        UserInRoom.GetClient().GetHabbo().Hospital = 1;
                                                        UserInRoom.GetClient().GetHabbo().updateHospitalEtat(UserInRoom, 3);
                                                    }
                                                    DieTimer.Stop();
                                                };
                                                DieTimer.Start();
                                            }
                                            else
                                            {
                                                if (UserInRoom.GetClient().GetRoleplay().Health < 20)
                                                {
                                                    UserInRoom.Say("barely manages to keep upright", 0, true);
                                                }

                                                UserInRoom.GetClient().GetHabbo().RPCache(1);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (RoomUser UserInRoom in CurrentRoom.GetRoomUserManager().GetUserList().ToList())
                                {
                                    if (UserInRoom == null || UserInRoom.IsBot || UserInRoom.GetClient() == null || UserInRoom.GetClient().GetHabbo() == null || UserInRoom.IsAsleep || UserInRoom.GetClient().GetHabbo().usingHeroinDrug || UserInRoom.GetClient().GetHabbo().IsWaitingForParamedic)
                                        continue;

                                    CurrentRoom.GetRoomUserManager().UpdateUserStatus(UserInRoom, true);

                                    if (UserInRoom.X == User.X && UserInRoom.Y == User.Y || UserInRoom.X == User.X && UserInRoom.Y == User.Y - 1 || UserInRoom.X == User.X && UserInRoom.Y == User.Y - 2 || UserInRoom.X == User.X - 1 && UserInRoom.Y == User.Y - 1 || UserInRoom.X == User.X + 1 && UserInRoom.Y == User.Y - 1 || UserInRoom.X == User.X - 1 && UserInRoom.Y == User.Y || UserInRoom.X == User.X - 2 && UserInRoom.Y == User.Y || UserInRoom.X == User.X + 1 && UserInRoom.Y == User.Y || UserInRoom.X == User.X + 2 && UserInRoom.Y == User.Y || UserInRoom.X == User.X - 1 && UserInRoom.Y == User.Y + 1 || UserInRoom.X == User.X && UserInRoom.Y == User.Y + 1 || UserInRoom.X == User.X + 1 && UserInRoom.Y == User.Y + 1 || UserInRoom.X == User.X && UserInRoom.Y == User.Y + 2 || UserInRoom.X == User.X - 2 && UserInRoom.Y == User.Y + 1 || UserInRoom.X == User.X - 2 && UserInRoom.Y == User.Y + 2 || UserInRoom.X == User.X - 1 && UserInRoom.Y == User.Y + 2 || UserInRoom.X == User.X - 2 && UserInRoom.Y == User.Y - 1 || UserInRoom.X == User.X - 2 && UserInRoom.Y == User.Y - 2 || UserInRoom.X == User.X - 1 && UserInRoom.Y == User.Y - 2 || UserInRoom.X == User.X + 1 && UserInRoom.Y == User.Y - 2 || UserInRoom.X == User.X + 2 && UserInRoom.Y == User.Y - 2 || UserInRoom.X == User.X + 2 && UserInRoom.Y == User.Y - 1 || UserInRoom.X == User.X + 2 && UserInRoom.Y == User.Y + 1 || UserInRoom.X == User.X + 2 && UserInRoom.Y == User.Y + 2 || UserInRoom.X == User.X + 1 && UserInRoom.Y == User.Y + 2)
                                    {
                                        if (UserInRoom.GetClient().GetHabbo().Id != Client.GetHabbo().Id)
                                        {
                                            UserInRoom.Say("takes 117 explosion damage", 3, true);
                                            UserInRoom.GetClient().GetRoleplay().Health -= 117;
                                            UserInRoom.GetClient().GetHabbo().updateHealth();

                                            if (UserInRoom.GetClient().GetRoleplay().Health == 0 || UserInRoom.GetClient().GetRoleplay().Health < 0)
                                            {
                                                UserInRoom.Freezed = true;
                                                if (UserInRoom.RotBody % 2 == 0)
                                                {
                                                    if (UserInRoom != null)
                                                    {
                                                        try
                                                        {
                                                            UserInRoom.Statusses.Add("lay", "1.0 null");
                                                            UserInRoom.Z -= 0.35;
                                                            UserInRoom.isLying = true;
                                                            UserInRoom.UpdateNeeded = true;
                                                        }
                                                        catch
                                                        {
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    UserInRoom.RotBody--;
                                                    UserInRoom.Statusses.Add("lay", "1.0 null");
                                                    UserInRoom.Z -= 0.35;
                                                    UserInRoom.isLying = true;
                                                    UserInRoom.UpdateNeeded = true;
                                                }

                                                PlusEnvironment.GetGame().GetClientManager().ParamedicCall(UserInRoom.GetClient().GetHabbo().Username, UserInRoom.GetClient().GetHabbo().CurrentRoom.Name);
                                                UserInRoom.GetClient().GetHabbo().IsWaitingForParamedic = true;
                                                UserInRoom.GetClient().GetRoleplay().Health = 0;
                                                UserInRoom.GetClient().GetHabbo().updateHealth();
                                                UserInRoom.GetClient().GetRoleplay().Deaths += 1;
                                                UserInRoom.GetClient().GetHabbo().updateDeaths();
                                                UserInRoom.GetClient().GetHabbo().RPCache(1);
                                                Client.GetRoleplay().Kills += 1;
                                                Client.GetHabbo().updateKills();

                                                int Credits = 0;
                                                if (UserInRoom.GetClient().GetHabbo().Rank < 3 && UserInRoom.GetClient().GetHabbo().Hospital == 0)
                                                {
                                                    decimal decimalCredits = Convert.ToInt32(UserInRoom.GetClient().GetHabbo().Credits);
                                                    Credits = Convert.ToInt32((decimalCredits / 100m) * 50m);
                                                }
                                                if (Credits > 0)
                                                {
                                                    UserInRoom.GetClient().GetHabbo().Credits -= Credits;
                                                    UserInRoom.GetClient().SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                                                    UserInRoom.GetClient().GetHabbo().RPCache(3);
                                                    Client.GetHabbo().Credits += Credits;
                                                    Client.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                                                    Client.GetHabbo().RPCache(3);
                                                    User.Say("ª lands the final blow on " + UserInRoom.GetClient().GetHabbo().Username + " and steals " + PlusEnvironment.ConvertToPrice(Credits) + " dollars", 0, true);
                                                    PlusEnvironment.GetGame().GetClientManager().LiveFeed("<span class=\"blue\">" + Client.GetHabbo().Username + "</span> knocked out <span class=\"red\">" + UserInRoom.GetClient().GetHabbo().Username + "</span>, stealing $" + Credits);
                                                }
                                                else
                                                {
                                                    User.Say("ª lands the final blow on " + UserInRoom.GetClient().GetHabbo().Username, 0, true);
                                                    PlusEnvironment.GetGame().GetClientManager().LiveFeed("<span class=\"blue\">" + Client.GetHabbo().Username + "</span> knocked out <span class=\"red\">" + UserInRoom.GetClient().GetHabbo().Username + "</span>");
                                                }

                                                System.Timers.Timer DieTimer = new System.Timers.Timer(PlusEnvironment.ConvertMinutesToMilliseconds(5));
                                                DieTimer.Interval = PlusEnvironment.ConvertMinutesToMilliseconds(5);
                                                DieTimer.Elapsed += delegate
                                                {
                                                    if (UserInRoom.GetClient().GetHabbo().IsWaitingForParamedic)
                                                    {
                                                        UserInRoom.GetClient().GetHabbo().Hospital = 1;
                                                        UserInRoom.GetClient().GetHabbo().updateHospitalEtat(UserInRoom, 3);
                                                    }
                                                    DieTimer.Stop();
                                                };
                                                DieTimer.Start();
                                            }
                                            else
                                            {
                                                if (UserInRoom.GetClient().GetRoleplay().Health < 20)
                                                {
                                                    UserInRoom.Say("barely manages to keep upright", 0, true);
                                                }

                                                UserInRoom.GetClient().GetHabbo().RPCache(1);
                                            }
                                        }
                                    }
                                }
                            }
                            BombStartTimer.Stop();
                        };
                        BombStartTimer.Start();
                    }
                    break;
                #endregion
                #region Lockpick
                case "lockpick":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Slot = ReceivedData[1];

                        if (!Client.GetHabbo().CheckInventory("lockpick"))
                        {
                            Client.SendWhisper("You need a lockpick to perform this action");
                            return;
                        }

                        if (Client.GetRoleplay().TargetId == 0)
                        {
                            Client.SendWhisper("Please specify a target");
                            return;
                        }

                        GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(Client.GetRoleplay().TargetId);
                        if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Client.GetHabbo().CurrentRoom)
                        {
                            Client.SendWhisper("Player not found in this room");
                            return;
                        }

                        RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetRoleplay().TargetId);

                        if (!TargetClient.GetHabbo().Cuffed)
                        {
                            Client.SendWhisper("You cannot perform this action to a player where isn't cuffed");
                            return;
                        }

                        if (Math.Abs(TargetUser.X - User.X) > 1 || Math.Abs(TargetUser.Y - User.Y) > 1)
                        {
                            Client.SendWhisper(TargetClient.GetHabbo().Username + " are too far away");
                            return;
                        }

                        Random TokenRand = new Random();
                        int tokenNumber = TokenRand.Next(1600, 2894354);
                        Client.GetHabbo().PlayToken = tokenNumber;

                        User.Say("attempts to lockpick " + TargetClient.GetHabbo().Username + "'s cuffs");
                        User.ApplyEffect(4);
                        Client.GetHabbo().LockpickingTo = TargetClient.GetHabbo().Username;
                        TargetClient.GetHabbo().LockpickingFrom = Client.GetHabbo().Username;

                        System.Timers.Timer Timer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(20));
                        Timer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(20);
                        Timer.Elapsed += delegate
                        {
                            if (Client.GetHabbo().LockpickingTo == TargetClient.GetHabbo().Username && TargetClient.GetHabbo().LockpickingFrom == Client.GetHabbo().Username && Client.GetHabbo().PlayToken == tokenNumber)
                            {
                                Client.GetHabbo().LockpickingTo = null;
                                TargetClient.GetHabbo().LockpickingFrom = null;
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-," + 1);
                                Client.GetHabbo().PlayToken = 0;
                                Client.GetHabbo().resetEffectEvent();

                                Random rand = new Random();
                                int Lockpick = rand.Next(1, 101);

                                if (Lockpick <= 40)
                                {
                                    User.Say("successfully lockpicks " + TargetClient.GetHabbo().Username + " from their cuffs");
                                    if (TargetClient.GetHabbo().EscortBy != null)
                                    {
                                        GameClient EscortBy = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(TargetClient.GetHabbo().EscortBy);

                                        TargetClient.GetHabbo().Escort = false;
                                        TargetClient.GetHabbo().EscortBy = null;
                                        EscortBy.GetHabbo().Escorting = false;
                                        EscortBy.GetHabbo().EscortUsername = null;
                                    }

                                    TargetUser.UltraFastWalking = false;
                                    TargetClient.GetHabbo().Cuffed = false;
                                    TargetClient.GetHabbo().resetAvatarEvent();
                                }
                                else if (Lockpick > 40)
                                {
                                    User.Say("fails to lockpick " + TargetClient.GetHabbo().Username + " cuffs");
                                }

                            }
                            Timer.Stop();
                        };
                        Timer.Start();
                    }
                break;
                #endregion
            }
        }
    }
}
