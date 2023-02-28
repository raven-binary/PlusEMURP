﻿using System;
using System.Collections.Generic;
using DotNetty.Transport.Channels;
using Plus.Communication.Packets.Outgoing;
using Plus.Communication.Packets.Outgoing.BuildersClub;
using Plus.Communication.Packets.Outgoing.Handshake;
using Plus.Communication.Packets.Outgoing.Inventory.Achievements;
using Plus.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.Communication.Packets.Outgoing.Navigator;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Sound;
using Plus.Core;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Permissions;
using Plus.HabboHotel.Roleplay.Combat;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Subscriptions;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Users.Messenger.FriendBar;
using Plus.HabboHotel.Users.UserData;
using Plus.Network.Codec;

namespace Plus.HabboHotel.GameClients
{
    public class GameClient
    {
        private Habbo _habbo;
        public string MachineId;
        private bool _disconnected;
        private readonly IChannelHandlerContext _channel;
        public int PingCount { get; set; }

        public GameClient(IChannelHandlerContext context)
        {
            _channel = context;
        }

        public bool TryAuthenticate(string authTicket)
        {
            try
            {
                UserData userData = UserDataFactory.GetUserData(authTicket, out byte errorCode);
                if (errorCode == 1 || errorCode == 2)
                {
                    Disconnect();
                    return false;
                }

                #region Ban Checking

                //Let's have a quick search for a ban before we successfully authenticate..
                if (!string.IsNullOrEmpty(MachineId))
                {
                    if (PlusEnvironment.GetGame().GetModerationManager().IsBanned(MachineId, out _))
                    {
                        if (PlusEnvironment.GetGame().GetModerationManager().MachineBanCheck(MachineId))
                        {
                            Disconnect();
                            return false;
                        }
                    }
                }

                if (userData.User != null)
                {
                    if (PlusEnvironment.GetGame().GetModerationManager().IsBanned(userData.User.Username, out _))
                    {
                        if (PlusEnvironment.GetGame().GetModerationManager().UsernameBanCheck(userData.User.Username))
                        {
                            Disconnect();
                            return false;
                        }
                    }
                }

                #endregion

                if (userData.User == null) //Possible NPE
                {
                    return false;
                }

                PlusEnvironment.GetGame().GetClientManager().RegisterClient(this, userData.UserId, userData.User.Username);
                _habbo = userData.User;
                if (_habbo != null)
                {
                    userData.User.Init(this, userData);

                    SendPacket(new AuthenticationOkComposer());
                    SendPacket(new AvatarEffectsComposer(_habbo.Effects().GetAllEffects));
                    SendPacket(new NavigatorSettingsComposer(_habbo.HomeRoom));
                    SendPacket(new FavouritesComposer(userData.User.FavoriteRooms));
                    SendPacket(new FigureSetIdsComposer(_habbo.GetClothing().GetClothingParts));
                    SendPacket(new UserRightsComposer(_habbo.Rank));
                    SendPacket(new AvailabilityStatusComposer());
                    SendPacket(new AchievementScoreComposer(_habbo.GetStats().AchievementPoints));
                    SendPacket(new BuildersClubMembershipComposer());
                    SendPacket(new CfhTopicsInitComposer(PlusEnvironment.GetGame().GetModerationManager().UserActionPresets));

                    SendPacket(new BadgeDefinitionsComposer(PlusEnvironment.GetGame().GetAchievementManager().Achievements));
                    SendPacket(new SoundSettingsComposer(_habbo.ClientVolume, _habbo.ChatPreference, _habbo.AllowMessengerInvites, _habbo.FocusPreference, FriendBarStateUtility.GetInt(_habbo.FriendBarState)));
                    //SendMessage(new TalentTrackLevelComposer());

                    if (GetHabbo().GetMessenger() != null)
                        GetHabbo().GetMessenger().OnStatusChanged(true);

                    if (!string.IsNullOrEmpty(MachineId))
                    {
                        if (_habbo.MachineId != MachineId)
                        {
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `users` SET `machine_id` = @MachineId WHERE `id` = @id LIMIT 1");
                                dbClient.AddParameter("MachineId", MachineId);
                                dbClient.AddParameter("id", _habbo.Id);
                                dbClient.RunQuery();
                            }
                        }

                        _habbo.MachineId = MachineId;
                    }

                    if (PlusEnvironment.GetGame().GetPermissionManager().TryGetGroup(_habbo.Rank, out PermissionGroup group))
                    {
                        if (!string.IsNullOrEmpty(group.Badge))
                            if (!_habbo.GetBadgeComponent().HasBadge(group.Badge))
                                _habbo.GetBadgeComponent().GiveBadge(group.Badge, true, this);
                    }

                    if (PlusEnvironment.GetGame().GetSubscriptionManager().TryGetSubscriptionData(_habbo.VipRank, out SubscriptionData subData))
                    {
                        if (!string.IsNullOrEmpty(subData.Badge))
                        {
                            if (!_habbo.GetBadgeComponent().HasBadge(subData.Badge))
                                _habbo.GetBadgeComponent().GiveBadge(subData.Badge, true, this);
                        }
                    }

                    if (!PlusEnvironment.GetGame().GetCacheManager().ContainsUser(_habbo.Id))
                        PlusEnvironment.GetGame().GetCacheManager().GenerateUser(_habbo.Id);

                    _habbo.Look = PlusEnvironment.GetFigureManager().ProcessFigure(_habbo.Look, _habbo.Gender, _habbo.GetClothing().GetClothingParts, true);
                    _habbo.InitProcess();

                    if (userData.User.GetPermissions().HasRight("mod_tickets"))
                    {
                        SendPacket(new ModeratorInitComposer(
                            PlusEnvironment.GetGame().GetModerationManager().UserMessagePresets,
                            PlusEnvironment.GetGame().GetModerationManager().RoomMessagePresets,
                            PlusEnvironment.GetGame().GetModerationManager().GetTickets));
                    }

                    if (PlusEnvironment.GetSettingsManager().TryGetValue("user.login.message.enabled") == "1")
                        SendPacket(new MotdNotificationComposer(PlusEnvironment.GetLanguageManager().TryGetValue("user.login.message")));

                    PlusEnvironment.GetGame().GetRewardManager().CheckRewards(this);
                    return true;
                }
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException(e);
            }

            return false;
        }

        public void SendWhisper(string message, int colour = 0)
        {
            if (GetHabbo() == null || GetHabbo().CurrentRoom == null)
                return;

            RoomUser user = GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(GetHabbo().Username);
            if (user == null)
                return;

            SendPacket(new WhisperComposer(user.VirtualId, message, 0, (colour == 0 ? user.LastBubble : colour)));
        }

        public void SendNotification(string message)
        {
            SendPacket(new BroadcastMessageAlertComposer(message));
        }

        public void SendPacket(MessageComposer message)
        {
            _channel.WriteAndFlushAsync(message);
        }

        public async void SendPacketsAsync(List<MessageComposer> messages)
        {
            foreach (MessageComposer message in messages)
            {
                await _channel.WriteAsync(message);
            }

            _channel.Flush();
        }

        public Habbo GetHabbo()
        {
            return _habbo;
        }

        public void Disconnect()
        {
            try
            {
                if (GetHabbo() != null)
                {
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery(GetHabbo().GetQueryString);
                    }

                    GetHabbo().OnDisconnect();
                }
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException(e);
            }

            if (!_disconnected)
            {
                _channel?.CloseAsync();
                _disconnected = true;
            }
        }

        public void Dispose()
        {
            if (GetHabbo() != null)
                GetHabbo().OnDisconnect();
                
                /*RoomUser User = GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(GetHabbo().Id);
                User.Say(GetHabbo().Username + " is logging out in 20 seconds", 1);
                GetHabbo().Effects().ApplyEffect(95);
                GetHabbo().isDisconnecting = true;
                User.isDisconnecting = true;
                User.Freezed = true;
                User.CanWalk = false;
                */

            MachineId = string.Empty;
            _disconnected = true;
            _habbo = null;
            _channel.DisconnectAsync();
        }

        public void EnableEncryption(byte[] sharedKey)
        {
            _channel.Channel.Pipeline.AddFirst("gameCrypto", new EncryptionDecoder(sharedKey));
        }
    }
}