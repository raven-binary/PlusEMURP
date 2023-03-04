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
using System.Threading;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class SuicideCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
        }

        public string TypeCommand
        {
            get { return "user"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Use suicide"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (Session.GetHabbo().IsWaitingForParamedic)
            {
                Session.SendWhisper("You can not perform this action while dead");
                return;
            }

            if (Session.GetHabbo().Suicidecool != 0)
            {
                Session.SendWhisper("Cooldown");
                return;
            }

            User.Say("grabs their pills and inserts them into each of their holes");
            Session.GetHabbo().usingSuicide = true;

            Random TokenRand = new Random();
            int tokenNumber = TokenRand.Next(1600, 2894354);
            Session.GetHabbo().PlayToken = tokenNumber;

            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "timed-action;" + "show" + ";" + "Using Suicide" + ";" + 10000 + ";" + 10);

            System.Timers.Timer timer1 = new System.Timers.Timer(10000);
            timer1.Interval = 10000;
            timer1.Elapsed += delegate
            {
                if (Session.GetHabbo().usingSuicide && Session.GetHabbo().PlayToken == tokenNumber && User.GetClient().GetHabbo().Suicidecool == 0 && !Session.GetHabbo().getCooldown("suicide"))
                {

                    User.Say("overdoses on pills, causing there organs to fail");
                   
                    User.GetClient().SendWhisper("You have died and lost " + Session.GetHabbo().Credits + " dollars");

                    int Oldcoins = Session.GetHabbo().Credits;
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "timed-action;hide");
                    Session.GetHabbo().Credits -= Session.GetHabbo().Credits;
                    Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits - Oldcoins));
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "my_stats;" + Session.GetHabbo().Credits + ";" + Session.GetHabbo().Duckets + ";" + Session.GetHabbo().EventPoints);
                    Session.GetRoleplay().RPCache(3);
                    Session.GetRoleplay().Health -= Session.GetRoleplay().Health;
                    Session.GetRoleplay().Aggression -= Session.GetRoleplay().Aggression;
                    Session.GetHabbo().HPBarley();
                    Session.GetHabbo().addCooldown("suicide", 6000000);
                    Session.GetHabbo().Suicidecool = 99990;

                  
                }
                timer1.Stop();
            };
            timer1.Start();

        }
    }
}
