using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.Communication.Packets.Outgoing.Inventory.Furni;
using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.HabboHotel.Items;
using Plus.Communication.Packets.Outgoing.Handshake;
using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using System.Data;
using WebHook;
using Plus.HabboRoleplay.Misc;

namespace Plus.HabboHotel.Users.Process
{
    sealed class ProcessComponent
    {
        private static readonly ILog log = LogManager.GetLogger("Plus.HabboHotel.Users.Process.ProcessComponent");

        /// <summary>
        /// Player to update, handle, change etc.
        /// </summary>
        private Habbo _player = null;

        /// <summary>
        /// ThreadPooled Timer.
        /// </summary>
        private Timer _timer = null;
        private Timer _workTimer = null;
        private Timer _blurTimer = null;
        private Timer _HospitalTimer = null;
        private Timer _captureTimer = null;
        private Timer _brulingTimer = null;
        private Timer _gpTimer = null;
        private Timer _gymTimer = null;
        public Timer _daynightTimer = null;
        public Timer _arenaTimer = null;
        public Timer _bountyTimer = null;
        public Timer _wantedTimer = null;

        /// <summary>
        /// Prevents the timer from overlapping itself.
        /// </summary>
        private bool _timerRunning = false;

        /// <summary>
        /// Checks if the timer is lagging behind (server can't keep up).
        /// </summary>
        private bool _timerLagging = false;

        /// <summary>
        /// Enable/Disable the timer WITHOUT disabling the timer itself.
        /// </summary>
        private bool _disabled = false;

        /// <summary>
        /// Used for disposing the ProcessComponent safely.
        /// </summary>
        private AutoResetEvent _resetEvent = new AutoResetEvent(true);

        /// <summary>
        /// How often the timer should execute.
        /// </summary>
        private static int _runtimeInSec = 60;

        /// <summary>
        /// Default.
        /// </summary>
        public ProcessComponent()
        {
        }

        /// <summary>
        /// Initializes the ProcessComponent.
        /// </summary>
        /// <param name="Player">Player.</param>
        public bool Init(Habbo Player)
        {
            if (Player == null)
                return false;
            else if (this._player != null)
                return false;

            this._player = Player;
            this._timer = new Timer(new TimerCallback(Run), null, _runtimeInSec * 1000, _runtimeInSec * 1000);
            this._workTimer = new Timer(new TimerCallback(WorkTimer), null, 60000, 60000);
            this._HospitalTimer = new Timer(new TimerCallback(HospitalTimer), null, 1000, 1000);
            this._blurTimer = new Timer(new TimerCallback(BlurTimer), null, 60000, 60000);
            this._captureTimer = new Timer(new TimerCallback(CaptureTimer), null, 2000, 2000);
            //this._brulingTimer = new Timer(new TimerCallback(BrulingTimer), null, 1000, 1000);
            this._gpTimer = new Timer(new TimerCallback(GPTimer), null, 60000, 60000);
            this._gymTimer = new Timer(new TimerCallback(GymTimer), null, 60000, 60000);
            this._daynightTimer = new Timer(new TimerCallback(DayNightTimer), null, 60000, 60000);
            this._bountyTimer = new Timer(new TimerCallback(BountyTimer), null, 60000, 60000);
            this._wantedTimer = new Timer(new TimerCallback(WantedTimer), null, 60000, 60000);
            return true;
        }

        public void WorkTimer(object State)
        {
            try
            {
                if (this._player == null || !this._player.InRoom)
                    return;

                RoomUser User = this._player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this._player.Username);
                if (User == null || !this._player.Working || this._player.Prison != 0 || this._player.Hospital == 1)
                    return;

                if (this._player.Timer <= 1)
                {
                    int Pay = this._player.RankInfo.Pay;

                    string VIP = null;
                    if (this._player.Rank > 1)
                    {
                        VIP = " (+$2 VIP)";
                        Pay += 2;
                    }

                    this._player.GetClient().SendWhisper("You have been paid $" + this._player.RankInfo.Pay + " for your shift" + VIP);
                    this._player.GetClient().GetRoleplay().Shifts += 1;
                    this._player.GetClient().GetRoleplay().CorpShifts += 1;
                    this._player.GetClient().GetRoleplay().WeeklyShifts += 1;
                    this._player.Credits = this._player.Credits + Pay;
                    this._player.GetClient().SendMessage(new CreditBalanceComposer(this._player.GetClient().GetHabbo().Credits + Pay));
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this._player.GetClient(), "my_stats;" + this._player.GetClient().GetHabbo().Credits + ";" + this._player.GetClient().GetHabbo().Duckets + ";" + this._player.GetClient().GetHabbo().EventPoints);

                    this._player.GetClient().GetHabbo().RPCache(3);
                    this._player.Timer = 10;
                    this._player.updateTimer();
                    return;
                }
                else
                {
                    this._player.Timer = this._player.Timer - 1;
                    this._player.updateTimer();
                    this._player.GetClient().SendWhisper("You will receive your next pay cheque in " + this._player.Timer + " minutes");
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[WorkTimer] An error has occurred.");
                Console.WriteLine(e.ToString());
            }
        }

        public void HospitalTimer(object State)
        {
            try
            {
                if (this._player == null || !this._player.InRoom || this._player.isDisconnecting)
                    return;

                if (this._player.Hospital == 0)
                    return;

                RoomUser User = this._player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this._player.Username);
                if (this._player.GetClient().GetRoleplay().Health == this._player.GetClient().GetRoleplay().HealthMax || this._player.GetClient().GetRoleplay().Health > this._player.GetClient().GetRoleplay().HealthMax)
                {
                    this._player.GetClient().GetRoleplay().Health = this._player.GetClient().GetRoleplay().HealthMax;
                    this._player.RPCache(1);
                    this._player.endHospital(User.GetClient(), 0);
                    return;
                }

                

                if (this._player.GetClient().GetHabbo().Rank == 1)
                {
                    this._player.GetClient().GetRoleplay().Health += 1;
                }
                else
                {
                    this._player.GetClient().GetRoleplay().Health += 2;
                }
                this._player.RPCache(1);
                RoleplayManager.UpdateTargetStats(this._player.Id);

                /*if (this._player.Timer <= 1)
                {
                    this._player.Timer = 0;
                    this._player.updateTimer();
                    this._player.GetRPStats().Health = this._player.GetRPStats().HealthMax;
                    this._player.RPCache(1);
                    RoomUser User = this._player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this._player.Username);
                    this._player.endHospital(User.GetClient(), 0);
                    return;
                }
                else
                {
                    this._player.Timer = this._player.Timer - 1;
                    this._player.updateTimer();
                    this._player.GetClient().SendWhisper("You will be released in " + this._player.Timer + " minutes from the hospital bed");
                    return;
                }*/
            }
            catch
            {
                //Console.WriteLine("[HospitalTimer] An error has occurred.");
            }
        }
        public void BlurTimer(object State)
        {
            try
            {
                if (this._player == null || !this._player.InRoom)
                    return;

                RoomUser User = this._player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this._player.Username);
                if (User == null)
                    return;

                if(this._player.SmokeTimer > 0)
                {
                    this._player.SmokeTimer -= 1;
                    if(this._player.SmokeTimer == 0)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this._player.GetClient(), "smoke;stop");
                    }
                }

                if (this._player.Blur == 0)
                    return;

                this._player.Blur -= 1;
                this._player.updateBlur();
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this._player.GetClient(), "blur;" + this._player.Blur);
            }
            catch
            {
                Console.WriteLine("[BlurTimer] An error has occurred.");
            }
        }
        public void CaptureTimer(object State)
        {
            try
            {
                if (this._player == null || !this._player.InRoom)
                    return;

                RoomUser User = this._player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this._player.Username);
                if (User == null || User.Capture == 0 || this._player.Gang == 0)
                    return;

                if (User.CaptureProgress >= 98)
                {
                    User.Capture = 0;
                    this._player.CurrentRoom.Capture = this._player.Gang;
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE rooms SET capture = @capture WHERE `id` = @id LIMIT 1");
                        dbClient.AddParameter("capture", this._player.Gang);
                        dbClient.AddParameter("id", this._player.CurrentRoom.Id);
                        dbClient.RunQuery();
                    }
                    this._player.updateGangXP(50);
                    User.OnChat(User.LastBubble, "* Stop with the income * ", true);
                    PlusEnvironment.GetGame().GetClientManager().sendGangMsg(this._player.Gang, this._player.Username + " conquered an area : [" + this._player.CurrentRoom.Id + "] " + this._player.CurrentRoom.Name + " .");
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this._player.GetClient(), "capture;hide");
                    return;
                }
                else
                {
                    User.CaptureProgress = User.CaptureProgress + 2;
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this._player.GetClient(), "capture;update;" + User.CaptureProgress);
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[CaptureTimer] An error has occurred.");
            }
        }
        /*public void BrulingTimer(object State)
        {
            try
            {
                if (this._player == null || !this._player.InRoom)
                    return;

                RoomUser User = this._player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this._player.Username);
                if (User == null || User.isBruling == false || User.IsBot || this._player.Hospital != 0 || User.Immunised == true)
                    return;

                if(this._player.Health > 5)
                {
                    this._player.Health -= 5;
                    this._player.updateSante();
                    this._player.GetClient().SendWhisper("Du verlierst dein Gesundheit [" + this._player.Health + "/" + this._player.HealthMax + "]");
                }
                else
                {
                    User.Say("is burned");

                    string Username = null;
                    foreach (Item item in this._player.CurrentRoom.GetRoomItemHandler().GetFloor)
                    {
                        if (item.Id == User.isBrulingItem)
                        {
                            Username = item.Username;
                            break;
                        }
                    }

                    GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);

                    if (TargetClient == null)
                    {
                        User.GetClient().SendNotification("Du bist von ein Molotow verbrannt. Du wirst jetzt ins Krankenhaus gebracht und in 10 Minuten komplett geheilt!");
                    }
                    else if (TargetClient.GetHabbo().Username == this._player.Username)
                    {
                        User.GetClient().SendNotification("Du hast dich selber mit dein Molotow verbrannt. Du wirst jetzt ins Krankenhaus gebracht und in 10 Minuten komplett geheilt!");
                    }
                    else
                    {
                        User.GetClient().SendNotification(TargetClient.GetHabbo().Username + " hat dich mit ein Molotow verbrannt. Du wirst jetzt ins Krankenhaus gebracht und in 10 Minuten komplett geheilt!");
                        TargetClient.GetHabbo().Kills += 1;
                        TargetClient.GetHabbo().updateKill();
                        TargetClient.GetHabbo().updateGangKill();
                        TargetClient.GetHabbo().updateGangXP(10);
                        TargetClient.SendWhisper("Du hast " + this._player.Username + " mit dein Molotow verbrannt.");
                    }

                    this._player.updateHospitalEtat(User, 10);
                    this._player.Health = 0;
                    this._player.updateSante();
                    this._player.Deaths += 1;
                    this._player.updateDeaths();
                    this._player.updateGangKill();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[BrulingTimer] An error has occurred.");
            }
        }*/

       


        public void GPTimer(object State)
        {
            try
            {
                if (this._player == null || !this._player.InRoom)
                    return;

                if (this._player.GetClient().GetRoleplay().GP == 0)
                    return;

                if (this._player.GetClient().GetRoleplay().GP <= 1)
                {
                    this._player.GetClient().SendWhisper("You are no longer under God Protection. Good luck!");
                    this._player.GetClient().GetRoleplay().GP = 0;
                    return;
                }
                else
                {
                    this._player.GetClient().GetRoleplay().GP = this._player.GetClient().GetRoleplay().GP - 1;
                    this._player.GetClient().SendWhisper("You have " + this._player.GetClient().GetRoleplay().GP + " minute(s) left until you lose your God Protection");
                    return;
                }
            }
            catch
            {
                Console.WriteLine("[GPTimer] An error has occurred.");
            }
        }
        public void GymTimer(object State)
        {
            try
            {
                if (this._player == null || !this._player.InRoom)
                    return;

                if (this._player.GetClient().GetRoleplay().GymMembership == 0)
                    return;

                if (this._player.GetClient().GetRoleplay().GymMembership <= 1)
                {
                    this._player.GetClient().SendWhisper("Your gym membership has expired!");
                    this._player.GetClient().GetRoleplay().GymMembership = 0;
                    return;
                }
                else
                {
                    this._player.GetClient().GetRoleplay().GymMembership = this._player.GetClient().GetRoleplay().GymMembership - 1;
                    this._player.GetClient().SendWhisper("You have " + this._player.GetClient().GetRoleplay().GymMembership + " minute(s) left before your gym membership expires");
                    return;
                }
            }
            catch
            {
                Console.WriteLine("[GymTimer] An error has occurred.");
            }
        }

        public void DayNightTimer(object State)
        {
            try
            {
                if (PlusStaticGameSettings.CurrentMood == "Day")
                {
                    PlusStaticGameSettings.DayNightTimer = PlusStaticGameSettings.DayNightTimer - 1;
                    if (PlusStaticGameSettings.DayNightTimer == 0)
                    {
                        PlusStaticGameSettings.CurrentMood = "Night";
                        PlusStaticGameSettings.DayNightTimer = PlusStaticGameSettings.NightTimer;
                    }
                }
                else if (PlusStaticGameSettings.CurrentMood == "Night")
                {
                    PlusStaticGameSettings.DayNightTimer = PlusStaticGameSettings.DayNightTimer - 1;
                    if (PlusStaticGameSettings.DayNightTimer == 0)
                    {
                        PlusStaticGameSettings.CurrentMood = "Day";
                        PlusStaticGameSettings.DayNightTimer = PlusStaticGameSettings.DayTimer;
                    }
                }
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this._player.GetClient(), "day-night;" + PlusStaticGameSettings.CurrentMood + ";" + PlusStaticGameSettings.DayNightTimer);
                return;
            }
            catch
            {
                //Console.WriteLine("[DayNightTimer] An error has occurred.");
            }
        }

        public void BountyTimer(object State)
        {
            try
            {
                if (this._player == null || !this._player.InRoom)
                    return;

                if (this._player.BountyTimer == 0 || !this._player.CurrentRoom.Fightable || this._player.GetClient().GetRoleplay().Passive)
                    return;

                if (this._player.GetClient().GetHabbo().BountyTimer <= 1 && this._player.GetClient().GetHabbo().CurrentRoomId != 75 && this._player.GetClient().GetHabbo().CurrentRoomId != 66 && this._player.GetClient().GetHabbo().CurrentRoomId != 62 && this._player.GetClient().GetHabbo().CurrentRoomId != 97
                    && this._player.GetClient().GetHabbo().CurrentRoomId != 70 && this._player.GetClient().GetHabbo().CurrentRoomId != 94 && this._player.GetClient().GetHabbo().CurrentRoomId != 47 && this._player.GetClient().GetHabbo().CurrentRoomId != 140 && this._player.GetClient().GetHabbo().CurrentRoomId != 28)
                {
                    DataRow Bounty = null;
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("SELECT * FROM `bounties` WHERE `user_id` = @id LIMIT 1");
                        dbClient.AddParameter("id", this._player.GetClient().GetHabbo().Id);
                        Bounty = dbClient.getRow();
                    }

                    this._player.GetClient().GetHabbo().Credits += Convert.ToInt32(Bounty["amount"]);
                    this._player.GetClient().SendMessage(new CreditBalanceComposer(this._player.GetClient().GetHabbo().Credits));
                    this._player.GetClient().GetHabbo().RPCache(3);

                    this._player.GetClient().GetHabbo().BountyTimer = 0;
                    PlusEnvironment.GetGame().GetClientManager().LiveFeed("<b>" + this._player.GetClient().GetHabbo().Username + "</b> has survived their $" + PlusEnvironment.ConvertToPrice(Convert.ToInt32(Bounty["amount"])) + " bounty");
                    PlusEnvironment.GetGame().GetClientManager().HotelWhisper(this._player.GetClient().GetHabbo().Username + " has survived their $" + PlusEnvironment.ConvertToPrice(Convert.ToInt32(Bounty["amount"])) + " bounty");
                    Webhook.SendWebhook(":moneybag: **" + this._player.GetClient().GetHabbo().Username + "** has survived their $" + PlusEnvironment.ConvertToPrice(Convert.ToInt32(Bounty["amount"])) + " bounty");
                    this._player.GetClient().SendWhisper("You has received $" + PlusEnvironment.ConvertToPrice(Convert.ToInt32(Bounty["amount"])) + " for surviving your bounty");

                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("DELETE FROM bounties WHERE user_id = '" + this._player.GetClient().GetHabbo().Id + "';");
                        dbClient.RunQuery();
                    }
                    return;
                }
                else
                {
                    if (this._player.GetClient().GetHabbo().CurrentRoomId != 75 && this._player.GetClient().GetHabbo().CurrentRoomId != 66 && this._player.GetClient().GetHabbo().CurrentRoomId != 62 && this._player.GetClient().GetHabbo().CurrentRoomId != 97
                    && this._player.GetClient().GetHabbo().CurrentRoomId != 70 && this._player.GetClient().GetHabbo().CurrentRoomId != 94 && this._player.GetClient().GetHabbo().CurrentRoomId != 47 && this._player.GetClient().GetHabbo().CurrentRoomId != 140 && this._player.GetClient().GetHabbo().CurrentRoomId != 28)
                    {
                        this._player.GetClient().SendWhisper("Your bounty is not paused because your in a FIGHT zone");
                        this._player.GetClient().GetHabbo().BountyTimer = this._player.GetClient().GetHabbo().BountyTimer - 1;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `bounties` SET `minutes` = " + this._player.GetClient().GetHabbo().BountyTimer + "  WHERE `user_id` = '" + this._player.GetClient().GetHabbo().Id + "' LIMIT 1;");
                            dbClient.RunQuery();
                        }
                        return;
                    }
                    else
                    {
                        this._player.GetClient().SendWhisper("Your bounty is paused because your in a NO FIGHT zone");
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `bounties` SET `minutes` = " + this._player.GetClient().GetHabbo().BountyTimer + "  WHERE `user_id` = '" + this._player.GetClient().GetHabbo().Id + "' LIMIT 1;");
                            dbClient.RunQuery();
                        }
                        return;
                    }
                }

            }
            catch
            {
                //Console.WriteLine("[DayNightTimer] An error has occurred.");
            }
        }

        public void WantedTimer(object State)
        {
            try
            {
                if (this._player == null || !this._player.InRoom || !this._player.GetClient().GetRoleplay().Wan.IsWanted)
                    return;

                if (this._player.GetClient().GetRoleplay().Wan.AddedTime.AddMinutes(15) < DateTime.Now)
                {
                    if (!this._player.GetClient().GetRoleplay().Wan.Passed)
                    {
                        this._player.GetClient().GetRoleplay().Wan.Passed = true;
                        RoleplayManager.LoadWantedList();
                    }
                }
                if (this._player.GetClient().GetRoleplay().Wan.AddedTime.AddMinutes(30) < DateTime.Now)
                {
                    this._player.GetClient().SendWhisper("Your crimes are cleared");
                    this._player.GetClient().GetRoleplay().Wan.Remove();
                }
            }
            catch
            {
                Console.WriteLine("[WantedTimer] An error has occurred");
            }
        }

       

        /// <summary>
        /// Called for each time the timer ticks.
        /// </summary>
        /// <param name="State"></param>
        public void Run(object State)
        {
            try
            {
                if (this._disabled)
                    return;

                if (this._timerRunning)
                {
                    this._timerLagging = true;
                    log.Warn("<Player " + this._player.Id + "> Server can't keep up, Player timer is lagging behind.");
                    return;
                }

                this._resetEvent.Reset();

                // BEGIN CODE

                #region Muted Checks
                if (this._player.TimeMuted > 0)
                    this._player.TimeMuted -= 60;
                #endregion

                #region Console Checks
                if (this._player.MessengerSpamTime > 0)
                    this._player.MessengerSpamTime -= 60;
                if (this._player.MessengerSpamTime <= 0)
                    this._player.MessengerSpamCount = 0;
                #endregion

                this._player.TimeAFK += 1;

                #region Respect checking
                if (this._player.GetStats().RespectsTimestamp != DateTime.Today.ToString("MM/dd"))
                {
                    this._player.GetStats().RespectsTimestamp = DateTime.Today.ToString("MM/dd");
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `user_stats` SET `dailyRespectPoints` = '" + (this._player.Rank == 1 && this._player.VIPRank == 0 ? 10 : this._player.VIPRank == 1 ? 15 : 20) + "', `dailyPetRespectPoints` = '" + (this._player.Rank == 1 && this._player.VIPRank == 0 ? 10 : this._player.VIPRank == 1 ? 15 : 20) + "', `respectsTimestamp` = '" + DateTime.Today.ToString("MM/dd") + "' WHERE `id` = '" + this._player.Id + "' LIMIT 1");
                    }

                    this._player.GetStats().DailyRespectPoints = (this._player.Rank == 1 && this._player.VIPRank == 0 ? 10 : this._player.VIPRank == 1 ? 15 : 20);
                    this._player.GetStats().DailyPetRespectPoints = (this._player.Rank == 1 && this._player.VIPRank == 0 ? 10 : this._player.VIPRank == 1 ? 15 : 20);

                    if (this._player.GetClient() != null)
                    {
                        this._player.GetClient().SendMessage(new UserObjectComposer(this._player));
                    }
                }
                #endregion

                #region Reset Scripting Warnings
                if (this._player.GiftPurchasingWarnings < 15)
                    this._player.GiftPurchasingWarnings = 0;

                if (this._player.MottoUpdateWarnings < 15)
                    this._player.MottoUpdateWarnings = 0;

                if (this._player.ClothingUpdateWarnings < 15)
                    this._player.ClothingUpdateWarnings = 0;
                #endregion


                if (this._player.GetClient() != null)
                    PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(this._player.GetClient(), "ACH_AllTimeHotelPresence", 1);

                // this._player.EnergyTimer();
                // this._player.TimerOneMinute();
                this._player.Effects().CheckEffectExpiry(this._player);

                // END CODE

                // Reset the values
                this._timerRunning = false;
                this._timerLagging = false;

                this._resetEvent.Set();
            }
            catch { }
        }

        /// <summary>
        /// Stops the timer and disposes everything.
        /// </summary>
        public void Dispose()
        {
            // Wait until any processing is complete first.
            try
            {
                this._resetEvent.WaitOne(TimeSpan.FromMinutes(5));
            }
            catch { } // give up

            // Set the timer to disabled
            this._disabled = true;

            // Dispose the timer to disable it.
            try
            {
                if (this._timer != null)
                    this._timer.Dispose();

                if (this._workTimer != null)
                    this._timer.Dispose();

                if (this._blurTimer != null)
                    this._blurTimer.Dispose();

                if (this._HospitalTimer != null)
                    this._HospitalTimer.Dispose();

                if (this._captureTimer != null)
                    this._captureTimer.Dispose();

                if (this._brulingTimer != null)
                    this._brulingTimer.Dispose();

                if (this._gpTimer != null)
                    this._gpTimer.Dispose();

                if (this._gymTimer != null)
                    this._gymTimer.Dispose();

                if (this._daynightTimer != null)
                    this._daynightTimer.Dispose();

                if (this._arenaTimer != null)
                    this._arenaTimer.Dispose();

                if (this._bountyTimer != null)
                    this._bountyTimer.Dispose();

                if (this._wantedTimer != null)
                    this._wantedTimer.Dispose();
            }
            catch { }

            // Remove reference to the timers.
            this._timer = null;
            this._workTimer = null;
            this._blurTimer = null;
            this._HospitalTimer = null;
            this._captureTimer = null;
            this._brulingTimer = null;
            this._gpTimer = null;
            this._gymTimer = null;
            this._daynightTimer = null;
            this._arenaTimer = null;
            this._bountyTimer = null;

            // Null the player so we don't reference it here anymore
            this._player = null;
        }
    }
}