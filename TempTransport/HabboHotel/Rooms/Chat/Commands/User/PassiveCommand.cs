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
    class PassiveCommand : IChatCommand
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
            get { return "Toggle passive mode, preventing you from being attacked"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (Room.Fightable)
            {
                Session.SendWhisper("You cannot toggle passive mode in none-safezone rooms");
                return;
            }

            Random TokenRand = new Random();
            int tokenNumber = TokenRand.Next(1600, 2894354);
            Session.GetHabbo().PlayToken = tokenNumber;

            if (Session.GetRoleplay().Passive)
            {
                User.Say("starts to leave passive mode");
                Session.GetHabbo().usingPassive = true;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "timed-action;" + "show" + ";" + "Leaving passive mode" + ";" + 15000 + ";" + 15);
                System.Timers.Timer timer1 = new System.Timers.Timer(15000);
                timer1.Interval = 15000;
                timer1.Elapsed += delegate
                {
                    if (Session.GetHabbo().usingPassive && Session.GetHabbo().PlayToken == tokenNumber)
                    {
                        User.Say("leaves passive mode");
                        Session.GetHabbo().usingPassive = false;
                        Session.GetRoleplay().Passive = false;
                        Session.GetHabbo().resetEffectEvent();
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "timed-action;" + "hide");
                        Session.GetHabbo().RPCache(1);
                    }
                    timer1.Stop();
                };
                timer1.Start();
            }
            else
            {
                if (User.GetClient().GetRoleplay().Health != User.GetClient().GetRoleplay().HealthMax)
                {
                    Session.SendWhisper("You need full health to enter passive mode");
                    return;
                }
                else
                {
                    User.Say("starts to enter passive mode");
                    Session.GetHabbo().usingPassive = true;
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "timed-action;" + "show" + ";" + "Entering passive mode" + ";" + 15000 + ";" + 15);
                    System.Timers.Timer timer1 = new System.Timers.Timer(15000);
                    timer1.Interval = 15000;
                    timer1.Elapsed += delegate
                    {
                        if (Session.GetHabbo().usingPassive && Session.GetHabbo().PlayToken == tokenNumber)
                        {
                            User.Say("enters passive mode");
                            Session.GetHabbo().usingPassive = false;
                            User.ApplyEffect(706);
                            Session.GetRoleplay().Passive = true;
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "timed-action;" + "hide");
                            Session.GetHabbo().RPCache(1);
                        }
                        timer1.Stop();
                    };
                    timer1.Start();
                }
            }
        }
    }
}
