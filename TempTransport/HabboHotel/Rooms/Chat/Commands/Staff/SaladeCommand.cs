using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class SaladeCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank == 8)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "staff"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Start an event"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if(PlusEnvironment.SaladeAttente == true || PlusEnvironment.Salade != 0)
            {
                Session.SendWhisper("An event is already organized.");
                return;
            }

            if(Session.GetHabbo().CurrentRoomId == 1 || Session.GetHabbo().CurrentRoomId == 3 || Session.GetHabbo().CurrentRoomId == 18 || Session.GetHabbo().CurrentRoomId == 20)
            {
                Session.SendWhisper("You cannot start an event in the city center, in the hospital or in the prison.");
                return;
            }

            if (Session.GetHabbo().CurrentRoom.Description.Contains("GHETTO"))
            {
                Session.SendWhisper("You cannot organize an event in a 'GHETTO' room.");
                return;
            }

            if(PlusEnvironment.Purge == true || PlusEnvironment.ManVsZombie != 0 || PlusEnvironment.ManVsZombieLoading == true)
            {
                Session.SendWhisper("You cannot start an event because another event is taking place.");
                return;
            }

            PlusEnvironment.SaladeAttente = true;
            int AppartId = Session.GetHabbo().CurrentRoomId;
            Room AppartSalade = Session.GetHabbo().CurrentRoom;

            PlusEnvironment.GetGame().GetClientManager().sendServerMessage("The event will start in 5 minutes in the [" + AppartSalade.Id + "] " + AppartSalade.Name + " to start.");
            PlusEnvironment.GetGame().GetClientManager().sendServerMessage("Don't lose money during the salad!");
            System.Timers.Timer timer2 = new System.Timers.Timer(120000);
            timer2.Interval = 120000;
            timer2.Elapsed += delegate
            {
                PlusEnvironment.GetGame().GetClientManager().sendServerMessage("The event will start in 3 Minutes in the [" + AppartSalade.Id + "] " + AppartSalade.Name + " to start.");
                timer2.Stop();
            };
            timer2.Start();
            
            System.Timers.Timer timer3 = new System.Timers.Timer(180000);
            timer3.Interval = 180000;
            timer3.Elapsed += delegate
            {
                PlusEnvironment.GetGame().GetClientManager().sendServerMessage("The event will start in 2 Minutes in the [" + AppartSalade.Id + "] " + AppartSalade.Name + " to start.");
                timer3.Stop();
            };
            timer3.Start();

            System.Timers.Timer timer4 = new System.Timers.Timer(240000);
            timer4.Interval = 240000;
            timer4.Elapsed += delegate
            {
                PlusEnvironment.GetGame().GetClientManager().sendServerMessage("The event will start in 1 Minutes in the [" + AppartSalade.Id + "] " + AppartSalade.Name + " to start.");
                timer4.Stop();
            };
            timer4.Start();

            System.Timers.Timer timer5 = new System.Timers.Timer(270000);
            timer5.Interval = 270000;
            timer5.Elapsed += delegate
            {
                PlusEnvironment.GetGame().GetClientManager().sendServerMessage("The event will start in 30 Seconds in the [" + AppartSalade.Id + "] " + AppartSalade.Name + " to start.");
                timer5.Stop();
            };
            timer5.Start();

            System.Timers.Timer timer6 = new System.Timers.Timer(280000);
            timer6.Interval = 280000;
            timer6.Elapsed += delegate
            {
                PlusEnvironment.GetGame().GetClientManager().sendServerMessage("The event will start in 20 Seconds in the [" + AppartSalade.Id + "] " + AppartSalade.Name + " to start.");
                timer6.Stop();
            };
            timer6.Start();

            System.Timers.Timer timer7 = new System.Timers.Timer(290000);
            timer7.Interval = 290000;
            timer7.Elapsed += delegate
            {
                PlusEnvironment.GetGame().GetClientManager().sendServerMessage("The event will start in 10 Seconds in the [" + AppartSalade.Id + "] " + AppartSalade.Name + " to start.");
                timer7.Stop();
            };
            timer7.Start();

            System.Timers.Timer timer8 = new System.Timers.Timer(295000);
            timer8.Interval = 295000;
            timer8.Elapsed += delegate
            {
                PlusEnvironment.GetGame().GetClientManager().sendServerMessage("The event will start in 5 Seconds in the [" + AppartSalade.Id + "] " + AppartSalade.Name + " to start.");
                timer8.Stop();
            };
            timer8.Start();

            System.Timers.Timer timer9 = new System.Timers.Timer(296000);
            timer9.Interval = 296000;
            timer9.Elapsed += delegate
            {
                PlusEnvironment.GetGame().GetClientManager().sendServerMessage("The event will start in 4 Seconds in the [" + AppartSalade.Id + "] " + AppartSalade.Name + " to start.");
                timer9.Stop();
            };
            timer9.Start();

            System.Timers.Timer timer10 = new System.Timers.Timer(297000);
            timer10.Interval = 297000;
            timer10.Elapsed += delegate
            {
                PlusEnvironment.GetGame().GetClientManager().sendServerMessage("The event will start in 3 Seconds in the [" + AppartSalade.Id + "] " + AppartSalade.Name + " to start.");
                timer10.Stop();
            };
            timer10.Start();

            System.Timers.Timer timer11 = new System.Timers.Timer(298000);
            timer11.Interval = 298000;
            timer11.Elapsed += delegate
            {
                PlusEnvironment.GetGame().GetClientManager().sendServerMessage("The event will start in 2 Seconds in the [" + AppartSalade.Id + "] " + AppartSalade.Name + " to start.");
                timer11.Stop();
            };
            timer11.Start();

            System.Timers.Timer timer12 = new System.Timers.Timer(299000);
            timer12.Interval = 299000;
            timer12.Elapsed += delegate
            {
                PlusEnvironment.GetGame().GetClientManager().sendServerMessage("The event will start in 1 Seconds in the [" + AppartSalade.Id + "] " + AppartSalade.Name + " to start.");
                timer12.Stop();
            };
            timer12.Start();

            System.Timers.Timer timer13 = new System.Timers.Timer(300000);
            timer13.Interval = 300000;
            timer13.Elapsed += delegate
            {
                PlusEnvironment.Salade = AppartId;
                PlusEnvironment.SaladeAttente = false;
                PlusEnvironment.GetGame().GetClientManager().sendServerMessage("The event has started.");
                timer13.Stop();
            };
            timer13.Start();
        }
    }
}