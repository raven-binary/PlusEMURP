using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plus;
using Fleck;
using Plus.HabboHotel.GameClients;
using System.Text.RegularExpressions;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using System.Data;

namespace Bobba.HabboRoleplay.Web.Outgoing
{
    class GangWebEvent : IWebEvent
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

                #region toggle
                case "toggle":
                    {
                        Socket.Send("gang;toggle;" + Client.GetHabbo().Gang + ";" + Client.GetHabbo().getNameOfGang());
                    }
                    break;
                #endregion
                #region create
                case "create":
                    {
                        if (Client.GetHabbo().Gang != 0)
                            return;

                        if (Client.GetHabbo().Credits < 500)
                        {
                            Client.SendWhisper("You need $500 to create a gang");
                            return;
                        }

                        int SplitData = Data.IndexOf(',');
                        string nameOFGang = Data.Substring(SplitData + 1);
                        nameOFGang = nameOFGang.Trim();

                        if (nameOFGang.Length < 1)
                        {
                            Client.SendWhisper("Your gang name must have at least 1 character");
                            return;
                        }

                        if (nameOFGang.Length > 10)
                        {
                            Client.SendWhisper("Your gang name doesn't have to be 10 characters long");
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(nameOFGang))
                        {
                            Client.SendWhisper("The gang name is invalid");
                            return;
                        }

                        var withoutSpecial = new string(nameOFGang.Where(c => Char.IsLetterOrDigit(c) || Char.IsPunctuation(c) || Char.IsWhiteSpace(c)).ToArray());
                        if (nameOFGang != withoutSpecial)
                        {
                            Client.SendWhisper("Your gang name contains unauthorized characters");
                            return;
                        }

                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT COUNT(0) FROM `gang` WHERE `name` = @name");
                            dbClient.AddParameter("name", nameOFGang);
                            int GangCount = dbClient.getInteger();
                            if (GangCount > 0)
                            {
                                Client.SendWhisper("This gang name is already taken");
                                return;
                            }
                        }

                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (User.isTradingItems)
                        {
                            Client.SendWhisper("You cannot create a gang while you are trading");
                            return;
                        }

                        Client.GetHabbo().Credits -= 500;
                        Client.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "my_stats;" + Client.GetHabbo().Credits + ";" + Client.GetHabbo().Duckets + ";" + Client.GetHabbo().EventPoints);
                        Client.GetHabbo().createGang(nameOFGang);

                        Socket.Send("gang;goToGang;" + Client.GetHabbo().getNameOfGang());
                        break;
                    }
                #endregion
                #region promouvoir
                case "promouvoir":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().getCooldown("gang"))
                        {
                            Client.SendWhisper("Warte kurz...");
                            return;
                        }

                        if (Client.GetHabbo().Gang == 0)
                            return;

                        if(Client.GetHabbo().GangRank == 1 || Client.GetHabbo().GangRank == 2)
                            return;

                        int SplitData = Data.IndexOf(',');
                        string Username = Data.Substring(SplitData + 1);

                        if (string.IsNullOrWhiteSpace(Username))
                            return;

                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT id, gang, gang_rank FROM `users` WHERE username = @username LIMIT 1");
                            dbClient.AddParameter("username", Username);
                            DataRow getUser = dbClient.getRow();

                            if (getUser["id"] == null)
                                return;

                            if (Convert.ToInt32(getUser["id"]) == Client.GetHabbo().Id)
                                return;

                            if (Convert.ToInt32(getUser["gang"]) != Client.GetHabbo().Gang)
                                return;

                            if (Convert.ToInt32(getUser["gang_rank"]) == 3 && Client.GetHabbo().GangRank == 3 || Convert.ToInt32(getUser["gang_rank"]) == 4)
                                return;

                            Client.GetHabbo().addCooldown("gang", 2000);
                            int newRank = 0;
                            if(Convert.ToInt32(getUser["gang_rank"]) == 1)
                            {
                                User.GetClient().Shout("*Fügt " + Username + " als Erfahrener in sein Gang*");
                                newRank = 2;
                            }
                            else if (Convert.ToInt32(getUser["gang_rank"]) == 2)
                            {
                                User.GetClient().Shout("*Fügt " + Username + " als Vertreter in sein Gang*");
                                newRank = 3;
                            }
                            else if (Convert.ToInt32(getUser["gang_rank"]) == 3)
                            {
                                User.GetClient().Shout("*Fügt " + Username + " als Besitzer in sein Gang*");
                                newRank = 4;
                            }

                            dbClient.SetQuery("UPDATE `users` SET `gang_rank` = @newrank WHERE `id` = @userId LIMIT 1");
                            dbClient.AddParameter("newrank", newRank);
                            dbClient.AddParameter("userId", Convert.ToInt32(getUser["id"]));
                            dbClient.RunQuery();

                            PlusEnvironment.GetGame().GetClientManager().sendGangMsg(Client.GetHabbo().Gang, Client.GetHabbo().Username + " hat " + Username + " geworben.");
                            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
                            if(TargetClient != null)
                            {
                                TargetClient.GetHabbo().GangRank = newRank;
                            }
                        }

                        Socket.Send("gang;goToGang;" + Client.GetHabbo().getNameOfGang());
                        break;
                    }
                #endregion
                #region retrograder
                case "retrograder":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().getCooldown("gang") == true)
                        {
                            Client.SendWhisper("Warte kurz...");
                            return;
                        }

                        if (Client.GetHabbo().Gang == 0)
                            return;

                        if (Client.GetHabbo().GangRank == 1 || Client.GetHabbo().GangRank == 2)
                            return;

                        int SplitData = Data.IndexOf(',');
                        string Username = Data.Substring(SplitData + 1);

                        if (string.IsNullOrWhiteSpace(Username))
                            return;

                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT id, gang, gang_rank FROM `users` WHERE username = @username LIMIT 1");
                            dbClient.AddParameter("username", Username);
                            DataRow getUser = dbClient.getRow();

                            if (getUser["id"] == null)
                                return;

                            if (Convert.ToInt32(getUser["id"]) == Client.GetHabbo().Id)
                                return;

                            if (Convert.ToInt32(getUser["gang"]) != Client.GetHabbo().Gang)
                                return;

                            if (Convert.ToInt32(getUser["gang_rank"]) == 4 && Client.GetHabbo().GangRank == 3 || Convert.ToInt32(getUser["gang_rank"]) == 1)
                                return;

                            if (Convert.ToInt32(getUser["gang_rank"]) == 4 && Client.GetHabbo().GangRank == 4 && Client.GetHabbo().getOwnerOfGang() != Client.GetHabbo().Id)
                                return;

                            Client.GetHabbo().addCooldown("gang", 2000);

                            int newRank = 0;
                            if (Convert.ToInt32(getUser["gang_rank"]) == 2)
                            {
                                User.GetClient().Shout("*Stuft " + Username + " als Erfahrener in seiner Gang*");
                                newRank = 1;
                            }
                            else if (Convert.ToInt32(getUser["gang_rank"]) == 3)
                            {
                                User.GetClient().Shout("*Stuft " + Username + " als Vertreter in seiner Gang*");
                                newRank = 2;
                            }
                            else if (Convert.ToInt32(getUser["gang_rank"]) == 4)
                            {
                                User.GetClient().Shout("*Stuft " + Username + " als Besitzer in seiner Gang*");
                                newRank = 3;
                            }

                            dbClient.SetQuery("UPDATE `users` SET `gang_rank` = @newrank WHERE `id` = @userId LIMIT 1");
                            dbClient.AddParameter("newrank", newRank);
                            dbClient.AddParameter("userId", Convert.ToInt32(getUser["id"]));
                            dbClient.RunQuery();

                            PlusEnvironment.GetGame().GetClientManager().sendGangMsg(Client.GetHabbo().Gang, Client.GetHabbo().Username + " stuft " + Username + " runter.");
                            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
                            if (TargetClient != null)
                            {
                                TargetClient.GetHabbo().GangRank = newRank;
                            }
                        }

                        Socket.Send("gang;goToGang;" + Client.GetHabbo().getNameOfGang());
                        break;
                    }
                #endregion
                #region virer
                case "virer":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().Gang == 0)
                            return;

                        if (Client.GetHabbo().GangRank == 1 || Client.GetHabbo().GangRank == 2 || Client.GetHabbo().GangRank == 3)
                            return;

                        int SplitData = Data.IndexOf(',');
                        string Username = Data.Substring(SplitData + 1);

                        if (string.IsNullOrWhiteSpace(Username))
                            return;

                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT id, gang, gang_rank FROM `users` WHERE username = @username LIMIT 1");
                            dbClient.AddParameter("username", Username);
                            DataRow getUser = dbClient.getRow();

                            if (getUser["id"] == null)
                                return;

                            if (Convert.ToInt32(getUser["id"]) == Client.GetHabbo().Id)
                                return;

                            if (Convert.ToInt32(getUser["gang"]) != Client.GetHabbo().Gang)
                                return;

                            if (Convert.ToInt32(getUser["gang_rank"]) == 4 && Client.GetHabbo().GangRank == 4 && Client.GetHabbo().getOwnerOfGang() != Client.GetHabbo().Id)
                                return;

                            dbClient.SetQuery("UPDATE `users` SET `gang` = '0', `gang_rank` = '0' WHERE `id` = @userId LIMIT 1");
                            dbClient.AddParameter("userId", Convert.ToInt32(getUser["id"]));
                            dbClient.RunQuery();
                            User.GetClient().Shout("*Kickt " + Username + " aus seiner Gang*");

                            PlusEnvironment.GetGame().GetClientManager().sendGangMsg(Client.GetHabbo().Gang, Client.GetHabbo().Username + " hat " + Username + " von der Gang gekickt.");
                            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
                            if (TargetClient != null)
                            {
                                TargetClient.GetHabbo().Gang = 0;
                                TargetClient.GetHabbo().GangRank = 0;
                            }
                        }

                        Socket.Send("gang;goToGang;" + Client.GetHabbo().getNameOfGang());
                        break;
                    }
                #endregion
                #region owner
                case "owner":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().Gang == 0)
                            return;

                        if (Client.GetHabbo().getOwnerOfGang() != Client.GetHabbo().Id)
                            return;

                        int SplitData = Data.IndexOf(',');
                        string Username = Data.Substring(SplitData + 1);

                        if (string.IsNullOrWhiteSpace(Username))
                            return;

                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT id, gang, gang_rank FROM `users` WHERE username = @username LIMIT 1");
                            dbClient.AddParameter("username", Username);
                            DataRow getUser = dbClient.getRow();

                            if (getUser["id"] == null)
                                return;

                            if (Convert.ToInt32(getUser["id"]) == Client.GetHabbo().Id)
                                return;

                            if (Convert.ToInt32(getUser["gang"]) != Client.GetHabbo().Gang)
                                return;

                            if (Convert.ToInt32(getUser["gang_rank"]) != 4)
                                return;

                            dbClient.SetQuery("UPDATE `gang` SET `owner` = @newOwner WHERE `id` = @gangId LIMIT 1");
                            dbClient.AddParameter("newOwner", Convert.ToInt32(getUser["id"]));
                            dbClient.AddParameter("gangId", Client.GetHabbo().Gang);
                            dbClient.RunQuery();
                            User.Say("*gives their gang to  " + Username);

                            PlusEnvironment.GetGame().GetClientManager().sendGangMsg(Client.GetHabbo().Gang, Client.GetHabbo().Username + " has left the gang so the gang belongs to " + Username + " now");
                        }

                        Socket.Send("gang;goToGang;" + Client.GetHabbo().getNameOfGang());
                        break;
                    }
                #endregion
                #region invite
                case "invite":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().getCooldown("gang") == true)
                        {
                            Client.SendWhisper("Wait before you invite to your gang again");
                            return;
                        }
                        
                        if (Client.GetHabbo().Gang == 0)
                            return;

                        if (Client.GetHabbo().GangRank == 1)
                            return;

                        int SplitData = Data.IndexOf(',');
                        string Username = Data.Substring(SplitData + 1);

                        if (string.IsNullOrWhiteSpace(Username))
                        {
                            Client.SendWhisper("Please enter a username");
                            return;
                        }

                        GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
                        if (TargetClient == null || TargetClient.GetHabbo().CurrentRoomId != Client.GetHabbo().CurrentRoomId)
                        {
                            Client.SendWhisper(Username + " could not be found in this room");
                            return;
                        }

                        if (TargetClient.GetHabbo().Gang == Client.GetHabbo().Gang)
                        {
                            Client.SendWhisper(TargetClient.GetHabbo().Username + " is already in this gang");
                            return;
                        }

                        if (TargetClient.GetHabbo().Gang != 0)
                        {
                            Client.SendWhisper(TargetClient.GetHabbo().Username + " is already in a gang");
                            return;
                        }

                        if (Client.GetHabbo().countUserOfGang() > 24)
                        {
                            Client.SendWhisper("You can only add 25 members to your gang");
                            return;
                        }

                        RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
                        if (TargetUser.Transaction != null)
                        {
                            Client.SendWhisper(TargetClient.GetHabbo().Username + " already has an invitation in process, try again later");
                            return;
                        }

                        Client.GetHabbo().addCooldown("gang", 2000);
                        PlusEnvironment.GetGame().GetClientManager().sendGangMsg(Client.GetHabbo().Gang, Client.GetHabbo().Username + " invited " + TargetClient.GetHabbo().Username + " to the gang");
                        TargetUser.Transaction = "gang:" + Client.GetHabbo().Gang;
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction;<b>" + Client.GetHabbo().Username + "</b> invite you in their gang <b>[" + Client.GetHabbo().getNameOfGang() + "]</b>.;0");
                        break;
                    }
                #endregion
                #region leave
                case "leave":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().Gang == 0)
                            return;

                        if (Client.GetHabbo().getOwnerOfGang() == Client.GetHabbo().Id && Client.GetHabbo().countUserOfGang() > 1)
                        {
                            Client.SendWhisper("You have to give your gang to someone before you leave the gang");
                            return;
                        }

                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            if(Client.GetHabbo().countUserOfGang() == 1)
                            {
                                dbClient.SetQuery("DELETE FROM `gang` WHERE `id` = @gang_id");
                                dbClient.AddParameter("gang_id", Client.GetHabbo().Gang);
                                dbClient.RunQuery();

                                dbClient.SetQuery("UPDATE rooms SET capture = 0 WHERE `capture` = @gang_capture");
                                dbClient.AddParameter("gang_capture", Client.GetHabbo().Gang);
                                dbClient.RunQuery();
                            }

                            PlusEnvironment.GetGame().GetClientManager().sendGangMsg(Client.GetHabbo().Gang, Client.GetHabbo().Username + " has left the gang");
                            Client.GetHabbo().Gang = 0;
                            Client.GetHabbo().updateGang();
                            Client.GetHabbo().GangRank = 0;
                            Client.GetHabbo().updateGangRank();
                            User.Say("leaves their gang");
                        }

                        Socket.Send("gang;toggle");
                        break;
                    }
                #endregion
                #region colors
                case "colors":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().Gang == 0)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string GangColours = ReceivedData[1];
                        string Color = ReceivedData[2];
                        if (GangColours == "primary")
                        {
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE gang SET color_1 = @color WHERE `id` = @id");
                                dbClient.AddParameter("color", Color);
                                dbClient.AddParameter("id", Client.GetHabbo().Gang);
                                dbClient.RunQuery();
                            }
                        }
                        else if (GangColours == "secondary")
                        {
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE gang SET color_2 = @color WHERE `id` = @id");
                                dbClient.AddParameter("color", Color);
                                dbClient.AddParameter("id", Client.GetHabbo().Gang);
                                dbClient.RunQuery();
                            }
                        }
                        break;
                    }
                #endregion


                #region add rank
                case "add-rank":
                    {
                        if (Client.GetHabbo().Gang == 0)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string RankName = ReceivedData[1];

                        if (RankName.Length > 15)
                        {
                            Client.SendWhisper("The rank name is too big");
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(RankName))
                        {
                            Client.SendWhisper("The rank name is invalid");
                            return;
                        }

                        var withoutSpecial = new string(RankName.Where(c => Char.IsLetterOrDigit(c) || Char.IsPunctuation(c) || Char.IsWhiteSpace(c)).ToArray());
                        if (RankName != withoutSpecial)
                        {
                            Client.SendWhisper("The rank name contains unauthorized characters");
                            return;
                        }

                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT COUNT(0) FROM `gang_ranks` WHERE `title` = @title");
                            dbClient.AddParameter("title", RankName);
                            int Count = dbClient.getInteger();
                            if (Count > 0)
                            {
                                Client.SendWhisper("The rank is already taken");
                                return;
                            }
                        }

                        DataRow LastRamk = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `gang_ranks` WHERE `gang_id` = @id ORDER BY rank_id DESC LIMIT 1");
                            dbClient.AddParameter("id", Client.GetHabbo().Gang);
                            LastRamk = dbClient.getRow();
                        }

                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("INSERT INTO `gang_ranks` (`gang_id`,`rank_id`,`title`,`list`) VALUES (@gang, @rank, @title, @list)");
                            dbClient.AddParameter("gang", Client.GetHabbo().Gang);
                            dbClient.AddParameter("rank", Convert.ToInt32(LastRamk["rank_id"]) + 1);
                            dbClient.AddParameter("title", RankName);
                            dbClient.AddParameter("list", Convert.ToInt32(LastRamk["rank_id"]) + 1);
                            dbClient.RunQuery();
                        }

                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "gang-load;");
                        break;
                    }
                #endregion
                #region rank-up
                case "rank-up":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().Gang == 0)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Username = ReceivedData[1];

                        DataRow Member = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `users` WHERE `username` = @username LIMIT 1");
                            dbClient.AddParameter("username", Username);
                            Member = dbClient.getRow();
                        }

                        if (Member == null || Convert.ToInt32(Member["gang"]) != Client.GetHabbo().Gang)
                            return;

                        DataRow UpRank = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `gang_ranks` WHERE `rank_id` = @rank LIMIT 1");
                            dbClient.AddParameter("rank", Convert.ToInt32(Member["gang_rank"]) - 1);
                            UpRank = dbClient.getRow();
                        }

                        if (UpRank != null)
                        {
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE users SET gang_rank = @rank WHERE `id` = @id");
                                dbClient.AddParameter("rank", Convert.ToInt32(Member["gang_rank"]) - 1);
                                dbClient.AddParameter("id", Convert.ToInt32(Member["id"]));
                                dbClient.RunQuery();
                            }

                            User.Say("promotes " + Member["username"] + " to " + UpRank["title"]);
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "gang-load;");
                        }

                        break;
                    }
                #endregion
                #region rank-down
                case "rank-down":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().Gang == 0)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Username = ReceivedData[1];

                        DataRow Member = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `users` WHERE `username` = @username LIMIT 1");
                            dbClient.AddParameter("username", Username);
                            Member = dbClient.getRow();
                        }

                        if (Member == null || Convert.ToInt32(Member["gang"]) != Client.GetHabbo().Gang)
                            return;

                        DataRow UpRank = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `gang_ranks` WHERE `rank_id` = @rank LIMIT 1");
                            dbClient.AddParameter("rank", Convert.ToInt32(Member["gang_rank"]) + 1);
                            UpRank = dbClient.getRow();
                        }

                        if (UpRank != null)
                        {
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE users SET gang_rank = @rank WHERE `id` = @id");
                                dbClient.AddParameter("rank", Convert.ToInt32(Member["gang_rank"]) + 1);
                                dbClient.AddParameter("id", Convert.ToInt32(Member["id"]));
                                dbClient.RunQuery();
                            }

                            User.Say("demotes " + Member["username"] + " to " + UpRank["title"]);
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "gang-load;");
                        }
                        break;
                    }
                #endregion
                #region kick
                case "kick":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().Gang == 0)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Username = ReceivedData[1];

                        DataRow Member = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT id, gang, gang_rank FROM `users` WHERE `username` = @username LIMIT 1");
                            dbClient.AddParameter("username", Username);
                            Member = dbClient.getRow();
                        }

                        if (Member == null || Convert.ToInt32(Member["gang"]) != Client.GetHabbo().Gang)
                            return;

                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE users SET gang = @gang AND gang_rank = @rank WHERE `id` = @id");
                            dbClient.AddParameter("gang", 0);
                            dbClient.AddParameter("rank", 0);
                            dbClient.AddParameter("id", Convert.ToInt32(Member["id"]));
                            dbClient.RunQuery();
                        }

                        GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
                        if (TargetClient != null)
                        {
                            TargetClient.GetHabbo().Gang = 0;
                            TargetClient.GetHabbo().GangRank = 0;
                            TargetClient.SendWhisper("You have been kicked from " + Client.GetHabbo().getNameOfGang());
                        }

                        Client.SendWhisper("");
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "gang-load;");
                        break;
                    }
                #endregion
                #region up-rank
                case "up-rank":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().Gang == 0)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string RankId = ReceivedData[1];

                        if (Convert.ToInt32(RankId) == Client.GetHabbo().GangRank)
                        {
                            Client.SendWhisper("test");
                            return;
                        }

                        DataRow fuck = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `gang_ranks` WHERE `rank_id` = @rank_id AND `gang_id` = @gang LIMIT 1;");
                            dbClient.AddParameter("rank_id", Convert.ToInt32(RankId));
                            dbClient.AddParameter("gang", Client.GetHabbo().Gang);
                            fuck = dbClient.getRow();
                        }


                        DataRow Rank = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `gang_ranks` WHERE `rank_id` = @rank_id AND `gang_id` = @gang LIMIT 1;");
                            dbClient.AddParameter("rank_id", Convert.ToInt32(RankId) - 1);
                            dbClient.AddParameter("gang", Client.GetHabbo().Gang);
                            Rank = dbClient.getRow();
                        }

                        if (Rank != null)
                        {
                            int NextRank = Convert.ToInt32(RankId) - 1;

                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `gang_ranks` SET `rank_id` = '" + NextRank + "' WHERE `gang_id` = '" + Client.GetHabbo().Gang + "' AND `rank_id` = '" + Convert.ToInt32(RankId) + "' LIMIT 1;");
                                dbClient.RunQuery();

                                dbClient.SetQuery("UPDATE `gang_ranks` SET `rank_id` = '" + Convert.ToInt32(RankId) + "' WHERE `gang_id` = '" + Client.GetHabbo().Gang + "' AND `list` = '" + Convert.ToInt32(Rank["list"]) + "' LIMIT 1;");
                                dbClient.RunQuery();
                            }

                            DataRow sex = null;
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("SELECT * FROM `gang_ranks` WHERE `title` = @rank_id AND `gang_id` = @gang LIMIT 1;");
                                dbClient.AddParameter("rank_id", Convert.ToString(Rank["title"]));
                                dbClient.AddParameter("gang", Client.GetHabbo().Gang);
                                sex = dbClient.getRow();
                            }

                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `users` SET `gang_rank` = '" + Convert.ToInt32(sex["rank_id"]) + "' WHERE `gang` = '" + Client.GetHabbo().Gang + "' AND `gang_rank` = '" + NextRank + "' LIMIT 100;");
                                dbClient.RunQuery();
                            }

                            Client.SendWhisper(Convert.ToString(sex["title"]) + " " + Convert.ToString(sex["rank_id"]));

                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "gang-load;");
                        }
                        break;
                    }
                #endregion
                #region down-rank
                case "down-rank":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().Gang == 0)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string RankId = ReceivedData[1];

                        break;
                    }
                #endregion

                #region star
                case "star":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().Gang == 0)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string RankId = ReceivedData[1];

                        DataRow Rank = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `gang_ranks` WHERE `rank_id` = @rank_id AND `gang_id` = @gang LIMIT 1;");
                            dbClient.AddParameter("rank_id", Convert.ToInt32(RankId));
                            dbClient.AddParameter("gang", Client.GetHabbo().Gang);
                            Rank = dbClient.getRow();
                        }

                        if (Rank != null)
                        {
                            if (Convert.ToInt32(Rank["star"]) == 1)
                            {
                                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("UPDATE `gang_ranks` SET `star` = '0' WHERE `gang_id` = '" + Client.GetHabbo().Gang + "' AND `rank_id` = '" + Convert.ToInt32(RankId) + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                            }
                            else if (Convert.ToInt32(Rank["star"]) == 0)
                            {
                                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("UPDATE `gang_ranks` SET `star` = '1' WHERE `gang_id` = '" + Client.GetHabbo().Gang + "' AND `rank_id` = '" + Convert.ToInt32(RankId) + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                            }
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "gang-load;");
                        }
                        break;
                    }
                #endregion

                #region Talk To Gang
                case "talktogang":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().Gang == 0)
                            return;

                        if (Client.GetHabbo().TalkToGang)
                        {
                            Client.GetHabbo().TalkToGang = false;
                            Client.SendWhisper("Disabled gang chat");
                            return;
                        }

                        Client.GetHabbo().TalkToGang = true;
                        Client.SendWhisper("Enabled gang chat");
                        break;
                    }
                #endregion
                #region View Gang
                case "view-gang":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string GangName = ReceivedData[1];

                        PlusEnvironment.GetGame().GetChatManager().GetCommands().Parse(Client, ":viewgang " + GangName);
                        break;
                    }
               #endregion
            }
        }
    }
}