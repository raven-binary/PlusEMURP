using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

using Plus.Core;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms.AI.Speech;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.HabboHotel.Rooms.AI.Responses;
using Plus.HabboHotel.Pathfinding;
using System.Data;
using Plus.Communication.Packets.Outgoing.Rooms.Session;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Avatar;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Users.Relationships;
using WebHook;
using Plus.HabboRoleplay.Misc;

namespace Plus.HabboHotel.Rooms.AI.Types
{
    public class GenericBot : BotAI
    {
        private int VirtualId;
        private int ActionTimer = 0;
        private int SpeechTimer = 0;

        public GenericBot(int VirtualId)
        {
            this.VirtualId = VirtualId;
        }

        public void Say(string message)
        {
            string prevName = GetBotData().Name;
            GetBotData().Name = "*" + GetBotData().Name;
            message = message + "*";
            GetRoomUser().GetRoom().SendMessage(new UsersComposer(GetRoomUser()));
            GetRoomUser().Chat("" + message + "", true, 0);
            GetBotData().Name = prevName;
            GetRoomUser().GetRoom().SendMessage(new UsersComposer(GetRoomUser()));
        }

        public override void OnSelfEnterRoom()
        {

        }

        public override void OnSelfLeaveRoom(bool Kicked)
        {

        }

        public override void OnUserEnterRoom(RoomUser User)
        {

        }

        public override void OnUserLeaveRoom(GameClient Client)
        {

        }

        public override void OnUserSay(RoomUser User, string Message)
        {

        }

        public override void OnUserShout(RoomUser User, string Message)
        {

        }

        public override void OnTimerTick()
        {
            if (GetBotData() == null)
                return;

            #region GymBot
            if (GetBotData().Name == "[Lisa]")
            {
                foreach (RoomUser User in GetRoom().GetRoomUserManager().GetRoomUsers())
                {
                    if (User.GetClient().GetHabbo().usingGymMembershipPurchase && User.LastMessage.Contains("buy gym") | User.LastMessage.Contains("Buy gym") | User.LastMessage.Contains("BUY GYM"))
                    {
                        User.LastMessage = "";
                        if (User.GetClient().GetRoleplay().GymMembership >= 1)
                        {
                            GetRoomUser().Chat("You already have a valid gym membership!", true, 0);
                            return;
                        }
                        else if (User.GetClient().GetRoleplay().CombatLevel == 25)
                        {
                            GetRoomUser().Chat("You already reached the maximum combat level.", true, 0);
                            return;
                        }
                        else if (25 > User.GetClient().GetHabbo().Credits)
                        {
                            GetRoomUser().Chat("You need $25 to buy a gym membership", true, 0);
                            return;
                        }
                        else
                        {
                            GetRoomUser().Chat("Here is your new gym membership, Enjoy!", true, 0);
                            User.GetClient().GetRoleplay().GymMembership = 60;
                            User.GetClient().GetHabbo().Credits -= 25;
                            User.GetClient().SendMessage(new CreditBalanceComposer(User.GetClient().GetHabbo().Credits));
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), "my_stats;" + User.GetClient().GetHabbo().Credits);
                            return;
                        }
                    }
                }
            }
            #endregion

            #region ChapelBot
            if (GetBotData().Name == "[Clergy]")
            {
                foreach (RoomUser User in GetRoom().GetRoomUserManager().GetRoomUsers())
                {
                    if (User.GetClient().GetHabbo().IsWaitingToMarry && User.GetClient().GetHabbo().MarryingTo != null)
                    {
                        GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(User.GetClient().GetHabbo().MarryingTo);

                        if (TargetClient.GetHabbo().MarryingTo == User.GetClient().GetHabbo().Username)
                        {
                            User.GetClient().GetHabbo().IsWaitingToMarry = false;
                            GetRoomUser().Chat("Dearly Beloved, we are gathered together here in the name of love", true, 30);

                            System.Timers.Timer timer1 = new System.Timers.Timer(2000);
                            timer1.Interval = 2000;
                            timer1.Elapsed += delegate
                            {
                                GetRoomUser().Chat("To join together " + User.GetClient().GetHabbo().Username + " and " + TargetClient.GetHabbo().Username + " in holy matrimony", true, 30);
                                timer1.Stop();
                            };
                            timer1.Start();

                            System.Timers.Timer timer2 = new System.Timers.Timer(4000);
                            timer2.Interval = 4000;
                            timer2.Elapsed += delegate
                            {
                                string Gender;
                                if (TargetClient.GetHabbo().Gender == "f")
                                {
                                    Gender = "wife";
                                }
                                else
                                {
                                    Gender = "husband";
                                }

                                User.GetClient().GetHabbo().IsWaitingToMarryReply = true;
                                GetRoomUser().Chat(User.GetClient().GetHabbo().Username + ", do you take " + TargetClient.GetHabbo().Username + " as your lawfully wedded " + Gender + "?", true, 30);
                                timer2.Stop();
                            };
                            timer2.Start();
                        }    
                    }
                    else if (User.GetClient().GetHabbo().IsWaitingToMarryReply && User.LastMessage == "I do" | User.LastMessage == "i do")
                    {
                        User.LastMessage = "";
                        GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(User.GetClient().GetHabbo().MarryingTo);

                        if (TargetClient.GetHabbo().IsWaitingToMarryReplyDone)
                        {
                            string Gender1;
                            string Gender2;
                            if (User.GetClient().GetHabbo().Gender == "f")
                            {
                                Gender1 = "wife";
                            }
                            else
                            {
                                Gender1 = "husband";
                            }

                            if (TargetClient.GetHabbo().Gender == "f")
                            {
                                Gender2 = "wife";
                            }
                            else
                            {
                                Gender2 = "husband";
                            }

                            TargetClient.GetRoleplay().MarriedTo = User.GetClient().GetHabbo().Id;
                            User.GetClient().GetRoleplay().MarriedTo = TargetClient.GetHabbo().Id;

                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `user_rp_stats` SET married_to = @target WHERE user_id = @uid");
                                dbClient.AddParameter("target", User.GetClient().GetHabbo().Id);
                                dbClient.AddParameter("uid", TargetClient.GetHabbo().Id);
                                dbClient.RunQuery();

                                dbClient.SetQuery("UPDATE `user_rp_stats` SET married_to = @target WHERE user_id = @uid");
                                dbClient.AddParameter("target", TargetClient.GetHabbo().Id);
                                dbClient.AddParameter("uid", User.GetClient().GetHabbo().Id);
                                dbClient.RunQuery();

                                dbClient.SetQuery("INSERT INTO `user_relationships` (`user_id`,`target`,`type`) VALUES ('" + TargetClient.GetHabbo().Id + "', @target, @type)");
                                dbClient.AddParameter("target", User.GetClient().GetHabbo().Id);
                                dbClient.AddParameter("type", 1);
                                int newId = Convert.ToInt32(dbClient.InsertQuery());
                                TargetClient.GetHabbo().Relationships.Add(User.GetClient().GetHabbo().Id, new Relationship(newId, User.GetClient().GetHabbo().Id, 1));

                                dbClient.SetQuery("INSERT INTO `user_relationships` (`user_id`,`target`,`type`) VALUES ('" + User.GetClient().GetHabbo().Id + "', @target, @type)");
                                dbClient.AddParameter("target", TargetClient.GetHabbo().Id);
                                dbClient.AddParameter("type", 1);
                                int newId2 = Convert.ToInt32(dbClient.InsertQuery());
                                User.GetClient().GetHabbo().Relationships.Add(TargetClient.GetHabbo().Id, new Relationship(newId2, TargetClient.GetHabbo().Id, 1));
                            }

                            TargetClient.GetHabbo().MarryingTo = null;
                            TargetClient.GetHabbo().IsWaitingToMarry = false;
                            TargetClient.GetHabbo().IsWaitingToMarryReply = false;
                            TargetClient.GetHabbo().IsWaitingToMarryReplyDone = false;

                            User.GetClient().GetHabbo().MarryingTo = null;
                            User.GetClient().GetHabbo().IsWaitingToMarry = false;
                            User.GetClient().GetHabbo().IsWaitingToMarryReply = false;
                            User.GetClient().GetHabbo().IsWaitingToMarryReplyDone = false;

                            GetRoomUser().Chat(TargetClient.GetHabbo().Username +" and " + User.GetClient().GetHabbo().Username + ", I now pronounce you " + Gender2 + " and " + Gender1 + "", true, 30);
                            PlusEnvironment.GetGame().GetClientManager().LiveFeed("<span class=\"blue\">" + TargetClient.GetHabbo().Username + "</span> has married <span class=\"blue\">" + User.GetClient().GetHabbo().Username + "</span>");
                            Webhook.SendWebhook(":ring: " + TargetClient.GetHabbo().Username + " has married " + User.GetClient().GetHabbo().Username);
                        }
                        else if (User.GetClient().GetHabbo().IsWaitingToMarryReply && User.LastMessage == "I dont" | User.LastMessage == "i dont" | User.LastMessage == "I don't" | User.LastMessage == "i don't")
                        {
                            User.LastMessage = "";
                            GetRoomUser().Chat("Bitch you better be joking", true, 30);
                        }
                        else
                        {
                            string Gender;
                            if (User.GetClient().GetHabbo().Gender == "f")
                            {
                                Gender = "wife";
                            }
                            else
                            {
                                Gender = "husband";
                            }

                            User.GetClient().GetHabbo().IsWaitingToMarryReplyDone = true;
                            User.GetClient().GetHabbo().IsWaitingToMarryReply = false;
                            TargetClient.GetHabbo().IsWaitingToMarryReply = true;
                            GetRoomUser().Chat(TargetClient.GetHabbo().Username + ", do you take " + User.GetClient().GetHabbo().Username + " as your lawfully wedded " + Gender + "?", true, 30);
                        }
                    }
                }
            }
            #endregion

            #region GymBot
            if (GetBotData().Name == "[Tom]")
            {
                foreach (RoomUser User in GetRoom().GetRoomUserManager().GetRoomUsers())
                {
                    if (User.GetClient().GetHabbo().UsingCityHallActionPoint && User.LastMessage == "Change username" || User.LastMessage == "change username")
                    {
                        User.LastMessage = "";
                        if (User.GetClient().GetHabbo().Rank < 2)
                            return;

                        GetRoomUser().Chat("Type a new username you wish to have", true, 0);
                    }
                }
            }
            #endregion

            #region CourtBot
            if (GetBotData().Name == "[Judge Lisa]")
            {
                GameClient Defendant = RoleplayManager.Defendant;

                if (RoleplayManager.CourtStarted && !RoleplayManager.CourtMembersTeleport)
                {
                    RoleplayManager.CourtMembersTeleport = true;

                    lock (RoleplayManager.InvitedUsersToJuryDuty)
                    {
                        foreach (GameClient Client in RoleplayManager.InvitedUsersToRemove)
                        {
                            if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().isDisconnecting)
                                continue;

                            RoomUser User = GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                            if (User == null)
                                return;

                            User.Freezed = true;
                            User.GetClient().GetHabbo().LastX = User.X;
                            User.GetClient().GetHabbo().LastY = User.Y;
                            User.GetClient().GetHabbo().LastRot = User.RotBody;
                            User.GetClient().GetHabbo().TeleportToCourtSeats(User, GetRoomUser().GetRoom());
                        }
                    }
                }
                else if (RoleplayManager.CourtStarted && RoleplayManager.CourtMembersTeleport && !GetBotData().CourtBotSpeech1)
                {
                    GetBotData().CourtBotSpeech1 = true;
                    if (Defendant.GetHabbo().CurrentRoomId != 113)
                    {
                        GetBotData().CourtBotSpeech1 = false;
                        RoleplayManager.InstantRL(Defendant, 113);
                        return;
                    }

                    RoomUser DefendantUser = Defendant.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Defendant.GetHabbo().Id);
                    DefendantUser.Freezed = true;

                    System.Timers.Timer Timer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(2));
                    Timer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(2);
                    Timer.Elapsed += delegate
                    {
                        GetRoomUser().GetRoom().RoomMuted = true;
                        GetRoomUser().Chat("Good morning ladies & gentlemen, Calling in the case of the People of the Apex vs " + Defendant.GetHabbo().Username, true, 30);
                        Timer.Stop();
                    };
                    Timer.Start();
                }
                else if (RoleplayManager.CourtStarted && !GetBotData().CourtBotSpeech2 && GetBotData().CourtBotSpeech1)
                {
                    GetBotData().CourtBotSpeech2 = true;
                    System.Timers.Timer Timer2 = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(4));
                    Timer2.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(4);
                    Timer2.Elapsed += delegate
                    {
                        GetRoomUser().Chat("The defendant is accused of the following crimes;", false, 30);
                        Timer2.Stop();
                    };
                    Timer2.Start();
                }
                else if (RoleplayManager.CourtStarted && !GetBotData().CourtBotSpeech3 && GetBotData().CourtBotSpeech2 && GetBotData().CourtBotSpeech1)
                {
                    GetBotData().CourtBotSpeech3 = true;

                    string Charges = string.Empty;
                    foreach (var Charge in Defendant.GetRoleplay().Wan.Charges.OrderBy(p => p.Key))
                    {
                        RoleplayManager.CourtAfterChargesSeconds += 1;
                        System.Timers.Timer Timer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(RoleplayManager.CourtAfterChargesSeconds));
                        Timer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(RoleplayManager.CourtAfterChargesSeconds);
                        Timer.Elapsed += delegate
                        {
                            GetRoomUser().Chat(Defendant.GetRoleplay().Wan.GetArrestTimeByCharge(Charge.Key) + " count of " + Charge.Key, false, 30);
                            Timer.Stop();
                        };
                        Timer.Start();
                    }

                    Defendant.GetRoleplay().Wan.Remove();
                    GetBotData().CourtBotSpeech3Completed = true;
                }
                else if (RoleplayManager.CourtStarted && !GetBotData().CourtBotSpeech4 && GetBotData().CourtBotSpeech3Completed && GetBotData().CourtBotSpeech3 && GetBotData().CourtBotSpeech2 && GetBotData().CourtBotSpeech1)
                {
                    GetBotData().CourtBotSpeech4 = true;
                    System.Timers.Timer Timer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(RoleplayManager.CourtAfterChargesSeconds + 2));
                    Timer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(RoleplayManager.CourtAfterChargesSeconds + 2);
                    Timer.Elapsed += delegate
                    {
                        GetRoomUser().Chat("The defendant is appleaing to the decision of the Apex Police Department that is he guilty", false, 30);
                        Timer.Stop();
                    };
                    Timer.Start();
                }
                else if (RoleplayManager.CourtStarted && !GetBotData().CourtBotSpeech5 && GetBotData().CourtBotSpeech3Completed && GetBotData().CourtBotSpeech4 && GetBotData().CourtBotSpeech3 && GetBotData().CourtBotSpeech2 && GetBotData().CourtBotSpeech1)
                {
                    GetBotData().CourtBotSpeech5 = true;
                    System.Timers.Timer Timer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(RoleplayManager.CourtAfterChargesSeconds + 4));
                    Timer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(RoleplayManager.CourtAfterChargesSeconds + 4);
                    Timer.Elapsed += delegate
                    {
                        GetRoomUser().Chat(Defendant.GetHabbo().Username + ", please explain to myself and the jury what excatly happened?", false, 30);
                        GetRoomUser().GetRoom().RoomMuted = false;
                        Timer.Stop();
                    };
                    Timer.Start();
                }
                else if (RoleplayManager.CourtStarted && !GetBotData().CourtBotSpeech6 && GetBotData().CourtBotSpeech3Completed && GetBotData().CourtBotSpeech5 && GetBotData().CourtBotSpeech4 && GetBotData().CourtBotSpeech3 && GetBotData().CourtBotSpeech2 && GetBotData().CourtBotSpeech1)
                {
                    GetBotData().CourtBotSpeech6 = true;
                    System.Timers.Timer Timer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(RoleplayManager.CourtAfterChargesSeconds + 20));
                    Timer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(RoleplayManager.CourtAfterChargesSeconds + 20);
                    Timer.Elapsed += delegate
                    {
                        GetRoomUser().GetRoom().RoomMuted = true;
                        GetRoomUser().Chat("Thank you for your statements.", false, 30);
                        Timer.Stop();
                    };
                    Timer.Start();
                }
                else if (RoleplayManager.CourtStarted && !GetBotData().CourtBotSpeech7 && GetBotData().CourtBotSpeech6 && GetBotData().CourtBotSpeech3Completed && GetBotData().CourtBotSpeech5 && GetBotData().CourtBotSpeech4 && GetBotData().CourtBotSpeech3 && GetBotData().CourtBotSpeech2 && GetBotData().CourtBotSpeech1)
                {
                    GetBotData().CourtBotSpeech7 = true;
                    System.Timers.Timer Timer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(RoleplayManager.CourtAfterChargesSeconds + 21));
                    Timer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(RoleplayManager.CourtAfterChargesSeconds + 21);
                    Timer.Elapsed += delegate
                    {
                        GetRoomUser().Chat(Defendant.GetHabbo().Username + ", if the jury finds that you are innocent you will be released from jail", false, 30);
                        Timer.Stop();
                    };
                    Timer.Start();
                }
                else if (RoleplayManager.CourtStarted && !GetBotData().CourtBotSpeech8 && GetBotData().CourtBotSpeech7 && GetBotData().CourtBotSpeech6 && GetBotData().CourtBotSpeech3Completed && GetBotData().CourtBotSpeech5 && GetBotData().CourtBotSpeech4 && GetBotData().CourtBotSpeech3 && GetBotData().CourtBotSpeech2 && GetBotData().CourtBotSpeech1)
                {
                    GetBotData().CourtBotSpeech8 = true;
                    System.Timers.Timer Timer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(RoleplayManager.CourtAfterChargesSeconds + 22));
                    Timer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(RoleplayManager.CourtAfterChargesSeconds + 22);
                    Timer.Elapsed += delegate
                    {
                        GetRoomUser().Chat("However if they find that you are guilty you will remain in jail to serve the remainder of your sentence.", false, 30);
                        Timer.Stop();
                    };
                    Timer.Start();
                }
                else if (RoleplayManager.CourtStarted && !GetBotData().CourtBotSpeech9 && GetBotData().CourtBotSpeech8 && GetBotData().CourtBotSpeech7 && GetBotData().CourtBotSpeech6 && GetBotData().CourtBotSpeech3Completed && GetBotData().CourtBotSpeech5 && GetBotData().CourtBotSpeech4 && GetBotData().CourtBotSpeech3 && GetBotData().CourtBotSpeech2 && GetBotData().CourtBotSpeech1)
                {
                    GetBotData().CourtBotSpeech9 = true;
                    System.Timers.Timer Timer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(RoleplayManager.CourtAfterChargesSeconds + 23));
                    Timer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(RoleplayManager.CourtAfterChargesSeconds + 23);
                    Timer.Elapsed += delegate
                    {
                        GetRoomUser().Chat("Jason, please remove " + Defendant.GetHabbo().Username, false, 30);
                        Timer.Stop();
                    };
                    Timer.Start();

                    System.Timers.Timer Timer2 = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(RoleplayManager.CourtAfterChargesSeconds + 25));
                    Timer2.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(RoleplayManager.CourtAfterChargesSeconds + 25);
                    Timer2.Elapsed += delegate
                    {
                        Defendant.GetRoleplay().Prison = true;
                        Defendant.GetRoleplay().SendToPrisonChair();
                        Timer2.Stop();
                    };
                    Timer2.Start();
                }
                else if (RoleplayManager.CourtStarted && !GetBotData().CourtBotSpeech10 && GetBotData().CourtBotSpeech9 && GetBotData().CourtBotSpeech8 && GetBotData().CourtBotSpeech7 && GetBotData().CourtBotSpeech6 && GetBotData().CourtBotSpeech3Completed && GetBotData().CourtBotSpeech5 && GetBotData().CourtBotSpeech4 && GetBotData().CourtBotSpeech3 && GetBotData().CourtBotSpeech2 && GetBotData().CourtBotSpeech1)
                {
                    GetBotData().CourtBotSpeech10 = true;
                    System.Timers.Timer Timer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(RoleplayManager.CourtAfterChargesSeconds + 27));
                    Timer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(RoleplayManager.CourtAfterChargesSeconds + 27);
                    Timer.Elapsed += delegate
                    {
                        RoleplayManager.CourtVoteStarted = true;
                        GetRoomUser().GetRoom().RoomMuted = false;
                        GetRoomUser().Chat("Thank you. It's now up to you, the jury decide the defendants fate. Please vote innocent or guilty.", false, 30);

                        lock (RoleplayManager.InvitedUsersToJuryDuty)
                        {
                            foreach (GameClient Client in RoleplayManager.InvitedUsersToRemove)
                            {
                                if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().isDisconnecting)
                                    continue;

                                Client.GetRoleplay().SendWeb("jury-vote;show;" + Defendant.GetHabbo().Username);
                            }
                        }
                        Timer.Stop();
                    };
                    Timer.Start();
                }
                else if (RoleplayManager.CourtStarted && !GetBotData().CourtBotSpeech11 && GetBotData().CourtBotSpeech10 && GetBotData().CourtBotSpeech9 && GetBotData().CourtBotSpeech8 && GetBotData().CourtBotSpeech7 && GetBotData().CourtBotSpeech6 && GetBotData().CourtBotSpeech3Completed && GetBotData().CourtBotSpeech5 && GetBotData().CourtBotSpeech4 && GetBotData().CourtBotSpeech3 && GetBotData().CourtBotSpeech2 && GetBotData().CourtBotSpeech1)
                {
                    GetBotData().CourtBotSpeech11 = true;
                    System.Timers.Timer Timer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(RoleplayManager.CourtAfterChargesSeconds + 43));
                    Timer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(RoleplayManager.CourtAfterChargesSeconds + 43);
                    Timer.Elapsed += delegate
                    {
                        RoleplayManager.CourtVoteStarted = false;
                        GetRoomUser().GetRoom().RoomMuted = true;

                        lock (RoleplayManager.InvitedUsersToJuryDuty)
                        {
                            foreach (GameClient Client in RoleplayManager.InvitedUsersToRemove)
                            {
                                if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().isDisconnecting)
                                    continue;

                                Client.GetRoleplay().SendWeb("jury-vote;hide");
                            }
                        }

                        if (RoleplayManager.CourtGuiltyVotes > RoleplayManager.CourtInnocentVotes || RoleplayManager.CourtGuiltyVotes == RoleplayManager.CourtInnocentVotes)
                        {
                            GetRoomUser().Chat("Thank you for your votes. The jury has found the defendant, " + Defendant.GetHabbo().Username + ", guilty of all crime", false, 30);
                            System.Timers.Timer Timer2 = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(2));
                            Timer2.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(2);
                            Timer2.Elapsed += delegate
                            {
                                GetRoomUser().Chat("The defendant will remain in prison and serve the out the rest of his sentence there.", false, 30);
                                Defendant.SendWhisper("The jury has made a decision; guilty. You gotta remain the rest of ur sentence in jail.");
                                Timer2.Stop();
                            };
                            Timer2.Start();
                        }
                        else if (RoleplayManager.CourtInnocentVotes > RoleplayManager.CourtGuiltyVotes)
                        {
                            GetRoomUser().Chat("Thank you for your votes. The jury has found the defendant, " + Defendant.GetHabbo().Username + ", innocent of all crime", false, 30);

                            System.Timers.Timer Timer2 = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(2));
                            Timer2.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(2);
                            Timer2.Elapsed += delegate
                            {
                                GetRoomUser().Chat("The defendant will be released out of jail.", false, 30);
                                Defendant.SendWhisper("The jury has made a decision; innocent. You are released and can walk out of jail.");
                                Defendant.GetRoleplay().EndPrison();
                                Timer2.Stop();
                            };
                            Timer2.Start();
                        }
                        Timer.Stop();
                    };
                    Timer.Start();
                }
                else if (RoleplayManager.CourtStarted && !GetBotData().CourtBotSpeech12 && GetBotData().CourtBotSpeech11 && GetBotData().CourtBotSpeech10 && GetBotData().CourtBotSpeech9 && GetBotData().CourtBotSpeech8 && GetBotData().CourtBotSpeech7 && GetBotData().CourtBotSpeech6 && GetBotData().CourtBotSpeech3Completed && GetBotData().CourtBotSpeech5 && GetBotData().CourtBotSpeech4 && GetBotData().CourtBotSpeech3 && GetBotData().CourtBotSpeech2 && GetBotData().CourtBotSpeech1)
                {
                    GetBotData().CourtBotSpeech12 = true;
                    System.Timers.Timer Timer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(RoleplayManager.CourtAfterChargesSeconds + 47));
                    Timer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(RoleplayManager.CourtAfterChargesSeconds + 47);
                    Timer.Elapsed += delegate
                    {
                        GetRoomUser().Chat("The jury is thanked and excused. Court is adjourned.", false, 30);
                        RoleplayManager.CourtEnded = true;
                        Timer.Stop();
                    };
                    Timer.Start();
                }
            }
            #endregion
            #region Jury Bot
            if (GetBotData().Name == "[Marie]")
            {
                if (RoleplayManager.CourtEnded)
                {
                    GameClient Defendant = RoleplayManager.Defendant;

                    RoleplayManager.CourtEnded = false;
                    Defendant.GetRoleplay().Trial = false;
                    System.Timers.Timer Timer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(2));
                    Timer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(2);
                    Timer.Elapsed += delegate
                    {
                        GetRoomUser().GetRoom().RoomMuted = false;
                        GetRoomUser().Chat("Jury members, we credited 40 dollars to you for your time, thank you!", false, 30);

                        lock (RoleplayManager.InvitedUsersToJuryDuty)
                        {
                            foreach (GameClient Client in RoleplayManager.InvitedUsersToRemove)
                            {
                                if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().isDisconnecting || !Client.GetRoleplay().CourtVoted)
                                    continue;

                                RoomUser User = GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                                if (User == null)
                                    return;

                                User.GetClient().GetHabbo().Credits += 40;
                                User.GetClient().GetRoleplay().RPCache(3);

                                Client.GetRoleplay().JoinedCourt = false;
                                Client.GetRoleplay().CourtVoted = false;
                                GetRoomUser().GetRoom().GetGameMap().UpdateUserMovement(new Point(User.X, User.Y), new Point(Client.GetHabbo().LastX, Client.GetHabbo().LastY), User);
                                User.X = Client.GetHabbo().LastX;
                                User.Y = Client.GetHabbo().LastY;
                                User.Freezed = false;
                                User.Statusses.Clear();
                                User.PathRecalcNeeded = true;
                                User.SetPos(Client.GetHabbo().LastX, Client.GetHabbo().LastY, 0);
                                User.SetRot(Client.GetHabbo().LastRot, false);
                                User.UpdateNeeded = true;

                                RoleplayManager.CourtUsing = false;
                                RoleplayManager.CourtStarting = false;
                                RoleplayManager.CourtStarted = false;
                                RoleplayManager.CourtMembersTeleport = false;
                                RoleplayManager.Defendant = null;
                                RoleplayManager.CourtVoteStarted = false;
                                RoleplayManager.CourtGuiltyVotes = 0;
                                RoleplayManager.CourtInnocentVotes = 0;
                                RoleplayManager.InvitedUsersToJuryDuty.Clear();

                            }
                        }
                        Timer.Stop();
                    };
                    Timer.Start();
                }
            }
            #endregion

            if (SpeechTimer <= 0)
            {
                if (GetBotData().RandomSpeech.Count > 0)
                {
                    if (GetBotData().AutomaticChat == false)
                        return;

                    RandomSpeech Speech = GetBotData().GetRandomSpeech();

                    string String = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Speech.Message);
                    if (String.Contains("<img src") || String.Contains("<font ") || String.Contains("</font>") || String.Contains("</a>") || String.Contains("<i>"))
                        String = "I really shouldn't be using HTML within bot speeches.";
                    GetRoomUser().Chat(String, false, GetBotData().ChatBubble);
                }
                SpeechTimer = GetBotData().SpeakingInterval;
            }
            else
                SpeechTimer--;

            if (ActionTimer <= 0)
            {
                Point nextCoord;
                switch (GetBotData().WalkingMode.ToLower())
                {
                    default:
                    case "stand":
                        // (8) Why is my life so boring?
                        if (!GetRoomUser().Statusses.ContainsKey("sit"))
                        {
                            if (!GetRoomUser().Statusses.ContainsKey("sit"))
                                GetRoomUser().Statusses.Add("sit", "1.2");
                        }

                        GetRoomUser().Z = 0;
                        GetRoomUser().RotHead = 2;
                        GetRoomUser().RotBody = 2;
                        GetRoomUser().UpdateNeeded = true;
                        break;

                    case "freeroam":
                        if (GetBotData().ForcedMovement)
                        {
                            if (GetRoomUser().Coordinate == GetBotData().TargetCoordinate)
                            {
                                GetBotData().ForcedMovement = false;
                                GetBotData().TargetCoordinate = new Point();

                                GetRoomUser().MoveTo(GetBotData().TargetCoordinate.X, GetBotData().TargetCoordinate.Y);
                            }
                        }
                        else if (GetBotData().ForcedUserTargetMovement > 0)
                        {
                            RoomUser Target = GetRoom().GetRoomUserManager().GetRoomUserByHabbo(GetBotData().ForcedUserTargetMovement);
                            if (Target == null)
                            {
                                GetBotData().ForcedUserTargetMovement = 0;
                                GetRoomUser().ClearMovement(true);
                            }
                            else
                            {
                                var Sq = new Point(Target.X, Target.Y);

                                if (Target.RotBody == 0)
                                {
                                    Sq.Y--;
                                }
                                else if (Target.RotBody == 2)
                                {
                                    Sq.X++;
                                }
                                else if (Target.RotBody == 4)
                                {
                                    Sq.Y++;
                                }
                                else if (Target.RotBody == 6)
                                {
                                    Sq.X--;
                                }


                                GetRoomUser().MoveTo(Sq);
                            }
                        }
                        else if (GetBotData().TargetUser == 0)
                        {
                            nextCoord = GetRoom().GetGameMap().getRandomWalkableSquare();
                            GetRoomUser().MoveTo(nextCoord.X, nextCoord.Y);
                        }
                        break;
                    case "specified_range":

                        break;
                }
                ActionTimer = new Random(DateTime.Now.Millisecond + this.VirtualId ^ 3).Next(10, 25);
            }
            else
                ActionTimer--;
        }
    }
}