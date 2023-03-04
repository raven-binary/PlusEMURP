using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Utilities;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.GameClients;

using Plus.HabboHotel.Rooms.Chat.Commands.User;
using Plus.HabboHotel.Rooms.Chat.Commands.Moderator;

using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Items.Wired;
using Plus.HabboHotel.Rooms.Chat.Commands.Administrator;

namespace Plus.HabboHotel.Rooms.Chat.Commands
{
    public class CommandManager
    {
        /// <summary>
        /// Command Prefix only applies to custom commands.
        /// </summary>
        private string _prefix = ":";

        /// <summary>
        /// Commands registered for use.
        /// </summary>
        private readonly Dictionary<string, IChatCommand> _commands;

        /// <summary>
        /// The default initializer for the CommandManager
        /// </summary>
        public CommandManager(string Prefix)
        {
            this._prefix = Prefix;
            this._commands = new Dictionary<string, IChatCommand>();

            this.RegisterUsers();
            this.RegisterTarget();
            this.RegisterFun();
            this.RegisterVIP();
            this.RegisterCombat();
            this.RegisterJob();
            this.RegisterManagers();
            this.RegisterGang();
            this.RegisterApparts();
            //this.RegisterItems();
            this.RegisterStaff();
        }

        /// <summary>
        /// Request the text to parse and check for commands that need to be executed.
        /// </summary>
        /// <param name="Session">Session calling this method.</param>
        /// <param name="Message">The message to parse.</param>
        /// <returns>True if parsed or false if not.</returns>
        public bool Parse(GameClient Session, string Message)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().CurrentRoom == null)
                return false;

            if (!Message.StartsWith(_prefix))
                return false;

            if (Message == _prefix + "commands")
            {
                StringBuilder List = new StringBuilder();
                List.Append("General Commands:\n_____________________________________________________\n");
                foreach (var cmdList in _commands.ToList())
                {
                    if (cmdList.Value.TypeCommand != "user")
                        continue;

                    if (!cmdList.Value.getPermission(Session))
                        continue;

                    List.Append(":" + cmdList.Key + " " + cmdList.Value.Parameters + " - " + cmdList.Value.Description + "\n");
                }
                List.Append("\nTarget Commands:\n_____________________________________________________\n");
                foreach (var cmdList in _commands.ToList())
                {
                    if (cmdList.Value.TypeCommand != "target")
                        continue;

                    if (!cmdList.Value.getPermission(Session))
                        continue;

                    List.Append(":" + cmdList.Key + " " + cmdList.Value.Parameters + " - " + cmdList.Value.Description + "\n");
                }
                List.Append("\nFun Commands:\n_____________________________________________________\n");
                foreach (var cmdList in _commands.ToList())
                {
                    if (cmdList.Value.TypeCommand != "fun")
                        continue;

                    if (!cmdList.Value.getPermission(Session))
                        continue;

                    List.Append(":" + cmdList.Key + " " + cmdList.Value.Parameters + " - " + cmdList.Value.Description + "\n");
                }
                if (Session.GetHabbo().Rank > 1)
                {
                    List.Append("\nVIP Commands:\n_____________________________________________________\n");
                    foreach (var cmdList in _commands.ToList())
                    {
                        if (cmdList.Value.TypeCommand != "vip")
                            continue;

                        if (!cmdList.Value.getPermission(Session))
                            continue;

                        List.Append(":" + cmdList.Key + " " + cmdList.Value.Parameters + " - " + cmdList.Value.Description + "\n");
                    }
                }
                List.Append("\nCombat Commands:\n_____________________________________________________\n");
                foreach (var cmdList in _commands.ToList())
                {
                    if (cmdList.Value.TypeCommand != "combat")
                        continue;

                    if (!cmdList.Value.getPermission(Session))
                        continue;

                    List.Append(":" + cmdList.Key + " " + cmdList.Value.Parameters + " - " + cmdList.Value.Description + "\n");
                }
                List.Append("\nApartment Commands:\n_____________________________________________________\n");
                foreach (var cmdList in _commands.ToList())
                {
                    if (cmdList.Value.TypeCommand != "appart")
                        continue;

                    if (!cmdList.Value.getPermission(Session))
                        continue;

                    List.Append(":" + cmdList.Key + " " + cmdList.Value.Parameters + " - " + cmdList.Value.Description + "\n");
                }
                /*List.Append("\nItem Commands:\n_____________________________________________________\n");
                foreach (var cmdList in _commands.ToList())
                {
                    if (cmdList.Value.TypeCommand != "items")
                        continue;

                    if (!cmdList.Value.getPermission(Session))
                        continue;

                    List.Append(":" + cmdList.Key + " " + cmdList.Value.Parameters + " - " + cmdList.Value.Description + "\n");
                }*/
                List.Append("\nJob Commands:\n_____________________________________________________\n");
                foreach (var cmdList in _commands.ToList())
                {
                    if (cmdList.Value.TypeCommand != "job")
                        continue;

                    if (!cmdList.Value.getPermission(Session))
                        continue;

                    List.Append(":" + cmdList.Key + " " + cmdList.Value.Parameters + " - " + cmdList.Value.Description + "\n");
                }
                List.Append("\nManager Commands:\n_____________________________________________________\n");
                foreach (var cmdList in _commands.ToList())
                {
                    if (cmdList.Value.TypeCommand != "manager")
                        continue;

                    if (!cmdList.Value.getPermission(Session))
                        continue;

                    List.Append(":" + cmdList.Key + " " + cmdList.Value.Parameters + " - " + cmdList.Value.Description + "\n");
                }
                List.Append("\nGang Commands:\n_____________________________________________________\n");
                foreach (var cmdList in _commands.ToList())
                {
                    if (cmdList.Value.TypeCommand != "gang")
                        continue;

                    if (!cmdList.Value.getPermission(Session))
                        continue;

                    List.Append(":" + cmdList.Key + " " + cmdList.Value.Parameters + " - " + cmdList.Value.Description + "\n");
                }
                if (Session.GetHabbo().Rank == 8 || Session.GetHabbo().Rank == 7 || Session.GetHabbo().Rank == 6)
                {
                    List.Append("\nStaff Commands:\n_____________________________________________________\n");
                    foreach (var cmdList in _commands.ToList())
                    {
                        if (cmdList.Value.TypeCommand != "staff")
                            continue;

                        if (!cmdList.Value.getPermission(Session))
                            continue;

                        List.Append(":" + cmdList.Key + " " + cmdList.Value.Parameters + " - " + cmdList.Value.Description + "\n");
                    }
                }
                if (Session.GetHabbo().Rank == 8 || Session.GetHabbo().Rank == 7)
                {
                    List.Append("\nTechnician Commands:\n_____________________________________________________\n");
                    foreach (var cmdList in _commands.ToList())
                    {
                        if (cmdList.Value.TypeCommand != "developer")
                            continue;

                        if (!cmdList.Value.getPermission(Session))
                            continue;

                        List.Append(":" + cmdList.Key + " " + cmdList.Value.Parameters + " - " + cmdList.Value.Description + "\n");
                    }
                }
                Session.SendMessage(new MOTDNotificationComposer(List.ToString()));
                return true;
            }

            Message = Message.Substring(1);
            string[] Split = Message.Split(' ');

            if (Split.Length == 0)
                return false;

            IChatCommand Cmd = null;
            if (_commands.TryGetValue(Split[0].ToLower(), out Cmd))
            {
                if (Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                    this.LogCommand(Session.GetHabbo().Id, Message, Session.GetHabbo().MachineId);

                if (!Cmd.getPermission(Session))
                    return false;


                Session.GetHabbo().IChatCommand = Cmd;
                Session.GetHabbo().CurrentRoom.GetWired().TriggerEvent(WiredBoxType.TriggerUserSaysCommand, Session.GetHabbo(), this);

                Cmd.Execute(Session, Session.GetHabbo().CurrentRoom, Split);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Registers the default set of commands.
        /// </summary>
        private void RegisterUsers()
        {
            //General
            this.Register("about", new AboutCommand());
            this.Register("discord", new DiscordCommand());
            this.Register("sit", new SitCommand());
            this.Register("ping", new PingCommand());
            this.Register("dance", new DanceCommand());
            this.Register("hotrooms", new HotRoomsCommand());
            this.Register("trade", new TradeCommand());
            this.Register("911", new CallPoliceCommand());
            this.Register("999", new CallPoliceCommand());
            this.Register("turfs", new TurfsCommand());
            this.Register("gp", new GPCommand());
            this.Register("ct", new ClickThrouhCommand());
            this.Register("inv", new InventorySlotCommand());
            this.Register("give", new GiveMoneyCommand());
            this.Register("quitjob", new QuitjobCommand());
            this.Register("passive", new PassiveCommand());
            this.Register("snack", new SnackCommand());
            this.Register("medkit", new MedkitCommand());
            this.Register("suicide", new SuicideCommand());
            this.Register("payticket", new PayTicketCommand());
            this.Register("bet", new BetCommand());
            this.Register("jukebox", new JukeboxCommand());
            this.Register("b", new BalCommand());
            this.Register("bal", new BalCommand());
            this.Register("trial", new TrialCommand());
            this.Register("marry", new MarryCommand());
            this.Register("divorce", new DivorceCommand());

            //offline commands
            //this.Register("xmouse", new XmouseCommand());
            //this.Register("bet", new MiserCommand());
            //this.Register("exit", new ExitCommand());
        }

        private void RegisterTarget()
        {
            this.Register("target", new GetTargetCommand());
            this.Register("t", new GetTargetCommand());
            this.Register("locktarget", new LockTargetCommand());
            this.Register("lt", new LockTargetCommand());
            this.Register("showprofile", new ShowProfileCommand());
            this.Register("sp", new ShowProfileCommand());
        }

        private void RegisterFun()
        {
            this.Register("slime", new SlimeCommand());
        }

        private void RegisterVIP()
        {
            // VIP
            this.Register("push", new PushCommand());
            this.Register("pull", new PullCommand());
            this.Register("moonwalk", new MoonwalkCommand());
            this.Register("skateboard", new SkateboardCommand());
            this.Register("wardrobe", new WardrobeCommand());
            this.Register("wd", new WardrobeCommand());
            this.Register("hug", new HugCommand());
            this.Register("kis", new KissCommand());
            this.Register("spit", new SpitCommand());
            this.Register("mf", new MiddleFingerCommand());
            this.Register("hideprofile", new HideProfileCommand());
        }

        private void RegisterCombat()
        {
            this.Register("eq", new EquipCommand());
            this.Register("hit", new HitCommand());
            this.Register("lockpick", new LockpickCommand());
            this.Register("spray", new SprayCommand());
            this.Register("1v1", new OneVsOneCommand());
            this.Register("bomb", new BombCommand());
            this.Register("flashbang", new FlashbangCommand());
            this.Register("throwknife", new ThrowingKnifeCommand());

            //offline commands
            //this.Register("shoot", new ShootCommand());
            //this.Register("taper", new TaperCommand());
            //this.Register("plant", new PlanterCommand());
            //this.Register("slap", new SlapCommand());
        }

        private void RegisterApparts()
        {
            this.Register("emptyitems", new EmptyItemsCommand());
            this.Register("pickall", new PickAllCommand());
            //this.Register("renew", new RenouvelerCommand());
        }

        private void RegisterJob()
        {
            this.Register("startwork", new StartWorkCommand());
            this.Register("stopwork", new StopWorkCommand());
            this.Register("stock", new StockCommand());

            //Forever21
            this.Register("style", new StyleCommand());

            //Gouvernement
            //this.Register("makeid", new PapierCommand());
            //this.Register("alert", new AlerteCommand());

            //LAPD (Police)
            this.Register("stun", new StunCommand());
            this.Register("cuff", new CuffCommand());
            this.Register("uncuff", new UnCuffCommand());
            this.Register("arrest", new ArrestCommand());
            this.Register("release", new ReleaseCommand());
            this.Register("timeleft", new TimeleftCommand());
            this.Register("charge", new ChargeCommand());
            this.Register("charges", new ChargesCommand());
            this.Register("ticket", new TicketCommand());
            this.Register("backup", new BackupCommand());
            this.Register("pardon", new PardonCommand());

            //Starbucks
            this.Register("sells", new SellDrinkCommand());
            //this.Register("bingo", new BingoCommand());
            //this.Register("winner", new GagnantCommand());
           //this.Register("eurorp", new EuroRPCommand());
            //this.Register("tabak", new TabacCommand());
            //this.Register("clipper", new ClipperCommand());
            //this.Register("confirm", new ConfirmCommand());

            //Hospital
            //this.Register("analyze", new AnalyzeCommand());
            //this.Register("heal", new HealCommand());
            //this.Register("pickup", new LeverCommand());
            this.Register("sellh", new SellHospitalCommand());
            this.Register("escort", new EscortCommand());
            this.Register("revive", new ReviveCommand());


            // Bank
            //this.Register("sellvoucher", new SellVoucherCommand());
            this.Register("deposit", new DepositCommand());
            this.Register("withdraw", new WithdrawCommand());
            this.Register("balance", new BalanceCommand());
            this.Register("transfer", new TransferCommand());
            this.Register("atms", new ATMsCommand());

            //Armoury
            this.Register("sell", new SellWeaponCommand());
        }

        private void RegisterItems()
        {
            this.Register("drink", new DrinkCommand());
            this.Register("smoke", new SmokeCommand());
            this.Register("eat", new EatCommand());
            this.Register("medicament", new MedicamentCommand());
        }

        private void RegisterManagers()
        {
            this.Register("hire", new HireCommand());
            this.Register("promote", new PromoteCommand());
            this.Register("demote", new DemoteCommand());
            this.Register("shifts", new ShiftsCommand());
            this.Register("sendhome", new SendhomeCommand());
            this.Register("dismiss", new DismissCommand());
        }

        private void RegisterGang()
        {
            this.Register("gang", new GangCommand());
            this.Register("ga", new GangCommand());
            this.Register("viewgang", new ViewGangCommand());
            this.Register("vg", new ViewGangCommand());
        }

        /// <summary>
        /// Registers the moderator set of commands.
        /// </summary>
        private void RegisterStaff()
        {
            this.Register("update", new UpdateCommand()); 
            this.Register("afk", new AFKCommand());
            this.Register("superkill", new SuperKillCommand());
            this.Register("superpull", new SuperPullCommand());

            this.Register("livefeed", new LiveFeedCommand());
            this.Register("itsme", new ItsMeCommand());
            this.Register("duty", new DutyCommand());
            this.Register("invisible", new InvisibleCommand());
            this.Register("superhire", new SuperHireCommand());
            this.Register("superrankup", new SuperRankUpCommand());
            this.Register("superrankdown", new SuperRankDownCommand());
            this.Register("superremovejob", new SuperRankDownCommand());
            this.Register("givecredits", new GiveCreditsCommand());
            this.Register("removecredits", new RemoveCredtisCommand());
            this.Register("givebadge", new GiveBadgeCommand());
            this.Register("restorestats", new RestoreStatsCommand());
            this.Register("follow", new FollowCommand());
            this.Register("senduser", new SendUserCommand());
            this.Register("set", new ShCommand());
            //events
            this.Register("quizz", new QuizzCommand());
            this.Register("question", new QuestionCommand());
            this.Register("reponse", new ReponseCommand());
            //
            this.Register("gep", new GiveEventPointsCommand());
            this.Register("loyer", new LoyerCommand());
            this.Register("coords", new CoordsCommand());
            this.Register("play", new YouTubeCommand());
            this.Register("purge", new PurgeCommand());
            this.Register("salade", new SaladeCommand());
            this.Register("zombie", new ZombieCommand());
            this.Register("reseau", new ReseauCommand());
            this.Register("chiffre", new GiveStockCommand());
            this.Register("roommute", new RoommuteCommand());
            this.Register("enable", new EnableCommand());
            this.Register("teleport", new TeleportCommand());
            this.Register("summon", new SummonCommand());
            this.Register("summonall", new SummonAllCommand());
            this.Register("freeze", new FreezeCommand());
            this.Register("unfreeze", new UnFreezeCommand());
            this.Register("taxi", new TaxiCommand());
            this.Register("tradeban", new TradeBanCommand());
            this.Register("ban", new BanCommand());
            this.Register("ipban", new IPBanCommand());
            this.Register("ui", new UserInfoCommand());
            this.Register("userinfo", new UserInfoCommand());
            this.Register("popalert", new PopAlertCommand());
            this.Register("ha", new HotelAlertCommand());
            this.Register("hal", new HotelAlertLinkCommand());
            this.Register("eha", new EventAlertCommand());
            this.Register("dc", new DisconnectCommand());
            this.Register("disconnect", new DisconnectCommand());
            this.Register("update_bans", new UpdateBansCommand());
            this.Register("bubble", new BubbleCommand());
            this.Register("restart", new RestartCommand());
            this.Register("restartnow", new NowRestartCommand());
            this.Register("safezone", new SafezoneToggleCommand());
        }

        /// <summary>
        /// Registers a Chat Command.
        /// </summary>
        /// <param name="CommandText">Text to type for this command.</param>
        /// <param name="Command">The command to execute.</param>
        public void Register(string CommandText, IChatCommand Command)
        {
            this._commands.Add(CommandText, Command);
        }

        public static string MergeParams(string[] Params, int Start)
        {
            var Merged = new StringBuilder();
            for (int i = Start; i < Params.Length; i++)
            {
                if (i > Start)
                    Merged.Append(" ");
                Merged.Append(Params[i]);
            }

            return Merged.ToString();
        }

        public static string MergeParamsByVirgule(string[] Params, int Start)
        {
            var Merged = new StringBuilder();
            for (int i = Start; i < Params.Length; i++)
            {
                if (i > Start)
                    Merged.Append(",");
                Merged.Append(Params[i]);
            }

            return Merged.ToString();
        }

        public void LogCommand(int UserId, string Data, string MachineId)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `logs_client_staff` (`user_id`,`data_string`,`machine_id`, `timestamp`) VALUES (@UserId,@Data,@MachineId,@Timestamp)");
                dbClient.AddParameter("UserId", UserId);
                dbClient.AddParameter("Data", Data);
                dbClient.AddParameter("MachineId", MachineId);
                dbClient.AddParameter("Timestamp", PlusEnvironment.GetUnixTimestamp());
                dbClient.RunQuery();
            }
        }

        public bool TryGetCommand(string Command, out IChatCommand IChatCommand)
        {
            return this._commands.TryGetValue(Command, out IChatCommand);
        }
    }
}
