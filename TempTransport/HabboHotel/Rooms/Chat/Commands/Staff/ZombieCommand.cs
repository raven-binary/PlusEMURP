using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class ZombieCommand : IChatCommand
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
            get { return "Starts Man vs Zombie Event"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
             
            if(PlusEnvironment.ManVsZombieLoading == true || PlusEnvironment.ManVsZombie != 0)
            {
                Session.SendWhisper("An event is already organized.");
                return;
            }

            if(Session.GetHabbo().CurrentRoomId == 1 || Session.GetHabbo().CurrentRoomId == 3 || Session.GetHabbo().CurrentRoomId == 18 || Session.GetHabbo().CurrentRoomId == 20)
            {
                Session.SendWhisper("You cannot throw salad in the city center, in the hospital or in the prison and its courtyard.");
                return;
            }

            if (Session.GetHabbo().CurrentRoom.Description.Contains("GHETTO"))
            {
                Session.SendWhisper("You can't organize Man VS Zombie in a ghetto.");
                return;
            }

            if(PlusEnvironment.Purge == true || PlusEnvironment.Salade != 0 || PlusEnvironment.SaladeAttente == true)
            {
                Session.SendWhisper("You cannot launch a Man VS Zombie because another event is organized.");
                return;
            }

            PlusEnvironment.ManVsZombieLoading = true;
            int AppartId = Session.GetHabbo().CurrentRoomId;
            Room AppartZombie = Session.GetHabbo().CurrentRoom;

            PlusEnvironment.GetGame().GetClientManager().sendServerMessage("The Zombie Event starts in 5 minutes at [" + AppartZombie.Id + "] " + AppartZombie.Name + ".");
            System.Timers.Timer timer2 = new System.Timers.Timer(120000);
            timer2.Interval = 120000;
            timer2.Elapsed += delegate
            {
                PlusEnvironment.GetGame().GetClientManager().sendServerMessage("The Zombie Event starts in 3 minutes at [" + AppartZombie.Id + "] " + AppartZombie.Name + ".");
                timer2.Stop();
            };
            timer2.Start();
            
            System.Timers.Timer timer3 = new System.Timers.Timer(180000);
            timer3.Interval = 180000;
            timer3.Elapsed += delegate
            {
                PlusEnvironment.GetGame().GetClientManager().sendServerMessage("The Zombie Event starts in 2 minutes at [" + AppartZombie.Id + "] " + AppartZombie.Name + ".");
                timer3.Stop();
            };
            timer3.Start();

            System.Timers.Timer timer4 = new System.Timers.Timer(240000);
            timer4.Interval = 240000;
            timer4.Elapsed += delegate
            {
                PlusEnvironment.GetGame().GetClientManager().sendServerMessage("The Zombie Event starts in 1 minutes at [" + AppartZombie.Id + "] " + AppartZombie.Name + ".");
                timer4.Stop();
            };
            timer4.Start();

            System.Timers.Timer timer5 = new System.Timers.Timer(270000);
            timer5.Interval = 270000;
            timer5.Elapsed += delegate
            {
                PlusEnvironment.GetGame().GetClientManager().sendServerMessage("The Zombie Event starts in 30 seconds at [" + AppartZombie.Id + "] " + AppartZombie.Name + ".");
                timer5.Stop();
            };
            timer5.Start();

            System.Timers.Timer timer6 = new System.Timers.Timer(280000);
            timer6.Interval = 280000;
            timer6.Elapsed += delegate
            {
                PlusEnvironment.GetGame().GetClientManager().sendServerMessage("The Zombie Event starts in 20 seconds at [" + AppartZombie.Id + "] " + AppartZombie.Name + ".");
                timer6.Stop();
            };
            timer6.Start();

            System.Timers.Timer timer7 = new System.Timers.Timer(290000);
            timer7.Interval = 290000;
            timer7.Elapsed += delegate
            {
                PlusEnvironment.GetGame().GetClientManager().sendServerMessage("The Zombie Event starts in 10 seconds at [" + AppartZombie.Id + "] " + AppartZombie.Name + ".");
                timer7.Stop();
            };
            timer7.Start();

            System.Timers.Timer timer8 = new System.Timers.Timer(295000);
            timer8.Interval = 295000;
            timer8.Elapsed += delegate
            {
                PlusEnvironment.GetGame().GetClientManager().sendServerMessage("The Zombie Event starts in 5 seconds at [" + AppartZombie.Id + "] " + AppartZombie.Name + ".");
                timer8.Stop();
            };
            timer8.Start();

            System.Timers.Timer timer9 = new System.Timers.Timer(296000);
            timer9.Interval = 296000;
            timer9.Elapsed += delegate
            {
                PlusEnvironment.GetGame().GetClientManager().sendServerMessage("The Zombie Event starts in 4 seconds at [" + AppartZombie.Id + "] " + AppartZombie.Name + ".");
                timer9.Stop();
            };
            timer9.Start();

            System.Timers.Timer timer10 = new System.Timers.Timer(297000);
            timer10.Interval = 297000;
            timer10.Elapsed += delegate
            {
                PlusEnvironment.GetGame().GetClientManager().sendServerMessage("The Zombie Event starts in 3 seconds at [" + AppartZombie.Id + "] " + AppartZombie.Name + ".");
                timer10.Stop();
            };
            timer10.Start();

            System.Timers.Timer timer11 = new System.Timers.Timer(298000);
            timer11.Interval = 298000;
            timer11.Elapsed += delegate
            {
                PlusEnvironment.GetGame().GetClientManager().sendServerMessage("The Zombie Event starts in 1 seconds at [" + AppartZombie.Id + "] " + AppartZombie.Name + ".");
                timer11.Stop();
            };
            timer11.Start();

            System.Timers.Timer timer12 = new System.Timers.Timer(299000);
            timer12.Interval = 299000;
            timer12.Elapsed += delegate
            {
                PlusEnvironment.GetGame().GetClientManager().sendServerMessage("The Zombie Event starts in 1 second at [" + AppartZombie.Id + "] " + AppartZombie.Name + ".");
                timer12.Stop();
            };
            timer12.Start();

            System.Timers.Timer timer13 = new System.Timers.Timer(300000);
            timer13.Interval = 300000;
            timer13.Elapsed += delegate
            {
                int count = 0;
                foreach (RoomUser UserInRoom in AppartZombie.GetRoomUserManager().GetUserList().ToList())
                {
                    if (UserInRoom == null || UserInRoom.IsBot || UserInRoom.GetClient() == null || UserInRoom.GetClient().GetHabbo() == null || UserInRoom.GetClient().GetHabbo().Rank == 8)
                        continue;

                    count++;
                }

                if(2 > count)
                {
                    PlusEnvironment.GetGame().GetClientManager().sendStaffMsg("The Zombie was canceled because there were not enough participants.");
                    PlusEnvironment.ManVsZombieLoading = false;
                    return;
                }

                int ZombieUser = Convert.ToInt32(count / 2);
                count = 0;
                foreach (RoomUser UserInRoom in AppartZombie.GetRoomUserManager().GetUserList().ToList())
                {
                    if (UserInRoom == null || UserInRoom.IsBot || UserInRoom.GetClient() == null || UserInRoom.GetClient().GetHabbo() == null || UserInRoom.GetClient().GetHabbo().Rank == 8)
                        continue;

                    count++;
                    if(count <= ZombieUser)
                    {
                        UserInRoom.ManVsZombieTeam = "zombie";
                        UserInRoom.GetClient().SendWhisper("You are a zombie, try to bite the most citizens without getting stung to win.");
                    }
                    else
                    {
                        UserInRoom.ManVsZombieTeam = "man";
                        UserInRoom.GetClient().SendWhisper("You are a citizen, try to heal as many zombies as possible without getting bitten to win.");
                    }

                    UserInRoom.GetClient().GetHabbo().resetAvatarEvent();
                }

                PlusEnvironment.ManVsZombie = AppartId;
                PlusEnvironment.ManVsZombieLoading = false;
                PlusEnvironment.GetGame().GetClientManager().sendServerMessage("The zobmie event has started.");
                timer13.Stop();
            };
            timer13.Start();
           
        }
    }
}