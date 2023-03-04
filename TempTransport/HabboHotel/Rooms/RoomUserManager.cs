using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Drawing;
using System.Linq;

using Plus.Communication.Packets.Outgoing.Rooms.Avatar;
using Plus.Core;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Users.Inventory.Bots;
using Plus.HabboHotel.Global;
using Plus.HabboHotel.Pathfinding;
using Plus.HabboHotel.Rooms.AI;
using Plus.HabboHotel.Quests;
using Plus.HabboHotel.Rooms.Games;

using Plus.HabboRoleplay.Misc;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Users.Inventory;
using Plus.Communication.Packets.Incoming;
using Plus.Communication.Packets.Outgoing.Inventory.Furni;
using Plus.Utilities;

using System.Data;
using Plus.Communication.Packets.Outgoing.Rooms.Session;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Permissions;
using Plus.Communication.Packets.Outgoing.Handshake;
using System.Text.RegularExpressions;
using Plus.HabboHotel.Rooms.Games.Teams;

using Plus.Database.Interfaces;
using Plus.HabboHotel.Rooms.AI.Speech;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using System.Threading.Tasks;

namespace Plus.HabboHotel.Rooms
{
    public class RoomUserManager
    {
        private Room _room;
        private ConcurrentDictionary<int, RoomUser> _users;
        public ConcurrentDictionary<int, RoomUser> _bots;
        private ConcurrentDictionary<int, RoomUser> _pets;

        private int primaryPrivateUserID;
        private int secondaryPrivateUserID;

        public int userCount;
        private int petCount;


        public RoomUserManager(Room room)
        {
            this._room = room;
            this._users = new ConcurrentDictionary<int, RoomUser>();
            this._pets = new ConcurrentDictionary<int, RoomUser>();
            this._bots = new ConcurrentDictionary<int, RoomUser>();

            this.primaryPrivateUserID = 0;
            this.secondaryPrivateUserID = 0;

            this.petCount = 0;
            this.userCount = 0;
        }

        public void Dispose()
        {
            this._users.Clear();
            this._pets.Clear();
            this._bots.Clear();

            this._users = null;
            this._pets = null;
            this._bots = null;
        }

        public void removeBotByForce(GameClient Session, int BotId, int RoomId)
        {
            Room Room = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(RoomId);
            if (Room == null)
                return;

            RoomUser BotUser = null;
            if (!Room.GetRoomUserManager().TryGetBot(BotId, out BotUser))
                return;

            Room.GetGameMap().RemoveUserFromMap(BotUser, new System.Drawing.Point(BotUser.X, BotUser.Y));
        }

        public RoomUser DeployBot(RoomBot Bot, Pet PetData)
        {
            var BotUser = new RoomUser(0, _room.RoomId, primaryPrivateUserID++, _room);
            Bot.VirtualId = primaryPrivateUserID;

            int PersonalID = secondaryPrivateUserID++;
            BotUser.InternalRoomID = PersonalID;
            _users.TryAdd(PersonalID, BotUser);

            DynamicRoomModel Model = _room.GetGameMap().Model;

            if ((Bot.X > 0 && Bot.Y > 0) && Bot.X < Model.MapSizeX && Bot.Y < Model.MapSizeY)
            {
                BotUser.SetPos(Bot.X, Bot.Y, Bot.Z);
                BotUser.SetRot(Bot.Rot, false);
            }
            else
            {
                Bot.X = Model.DoorX;
                Bot.Y = Model.DoorY;

                BotUser.SetPos(Model.DoorX, Model.DoorY, Model.DoorZ);
                BotUser.SetRot(Model.DoorOrientation, false);
            }

            BotUser.BotData = Bot;
            BotUser.BotAI = Bot.GenerateBotAI(BotUser.VirtualId);

            if (BotUser.IsPet)
            {
                BotUser.BotAI.Init(Bot.BotId, BotUser.VirtualId, _room.RoomId, BotUser, _room);
                BotUser.PetData = PetData;
                BotUser.PetData.VirtualId = BotUser.VirtualId;
            }
            else
                BotUser.BotAI.Init(Bot.BotId, BotUser.VirtualId, _room.RoomId, BotUser, _room);

            //UpdateUserStatus(BotUser, false);
            BotUser.UpdateNeeded = true;

            _room.SendMessage(new UsersComposer(BotUser));

            if (BotUser.IsPet)
            {
                if (_pets.ContainsKey(BotUser.PetData.PetId)) //Pet allready placed
                    _pets[BotUser.PetData.PetId] = BotUser;
                else
                    _pets.TryAdd(BotUser.PetData.PetId, BotUser);

                petCount++;
            }
            else if (BotUser.IsBot)
            {
                if (_bots.ContainsKey(BotUser.BotData.BotId))
                    _bots[BotUser.BotData.BotId] = BotUser;
                else
                    _bots.TryAdd(BotUser.BotData.Id, BotUser);
                _room.SendMessage(new DanceComposer(BotUser, BotUser.BotData.DanceId));
            }
            return BotUser;
        }

        public void RemoveBot(int VirtualId, bool Kicked)
        {
            RoomUser User = GetRoomUserByVirtualId(VirtualId);
            if (User == null || !User.IsBot)
                return;

            if (User.IsPet)
            {
                RoomUser PetRemoval = null;

                _pets.TryRemove(User.PetData.PetId, out PetRemoval);
                petCount--;
            }
            else
            {
                RoomUser BotRemoval = null;
                _bots.TryRemove(User.BotData.Id, out BotRemoval);
            }

            User.BotAI.OnSelfLeaveRoom(Kicked);

            _room.SendMessage(new UserRemoveComposer(User.VirtualId));

            RoomUser toRemove;

            if (_users != null)
                _users.TryRemove(User.InternalRoomID, out toRemove);

            onRemove(User);
        }

        public RoomUser GetUserForSquare(int x, int y)
        {
            return _room.GetGameMap().GetRoomUsers(new Point(x, y)).FirstOrDefault();
        }

        public bool AddAvatarToRoom(GameClient Session)
        {
            if (_room == null)
                return false;

            if (Session == null)
                return false;

            if (Session.GetHabbo().CurrentRoom == null)
                return false;

            #region Old Stuff
            RoomUser User = new RoomUser(Session.GetHabbo().Id, _room.RoomId, primaryPrivateUserID++, _room);

            if (User == null || User.GetClient() == null)
                return false;

            User.UserId = Session.GetHabbo().Id;

            Session.GetHabbo().TentId = 0;

            int PersonalID = secondaryPrivateUserID++;
            User.InternalRoomID = PersonalID;


            Session.GetHabbo().CurrentRoomId = _room.RoomId;
            if (!this._users.TryAdd(PersonalID, User))
                return false;
            #endregion

            DynamicRoomModel Model = _room.GetGameMap().Model;
            if (Model == null)
                return false;

            if (!_room.PetMorphsAllowed && Session.GetHabbo().PetId != 0)
                Session.GetHabbo().PetId = 0;

            if (Session.GetRoleplay().Prison && _room.Id != 99 && _room.Id != 151)
            {
                Session.GetRoleplay().SendToPrisonChair();
            }
            else if (Session.GetRoleplay().IsInCourt)
            {
                User.SetPos(3, 18, Model.DoorZ);
                User.SetRot(2, false);
            }
            else if (!User.IsBot && !User.IsPet && User.GetClient().GetRoleplay().Escort)
            {
                #region user
                var user = GetRoomUserByHabbo(User.GetClient().GetRoleplay().EscortBy);
                int x = 0;
                int y = 0;
                if (user.RotBody == 4)
                {
                    x = user.X;
                    y = user.Y + 1;
                }
                if (user.RotBody == 6)
                {
                    x = user.X - 1;
                    y = user.Y;
                }
                if (user.RotBody == 0)
                {
                    x = user.X;
                    y = user.Y - 1;
                }
                if (user.RotBody == 2)
                {
                    x = user.X + 1;
                    y = user.Y;
                }
                if (user.RotBody == 5)
                {
                    x = user.X - 1;
                    y = user.Y + 1;
                }
                if (user.RotBody == 1)
                {
                    x = user.X + 1;
                    y = user.Y - 1;
                }
                if (user.RotBody == 3)
                {
                    x = user.X + 1;
                    y = user.Y + 1;
                }
                if (user.RotBody == 7)
                {
                    x = user.X - 1;
                    y = user.Y - 1;
                }
                if (!_room.GetGameMap().Path(x, y, User) && !user.GetClient().GetHabbo().InRoom)
                {
                    x = Model.DoorX;
                    y = Model.DoorY;
                }
                User.SetPos(x, y, Model.DoorZ);
                User.SetRot(user.RotBody, false);
                #endregion
            }
            else if (!Session.GetHabbo().IsTeleporting && !Session.GetHabbo().IsHopping)
            {
                if (!Model.DoorIsValid())
                {
                    Point Square = _room.GetGameMap().getRandomWalkableSquare();
                    Model.DoorX = Square.X;
                    Model.DoorY = Square.Y;
                    Model.DoorZ = _room.GetGameMap().GetHeightForSquareFromData(Square);
                }

                User.SetPos(Model.DoorX, Model.DoorY, Model.DoorZ);
                User.SetRot(Model.DoorOrientation, false);
            }
            else if (!User.IsBot && (User.GetClient().GetHabbo().IsTeleporting || User.GetClient().GetHabbo().IsHopping))
            {
                Item Item = null;
                if (Session.GetHabbo().IsTeleporting)
                    Item = _room.GetRoomItemHandler().GetItem(Session.GetHabbo().TeleporterId);
                else if (Session.GetHabbo().IsHopping)
                    Item = _room.GetRoomItemHandler().GetItem(Session.GetHabbo().HopperId);

                if (Item != null)
                {
                    if (Session.GetHabbo().IsTeleporting)
                    {
                        Item.ExtraData = "2";
                        Item.UpdateState(false, true);
                        User.SetPos(Item.GetX, Item.GetY, Item.GetZ);
                        User.SetRot(Item.Rotation, false);
                        Item.InteractingUser2 = Session.GetHabbo().Id;
                        Item.ExtraData = "1";
                        Item.UpdateState(false, true);
                    }
                    else if (Session.GetHabbo().IsHopping)
                    {
                        Item.ExtraData = "1";
                        Item.UpdateState(false, true);
                        User.SetPos(Item.GetX, Item.GetY, Item.GetZ);
                        User.SetRot(Item.Rotation, false);
                        User.AllowOverride = false;
                        Item.InteractingUser2 = Session.GetHabbo().Id;
                        Item.ExtraData = "2";
                        Item.UpdateState(false, true);
                    }
                }
                else
                {
                    User.SetPos(Model.DoorX, Model.DoorY, Model.DoorZ - 1);
                    User.SetRot(Model.DoorOrientation, false);
                }
            }

            if (Session.GetHabbo().Working)
            {
                if (Session.GetHabbo().JobId == 1 && Session.GetHabbo().Working)
                {

                }
                else if (Session.GetHabbo().JobId == 2 && Session.GetHabbo().Working == true && Session.GetHabbo().UsingParamedic == false)
                {
                    Session.GetHabbo().Working = false;
                    Session.GetHabbo().resetAvatarEvent();
                }
                else if (Session.GetHabbo().JobId == 4 && Session.GetHabbo().Working && Session.GetHabbo().RankId == 1 && (Session.GetHabbo().CurrentRoomId == 75) | (Session.GetHabbo().CurrentRoomId == 104))
                {

                }
                else if (Session.GetHabbo().JobId == 5 && Session.GetHabbo().Working && Session.GetHabbo().RankId == 1 && (Session.GetHabbo().CurrentRoomId == 71) | (Session.GetHabbo().CurrentRoomId == 70))
                {

                }
                else if (Session.GetHabbo().JobId == 3 && Session.GetHabbo().Working)
                {
                    if (Session.GetHabbo().ClaimedATM == 0)
                    {
                        Session.GetHabbo().Working = false;
                        Session.GetHabbo().resetAvatarEvent();
                    }
                }
                else
                {
                    Session.GetHabbo().Working = false;
                    Session.GetHabbo().resetAvatarEvent();
                }
            }


            Session.GetHabbo().CanChangeRoom = false;

            this.UpdateUserStatusses();

            _room.SendMessage(new UsersComposer(User));

            //Below = done
            if (_room.CheckRights(Session, true))
            {
                User.SetStatus("flatctrl", "useradmin");
                Session.SendMessage(new YouAreOwnerComposer());
                Session.SendMessage(new YouAreControllerComposer(4));
            }
            else if (_room.CheckRights(Session, false) && _room.Group == null)
            {
                User.SetStatus("flatctrl", "1");
                Session.SendMessage(new YouAreControllerComposer(1));
            }
            else if (_room.Group != null && _room.CheckRights(Session, false, true))
            {
                User.SetStatus("flatctrl", "3");
                Session.SendMessage(new YouAreControllerComposer(3));
            }
            else
                Session.SendMessage(new YouAreNotControllerComposer());

            User.UpdateNeeded = true;

            foreach (RoomUser Bot in this._bots.Values.ToList())
            {
                if (Bot == null || Bot.BotAI == null)
                    continue;

                Bot.BotAI.OnUserEnterRoom(User);
            }
            return true;
        }

        public void RemoveUserFromRoom(GameClient Session, Boolean NotifyClient, Boolean NotifyKick = false)
        {
            try
            {
                if (_room == null || Session == null || Session.GetHabbo() == null)
                {
                    return;
                }

                if (NotifyKick)
                    Session.SendMessage(new GenericErrorComposer(4008));

                if (NotifyClient)
                    Session.SendMessage(new CloseConnectionComposer());

                if (Session.GetHabbo().TentId > 0)
                    Session.GetHabbo().TentId = 0;

                RoomUser User = GetRoomUserByHabbo(Session.GetHabbo().Id);
                if (User != null)
                {
                    if (User.RidingHorse)
                    {
                        User.RidingHorse = false;
                        RoomUser UserRiding = GetRoomUserByVirtualId(User.HorseID);
                        if (UserRiding != null)
                        {
                            UserRiding.RidingHorse = false;
                            UserRiding.HorseID = 0;
                        }
                    }

                    if (User.Team != TEAM.NONE)
                    {
                        TeamManager Team = this._room.GetTeamManagerForFreeze();
                        if (Team != null)
                        {
                            Team.OnUserLeave(User);

                            User.Team = TEAM.NONE;

                            if (User.GetClient().GetHabbo().Effects().CurrentEffect != 0)
                                User.GetClient().GetHabbo().Effects().ApplyEffect(0);
                        }
                    }
                    #region duel
                    if (!User.IsBot && User.DuelUser != null)
                    {
                        User.setDuelWinner();
                    }
                    #endregion
                    #region casier
                    if (!User.IsBot && User.usingCasier)
                    {
                        User.usingCasier = false;
                        User.GetClient().Shout("*Locks his locker with his padlock*");
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "casier;hide");
                    }
                    #endregion
                    #region fish
                    if (!User.IsBot && User.usingFish)
                    {
                        User.usingFish = false;
                        User.GetClient().Shout("*Stops fishing*");
                        User.ApplyEffect(0);

                    }
                    #endregion
                    #region Sellfish
                    if (!User.IsBot && User.usingSellfish)
                    {
                        User.usingSellfish = false;
                        User.GetClient().Shout("*Close the laptop*");
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), "sellfish;close");
                    }
                    #endregion
                    #region SellWeapon
                    if (!User.IsBot && User.usingSellWeapon)
                    {
                        User.usingSellWeapon = false;
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), "sellweapon;close");
                    }
                    #endregion
                    #region PhoneStore
                    if (!User.IsBot && User.usingPhoneStore)
                    {
                        User.usingPhoneStore = false;
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), "phonestore;close");
                    }
                    #endregion
                    #region PhoneCreditStore
                    if (!User.IsBot && User.usingPhoneCreditStore)
                    {
                        User.usingPhoneCreditStore = false;
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), "phonecreditstore;close");
                    }
                    #endregion
                    #region CasinoChips
                    if (!User.IsBot && User.usingCasinoChips)
                    {
                        User.usingCasinoChips = false;
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), "casinochips;close");
                    }
                    #endregion
                    #region taxi
                    if (!User.IsBot && User.usingTaxi)
                    {
                        User.usingTaxi = false;
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "taxi;close");
                        User.GetClient().Shout("*cancels their taxi*", 0);
                        User.ApplyEffect(0);

                    }
                    #endregion
                    #region Shop
                    if (!User.IsBot && User.usingShop)
                    {
                        User.usingShop = false;
                        User.GetClient().Shout("*Closes the PC*");
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), "shop;close");

                    }
                    #endregion
                    #region Farm
                    if (!User.IsBot && User.usingFarm)
                    {
                        User.usingFarm = false;
                        User.GetClient().Shout("*Stops farming*");
                        User.ApplyEffect(0);

                    }
                    #endregion
                    #region Crosstrainer
                    if (!User.IsBot && User.GetClient().GetHabbo().usingCrosstrainer)
                    {
                        User.GetClient().GetHabbo().usingCrosstrainer = false;
                        User.GetClient().SendWhisper("You stop working out on the Cross Trainer");
                        User.ApplyEffect(0);
                    }
                    #endregion
                    #region Bank
                    if (!User.IsBot && User.usingBank)
                    {
                        User.usingBank = false;


                    }
                    #endregion


                    RemoveRoomUser(User);

                    if (User.CurrentItemEffect != ItemEffectType.NONE)
                    {
                        if (Session.GetHabbo().Effects() != null)
                            Session.GetHabbo().Effects().CurrentEffect = -1;
                    }

                    if (_room != null)
                    {
                        if (_room.HasActiveTrade(Session.GetHabbo().Id))
                            _room.TryStopTrade(Session.GetHabbo().Id);
                    }

                    if (Session.GetHabbo().GetMessenger() != null)
                        Session.GetHabbo().GetMessenger().OnStatusChanged(true);

                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `rooms` SET `users_now` = '" + _room.UsersNow + "' WHERE `id` = '" + _room.RoomId + "' LIMIT 1");
                    }

                    if (User != null)
                        User.Dispose();
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e.ToString());
            }
        }

        private void onRemove(RoomUser user)
        {
            try
            {

                GameClient session = user.GetClient();
                if (session == null)
                    return;

                List<RoomUser> Bots = new List<RoomUser>();

                try
                {
                    foreach (RoomUser roomUser in GetUserList().ToList())
                    {
                        if (roomUser == null)
                            continue;

                        if (roomUser.IsBot && !roomUser.IsPet)
                        {
                            if (!Bots.Contains(roomUser))
                                Bots.Add(roomUser);
                        }
                    }
                }
                catch { }

                List<RoomUser> PetsToRemove = new List<RoomUser>();
                foreach (RoomUser Bot in Bots.ToList())
                {
                    if (Bot == null || Bot.BotAI == null)
                        continue;

                    Bot.BotAI.OnUserLeaveRoom(session);

                    if (Bot.IsPet && Bot.PetData.OwnerId == user.UserId && !_room.CheckRights(session, true))
                    {
                        if (!PetsToRemove.Contains(Bot))
                            PetsToRemove.Add(Bot);
                    }
                }

                foreach (RoomUser toRemove in PetsToRemove.ToList())
                {
                    if (toRemove == null)
                        continue;

                    if (user.GetClient() == null || user.GetClient().GetHabbo() == null || user.GetClient().GetHabbo().GetInventoryComponent() == null)
                        continue;

                    user.GetClient().GetHabbo().GetInventoryComponent().TryAddPet(toRemove.PetData);
                    RemoveBot(toRemove.VirtualId, false);
                }

                _room.GetGameMap().RemoveUserFromMap(user, new Point(user.X, user.Y));
            }
            catch (Exception e)
            {
                Logging.LogCriticalException(e.ToString());
            }
        }

        private void RemoveRoomUser(RoomUser user)
        {
            if (user.SetStep)
            {
                _room.GetGameMap().GameMap[user.SetX, user.SetY] = user.SqState;
            }
            else
            {
                _room.GetGameMap().GameMap[user.X, user.Y] = user.SqState;
            }
            _room.GetGameMap().RemoveUserFromMap(user, new Point(user.X, user.Y));
            _room.SendMessage(new UserRemoveComposer(user.VirtualId));
            RoomUser toRemove = null;
            if (_users.TryRemove(user.InternalRoomID, out toRemove))
            {
            }
            user.InternalRoomID = -1;
            onRemove(user);
        }

        public bool TryGetPet(int PetId, out RoomUser Pet)
        {
            return this._pets.TryGetValue(PetId, out Pet);
        }

        public bool TryGetBot(int BotId, out RoomUser Bot)
        {
            return this._bots.TryGetValue(BotId, out Bot);
        }

        public RoomUser GetBotByName(string Name)
        {
            bool FoundBot = this._bots.Where(x => x.Value.BotData != null && x.Value.BotData.Name.ToLower() == Name.ToLower()).ToList().Count() > 0;
            if (FoundBot)
            {
                int Id = this._bots.FirstOrDefault(x => x.Value.BotData != null && x.Value.BotData.Name.ToLower() == Name.ToLower()).Value.BotData.Id;

                return this._bots[Id];
            }

            return null;
        }

        public void UpdateUserCount(int count)
        {
            userCount = count;
            _room.RoomData.UsersNow = count;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `rooms` SET `users_now` = '" + count + "' WHERE `id` = '" + _room.RoomId + "' LIMIT 1");
            }
        }

        public RoomUser GetRoomUserByVirtualId(int VirtualId)
        {
            RoomUser User = null;
            if (!_users.TryGetValue(VirtualId, out User))
                return null;
            return User;
        }

        public RoomUser GetRoomUserByHabbo(int Id)
        {
            if (this == null)
                return null;

            RoomUser User = this.GetUserList().Where(x => x != null && x.GetClient() != null && x.GetClient().GetHabbo() != null && x.GetClient().GetHabbo().Id == Id).FirstOrDefault();

            if (User != null)
                return User;

            return null;
        }

        public List<RoomUser> GetRoomUsers()
        {
            List<RoomUser> List = new List<RoomUser>();

            List = this.GetUserList().Where(x => (!x.IsBot)).ToList();

            return List;
        }

        public List<RoomUser> GetRoomUserByRank(int minRank)
        {
            var returnList = new List<RoomUser>();
            foreach (RoomUser user in GetUserList().ToList())
            {
                if (user == null)
                    continue;

                if (!user.IsBot && user.GetClient() != null && user.GetClient().GetHabbo() != null && user.GetClient().GetHabbo().Rank >= minRank)
                    returnList.Add(user);
            }

            return returnList;
        }

        public RoomUser GetRoomUserByHabbo(string pName)
        {
            RoomUser User = this.GetUserList().Where(x => x != null && x.GetClient() != null && x.GetClient().GetHabbo() != null && x.GetClient().GetHabbo().Username.Equals(pName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            if (User != null)
                return User;

            return null;
        }

        public void UpdatePets()
        {
            foreach (Pet Pet in GetPets().ToList())
            {
                if (Pet == null)
                    continue;

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    if (Pet.DBState == DatabaseUpdateState.NeedsInsert)
                    {
                        dbClient.SetQuery("INSERT INTO `bots` (`id`,`user_id`,`room_id`,`name`,`x`,`y`,`z`) VALUES ('" + Pet.PetId + "','" + Pet.OwnerId + "','" + Pet.RoomId + "',@name,'0','0','0')");
                        dbClient.AddParameter("name", Pet.Name);
                        dbClient.RunQuery();

                        dbClient.SetQuery("INSERT INTO `bots_petdata` (`type`,`race`,`color`,`experience`,`energy`,`createstamp`,`nutrition`,`respect`) VALUES ('" + Pet.Type + "',@race,@color,'0','100','" + Pet.CreationStamp + "','0','0')");
                        dbClient.AddParameter(Pet.PetId + "race", Pet.Race);
                        dbClient.AddParameter(Pet.PetId + "color", Pet.Color);
                        dbClient.RunQuery();
                    }
                    else if (Pet.DBState == DatabaseUpdateState.NeedsUpdate)
                    {
                        //Surely this can be *99 better?
                        RoomUser User = GetRoomUserByVirtualId(Pet.VirtualId);

                        dbClient.RunQuery("UPDATE `bots` SET room_id = " + Pet.RoomId + ", x = " + (User != null ? User.X : 0) + ", Y = " + (User != null ? User.Y : 0) + ", Z = " + (User != null ? User.Z : 0) + " WHERE `id` = '" + Pet.PetId + "' LIMIT 1");
                        dbClient.RunQuery("UPDATE `bots_petdata` SET `experience` = '" + Pet.experience + "', `energy` = '" + Pet.Energy + "', `nutrition` = '" + Pet.Nutrition + "', `respect` = '" + Pet.Respect + "' WHERE `id` = '" + Pet.PetId + "' LIMIT 1");
                    }

                    Pet.DBState = DatabaseUpdateState.Updated;
                }
            }
        }

        public List<Pet> GetPets()
        {
            List<Pet> Pets = new List<Pet>();
            foreach (RoomUser User in this._pets.Values.ToList())
            {
                if (User == null || !User.IsPet)
                    continue;

                Pets.Add(User.PetData);
            }

            return Pets;
        }

        public void SerializeStatusUpdates()
        {
            List<RoomUser> Users = new List<RoomUser>();
            ICollection<RoomUser> RoomUsers = GetUserList();

            if (RoomUsers == null)
                return;

            foreach (RoomUser User in RoomUsers.ToList())
            {
                if (User == null || !User.UpdateNeeded || Users.Contains(User))
                    continue;

                User.UpdateNeeded = false;
                Users.Add(User);
            }

            if (Users.Count > 0)
                _room.SendMessage(new UserUpdateComposer(Users));
        }

        public void UpdateUserStatusses()
        {
            foreach (RoomUser user in GetUserList().ToList())
            {
                if (user == null)
                    continue;

                UpdateUserStatus(user, false);
            }
        }

        private bool isValid(RoomUser user)
        {
            if (user == null)
                return false;
            if (user.IsBot)
                return true;
            if (user.GetClient() == null)
                return false;
            if (user.GetClient().GetHabbo() == null)
                return false;
            if (user.GetClient().GetHabbo().CurrentRoomId != _room.RoomId)
                return false;
            return true;
        }

        public void OnCycle()
        {
            int userCounter = 0;

            try
            {
                foreach (RoomUser User in GetUserList().ToList())
                {
                    if (User == null)
                        continue;

                    if (!isValid(User))
                    {
                        if (User.GetClient() != null)
                            RemoveUserFromRoom(User.GetClient(), false, false);
                        else
                            RemoveRoomUser(User);
                    }

                    if (!User.IsBot && !User.IsPet)
                        User.GetClient().GetRoleplay().RPTimer.OnCycle();

                    if (!User.IsBot && User.jailgate > 0)
                    {
                        User.jailgate_timer--;
                        if (User.jailgate_timer < 1)
                        {
                            Item Item = _room.GetRoomItemHandler().GetItem(User.jailgate);
                            if (Item != null)
                            {
                                int Distance = Math.Abs(User.X - Item.GetX) + Math.Abs(User.Y - Item.GetY);
                                if (Distance == 0)
                                    User.jailgate_timer = 2;
                                else
                                {
                                    Item.ExtraData = "0";
                                    Item.UpdateState(false, true);
                                    User.jailgate = 0;
                                }
                            }
                            else
                            {
                                User.jailgate = 0;
                                User.jailgate_timer = 0;
                            }
                            /*if (!User.IsBot && !User.IsPet)
                            {
                                User.GetClient().GetHabbo().CanUseJailGate = false;
                            }*/
                        }
                    }

                    bool updated = false;
                    User.IdleTime++;
                    User.HandleSpamTicks();
                    if (!User.IsBot && !User.IsAsleep && User.IdleTime >= 600)
                    {
                        User.IsAsleep = true;
                        _room.SendMessage(new SleepComposer(User, true));

                        if (User.GetClient().GetHabbo().Working == true)
                        {
                            User.Say("falls asleep on the job ...ZZzzz...", 7);
                            User.GetClient().GetHabbo().stopWork();
                        }
                    }

                    if (User.CarryItemID > 0)
                    {
                        User.CarryTimer--;
                        if (User.CarryTimer <= 0)
                            User.CarryItem(0);
                    }

                    if (_room.GotFreeze())
                        _room.GetFreeze().CycleUser(User);

                    bool InvalidStep = false;

                    if (User.isRolling)
                    {
                        if (User.rollerDelay <= 0)
                        {
                            User.isBruling = false;
                            UpdateUserStatus(User, false);
                            User.isRolling = false;
                        }
                        else
                            User.rollerDelay--;
                    }

                    if (User.SetStep)
                    {
                        if (_room.GetGameMap().IsValidStep2(User, new Vector2D(User.X, User.Y), new Vector2D(User.SetX, User.SetY), (User.GoalX == User.SetX && User.GoalY == User.SetY), User.AllowOverride))
                        {
                            if (!User.RidingHorse)
                                _room.GetGameMap().UpdateUserMovement(new Point(User.Coordinate.X, User.Coordinate.Y), new Point(User.SetX, User.SetY), User);

                            List<Item> items = _room.GetGameMap().GetCoordinatedItems(new Point(User.X, User.Y));
                            foreach (Item Item in items.ToList())
                            {
                                Item.UserWalksOffFurni(User);
                            }

                            if (!User.IsBot)
                            {
                                User.X = User.SetX;
                                User.Y = User.SetY;
                                User.Z = User.SetZ;
                            }
                            else if (User.IsBot && !User.RidingHorse)
                            {
                                User.X = User.SetX;
                                User.Y = User.SetY;
                                User.Z = User.SetZ;
                            }

                            if (!User.IsBot && User.RidingHorse)
                            {
                                RoomUser Horse = GetRoomUserByVirtualId(User.HorseID);
                                if (Horse != null)
                                {
                                    Horse.X = User.SetX;
                                    Horse.Y = User.SetY;
                                }
                            }

                            List<Item> Items = _room.GetGameMap().GetCoordinatedItems(new Point(User.X, User.Y));
                            foreach (Item Item in Items.ToList())
                            {
                                Item.UserWalksOnFurni(User);
                            }

                            UpdateUserStatus(User, true);
                        }
                        else
                            InvalidStep = true;
                        User.SetStep = false;
                    }

                    if (User.PathRecalcNeeded)
                    {
                        if (User.Path.Count > 1)
                            User.Path.Clear();

                        User.Path = PathFinder.FindPath(User, this._room.GetGameMap().DiagonalEnabled, this._room.GetGameMap(), new Vector2D(User.X, User.Y), new Vector2D(User.GoalX, User.GoalY));

                        if (User.Path.Count > 1)
                        {
                            User.PathStep = 1;
                            User.IsWalking = true;
                            User.PathRecalcNeeded = false;
                        }
                        else
                        {
                            User.PathRecalcNeeded = false;
                            if (User.Path.Count > 1)
                                User.Path.Clear();
                        }
                    }

                    if (User.IsWalking && !User.Freezed)
                    //if (User.IsWalking && !User.Freezed && User.GetClient().GetHabbo().Hospital == 0 && !User.Disconnect && (User.Transaction == null || User.Transaction != null && User.Transaction.StartsWith("duel")) && User.isDemarreCar == false && User.isFarmingRock == 0 && User.usernameCoiff == null && User.Tased == false && User.isTradingItems == false && User.isDisconnecting == false)
                    {
                        if (InvalidStep || (User.PathStep >= User.Path.Count) || (User.GoalX == User.X && User.GoalY == User.Y)) //No path found, or reached goal (:
                        {
                            User.IsWalking = false;
                            User.RemoveStatus("mv");

                            if (!User.IsBot && !User.IsPet && User.GetClient().GetRoleplay().Escorting)
                            {
                                User.GetClient().GetRoleplay().EscortMovement(User.X, User.Y, User.RotBody);
                            }

                            if (User.Statusses.ContainsKey("sign"))
                                User.RemoveStatus("sign");

                            if (User.IsBot && User.BotData.TargetUser > 0)
                            {
                                if (User.CarryItemID > 0)
                                {
                                    RoomUser Target = _room.GetRoomUserManager().GetRoomUserByHabbo(User.BotData.TargetUser);

                                    if (Target != null && Gamemap.TilesTouching(User.X, User.Y, Target.X, Target.Y))
                                    {
                                        User.SetRot(Rotation.Calculate(User.X, User.Y, Target.X, Target.Y), false);
                                        Target.SetRot(Rotation.Calculate(Target.X, Target.Y, User.X, User.Y), false);
                                        Target.CarryItem(User.CarryItemID);
                                    }
                                }

                                User.CarryItem(0);
                                User.BotData.TargetUser = 0;
                            }

                            if (User.RidingHorse && User.IsPet == false && !User.IsBot)
                            {
                                RoomUser mascotaVinculada = GetRoomUserByVirtualId(User.HorseID);
                                if (mascotaVinculada != null)
                                {
                                    mascotaVinculada.IsWalking = false;
                                    mascotaVinculada.RemoveStatus("mv");
                                    mascotaVinculada.UpdateNeeded = true;
                                }
                            }
                        }
                        else
                        {
                            Vector2D NextStep = User.Path[(User.Path.Count - User.PathStep) - 1];
                            User.PathStep++;

                            if (User.FastWalking && User.PathStep < User.Path.Count)
                            {
                                int s2 = (User.Path.Count - User.PathStep) - 1;
                                NextStep = User.Path[s2];
                                User.PathStep++;
                            }

                            if (User.SuperFastWalking && User.PathStep < User.Path.Count)
                            {
                                int s2 = (User.Path.Count - User.PathStep) - 1;
                                NextStep = User.Path[s2];
                                User.PathStep++;
                                User.PathStep++;
                            }

                            if (User.UltraFastWalking && User.PathStep < User.Path.Count)
                            {
                                int s2 = (User.Path.Count - User.PathStep) - 1;
                                NextStep = User.Path[s2];
                                User.PathStep++;
                                User.PathStep++;
                                User.PathStep++;
                                User.PathStep++;
                                User.PathStep++;
                            }

                            #region prison gate
                            if (!User.IsBot && User.GetClient().GetHabbo().JobId == 1 && User.GetClient().GetHabbo().Working || !User.IsBot && User.GetClient().GetRoleplay().Escort)
                            {
                                foreach (Item Item in _room.GetRoomItemHandler().GetFurniObjects(NextStep.X, NextStep.Y).ToList())
                                {
                                    if (Item.BaseItem == 362)
                                    {
                                        Item.ExtraData = "1";
                                        Item.UpdateState(false, true);
                                        if (User.jailgate > 0)
                                        {
                                            Item item = _room.GetRoomItemHandler().GetItem(User.jailgate);
                                            if (Item != null)
                                            {
                                                item.ExtraData = "0";
                                                item.UpdateState(false, true);
                                                User.jailgate = 0;
                                                User.jailgate_timer = 0;
                                            }
                                        }
                                        User.jailgate = Item.Id;
                                        User.jailgate_timer = 2;
                                    }
                                }
                            }
                            #endregion

                            int nextX = NextStep.X;
                            int nextY = NextStep.Y;
                            User.RemoveStatus("mv");

                            if (_room.GetGameMap().IsValidStep2(User, new Vector2D(User.X, User.Y), new Vector2D(nextX, nextY), (User.GoalX == nextX && User.GoalY == nextY), User.AllowOverride))
                            {
                                double nextZ = _room.GetGameMap().SqAbsoluteHeight(nextX, nextY);

                                if (!User.IsBot)
                                {
                                    if (User.isSitting)
                                    {
                                        User.Statusses.Remove("sit");
                                        User.Z += 0.35;
                                        User.isSitting = false;
                                        User.UpdateNeeded = true;
                                    }
                                    else if (User.isLying)
                                    {
                                        User.Statusses.Remove("sit");
                                        User.Z += 0.35;
                                        User.isLying = false;
                                        User.UpdateNeeded = true;
                                    }
                                }
                                if (!User.IsBot)
                                {
                                    User.Statusses.Remove("lay");
                                    User.Statusses.Remove("sit");
                                }

                                if (!User.IsBot && !User.IsPet && User.GetClient() != null)
                                {
                                    if (User.GetClient().GetHabbo().IsTeleporting)
                                    {
                                        User.GetClient().GetHabbo().IsTeleporting = false;
                                        User.GetClient().GetHabbo().TeleporterId = 0;
                                    }
                                    else if (User.GetClient().GetHabbo().IsHopping)
                                    {
                                        User.GetClient().GetHabbo().IsHopping = false;
                                        User.GetClient().GetHabbo().HopperId = 0;
                                    }
                                }

                                if (!User.IsBot && User.RidingHorse && User.IsPet == false)
                                {
                                    RoomUser Horse = GetRoomUserByVirtualId(User.HorseID);
                                    if (Horse != null)
                                        Horse.AddStatus("mv", nextX + "," + nextY + "," + TextHandling.GetString(nextZ));

                                    User.AddStatus("mv", + nextX + "," + nextY + "," + TextHandling.GetString(nextZ + 1));

                                    User.UpdateNeeded = true;
                                    Horse.UpdateNeeded = true;
                                }
                                else
                                    User.AddStatus("mv", nextX + "," + nextY + "," + TextHandling.GetString(nextZ));

                                int newRot = Rotation.Calculate(User.X, User.Y, nextX, nextY, User.moonwalkEnabled);

                                if (!User.GetClient().GetRoleplay().Escort)
                                {
                                    User.RotBody = newRot;
                                    User.RotHead = newRot;
                                }

                                User.SetStep = true;
                                User.SetX = nextX;
                                User.SetY = nextY;
                                User.SetZ = nextZ;
                                UpdateUserEffect(User, User.SetX, User.SetY);

                                updated = true;

                                #region LVPD Escort Movement
                                if (!User.IsBot && !User.IsPet && User.GetClient().GetRoleplay().Escorting)
                                {
                                    User.GetClient().GetRoleplay().EscortMovement(nextX, nextY, newRot);
                                }
                                #endregion

                                #region Paramedic Escort Movement
                                if (!User.IsBot && User.GetClient().GetHabbo().ParamedicUsername != null)
                                {
                                    var user = GetRoomUserByHabbo(User.GetClient().GetHabbo().ParamedicUsername);
                                    RoomUser UserMenotted = GetRoomUserByHabbo(User.GetClient().GetHabbo().ParamedicUsername);

                                    var roomUser = User;
                                    if (User.AllowOverride != true)
                                        User.AllowOverride = true;

                                    if (user.AllowOverride != true)
                                        user.AllowOverride = true;

                                    if (newRot == 0)
                                        user.MoveTo(roomUser.SetX, roomUser.SetY + 1);
                                    else if (newRot == 1)
                                        user.MoveTo(roomUser.SetX - 1, roomUser.SetY + 1);
                                    else if (newRot == 2)
                                        user.MoveTo(roomUser.SetX - 1, roomUser.SetY);
                                    else if (newRot == 3)
                                        user.MoveTo(roomUser.SetX - 1, roomUser.SetY - 1);
                                    else if (newRot == 4)
                                        user.MoveTo(roomUser.SetX, roomUser.SetY - 1);
                                    else if (newRot == 5)
                                        user.MoveTo(roomUser.SetX + 1, roomUser.SetY - 1);
                                    else if (newRot == 6)
                                        user.MoveTo(roomUser.SetX + 1, roomUser.SetY);
                                    else if (newRot == 7)
                                        user.MoveTo(roomUser.SetX + 1, roomUser.SetY + 1);
                                    if (user.RotBody != User.RotBody)
                                        user.SetRot(User.RotBody, false);
                                }
                                #endregion

                                if (User.RidingHorse && User.IsPet == false && !User.IsBot)
                                {
                                    RoomUser Horse = GetRoomUserByVirtualId(User.HorseID);
                                    if (Horse != null)
                                    {
                                        Horse.RotBody = newRot;
                                        Horse.RotHead = newRot;

                                        Horse.SetStep = true;
                                        Horse.SetX = nextX;
                                        Horse.SetY = nextY;
                                        Horse.SetZ = nextZ;
                                    }
                                }

                                _room.GetGameMap().GameMap[User.X, User.Y] = User.SqState; //Restore the old one
                                User.SqState = _room.GetGameMap().GameMap[User.SetX, User.SetY]; //Backup the new one

                                if (_room.RoomBlockingEnabled == 0)
                                {
                                    RoomUser Users = _room.GetRoomUserManager().GetUserForSquare(nextX, nextY);
                                    if (Users != null)
                                        _room.GetGameMap().GameMap[nextX, nextY] = 0;
                                }
                                else
                                    _room.GetGameMap().GameMap[nextX, nextY] = 1;
                            }
                        }

                        if (!User.RidingHorse)
                            User.UpdateNeeded = true;
                    }
                    else
                    {
                        if (User.Statusses.ContainsKey("mv"))
                        {
                            User.RemoveStatus("mv");
                            User.UpdateNeeded = true;

                            if (User.RidingHorse)
                            {
                                RoomUser Horse = GetRoomUserByVirtualId(User.HorseID);
                                if (Horse != null)
                                {
                                    Horse.RemoveStatus("mv");
                                    Horse.UpdateNeeded = true;
                                }
                            }
                        }
                    }

                    if (User.RidingHorse)
                        User.ApplyEffect(77);

                    if (User.IsBot && User.BotAI != null)
                        User.BotAI.OnTimerTick();
                    else
                        userCounter++;

                    if (!updated)
                    {
                        UpdateUserEffect(User, User.X, User.Y);
                    }
                }

                /*foreach (RoomUser toRemove in ToRemove.ToList())
                {
                    GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(toRemove.HabboId);
                    if (client != null)
                    {
                        RemoveUserFromRoom(client, true, false);
                    }
                    else
                        RemoveRoomUser(toRemove);
                }*/

                if (userCount != userCounter)
                    UpdateUserCount(userCounter);
            }
            catch (Exception e)
            {
                int rId = 0;
                if (_room != null)
                    rId = _room.Id;

                Logging.LogCriticalException("Affected Room - ID: " + rId + " - " + e.ToString());
            }
        }

        public void UpdateUserStatus(RoomUser User, bool cyclegameitems)
        {
            if (User == null)
                return;

            try
            {
                bool isBot = User.IsBot;
                if (isBot)
                    cyclegameitems = false;

                if (PlusEnvironment.GetUnixTimestamp() > PlusEnvironment.GetUnixTimestamp() + User.SignTime)
                {
                    if (User.Statusses.ContainsKey("sign"))
                    {
                        User.Statusses.Remove("sign");
                        User.UpdateNeeded = true;
                    }
                }

                if ((User.Statusses.ContainsKey("lay") && !User.isLying) || (User.Statusses.ContainsKey("sit") && !User.isSitting))
                {
                    if (User.Statusses.ContainsKey("lay"))
                        User.Statusses.Remove("lay");
                    if (User.Statusses.ContainsKey("sit"))
                        User.Statusses.Remove("sit");
                    User.UpdateNeeded = true;
                }
                else if (User.isLying || User.isSitting)
                    return;

                double newZ;
                List<Item> ItemsOnSquare = _room.GetGameMap().GetAllRoomItemForSquare(User.X, User.Y);
                if (ItemsOnSquare != null || ItemsOnSquare.Count != 0)
                {
                    if (User.RidingHorse && User.IsPet == false)
                        newZ = _room.GetGameMap().SqAbsoluteHeight(User.X, User.Y, ItemsOnSquare.ToList()) + 1;
                    else
                        newZ = _room.GetGameMap().SqAbsoluteHeight(User.X, User.Y, ItemsOnSquare.ToList());
                }
                else
                {
                    newZ = 1;
                }

                if (newZ != User.Z)
                {
                    User.Z = newZ;
                    User.UpdateNeeded = true;
                }

                DynamicRoomModel Model = _room.GetGameMap().Model;
                if (Model.SqState[User.X, User.Y] == SquareState.SEAT)
                {
                    if (!User.Statusses.ContainsKey("sit"))
                        User.Statusses.Add("sit", "1.0");
                    User.Z = Model.SqFloorHeight[User.X, User.Y];
                    User.RotHead = Model.SqSeatRot[User.X, User.Y];
                    User.RotBody = Model.SqSeatRot[User.X, User.Y];

                    User.UpdateNeeded = true;
                }

                if (ItemsOnSquare.Count == 0)
                    User.LastItem = null;

                /*if (User.GetClient().GetHabbo().CurrentRoom.Assault > 0)
                {
                    RoomUser ClaimUser = this.GetRoomUserByHabbo(User.GetClient().GetHabbo().CurrentRoom.ClaimUserId);

                    foreach (RoomUser UserInRoom in User.GetClient().GetHabbo().CurrentRoom.GetRoomUserManager().GetUserList().ToList())
                    {
                        if (UserInRoom == null || UserInRoom.IsBot || UserInRoom.GetClient() == null || UserInRoom.GetClient().GetHabbo() == null)
                            continue;

                        if (UserInRoom.GetClient().GetHabbo().Id != ClaimUser.GetClient().GetHabbo().Id && UserInRoom.GetClient().GetHabbo().Gang != ClaimUser.GetClient().GetHabbo().Gang)
                        {
                            PlusEnvironment.StopGangClaim.Add(ClaimUser.GetClient().GetHabbo().CurrentRoomId, ClaimUser.GetClient().GetHabbo().CurrentRoomId);
                        }
                        else
                        {
                            PlusEnvironment.StopGangClaim.Remove(ClaimUser.GetClient().GetHabbo().CurrentRoomId);
                        }
                    }
                }*/

                foreach (Item Item in ItemsOnSquare.ToList())
                {
                    User.Item_On = Item.GetBaseItem().SpriteId;

                    if (Item.GetBaseItem().Id == 99998 && Item.Id == 34497 | Item.Id == 34496 | Item.Id == 117050 | Item.Id == 32382 | Item.Id == 24341 | Item.Id == 24340 | Item.Id == 24339 | Item.Id == 24338)
                    {
                        User.GetClient().GetHabbo().usingArrestActionPoint = true;
                    }


                    if (!User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 76 && Item.GetBaseItem().Id == 99998 && Item.Id == 136718)
                    {
                        User.GetClient().GetHabbo().usingGymMembershipPurchase = true;
                    }

                    if (!User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 117 && Item.GetBaseItem().Id == 146172)
                    {
                        User.GetClient().GetHabbo().UsingCityHallActionPoint = true;
                    }
                    /*if (!User.IsBot && User.GetClient().GetHabbo().TaxiRide)
                    {
                        if (User.X == User.GetRoom().taxiOutX && User.Y == User.GetRoom().taxiOutY)
                        {
                            User.GetClient().GetHabbo().CanChangeRoom = true;
                            User.GetClient().GetHabbo().PrepareRoom(User.GetRoom().taxiNextRoom, "");
                        }
                    }*/

                    RoomUser RoomUser = User.GetClient().GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(User.GetClient().GetHabbo().Id);
                    if (Item.GetBaseItem().Id == 20285)
                    {
                        RoomUser.Say("starts browsing for a hair style");
                        RoomUser.GetClient().GetHabbo().updateAvatarEvent(RoomUser.GetClient().GetHabbo().Look + ".fa-990000490-62.ca-201422-73-62", RoomUser.GetClient().GetHabbo().Look + ".fa-990000490-62.ca-201422-73-62", "");
                    }


                    if (Item.GetBaseItem().Id == 500022)
                    {
                        DataRow StatRow = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `world_event_joins` WHERE `user_id` = @user_id LIMIT 1");
                            dbClient.AddParameter("user_id", User.GetClient().GetHabbo().Id);
                            StatRow = dbClient.getRow();

                            if (StatRow == null)//No row, add it yo
                            {
                                dbClient.RunQuery("INSERT INTO `world_event_joins` (`user_id`) VALUES ('" + User.GetClient().GetHabbo().Id + "')");
                                dbClient.SetQuery("UPDATE world_event_joins SET collected = collected + 1 WHERE user_id = @userId");
                                dbClient.AddParameter("userId", User.GetClient().GetHabbo().Id);
                                dbClient.RunQuery();
                            }
                            else
                            {
                                dbClient.SetQuery("UPDATE world_event_joins SET collected = collected + 1 WHERE user_id = @userId");
                                dbClient.AddParameter("userId", User.GetClient().GetHabbo().Id);
                                dbClient.RunQuery();
                            }
                        }
                        User.GetClient().GetHabbo().CurrentRoom.GetRoomItemHandler().RemoveFurniture(null, Item.Id);
                    }
                  
                    //f21
                    if (!User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 70 && Item.GetBaseItem().Id == 99998 && Item.GetX != 12 && Item.GetY != 19 
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 70 && Item.GetBaseItem().Id == 9998 && Item.GetX != 12 && Item.GetY != 19)
                    {
                        User.usingClothShop = true;
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), "Clothing;open;");
                    }
                    if (!User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 70 && Item.GetBaseItem().Id == 99998 && Item.GetX == 12 && Item.GetY == 19 
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 70 && Item.GetBaseItem().Id == 9998 && Item.GetX == 12 && Item.GetY == 19 
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 94 && Item.Id == 52291 && Item.GetX == 21 && Item.GetY == 15
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 31 && Item.Id == 136611 && Item.GetX == 6 && Item.GetY == 19
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 31 && Item.Id == 136611 && Item.GetX == 7 && Item.GetY == 19
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 31 && Item.Id == 136611 && Item.GetX == 6 && Item.GetY == 20
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 31 && Item.Id == 136611 && Item.GetX == 5 && Item.GetY == 19
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 31 && Item.Id == 136611 && Item.GetX == 6 && Item.GetY == 18
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 29 && Item.Id == 6635 && Item.GetX == 15 && Item.GetY == 9
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 29 && Item.Id == 155853 && Item.GetX == 15 && Item.GetY == 9
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 27 && Item.Id == 29310 && Item.GetX == 10 && Item.GetY == 12
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 27 && Item.Id == 155924 && Item.GetX == 10 && Item.GetY == 12
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 165 && Item.Id == 13119 && Item.GetX == 18 && Item.GetY == 7
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 165 && Item.Id == 155699 && Item.GetX == 18 && Item.GetY == 7
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 163 && Item.Id == 6640 && Item.GetX == 5 && Item.GetY == 18
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 163 && Item.Id == 158427 && Item.GetX == 5 && Item.GetY == 18
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 153 && Item.Id == 15112 && Item.GetX == 12 && Item.GetY == 3
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 153 && Item.Id == 154149 && Item.GetX == 12 && Item.GetY == 3
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 23 && Item.Id == 481 && Item.GetX == 6 && Item.GetY == 16
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 23 && Item.Id == 154204 && Item.GetX == 6 && Item.GetY == 16
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 22 && Item.Id == 20193 && Item.GetX == 8 && Item.GetY == 18
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 22 && Item.Id == 154061 && Item.GetX == 8 && Item.GetY == 18
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 24 && Item.Id == 4929 && Item.GetX == 11 && Item.GetY == 9
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 24 && Item.Id == 154463 && Item.GetX == 11 && Item.GetY == 9
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 142 && Item.Id == 129979 && Item.GetX == 22 && Item.GetY == 13
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 142 && Item.Id == 154695 && Item.GetX == 22 && Item.GetY == 13
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 19 && Item.Id == 85960 && Item.GetX == 17 && Item.GetY == 12
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 19 && Item.Id == 159274 && Item.GetX == 17 && Item.GetY == 12
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 214 && Item.Id == 116630 && Item.GetX == 12 && Item.GetY == 13
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 214 && Item.Id == 153019 && Item.GetX == 12 && Item.GetY == 13
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 14 && Item.Id == 12652 && Item.GetX == 10 && Item.GetY == 13
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 14 && Item.Id == 151982 && Item.GetX == 10 && Item.GetY == 13
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 15 && Item.Id == 115572 && Item.GetX == 4 && Item.GetY == 14
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 15 && Item.Id == 152072 && Item.GetX == 4 && Item.GetY == 14
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 12 && Item.Id == 115916 && Item.GetX == 15 && Item.GetY == 6
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 12 && Item.Id == 151882 && Item.GetX == 15 && Item.GetY == 6
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 13 && Item.Id == 15823 && Item.GetX == 19 && Item.GetY == 13
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 13 && Item.Id == 151469 && Item.GetX == 19 && Item.GetY == 13
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 10 && Item.Id == 6636 && Item.GetX == 13 && Item.GetY == 8
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 10 && Item.Id == 161329 && Item.GetX == 13 && Item.GetY == 8
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 156 && Item.Id == 4930 && Item.GetX == 8 && Item.GetY == 17
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 156 && Item.Id == 158257 && Item.GetX == 8 && Item.GetY == 17
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 160 && Item.Id == 6638 && Item.GetX == 13 && Item.GetY == 7
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 160 && Item.Id == 157434 && Item.GetX == 13 && Item.GetY == 7
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 162 && Item.Id == 189897 && Item.GetX == 9 && Item.GetY == 14
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 162 && Item.Id == 160133 && Item.GetX == 9 && Item.GetY == 14
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 70 && Item.Id == 6639 && Item.GetX == 15 && Item.GetY == 15
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 70 && Item.Id == 156952 && Item.GetX == 15 && Item.GetY == 15
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 30 && Item.Id == 114552 && Item.GetX == 15 && Item.GetY == 5
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 30 && Item.Id == 155799 && Item.GetX == 15 && Item.GetY == 5
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 29 && Item.Id == 6635 && Item.GetX == 15 && Item.GetY == 9
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 29 && Item.Id == 155853 && Item.GetX == 15 && Item.GetY == 9
                        || !User.IsBot && User.GetClient().GetHabbo().CurrentRoomId == 25 && Item.Id == 6639 && Item.GetX == 15 && Item.GetY == 15)
                    {
                        User.TaxiOpen = true;

                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), "taxi;open;");
                    }

                    if (Item == null)
                        continue;

                    if (Item.GetBaseItem().IsSeat)
                    {
                        if (!User.Statusses.ContainsKey("sit"))
                        {
                            if (!User.Statusses.ContainsKey("sit"))
                                User.Statusses.Add("sit", TextHandling.GetString(Item.GetBaseItem().Height));
                        }

                        User.Z = Item.GetZ;
                        User.RotHead = Item.Rotation;
                        User.RotBody = Item.Rotation;
                        User.UpdateNeeded = true;
                    }

                    switch (Item.GetBaseItem().InteractionType)
                    {
                        #region *s & Tents
                        case InteractionType.BED:
                        case InteractionType.TENT_SMALL:
                            {
                                if (!User.Statusses.ContainsKey("lay"))
                                    User.Statusses.Add("lay", TextHandling.GetString(Item.GetBaseItem().Height) + " null");

                                User.Z = Item.GetZ;
                                User.RotHead = Item.Rotation;
                                User.RotBody = Item.Rotation;

                                User.UpdateNeeded = true;
                                break;
                            }
                        #endregion
                        #region Banzai Gates
                        case InteractionType.banzaigategreen:
                        case InteractionType.banzaigateblue:
                        case InteractionType.banzaigatered:
                        case InteractionType.banzaigateyellow:
                            {
                                if (cyclegameitems)
                                {
                                    int effectID = Convert.ToInt32(Item.team + 32);
                                    TeamManager t = User.GetClient().GetHabbo().CurrentRoom.GetTeamManagerForBanzai();

                                    if (User.Team == TEAM.NONE)
                                    {
                                        if (t.CanEnterOnTeam(Item.team))
                                        {
                                            if (User.Team != TEAM.NONE)
                                                t.OnUserLeave(User);
                                            User.Team = Item.team;

                                            t.AddUser(User);

                                            if (User.GetClient().GetHabbo().Effects().CurrentEffect != effectID)
                                                User.GetClient().GetHabbo().Effects().ApplyEffect(effectID);
                                        }
                                    }
                                    else if (User.Team != TEAM.NONE && User.Team != Item.team)
                                    {
                                        t.OnUserLeave(User);
                                        User.Team = TEAM.NONE;
                                        User.GetClient().GetHabbo().Effects().ApplyEffect(0);
                                    }
                                    else
                                    {
                                        //usersOnTeam--;
                                        t.OnUserLeave(User);
                                        if (User.GetClient().GetHabbo().Effects().CurrentEffect == effectID)
                                            User.GetClient().GetHabbo().Effects().ApplyEffect(0);
                                        User.Team = TEAM.NONE;
                                    }
                                    //Item.ExtraData = usersOnTeam.ToString();
                                    //Item.UpdateState(false, true);                                
                                }
                                break;
                            }
                        #endregion
                        #region Freeze Gates
                        case InteractionType.FREEZE_YELLOW_GATE:
                        case InteractionType.FREEZE_RED_GATE:
                        case InteractionType.FREEZE_GREEN_GATE:
                        case InteractionType.FREEZE_BLUE_GATE:
                            {
                                if (cyclegameitems)
                                {
                                    int effectID = Convert.ToInt32(Item.team + 39);
                                    TeamManager t = User.GetClient().GetHabbo().CurrentRoom.GetTeamManagerForFreeze();

                                    if (User.Team == TEAM.NONE)
                                    {
                                        if (t.CanEnterOnTeam(Item.team))
                                        {
                                            if (User.Team != TEAM.NONE)
                                                t.OnUserLeave(User);
                                            User.Team = Item.team;
                                            t.AddUser(User);

                                            if (User.GetClient().GetHabbo().Effects().CurrentEffect != effectID)
                                                User.GetClient().GetHabbo().Effects().ApplyEffect(effectID);
                                        }
                                    }
                                    else if (User.Team != TEAM.NONE && User.Team != Item.team)
                                    {
                                        t.OnUserLeave(User);
                                        User.Team = TEAM.NONE;
                                        User.GetClient().GetHabbo().Effects().ApplyEffect(0);
                                    }
                                    else
                                    {
                                        //usersOnTeam--;
                                        t.OnUserLeave(User);
                                        if (User.GetClient().GetHabbo().Effects().CurrentEffect == effectID)
                                            User.GetClient().GetHabbo().Effects().ApplyEffect(0);
                                        User.Team = TEAM.NONE;
                                    }
                                    //Item.ExtraData = usersOnTeam.ToString();
                                    //Item.UpdateState(false, true);                                
                                }
                                break;
                            }
                        #endregion
                        #region Banzai Teles
                        case InteractionType.banzaitele:
                            {
                                if (User.Statusses.ContainsKey("mv"))
                                    _room.GetGameItemHandler().onTeleportRoomUserEnter(User, Item);
                                break;
                            }
                        #endregion
                        #region Football Gate

                        #endregion
                        #region Arrows
                        case InteractionType.ARROW:
                            {
                                if (User.GoalX == Item.GetX && User.GoalY == Item.GetY)
                                {
                                    if (User.IsBot)
                                    {
                                        RemoveBot(User.VirtualId, true);
                                        return;
                                    }

                                    if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                                        continue;

                                    if (User.GetClient().GetRoleplay().Escort || User.GetClient().GetHabbo().IsWaitingForParamedic || User.GetClient().GetHabbo().Stunned)
                                        continue;

                                    Room Room;

                                    if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(User.GetClient().GetHabbo().CurrentRoomId, out Room))
                                        return;

                                    if (User.GetClient().GetHabbo().CurrentRoomId == 9 && Item.Id == 25836 | Item.Id == 25829)
                                    {
                                        User.GetClient().SendMessage(new RoomForwardComposer(127));
                                    }

                                    if (User.GetClient().GetHabbo().CurrentRoomId == 109 && Item.Id == 37441)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), "apartments-lift;show");
                                    }

                                    //END EVENT

                                    if (!ItemTeleporterFinder.IsTeleLinked(Item.Id, Room))
                                        User.UnlockWalking();
                                    else
                                    {
                                        int LinkedTele = ItemTeleporterFinder.GetLinkedTele(Item.Id, Room);
                                        int TeleRoomId = ItemTeleporterFinder.GetTeleRoomId(LinkedTele, Room);

                                        if (TeleRoomId == 40 | TeleRoomId == 44 | TeleRoomId == 47 && User.GetClient().GetRoleplay().Passive)
                                        {
                                            User.GetClient().SendWhisper("You cannot enter Turfs while in passive mode");
                                            return;
                                        }

                                        if (TeleRoomId == Room.RoomId)
                                        {
                                            Item TargetItem = Room.GetRoomItemHandler().GetItem(LinkedTele);
                                            if (TargetItem == null)
                                            {
                                                if (User.GetClient() != null)
                                                    User.GetClient().SendWhisper("There is a problem with this arrow :/");
                                                return;
                                            }
                                            else
                                            {
                                                Room.GetGameMap().TeleportToItem(User, TargetItem);

                                                if (User.GetClient().GetHabbo().MenottedUsername != null)
                                                {
                                                    GameClient MenottedClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(User.GetClient().GetHabbo().MenottedUsername);
                                                    RoomUser UserMenotted = Room.GetRoomUserManager().GetRoomUserByHabbo(MenottedClient.GetHabbo().Id);
                                                    UserMenotted.IsWalking = false;
                                                    Room.GetGameMap().TeleportToItem(UserMenotted, TargetItem);
                                                }
                                            }
                                        }
                                        else if (TeleRoomId != Room.RoomId)
                                        {
                                            Room room = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(TeleRoomId);

                                            if (User != null && !User.IsBot && User.GetClient() != null && User.GetClient().GetHabbo() != null)
                                            {
                                                if (Item.Id == 119738 || Item.Id == 129398 || Item.Id == 120096 || Item.Id == 120099 || Item.Id == 33763 || Item.Id == 64456)
                                                {
                                                    User.GetClient().GetHabbo().EnterTurf(TeleRoomId, LinkedTele, "enter");
                                                    return;
                                                }
                                                else if (Item.Id == 119737 || Item.Id == 129397 || Item.Id == 120097 || Item.Id == 120098 || Item.Id == 33762 || Item.Id == 64455)
                                                {
                                                    User.GetClient().GetHabbo().EnterTurf(TeleRoomId, LinkedTele, "leave");
                                                    return;
                                                }
                                                else if (Item.Id == 123891 || Item.Id == 123888)
                                                {
                                                    User.GetClient().GetHabbo().CanUseJailGate = true;
                                                }

                                                User.GetClient().GetHabbo().IsTeleporting = true;
                                                User.GetClient().GetHabbo().TeleportingRoomID = TeleRoomId;
                                                User.GetClient().GetHabbo().TeleporterId = LinkedTele;
                                                RoleplayManager.InstantRL(User.GetClient(), TeleRoomId);

                                                if (User.GetClient().GetHabbo().ParamedicUsername != null)
                                                {
                                                    GameClient ParamedicUser = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(User.GetClient().GetHabbo().ParamedicUsername);
                                                    RoomUser UserMenotted = Room.GetRoomUserManager().GetRoomUserByHabbo(ParamedicUser.GetHabbo().Id);
                                                    UserMenotted.IsWalking = false;
                                                    ParamedicUser.GetHabbo().IsTeleporting = true;
                                                    ParamedicUser.GetHabbo().TeleportingRoomID = TeleRoomId;
                                                    ParamedicUser.GetHabbo().TeleporterId = LinkedTele;
                                                    ParamedicUser.GetHabbo().CanChangeRoom = true;
                                                    ParamedicUser.GetHabbo().PrepareRoom(TeleRoomId, "");
                                                }
                                            }
                                        }
                                        else if (this._room.GetRoomItemHandler().GetItem(LinkedTele) != null)
                                        {
                                            User.SetPos(Item.GetX, Item.GetY, Item.GetZ);
                                            User.SetRot(Item.Rotation, false);
                                        }
                                        else
                                            User.UnlockWalking();
                                    }
                                }
                                break;
                            }
                        #endregion
                        //RP Interactions Types
                        #region ActionPoint01
                        case InteractionType.ACTIONPOINT:
                            {
                                if (User.GoalX == Item.GetX && User.GoalY == Item.GetY)
                                {
                                    if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                                        continue;

                                    Room Room;

                                    if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(User.GetClient().GetHabbo().CurrentRoomId, out Room))
                                        return;

                                    if (Item.Id == 120785 | Item.Id == 57370 | Item.Id == 55639 | Item.Id == 97903 | Item.Id == 24890 | Item.Id == 100880)
                                    {
                                        User.GetClient().GetHabbo().UsingManagerMail = true;
                                        
                                    }

                                    
                                     else if (Item.Id == 26915)
                                    {
                                        User.GetClient().GetHabbo().UsingArmouryMerchandise = true;
                                       
                                    }


                                    else if (Item.Id == 52286)
                                    {
                                        User.GetClient().GetHabbo().usingSellingStock = true;
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), "selling-stock;show;Coffee Beans;coffee_bean;5;4");
                                    }
                                    else if (Item.Id == 102641)
                                    {
                                        User.GetClient().GetHabbo().usingSellingStock = true;
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), "selling-stock;show;Mending Weed;healingcrop;5;4");
                                    }
                                    else if (Item.Id == 102645)
                                    {
                                        User.GetClient().GetHabbo().usingSellingStock = true;
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), "selling-stock;show;Wool;wool;5;4");
                                    }
                                    else if (Item.Id == 149010 | Item.Id == 160364)
                                    {
                                        User.GetClient().GetHabbo().usingSellingStock = true;
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), "selling-stock;show;Iron Ore;ironore;3;4");
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), "selling-stock;show;Coal;coal;5;7");
                                    }

                                    if (Item.Id == 137723 | Item.Id == 137724 | Item.Id == 137721 | Item.Id == 137722 | Item.Id == 137728)
                                    {
                                        User.usingJobcenter = true;
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), "jobcenter;show");
                                    }

                                    if (Item.Id == 26918 | Item.Id == 27016 && !User.GetClient().GetHabbo().UsingBounties)
                                    {
                                        User.GetClient().GetHabbo().UsingBounties = true;
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), "bounty;show");
                                    }

                                    if (Item.Id >= 36738 && Item.Id <= 36743 && !User.GetClient().GetHabbo().usingSellingStock)
                                    {
                                        User.GetClient().GetHabbo().usingDepositBox = true;

                                        if (User.GetClient().GetRoleplay().DepositRent < DateTime.Now)
                                        {
                                            User.GetClient().SendWhisper("Your deposit box rent is overdue");
                                        }
                                        else
                                        {
                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), "deposit-box;show");
                                        }
                                    }

                                    if (Item.Id == 57046 | Item.Id == 57045 && !User.GetClient().GetHabbo().usingChapelActionPoint)
                                    {
                                        User.GetClient().GetHabbo().usingChapelActionPoint = true;
                                    }

                                    if (Item.Id == 2680 | Item.Id == 146173 | Item.Id == 156493 | Item.Id == 156494 | Item.Id == 64594 | Item.Id == 64595 && !User.GetClient().GetHabbo().usingMarketplace)
                                    {
                                        User.GetClient().GetHabbo().usingMarketplace = true;
                                        User.Say("browses the marketplace");
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), "marketplace;show");
                                    }

                                    #region Taxi
                                    if (Item.Id == 24342 | Item.Id == 22578 | Item.Id == 52291 | Item.Id == 159787 | Item.Id == 149007/*(Armoury)*/ | Item.Id == 150656/*(Forever21)*/)
                                    {
                                        if (!User.GetClient().GetRoleplay().usingTaxiRide)
                                            Taxi.Load(User.GetClient());
                                        else
                                            Taxi.Reset(User.GetClient());
                                    }
                                    #endregion
                                }
                                break;
                            }
                        #endregion
                        #region Taxi
                        case InteractionType.TAXI:
                            {
                                if (User.GoalX == Item.GetX && User.GoalY == Item.GetY)
                                {
                                    if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                                        continue;

                                    Room Room;

                                    if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(User.GetClient().GetHabbo().CurrentRoomId, out Room))
                                        return;

                                    if (!User.GetClient().GetRoleplay().usingTaxiRide)
                                        Taxi.Load(User.GetClient());
                                    else
                                        Taxi.Reset(User.GetClient());
                                }
                                break;
                            }
                        #endregion
                        #region Cross Trainer
                        case InteractionType.CROSSTRAINER:
                            {
                                if (User.GoalX == Item.GetX && User.GoalY == Item.GetY)
                                {
                                    if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                                        continue;

                                    Room Room;

                                    if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(User.GetClient().GetHabbo().CurrentRoomId, out Room))
                                        return;

                                    if (User.GetClient().GetRoleplay().GymMembership == 0)
                                    {

                                        User.GetClient().SendWhisper("You need a valid gym membership to work out! Speak to the receptionist for assistance.");
                                        return;
                                    }

                                    if (User.GetClient().GetRoleplay().CombatLevel == 25)
                                    {
                                        User.GetClient().SendWhisper("You have already reached the maximum combat level, more is not possible!");
                                        return;
                                    }

                                    if (User.GetClient().GetRoleplay().Energy < 3)
                                    {
                                        User.GetClient().SendWhisper("You need at least 3 energy to work out");
                                        return;
                                    }

                                    if (User.GetClient().GetHabbo().usedCrosstrainer == true)
                                    {
                                        User.GetClient().SendWhisper("You have already completed the Cross Trainer appartus");
                                        return;
                                    }

                                    User.RotHead = Item.Rotation;
                                    User.RotBody = Item.Rotation;
                                    User.UpdateNeeded = true;
                                    User.GetClient().GetHabbo().usingCrosstrainer = true;
                                    User.ApplyEffect(195);
                                    User.GetClient().SendWhisper("You started to working out on the Cross Trainer");


                                    Random TokenRand = new Random();
                                    int tokenNumber = TokenRand.Next(1600, 2894354);
                                    User.GetClient().GetHabbo().PlayToken = tokenNumber;

                                    System.Timers.Timer CrossTrainerTimer = new System.Timers.Timer(15000);
                                    CrossTrainerTimer.Interval = 15000;
                                    CrossTrainerTimer.Elapsed += delegate
                                    {
                                        if (User.GetClient().GetHabbo().usingCrosstrainer && User.GetClient().GetHabbo().PlayToken == tokenNumber)
                                        {
                                            User.GetClient().GetHabbo().PlayToken = 0;
                                            User.GetClient().GetHabbo().resetEffectEvent();
                                            User.GetClient().SendWhisper("You have completed your workout on the Cross Trainer");
                                            User.GetClient().GetHabbo().usingCrosstrainer = false;
                                            User.GetClient().GetHabbo().usedCrosstrainer = true;
                                            User.GetClient().GetRoleplay().Energy -= 3;
                                            User.GetClient().GetHabbo().GymXP();
                                            PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(User.GetClient(), "login", "user-stats");
                                        }
                                        CrossTrainerTimer.Stop();
                                    };
                                    CrossTrainerTimer.Start();
                                }
                                break;
                            }
                        #endregion
                        #region Treadmill
                        case InteractionType.TREADMILL:
                            {
                                if (User.GoalX == Item.GetX && User.GoalY == Item.GetY)
                                {
                                    if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                                        continue;

                                    Room Room;

                                    if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(User.GetClient().GetHabbo().CurrentRoomId, out Room))
                                        return;

                                    if (User.GetClient().GetRoleplay().GymMembership == 0)
                                    {
                                        User.GetClient().SendWhisper("You need a valid gym membership to work out! Speak to the receptionist for assistance.");
                                        return;
                                    }

                                    if (User.GetClient().GetRoleplay().CombatLevel == 25)
                                    {
                                        User.GetClient().SendWhisper("You have already reached the maximum combat level, more is not possible!");
                                        return;
                                    }

                                    if (User.GetClient().GetRoleplay().Energy < 3)
                                    {
                                        User.GetClient().SendWhisper("You need at least 3 energy to work out");
                                        return;
                                    }

                                    if (User.GetClient().GetHabbo().usedTreadMill == true)
                                    {
                                        User.GetClient().SendWhisper("You have already completed the Treadmill appartus");
                                        return;
                                    }

                                    User.RotHead = Item.Rotation;
                                    User.RotBody = Item.Rotation;
                                    User.UpdateNeeded = true;
                                    User.GetClient().GetHabbo().usingTreadMill = true;
                                    User.ApplyEffect(194);
                                    User.GetClient().SendWhisper("You started to working out on the Treadmill");

                                    Random TokenRand = new Random();
                                    int tokenNumber = TokenRand.Next(1600, 2894354);
                                    User.GetClient().GetHabbo().PlayToken = tokenNumber;

                                    System.Timers.Timer TreadMillTimer = new System.Timers.Timer(15000);
                                    TreadMillTimer.Interval = 15000;
                                    TreadMillTimer.Elapsed += delegate
                                    {
                                        if (User.GetClient().GetHabbo().usingTreadMill && User.GetClient().GetHabbo().PlayToken == tokenNumber)
                                        {
                                            User.GetClient().GetHabbo().PlayToken = 0;
                                            User.GetClient().GetHabbo().resetEffectEvent();
                                            User.GetClient().SendWhisper("You have completed your workout on the Treadmill");
                                            User.GetClient().GetHabbo().usingTreadMill = false;
                                            User.GetClient().GetHabbo().usedTreadMill = true;
                                            User.GetClient().GetRoleplay().Energy -= 3;
                                            User.GetClient().GetHabbo().GymXP();
                                            PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(User.GetClient(), "login", "user-stats");
                                        }
                                        TreadMillTimer.Stop();
                                    };
                                    TreadMillTimer.Start();
                                }
                                break;
                            }
                        #endregion
                        #region Trampoline
                        case InteractionType.TRAMPOLINE:
                            {
                                if (User.GoalX == Item.GetX && User.GoalY == Item.GetY)
                                {
                                    if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                                        continue;

                                    Room Room;

                                    if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(User.GetClient().GetHabbo().CurrentRoomId, out Room))
                                        return;

                                    if (User.GetClient().GetRoleplay().GymMembership == 0)
                                    {
                                        User.GetClient().SendWhisper("You need a valid gym membership to work out! Speak to the receptionist for assistance.");
                                        return;
                                    }

                                    if (User.GetClient().GetRoleplay().CombatLevel == 25)
                                    {
                                        User.GetClient().SendWhisper("You have already reached the maximum combat level, more is not possible!");
                                        return;
                                    }

                                    if (User.GetClient().GetRoleplay().Energy < 3)
                                    {
                                        User.GetClient().SendWhisper("You need at least 3 energy to work out");
                                        return;
                                    }

                                    if (User.GetClient().GetHabbo().usedTrampoline == true)
                                    {
                                        User.GetClient().SendWhisper("You have already completed the Trampoline appartus");
                                        return;
                                    }

                                    User.RotHead = Item.Rotation;
                                    User.RotBody = Item.Rotation;
                                    User.UpdateNeeded = true;
                                    User.GetClient().GetHabbo().usingTrampoline = true;
                                    User.ApplyEffect(193);
                                    User.GetClient().SendWhisper("You started to working out on the Trampoline");
                                    Random TokenRand = new Random();
                                    int tokenNumber = TokenRand.Next(1600, 2894354);
                                    User.GetClient().GetHabbo().PlayToken = tokenNumber;

                                    System.Timers.Timer TrampolineTimer = new System.Timers.Timer(15000);
                                    TrampolineTimer.Interval = 15000;
                                    TrampolineTimer.Elapsed += delegate
                                    {
                                        if (User.GetClient().GetHabbo().usingTrampoline && User.GetClient().GetHabbo().PlayToken == tokenNumber)
                                        {
                                            User.GetClient().GetHabbo().PlayToken = 0;
                                            User.GetClient().GetHabbo().resetEffectEvent();
                                            User.GetClient().SendWhisper("You have completed your workout on the Trampoline");
                                            User.GetClient().GetHabbo().usingTrampoline = false;
                                            User.GetClient().GetHabbo().usedTrampoline = true;
                                            User.GetClient().GetRoleplay().Energy -= 3;
                                            User.GetClient().GetHabbo().GymXP();
                                            PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(User.GetClient(), "login", "user-stats");
                                        }
                                        TrampolineTimer.Stop();
                                    };
                                    TrampolineTimer.Start();
                                }
                                break;
                            }
                        #endregion
                    }
                }

                if (User.isSitting && User.TeleportEnabled)
                {
                    User.Z -= 0.35;
                    User.UpdateNeeded = true;
                }

                if (cyclegameitems)
                {
                    if (_room.GotSoccer())
                        _room.GetSoccer().OnUserWalk(User);

                    if (_room.GotBanzai())
                        _room.GetBanzai().OnUserWalk(User);

                    if (_room.GotFreeze())
                        _room.GetFreeze().OnUserWalk(User);
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e.ToString());
            }
        }

        private void UpdateUserEffect(RoomUser User, int x, int y)
        {
            if (User == null || User.IsBot || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                return;

            try
            {
                byte NewCurrentUserItemEffect = _room.GetGameMap().EffectMap[x, y];
                if (NewCurrentUserItemEffect > 0)
                {
                    if (User.GetClient().GetHabbo().Effects().CurrentEffect == 0)
                        User.CurrentItemEffect = ItemEffectType.NONE;

                    ItemEffectType Type = ByteToItemEffectEnum.Parse(NewCurrentUserItemEffect);
                    if (Type != User.CurrentItemEffect)
                    {
                        switch (Type)
                        {
                            case ItemEffectType.Iceskates:
                                {
                                    User.GetClient().GetHabbo().Effects().ApplyEffect(User.GetClient().GetHabbo().Gender == "M" ? 38 : 39);
                                    User.CurrentItemEffect = ItemEffectType.Iceskates;
                                    break;
                                }

                            case ItemEffectType.Normalskates:
                                {
                                    User.GetClient().GetHabbo().Effects().ApplyEffect(User.GetClient().GetHabbo().Gender == "M" ? 55 : 56);
                                    User.CurrentItemEffect = Type;
                                    break;
                                }
                            case ItemEffectType.SWIM:
                                {
                                    User.GetClient().GetHabbo().Effects().ApplyEffect(29);
                                    User.CurrentItemEffect = Type;
                                    break;
                                }
                            case ItemEffectType.SwimLow:
                                {
                                    User.GetClient().GetHabbo().Effects().ApplyEffect(30);
                                    User.CurrentItemEffect = Type;
                                    break;
                                }
                            case ItemEffectType.SwimHalloween:
                                {
                                    User.GetClient().GetHabbo().Effects().ApplyEffect(37);
                                    User.CurrentItemEffect = Type;
                                    break;
                                }

                            case ItemEffectType.NONE:
                                {
                                    User.GetClient().GetHabbo().Effects().ApplyEffect(-1);
                                    User.CurrentItemEffect = Type;
                                    break;
                                }
                        }
                    }
                }
                else if (User.CurrentItemEffect != ItemEffectType.NONE && NewCurrentUserItemEffect == 0)
                {
                    User.GetClient().GetHabbo().Effects().ApplyEffect(-1);
                    User.CurrentItemEffect = ItemEffectType.NONE;
                }
            }
            catch
            {
            }
        }

        public int PetCount
        {
            get { return petCount; }
        }

        public object BotSpeechList { get; private set; }

        public ICollection<RoomUser> GetUserList()
        {
            try
            {
                return this._users.Values;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}