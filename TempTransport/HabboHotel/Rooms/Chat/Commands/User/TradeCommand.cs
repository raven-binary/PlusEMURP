using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class TradeCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
        }

        public string TypeCommand
        {
            get { return "items"; }
        }

        public string Parameters
        {
            get { return "<username>"; }
        }

        public string Description
        {
            get { return "Trade Items/Credits with a target"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Session.GetHabbo().isTradingWith != null)
                return;

            if (Session.GetHabbo().TradingLockExpiry > 0)
            {
                if (Session.GetHabbo().TradingLockExpiry > PlusEnvironment.GetUnixTimestamp())
                {
                    Session.SendWhisper("You're currently banned from trading");
                    return;
                }
                else
                {
                    Session.GetHabbo().TradingLockExpiry = 0;
                    Session.SendWhisper("Your trading ban has now expired, please do not scam again");

                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `user_info` SET `trading_locked` = '0' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                    }
                }
            }

            if (Params.Length < 2)
            {
                Session.SendWhisper("Please specify a target");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper("Player not found in this room");
                return;
            }

            if (TargetClient.GetHabbo().Id == Session.GetHabbo().Id)
                return;

            if (TargetClient.GetHabbo().TradingLockExpiry > 0)
            {
                Session.SendNotification(TargetClient.GetHabbo().Username + " is currently banned from trading");
                return;
            }

            if (TargetClient.GetHabbo().isTradingWith != null)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " is already in a trade");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

            if (TargetUser.IsBot)
            {
                Session.SendWhisper("You cannot trade with bots");
                return;
            }

            if (Math.Abs(User.Y - TargetUser.Y) > 1 || Math.Abs(User.X - TargetUser.X) > 1)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " are too far away");
                return;
            }

            User.Say("wishes to trade with " + TargetClient.GetHabbo().Username);
            TargetClient.GetHabbo().TransactionFrom = Session.GetHabbo().Username;
            Session.GetHabbo().TransactionTo = TargetClient.GetHabbo().Username;
            TargetUser.Transaction = "trade:" + Session.GetHabbo().Username;
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction;<b>" + Session.GetHabbo().Username + "</b> wishes to trade with you");

            System.Timers.Timer Timer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(5));
            Timer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(5);
            Timer.Elapsed += delegate
            {
                if (Session.GetHabbo().TradeToken == 0 && Session.GetHabbo().isTradingWith != TargetClient.GetHabbo().Username && TargetClient.GetHabbo().isTradingWith != Session.GetHabbo().Username)
                {
                    Session.GetHabbo().TransactionTo = null;
                    TargetUser.Transaction = null;
                    TargetClient.GetHabbo().TransactionFrom = null;

                    Session.SendWhisper("The trade has expired");
                    TargetClient.SendWhisper("The trade has expired");
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction-hide;");
                }
                Timer.Stop();
            };
            Timer.Start();
        }
    }
}