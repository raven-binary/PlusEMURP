using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plus;
using Fleck;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Rooms.Notifications;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.Items;
using System.Data;
using Plus.Communication.Packets.Outgoing.Rooms.Session;

namespace Bobba.HabboRoleplay.Web.Outgoing
{
    class TransactionWebEvent : IWebEvent
    {
        /// <summary>
        /// Executes socket data.
        /// </summary>
        /// <param name="Client"></param>
        /// <param name="Data"></param>
        /// <param name="Socket"></param>
        public void Execute(GameClient Client, string Data, IWebSocketConnection Socket)
        {

            if (!PlusEnvironment.GetGame().GetWebEventManager().SocketReady(Client, true) || !PlusEnvironment.GetGame().GetWebEventManager().SocketReady(Socket))
                return;


            string Action = (Data.Contains(',') ? Data.Split(',')[0] : Data);

            switch (Action)
            {
                #region Accept
                case "accept":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (User.Transaction == null)
                        {
                            Client.SendWhisper("You have no ongoing transactions");
                            return;
                        }

                        GameClient TransactionClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Client.GetHabbo().TransactionFrom);
                        if (TransactionClient == null)
                            return;

                        RoomUser TransactionUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TransactionClient.GetHabbo().Id);
                        if (TransactionUser == null)
                            return;

                        #region gang
                        if (User.Transaction.StartsWith("gang"))
                        {
                            if (Client.GetHabbo().Gang == 0)
                            {
                                string[] gangId = User.Transaction.Split(':');
                                Client.GetHabbo().Gang = Convert.ToInt32(gangId[1]);
                                Client.GetHabbo().updateGang();
                                Client.GetHabbo().GangRank = 1;
                                Client.GetHabbo().updateGangRank();
                                User.Say("accepts the gang invite " + Client.GetHabbo().getNameOfGang());
                            }
                        }
                        #endregion
                        #region fixed 1v1
                        if (User.Transaction.StartsWith("1v1"))
                        {

                            string[] duelData = User.Transaction.Split(':');
                            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(duelData[1]);
                            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Client.GetHabbo().CurrentRoom)
                            {
                                Client.SendWhisper(duelData[1] + " could not be found in " + Client.GetHabbo().CurrentRoom.Name + " room");
                            }
                            else
                            {
                                RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
                                if (TargetUser.DuelUser != null)
                                {
                                    Client.SendWhisper(duelData[1] + " is already in a 1v1");
                                }
                                else
                                {
                                    if (!PlusEnvironment.ArenaUsing && PlusEnvironment.ArenaQueue == 0 && PlusEnvironment.ArenaTimer == 0)
                                    {

                                        PlusEnvironment.ArenaTimer = 5;
                                        Random TokenRand = new Random();
                                        int tokenNumber = TokenRand.Next(1600, 2894354);
                                        string Token = "DUEL-" + tokenNumber;

                                        TargetUser.DuelUser = Client.GetHabbo().Username;
                                        User.DuelUser = TargetClient.GetHabbo().Username;
                                        User.DuelToken = Token;
                                        User.Say("accepts the challenge from " + TargetClient.GetHabbo().Username + " to 1v1");
                                        RoomUser User2 = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
                                        User.CanWalk = false;
                                        User2.CanWalk = false;
                                        PlusEnvironment.ArenaUsing = true;
                                        Client.GetHabbo().updateAvatarEvent("ch-665-96.hd-787593-1370.hr-100-31.lg-3116-1194-92", "ch-3203-1194.hd-180-1.hr-100-31.lg-3116-1194-92", "[Arena 1v1]");
                                        TargetClient.GetHabbo().updateAvatarEvent("ch-665-96.hd-787593-1370.hr-100-31.lg-3116-1194-92", "ch-3203-1194.hd-180-1.hr-100-31.lg-3116-1194-92", "[Arena 1v1]");

                                        foreach (Item item in Room.GetRoomItemHandler().GetFloor)
                                        {
                                            if (item.Id == 74914)
                                            {
                                                Room.GetGameMap().TeleportToItem(User, item);
                                                Room.GetRoomUserManager().UpdateUserStatusses();
                                                User.SetPos(item.GetX, item.GetY, item.GetZ);
                                                User.SetRot(item.Rotation, false);
                                                if (!User.Statusses.ContainsKey("sit"))
                                                {
                                                    if ((User.RotBody % 2) == 0)
                                                    {
                                                        if (User == null)
                                                            return;

                                                        try
                                                        {
                                                            User.Statusses.Add("sit", "1.0");
                                                            User.Z -= 0.35;
                                                            User.isSitting = true;
                                                            User.UpdateNeeded = true;
                                                        }
                                                        catch { }
                                                    }
                                                    else
                                                    {
                                                        User.RotBody--;
                                                        User.Statusses.Add("sit", "1.0");
                                                        User.Z -= 0.35;
                                                        User.isSitting = true;
                                                        User.UpdateNeeded = true;
                                                    }
                                                }
                                                else if (User.isSitting == true)
                                                {
                                                    User.Z += 0.35;
                                                    User.Statusses.Remove("sit");
                                                    User.Statusses.Remove("1.0");
                                                    User.isSitting = false;
                                                    User.UpdateNeeded = true;
                                                }
                                            }

                                            if (item.Id == 74913)
                                            {
                                                Room.GetGameMap().TeleportToItem(User2, item);
                                                Room.GetRoomUserManager().UpdateUserStatusses();
                                                User2.SetPos(item.GetX, item.GetY, item.GetZ);
                                                User2.SetRot(item.Rotation, false);
                                                if (!User2.Statusses.ContainsKey("sit"))
                                                {
                                                    if ((User2.RotBody % 2) == 0)
                                                    {
                                                        if (User2 == null)
                                                            return;

                                                        try
                                                        {
                                                            User2.Statusses.Add("sit", "1.0");
                                                            User2.Z -= 0.35;
                                                            User2.isSitting = true;
                                                            User2.UpdateNeeded = true;
                                                        }
                                                        catch { }
                                                    }
                                                    else
                                                    {
                                                        User2.RotBody--;
                                                        User2.Statusses.Add("sit", "1.0");
                                                        User2.Z -= 0.35;
                                                        User2.isSitting = true;
                                                        User2.UpdateNeeded = true;
                                                    }
                                                }
                                                else if (User2.isSitting == true)
                                                {
                                                    User2.Z += 0.35;
                                                    User2.Statusses.Remove("sit");
                                                    User2.Statusses.Remove("1.0");
                                                    User2.isSitting = false;
                                                    User2.UpdateNeeded = true;
                                                }
                                            }
                                        }
                                        TargetClient.SendWhisper("Get ready! Match starting in 5 seconds");
                                        Client.SendWhisper("Get ready! Match starting in 5 seconds");
                                        TargetClient.SendWhisper("The match will automatically end in 5 minutes");
                                        Client.SendWhisper("The match will automatically end in 5 minutes");
                                        Client.SendWhisper("You have joined the arena queue to fight against " + TargetClient.GetHabbo().Username);
                                        TargetClient.SendWhisper("You have joined the arena queue to fight against " + Client.GetHabbo().Username);
                                        TargetClient.GetHabbo().usingArena = true;
                                        Client.GetHabbo().usingArena = true;
                                        Client.GetHabbo().ArenaHealth = 100;
                                        Client.GetHabbo().ArenaHealthMax = 100;
                                        Client.GetHabbo().ArenaEnergy = 100;
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "user-stats;" + Client.GetHabbo().Id + ";" + Client.GetHabbo().Username + ";" + Client.GetHabbo().Look + ";" + Client.GetRoleplay().Passive + ";" + Client.GetHabbo().ArenaHealth + ";" + Client.GetHabbo().ArenaHealthMax + ";" + Client.GetHabbo().ArenaEnergy);
                                        TargetClient.GetHabbo().ArenaHealth = 100;
                                        TargetClient.GetHabbo().ArenaHealthMax = 100;
                                        TargetClient.GetHabbo().ArenaEnergy = 100;
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "user-stats;" + TargetClient.GetHabbo().Id + ";" + TargetClient.GetHabbo().Username + ";" + TargetClient.GetHabbo().Look + ";" + TargetClient.GetRoleplay().Passive + ";" + TargetClient.GetHabbo().ArenaHealth + ";" + TargetClient.GetHabbo().ArenaHealthMax + ";" + TargetClient.GetHabbo().ArenaEnergy);

                                        System.Timers.Timer in5 = new System.Timers.Timer(1000);
                                        in5.Interval = 1000;
                                        in5.Elapsed += delegate
                                        {
                                            TargetClient.SendWhisper("5...");
                                            Client.SendWhisper("5...");
                                            in5.Stop();
                                        };
                                        in5.Start();
                                        System.Timers.Timer in4 = new System.Timers.Timer(2000);
                                        in4.Interval = 2000;
                                        in4.Elapsed += delegate
                                        {
                                            TargetClient.SendWhisper("4...");
                                            Client.SendWhisper("4...");
                                            in4.Stop();
                                        };
                                        in4.Start();
                                        System.Timers.Timer in3 = new System.Timers.Timer(3000);
                                        in3.Interval = 3000;
                                        in3.Elapsed += delegate
                                        {
                                            TargetClient.SendWhisper("3...");
                                            Client.SendWhisper("3...");
                                            in3.Stop();
                                        };
                                        in3.Start();
                                        System.Timers.Timer in2 = new System.Timers.Timer(4000);
                                        in2.Interval = 4000;
                                        in2.Elapsed += delegate
                                        {
                                            TargetClient.SendWhisper("2...");
                                            Client.SendWhisper("2...");
                                            in2.Stop();
                                        };
                                        in2.Start();
                                        System.Timers.Timer in1 = new System.Timers.Timer(5000);
                                        in1.Interval = 5000;
                                        in1.Elapsed += delegate
                                        {
                                            TargetClient.SendWhisper("1...");
                                            Client.SendWhisper("1...");
                                            in1.Stop();
                                        };
                                        in1.Start();
                                        System.Timers.Timer in0 = new System.Timers.Timer(6000);
                                        in0.Interval = 6000;
                                        in0.Elapsed += delegate
                                        {
                                            TargetClient.SendWhisper("FIGHT!");
                                            Client.SendWhisper("FIGHT!");
                                            User.CanWalk = true;
                                            User2.CanWalk = true;
                                            User.ApplyEffect(705);
                                            User2.ApplyEffect(705);
                                            in0.Stop();
                                        };
                                        in0.Start();

                                        if (Client.GetHabbo().ArmeEquiped == "cocktail")
                                        {
                                            Client.GetHabbo().ArmeEquiped = null;
                                            Client.GetHabbo().resetEffectEvent();
                                        }

                                        if (TargetClient.GetHabbo().ArmeEquiped == "cocktail")
                                        {
                                            TargetClient.GetHabbo().ArmeEquiped = null;
                                            TargetClient.GetHabbo().resetEffectEvent();
                                        }

                                        System.Timers.Timer timer1 = new System.Timers.Timer(300000);
                                        timer1.Interval = 300000;
                                        timer1.Elapsed += delegate
                                        {
                                            if (User.DuelUser == User2.DuelUser && User.DuelToken == "DUEL-" + Token)
                                            {
                                                foreach (Item item in Room.GetRoomItemHandler().GetFloor)
                                                {
                                                    if (item.GetBaseItem().SpriteId == 2536)
                                                    {
                                                        Room.GetGameMap().TeleportToItem(User, item);
                                                        Room.GetGameMap().TeleportToItem(TargetUser, item);
                                                        Room.GetRoomUserManager().UpdateUserStatusses();
                                                        Client.GetHabbo().usingArena = false;
                                                        TargetClient.GetHabbo().usingArena = false;
                                                        User.DuelToken = null;
                                                        TargetUser.DuelToken = null;
                                                        User.DuelUser = null;
                                                        TargetUser.DuelUser = null;
                                                        Client.GetHabbo().resetEffectEvent();
                                                        Client.GetHabbo().resetAvatarEvent();
                                                        TargetClient.GetHabbo().resetAvatarEvent();
                                                        TargetClient.GetHabbo().resetEffectEvent();
                                                        PlusEnvironment.ArenaUsing = false;
                                                    }
                                                }
                                            }
                                            timer1.Stop();
                                        };
                                        timer1.Start();
                                    }
                                    else
                                    {
                                        PlusEnvironment.ArenaQueue += 1;
                                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                        {
                                            dbClient.SetQuery("INSERT INTO `arenas` (`id`,`user1`,`user2`) VALUES (@Id, @user1, @user2)");
                                            dbClient.AddParameter("Id", PlusEnvironment.ArenaQueue);
                                            dbClient.AddParameter("user1", Client.GetHabbo().Username);
                                            dbClient.AddParameter("user2", TargetClient.GetHabbo().Username);
                                            dbClient.RunQuery();
                                        }

                                        Client.SendWhisper("Your arena battle with [" + Convert.ToString(TargetClient.GetHabbo().Username) + "] is in queue", 6);
                                    }
                                }
                            }

                        }
                        #endregion
                        #region 1v1
                        if (User.Transaction.StartsWith("duel"))
                        {
                            
                            string[] duelData = User.Transaction.Split(':');
                            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(duelData[1]);
                            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Client.GetHabbo().CurrentRoom)
                            {
                                Client.SendWhisper(duelData[1] + " could not be found in " + Client.GetHabbo().CurrentRoom.Name + " room");
                            }
                            else
                            {
                                RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
                                if (TargetUser.DuelUser != null)
                                {
                                    Client.SendWhisper(duelData[1] + " is already in a 1v1");
                                }
                                else
                                {
                                    Random TokenRand = new Random();
                                    int tokenNumber = TokenRand.Next(1600, 2894354);
                                    string Token = "DUEL-" + tokenNumber;

                                    TargetUser.DuelUser = Client.GetHabbo().Username;
                                    User.DuelUser = TargetClient.GetHabbo().Username;
                                    User.DuelToken = Token;
                                    User.Say("accepts the challenge from " + TargetClient.GetHabbo().Username + " to 1v1");
                                    RoomUser User2 = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
                                    User.CanWalk = false;
                                    User2.CanWalk = false;
                                    PlusEnvironment.ArenaUsing = true;
                                    Client.GetHabbo().updateAvatarEvent("ch-665-96.hd-787593-1370.hr-100-31.lg-3116-1194-92", "ch-3203-1194.hd-180-1.hr-100-31.lg-3116-1194-92", "[Arena 1v1]");
                                    TargetClient.GetHabbo().updateAvatarEvent("ch-665-96.hd-787593-1370.hr-100-31.lg-3116-1194-92", "ch-3203-1194.hd-180-1.hr-100-31.lg-3116-1194-92", "[Arena 1v1]");

                                    foreach (Item item in Room.GetRoomItemHandler().GetFloor)
                                    {
                                        if (item.Id == 74914)
                                        {
                                            Room.GetGameMap().TeleportToItem(User, item);
                                            Room.GetRoomUserManager().UpdateUserStatusses();
                                            User.SetPos(item.GetX, item.GetY, item.GetZ);
                                            User.SetRot(item.Rotation, false);
                                            if (!User.Statusses.ContainsKey("sit"))
                                            {
                                                if ((User.RotBody % 2) == 0)
                                                {
                                                    if (User == null)
                                                        return;

                                                    try
                                                    {
                                                        User.Statusses.Add("sit", "1.0");
                                                        User.Z -= 0.35;
                                                        User.isSitting = true;
                                                        User.UpdateNeeded = true;
                                                    }
                                                    catch { }
                                                }
                                                else
                                                {
                                                    User.RotBody--;
                                                    User.Statusses.Add("sit", "1.0");
                                                    User.Z -= 0.35;
                                                    User.isSitting = true;
                                                    User.UpdateNeeded = true;
                                                }
                                            }
                                            else if (User.isSitting == true)
                                            {
                                                User.Z += 0.35;
                                                User.Statusses.Remove("sit");
                                                User.Statusses.Remove("1.0");
                                                User.isSitting = false;
                                                User.UpdateNeeded = true;
                                            }
                                        }

                                        if (item.Id == 74913)
                                        {
                                            Room.GetGameMap().TeleportToItem(User2, item);
                                            Room.GetRoomUserManager().UpdateUserStatusses();
                                            User2.SetPos(item.GetX, item.GetY, item.GetZ);
                                            User2.SetRot(item.Rotation, false);
                                            if (!User2.Statusses.ContainsKey("sit"))
                                            {
                                                if ((User2.RotBody % 2) == 0)
                                                {
                                                    if (User2 == null)
                                                        return;

                                                    try
                                                    {
                                                        User2.Statusses.Add("sit", "1.0");
                                                        User2.Z -= 0.35;
                                                        User2.isSitting = true;
                                                        User2.UpdateNeeded = true;
                                                    }
                                                    catch { }
                                                }
                                                else
                                                {
                                                    User2.RotBody--;
                                                    User2.Statusses.Add("sit", "1.0");
                                                    User2.Z -= 0.35;
                                                    User2.isSitting = true;
                                                    User2.UpdateNeeded = true;
                                                }
                                            }
                                            else if (User2.isSitting == true)
                                            {
                                                User2.Z += 0.35;
                                                User2.Statusses.Remove("sit");
                                                User2.Statusses.Remove("1.0");
                                                User2.isSitting = false;
                                                User2.UpdateNeeded = true;
                                            }
                                        }
                                    }
                                    TargetClient.SendWhisper("Get ready! Match starting in 5 seconds");
                                    Client.SendWhisper("Get ready! Match starting in 5 seconds");
                                    TargetClient.SendWhisper("The match will automatically end in 5 minutes");
                                    Client.SendWhisper("The match will automatically end in 5 minutes");
                                    Client.SendWhisper("You have joined the arena queue to fight against " + TargetClient.GetHabbo().Username);
                                    TargetClient.SendWhisper("You have joined the arena queue to fight against " + Client.GetHabbo().Username);
                                    TargetClient.GetHabbo().usingArena = true;
                                    Client.GetHabbo().usingArena = true;
                                    Client.GetHabbo().ArenaHealth = 100;
                                    Client.GetHabbo().ArenaHealthMax = 100;
                                    Client.GetHabbo().ArenaEnergy = 100;
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "user-stats;" + Client.GetHabbo().Id + ";" + Client.GetHabbo().Username + ";" + Client.GetHabbo().Look + ";" + Client.GetRoleplay().Passive + ";" + Client.GetHabbo().ArenaHealth + ";" + Client.GetHabbo().ArenaHealthMax + ";" + Client.GetHabbo().ArenaEnergy);
                                    TargetClient.GetHabbo().ArenaHealth = 100;
                                    TargetClient.GetHabbo().ArenaHealthMax = 100;
                                    TargetClient.GetHabbo().ArenaEnergy = 100;
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "user-stats;" + TargetClient.GetHabbo().Id + ";" + TargetClient.GetHabbo().Username + ";" + TargetClient.GetHabbo().Look + ";" + TargetClient.GetRoleplay().Passive + ";" + TargetClient.GetHabbo().ArenaHealth + ";" + TargetClient.GetHabbo().ArenaHealthMax + ";" + TargetClient.GetHabbo().ArenaEnergy);

                                    System.Timers.Timer in5 = new System.Timers.Timer(1000);
                                    in5.Interval = 1000;
                                    in5.Elapsed += delegate
                                    {
                                        TargetClient.SendWhisper("5...");
                                        Client.SendWhisper("5...");
                                        in5.Stop();
                                    };
                                    in5.Start();
                                    System.Timers.Timer in4 = new System.Timers.Timer(2000);
                                    in4.Interval = 2000;
                                    in4.Elapsed += delegate
                                    {
                                        TargetClient.SendWhisper("4...");
                                        Client.SendWhisper("4...");
                                        in4.Stop();
                                    };
                                    in4.Start();
                                    System.Timers.Timer in3 = new System.Timers.Timer(3000);
                                    in3.Interval = 3000;
                                    in3.Elapsed += delegate
                                    {
                                        TargetClient.SendWhisper("3...");
                                        Client.SendWhisper("3...");
                                        in3.Stop();
                                    };
                                    in3.Start();
                                    System.Timers.Timer in2 = new System.Timers.Timer(4000);
                                    in2.Interval = 4000;
                                    in2.Elapsed += delegate
                                    {
                                        TargetClient.SendWhisper("2...");
                                        Client.SendWhisper("2...");
                                        in2.Stop();
                                    };
                                    in2.Start();
                                    System.Timers.Timer in1 = new System.Timers.Timer(5000);
                                    in1.Interval = 5000;
                                    in1.Elapsed += delegate
                                    {
                                        TargetClient.SendWhisper("1...");
                                        Client.SendWhisper("1...");
                                        in1.Stop();
                                    };
                                    in1.Start();
                                    System.Timers.Timer in0 = new System.Timers.Timer(6000);
                                    in0.Interval = 6000;
                                    in0.Elapsed += delegate
                                    {
                                        TargetClient.SendWhisper("FIGHT!");
                                        Client.SendWhisper("FIGHT!");
                                        User.CanWalk = true;
                                        User2.CanWalk = true;
                                        User.ApplyEffect(705);
                                        User2.ApplyEffect(705);
                                        in0.Stop();
                                    };
                                    in0.Start();

                                    if (Client.GetHabbo().ArmeEquiped == "cocktail")
                                    {
                                        Client.GetHabbo().ArmeEquiped = null;
                                        Client.GetHabbo().resetEffectEvent();
                                    }

                                    if (TargetClient.GetHabbo().ArmeEquiped == "cocktail")
                                    {
                                        TargetClient.GetHabbo().ArmeEquiped = null;
                                        TargetClient.GetHabbo().resetEffectEvent();
                                    }

                                    System.Timers.Timer timer1 = new System.Timers.Timer(300000);
                                    timer1.Interval = 300000;
                                    timer1.Elapsed += delegate
                                    {
                                        if (User.DuelUser == User2.DuelUser && User.DuelToken == "DUEL-" + Token)
                                        {
                                            foreach (Item item in Room.GetRoomItemHandler().GetFloor)
                                            {
                                                if (item.GetBaseItem().SpriteId == 2536)
                                                {
                                                    Room.GetGameMap().TeleportToItem(User, item);
                                                    Room.GetGameMap().TeleportToItem(TargetUser, item);
                                                    Room.GetRoomUserManager().UpdateUserStatusses();
                                                    Client.GetHabbo().usingArena = false;
                                                    TargetClient.GetHabbo().usingArena = false;
                                                    User.DuelToken = null;
                                                    TargetUser.DuelToken = null;
                                                    User.DuelUser = null;
                                                    TargetUser.DuelUser = null;
                                                    Client.GetHabbo().resetEffectEvent();
                                                    Client.GetHabbo().resetAvatarEvent();
                                                    TargetClient.GetHabbo().resetAvatarEvent();
                                                    TargetClient.GetHabbo().resetEffectEvent();
                                                    PlusEnvironment.ArenaUsing = false;
                                                }
                                            }
                                        }
                                        timer1.Stop();
                                    };
                                    timer1.Start();
                                }
                            }
                        }
                        #endregion
                        #region Casino High/Low Game
                        if (User.Transaction.StartsWith("cards"))
                        {
                            string[] duelData = User.Transaction.Split(':');
                            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(duelData[1]);
                            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Client.GetHabbo().CurrentRoom)
                            {
                                Client.SendWhisper(duelData[1] + " could not be found in " + Client.GetHabbo().CurrentRoom.Name + " room");
                            }
                            else
                            {
                                RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
                                Random TokenRand = new Random();
                                int tokenNumber = TokenRand.Next(1600, 2894354);
                                string Token = "DUEL-" + tokenNumber;

                                User.Say("accepts the " + PlusEnvironment.ConvertToPrice(TargetClient.GetHabbo().CardsOffer) + " dollars bet on cards with " + TargetClient.GetHabbo().Username);

                                User.Freezed = true;
                                TargetUser.Freezed = true;

                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "card-table-close;hide");
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "card-table-close;hide");

                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "card-table;show");
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "card-table;show");

                                TargetClient.GetHabbo().CardsPlaying = true;
                                Client.GetHabbo().CardsPlaying = true;
                                TargetClient.GetHabbo().CardsWith = Client.GetHabbo().Username;
                                Client.GetHabbo().CardsWith = TargetClient.GetHabbo().Username;
                                Client.GetHabbo().CardsOffer = TargetClient.GetHabbo().CardsOffer;
                                TargetClient.GetHabbo().MyTurn = true;

                                TargetClient.GetHabbo().Credits -= Client.GetHabbo().CardsOffer;
                                TargetClient.SendMessage(new CreditBalanceComposer(TargetClient.GetHabbo().Credits));
                                TargetClient.GetHabbo().RPCache(3);

                                Client.GetHabbo().Credits -= TargetClient.GetHabbo().CardsOffer;
                                Client.SendMessage(new CreditBalanceComposer(TargetClient.GetHabbo().Credits));
                                Client.GetHabbo().RPCache(3);

                                TargetUser.Transaction = null;

                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "cards", "random");
                            }
                        }
                        #endregion

                        //Corps
                        #region Hospital
                        if (User.Transaction.StartsWith("hospital"))
                        {
                            string[] ReceivedData = Data.Split(',');
                            string[] Datas = User.Transaction.Split(':');

                            string Item = Datas[1];
                            int Credits = Convert.ToInt32(Datas[2]);
                            int Quantity = Convert.ToInt32(Datas[3]);
                            int Stock = Convert.ToInt32(Datas[4]);
                            int Tip = Credits * 12 / 100;
                            int CashTill = Credits * 88 / 100;

                            Client.GetHabbo().Credits -= Credits;
                            Client.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                            Client.GetHabbo().RPCache(3);

                            User.Say("accepts " + Client.GetHabbo().TransactionFrom + "'s offer");

                            if (Item == "Heal")
                            {
                                Client.GetRoleplay().Health += 25;
                                Client.GetHabbo().RPCache(1);
                                Client.GetHabbo().UsingHospitalHeal = true;

                                if (Client.GetRoleplay().Health == Client.GetRoleplay().HealthMax)
                                {
                                    Client.GetHabbo().UsingHospitalHeal = false;
                                    Client.GetHabbo().RPCache(1);
                                    return;
                                }

                                if (Client.GetRoleplay().Health > Client.GetRoleplay().HealthMax)
                                {
                                    Client.GetHabbo().UsingHospitalHeal = false;
                                    Client.GetRoleplay().Health = Client.GetRoleplay().HealthMax;
                                    Client.GetHabbo().RPCache(1);
                                    return;
                                }

                                System.Timers.Timer HospitalHealTimer = new System.Timers.Timer(500);
                                HospitalHealTimer.Interval = 500;
                                HospitalHealTimer.Elapsed += delegate
                                {
                                    if (Client.GetRoleplay().Health < Client.GetRoleplay().HealthMax && Client.GetHabbo().UsingHospitalHeal)
                                    {
                                        Client.GetRoleplay().Health += 3;
                                        if (Client.GetRoleplay().Health == Client.GetRoleplay().HealthMax || Client.GetRoleplay().Health > Client.GetRoleplay().HealthMax)
                                        {
                                            Client.GetRoleplay().Health = Client.GetRoleplay().HealthMax;
                                            Client.GetHabbo().RPCache(1);
                                            HospitalHealTimer.Stop();
                                            Client.GetHabbo().UsingHospitalHeal = false;
                                            User.Transaction = null;
                                            return;
                                        }
                                        Client.GetHabbo().RPCache(1);
                                        HospitalHealTimer.Start();
                                    }
                                    else if (Client.GetRoleplay().Health == Client.GetRoleplay().HealthMax && Client.GetHabbo().UsingHospitalHeal)
                                    {
                                        Client.GetRoleplay().Health = Client.GetRoleplay().HealthMax;
                                        Client.GetHabbo().RPCache(1);
                                        Client.GetHabbo().UsingHospitalHeal = false;
                                        HospitalHealTimer.Stop();
                                    }
                                    else if (Client.GetRoleplay().Health > Client.GetRoleplay().HealthMax && Client.GetHabbo().UsingHospitalHeal)
                                    {
                                        Client.GetRoleplay().Health = Client.GetRoleplay().HealthMax;
                                        Client.GetHabbo().RPCache(1);
                                        Client.GetHabbo().UsingHospitalHeal = false;
                                        HospitalHealTimer.Stop();
                                    }
                                    else
                                    {
                                        HospitalHealTimer.Stop();
                                    }
                                };
                                HospitalHealTimer.Start();
                            }
                            else if (Item == "Medkit")
                            {
                                Client.GetHabbo().AddToInventory2("medkit", Quantity);
                            }

                            TransactionClient.SendWhisper("You have earned $" + Tip + " tip from this sale");
                            TransactionClient.GetHabbo().Credits += Tip;
                            TransactionClient.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                            TransactionClient.GetHabbo().RPCache(3);
                            TransactionClient.GetHabbo().UpdateSales();

                            Group Hospital = null;
                            if (PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(2, out Hospital))
                            {
                                Hospital.ChiffreAffaire -= Stock;
                                Hospital.updateChiffre();
                                Hospital.Cash += CashTill;
                                Hospital.updateCash();
                            }
                        }
                        #endregion
                        #region Armoury
                        if (User.Transaction.StartsWith("armoury"))
                        {
                            string[] ReceivedData = Data.Split(',');
                            string[] Datas = User.Transaction.Split(':');

                            string Item = Datas[1];
                            int Credits = Convert.ToInt32(Datas[2]);
                            int Quantity = Convert.ToInt32(Datas[3]);
                            int Stock = Convert.ToInt32(Datas[4]); //the result comes from :sell command
                            int Tip = Credits * 10 / 100;
                            int CashTill = Credits* 20 / 100;

                            Client.GetHabbo().Credits -= Credits;
                            User.CarryItem(0);
                            Client.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                            Client.GetHabbo().RPCache(3);

                            User.Say("accepts " + Client.GetHabbo().TransactionFrom + "'s offer");

                            if (Item != "Lockpick" && Item != "Throwing Knife")
                            {
                                if (Item == "Vest")
                                {
                                    Item = "bodyarmour";
                                }

                                for (int i = 1; i <= Quantity; i++)
                                {
                                    Client.GetHabbo().AddToInventory(Item.ToLower(), 300);
                                }
                            }
                            else
                            {
                                if (Item == "Throwing Knife")
                                {
                                    Client.GetHabbo().AddToInventory2("throwingknife", Quantity);
                                }
                                else if (Item == "Lockpick")
                                {
                                    Client.GetHabbo().AddToInventory2("lockpick", Quantity);
                                }
                            }

                            TransactionClient.SendWhisper("You have earned $" + Tip + " tip from this sale");
                            TransactionClient.GetHabbo().Credits += Tip;
                            TransactionClient.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                            TransactionClient.GetHabbo().RPCache(3);
                            TransactionClient.GetHabbo().UpdateSales();

                            Group Armoury = null;
                            if (PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(4, out Armoury))
                            {
                                Armoury.ChiffreAffaire -= Stock;
                                Armoury.updateChiffre();
                                Armoury.Cash += CashTill;
                                Armoury.updateCash();
                            }
                        }
                        #endregion
                        #region Starbucks
                        if (User.Transaction.StartsWith("starbucks"))
                        {
                            string[] ReceivedData = Data.Split(',');
                            string[] Datas = User.Transaction.Split(':');

                            string Item = Datas[1];
                            int Credits = Convert.ToInt32(Datas[2]);
                            int Quantity = Convert.ToInt32(Datas[3]);
                            int Stock = Convert.ToInt32(Datas[4]);
                            int Tip = Credits * 20 / 100;
                            int CashTill = Credits * 25 / 100;

                            Client.GetHabbo().Credits -= Credits;
                            Client.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                            Client.GetHabbo().RPCache(3);

                            User.Say("accepts " + Client.GetHabbo().TransactionFrom + "'s offer");

                            if (Item == "Coffee")
                            {
                                User.CarryItem(41);
                                int GiveEnergy = 0;
                                System.Timers.Timer EnergyTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(1));
                                EnergyTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(1);
                                EnergyTimer.Elapsed += delegate
                                {
                                    if (User.CarryItemID == 41)
                                    {
                                        GiveEnergy += 3;
                                        Client.GetRoleplay().Energy += 3;
                                        Client.GetHabbo().RPCache(1);

                                        if (GiveEnergy >= 50 || Client.GetRoleplay().Energy >= 100)
                                        {
                                            if (Client.GetRoleplay().Energy > 100)
                                            {
                                                Client.GetRoleplay().Energy = 100;
                                                Client.GetHabbo().RPCache(1);
                                            }
                                            User.CarryItem(0);
                                            EnergyTimer.Stop();
                                        }
                                    }
                                };
                                EnergyTimer.Start();
                            }
                            else if (Item == "Snack")
                            {
                                Client.GetHabbo().AddToInventory2("snack", Quantity);
                            }

                            TransactionClient.SendWhisper("You have earned $" + Tip + " tip from this sale");
                            TransactionClient.GetHabbo().Credits += Tip;
                            TransactionClient.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                            TransactionClient.GetHabbo().RPCache(3);
                            TransactionClient.GetHabbo().UpdateSales();

                            Group Starbucks = null;
                            if (PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(6, out Starbucks))
                            {
                                Starbucks.ChiffreAffaire -= Stock;
                                Starbucks.updateChiffre();
                                Starbucks.Cash += CashTill;
                                Starbucks.updateCash();
                            }
                        }
                        #endregion                        
                        #region Bank
                        if (User.Transaction.StartsWith("bank"))
                        {
                            string[] ReceivedData = Data.Split(',');
                            string[] Datas = User.Transaction.Split(':');
                            string Aktion = Datas[1];

                            if (Aktion == "deposit")
                            {
                                int Amount = Convert.ToInt32(Datas[2]);
                                int Fee = Convert.ToInt32(Datas[3]) / 2; //aka tip

                                TransactionUser.Say("takes $" + PlusEnvironment.ConvertToPrice(Amount) + " from " + User.GetClient().GetHabbo().Username + " and deposits into their bank account");

                                Client.SendWhisper("You have been charged a $" + PlusEnvironment.ConvertToPrice(Fee * 2) + " fee for this transfer");
                                Client.GetHabbo().Credits -= Amount + Fee * 2;
                                Client.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                                Client.GetHabbo().RPCache(3);
                                Client.GetHabbo().Banque += Amount;
                                Client.GetHabbo().updateBanque();

                                TransactionClient.SendWhisper("You have earned $" + Fee + " tip from this transfer");
                                TransactionClient.GetHabbo().Credits += Fee;
                                TransactionClient.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                                TransactionClient.GetHabbo().RPCache(3);

                                if (Fee > 1)
                                {
                                    Group Bank = null;
                                    if (PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(3, out Bank))
                                    {
                                        Bank.Cash += Fee;
                                        Bank.updateCash();
                                    }
                                }
                            }
                            else if (Aktion == "withdraw")
                            {
                                int Amount = Convert.ToInt32(Datas[2]);

                                TransactionUser.Say("takes $" + PlusEnvironment.ConvertToPrice(Convert.ToInt32(Amount)) + " from " + Client.GetHabbo().Username + "'s bank account and hands it to them");

                                Client.GetHabbo().Banque -= Amount;
                                Client.GetHabbo().updateBanque();

                                Client.GetHabbo().Credits += Amount;
                                Client.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                                Client.GetHabbo().RPCache(3);
                            }
                            else if (Aktion == "transfer")
                            {
                                string To = Datas[2];
                                int Amount = Convert.ToInt32(Datas[3]);
                                int Fee = Convert.ToInt32(Datas[4]) / 2; //aka tip

                                TransactionUser.Say("transfers $" + PlusEnvironment.ConvertToPrice(Amount) + " from " + Client.GetHabbo().Username + "'s bank account to " + To + "'s bank account");

                                GameClient ToPlayer = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(To);
                                if (ToPlayer == null)
                                {
                                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.SetQuery("UPDATE `users` SET `banque` = `banque` + " + Amount + " WHERE `username` = '" + To + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                    }
                                }
                                else
                                {
                                    ToPlayer.GetHabbo().Banque += Amount;
                                    ToPlayer.GetHabbo().updateBanque();
                                }

                                Client.SendWhisper("You have been charged a $" + PlusEnvironment.ConvertToPrice(Fee * 2) + " fee for this transfer");
                                Client.GetHabbo().Banque -= Amount + Fee * 2;
                                Client.GetHabbo().updateBanque();

                                TransactionClient.SendWhisper("You have earned $" + Fee + " tip from this transfer");
                                TransactionClient.GetHabbo().Credits += Fee;
                                TransactionClient.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                                TransactionClient.GetHabbo().RPCache(3);

                                if (Fee > 1)
                                {
                                    Group Bank = null;
                                    if (PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(3, out Bank))
                                    {
                                        Bank.Cash += Fee;
                                        Bank.updateCash();
                                    }
                                }
                            }
                            else if (Aktion == "balance")
                            {
                                TransactionUser.Say("Enters " + Client.GetHabbo().Username + "’s details in to the computer and checks their balance of $" + PlusEnvironment.ConvertToPrice(Client.GetHabbo().Banque));
                            }
                            else if (Aktion == "sell")
                            {
                                string Item = Convert.ToString(Datas[2]);
                                int Amount = Convert.ToInt32(Datas[3]);

                                User.Say("accepts the Deposit Box Rent offer");

                                if (Item == "Deposit Box Rent")
                                {
                                    DataRow Deposit = null;
                                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.SetQuery("SELECT * FROM `user_deposit_box` WHERE `user_id` = @id LIMIT 1;");
                                        dbClient.AddParameter("id", Client.GetHabbo().Id);
                                        Deposit = dbClient.getRow();
                                    }

                                    if (Deposit == null)
                                    {
                                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                        {
                                            dbClient.RunQuery("INSERT INTO `user_deposit_box` (`user_id`) VALUES ('" + Client.GetHabbo().Id + "')");
                                        }
                                    }

                                    Client.GetRoleplay().DepositRent = DateTime.Now.AddDays(7);
                                    Client.GetHabbo().Credits -= Amount;
                                    Client.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                                    Client.GetHabbo().RPCache(3);
                                    TransactionClient.GetHabbo().UpdateSales();

                                }
                            }
                        }
                        #endregion

                        #region Trade
                        if (User.Transaction.StartsWith("trade"))
                        {
                            User.Say("accepts the trade");
                            string[] From = User.Transaction.Split(':');

                            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(From[1]);
                            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Client.GetHabbo().CurrentRoom)
                            {
                                Client.SendWhisper(From[1] + " could not be found in " + Client.GetHabbo().CurrentRoom.Name + " room");
                                return;
                            }

                            Random TokenRand = new Random();
                            int tokenNumber = TokenRand.Next(1600, 2894354);

                            Client.GetHabbo().TradeToken = tokenNumber;
                            TargetClient.GetHabbo().TradeToken = tokenNumber;

                            Client.GetHabbo().isTradingWith = TargetClient.GetHabbo().Username;
                            TargetClient.GetHabbo().isTradingWith = Client.GetHabbo().Username;

                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade;new;" + TargetClient.GetHabbo().Username);
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade;new;" + Client.GetHabbo().Username);
                        }
                        #endregion

                        Client.GetHabbo().TransactionFrom = null;
                        TransactionUser.Transaction = null;
                        TransactionClient.GetHabbo().TransactionTo = null;
                        Client.GetHabbo().OfferToken = 0;
                        User.Transaction = null;
                        return;
                    }
                #endregion
                #region Decline
                case "refuser":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        GameClient TransactionUser = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Client.GetHabbo().TransactionFrom);
                        if (TransactionUser == null)
                            return;

                        RoomUser TransactionUser2 = Room.GetRoomUserManager().GetRoomUserByHabbo(TransactionUser.GetHabbo().Id);
                        if (TransactionUser2 == null)
                            return;

                        if (User.Transaction == null)
                        {
                            Client.SendWhisper("You have no ongoing transactions");
                            return;
                        }

                        if (User.Transaction.StartsWith("paramedic"))
                        {
                            string[] ReceivedData = Data.Split(',');
                            string[] Datas = User.Transaction.Split(':');
                            string Username = Datas[1];

                            GameClient ParamedicUser = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
                            if (ParamedicUser == null)
                                return;

                            User.Say("declines the paramedic call for " + ParamedicUser.GetHabbo().Username);
                        }

                        if (User.Transaction.StartsWith("bank"))
                        {
                            User.Say("declines " + TransactionUser.GetHabbo().Username + "'s offer");
                        }

                        if (User.Transaction.StartsWith("portarme"))
                        {
                            User.GetClient().Shout("*Weigere sich, ein Waffenlizenz zu kaufen*");
                        }

                        if (User.Transaction.StartsWith("armoury"))
                        {
                            User.Say("declines " + Client.GetHabbo().TransactionFrom + "'s offer");
                            TransactionUser.SendWhisper(Client.GetHabbo().Username + " has declined your offer");
                        }

                        if (User.Transaction.StartsWith("hospital"))
                        {
                            User.Say("declines " + Client.GetHabbo().TransactionFrom + "'s offer");
                            TransactionUser.SendWhisper(Client.GetHabbo().Username + " has declined your offer");
                        }

                        if (User.Transaction.StartsWith("echange_items"))
                        {
                            string[] tradeInvitUser = User.Transaction.Split(':');
                            User.OnChat(User.LastBubble, "* Verweigert den Tausch von " + tradeInvitUser[1] + " *", true);
                        }

                        if (User.Transaction.StartsWith("cards"))
                        {
                            User.Say("declines the bet of " + PlusEnvironment.ConvertToPrice(TransactionUser2.GetClient().GetHabbo().CardsOffer) + " dollars from " + TransactionUser2.GetClient().GetHabbo().Username);
                            TransactionUser2.GetClient().SendWhisper(Client.GetHabbo().Username + " has declined your bet");
                            TransactionUser2.GetClient().GetHabbo().CardsOffer = 0;
                            Client.GetHabbo().TransactionFrom = null;
                        }

                        if (User.Transaction.StartsWith("gang"))
                        {
                            User.Say("declines the gang invitation");
                        }

                        if (User.Transaction.StartsWith("duel"))
                        {
                            User.Say("declines the 1v1 challenge");
                        }

                        if (User.Transaction.StartsWith("trade"))
                        {
                            User.Say("refuses the trade");
                        }

                        User.Transaction = null;
                        return;
                    }
                #endregion
                #region checkIfGetCB
                case "checkIfGetCB":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().Cb == "null")
                        {
                            Client.SendWhisper("You don't have a bank card");
                            return;
                        }

                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "transaction_code;gotocode");
                        return;
                    }
                #endregion
                #region checkCode
                case "checkCode":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().Cb == "null")
                            return;

                        string[] ReceivedData = Data.Split(',');
                        if (ReceivedData[1] == null || ReceivedData[1] != Client.GetHabbo().Cb)
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "transaction_code;error");
                            return;
                        }

                        User.canUseCB = true;
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "transaction_code;close");
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "transaction", "accepter,cb");
                        return;
                    }
                    #endregion
            }
        }
    }
}
