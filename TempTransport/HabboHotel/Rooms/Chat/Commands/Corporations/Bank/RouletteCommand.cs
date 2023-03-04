using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.HabboHotel.Groups;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class RouletteCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
        }

        public string TypeCommand
        {
            get { return "job"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Start the roulette"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (PlusEnvironment.RouletteTurning == true)
            {
                Session.SendWhisper("Please wait, the roulette is spinning...");
                return;
            }

            if (PlusEnvironment.RouletteEtat == 0)
            {
                Room CurrentRoom = Session.GetHabbo().CurrentRoom;

                int CountMiser = 0;
                foreach (RoomUser UserInRoom in CurrentRoom.GetRoomUserManager().GetUserList().ToList())
                {
                    if (UserInRoom == null || UserInRoom.IsBot || UserInRoom.GetClient() == null || UserInRoom.GetClient().GetHabbo() == null)
                        continue;

                    if (UserInRoom.participateRoulette == true)
                        CountMiser += 1;
                }

                if (CountMiser == 0)
                {
                    Session.SendWhisper("Please wait until at least one person has bet before you start the roulette.");
                    return;
                }

                PlusEnvironment.RouletteEtat = 1;
                PlusEnvironment.RouletteTurning = true;

                Random rouletteNumber = new Random();
                int winNumber = rouletteNumber.Next(0, 37);

                foreach (RoomUser UserInRoom in CurrentRoom.GetRoomUserManager().GetUserList().ToList())
                {
                    if (UserInRoom == null || UserInRoom.IsBot || UserInRoom.GetClient() == null || UserInRoom.GetClient().GetHabbo() == null)
                        continue;

                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(UserInRoom.GetClient(), "roulette_casino;spinTo;" + winNumber);
                }
                User.GetClient().Shout("*Starts the roulette*");

                System.Timers.Timer timer1 = new System.Timers.Timer(9000);
                timer1.Interval = 9000;
                timer1.Elapsed += delegate
                {
                    int[] red = { 32, 19, 21, 25, 34, 27, 36, 30, 23, 5, 16, 1, 14, 9, 18, 7, 12, 3 };

                    foreach (RoomUser UserInRoom in CurrentRoom.GetRoomUserManager().GetUserList().ToList())
                    {
                        if (UserInRoom == null || UserInRoom.IsBot || UserInRoom.GetClient() == null || UserInRoom.GetClient().GetHabbo() == null)
                            continue;
                        
                        if (UserInRoom.participateRoulette == true)
                        {
                            if (UserInRoom.numberRoulette == 0 && winNumber == 0)
                            {
                                int WinJetons = UserInRoom.miseRoulette * 10;
                                UserInRoom.GetClient().GetHabbo().Casino_Jetons += WinJetons;
                                UserInRoom.GetClient().GetHabbo().updateCasinoJetons();
                                User.GetClient().Shout("*Wins " + (WinJetons - UserInRoom.miseRoulette) + " chips in roulette (by betting " + UserInRoom.miseRoulette + " chips)*");
                            }
                            else if (UserInRoom.numberRoulette == winNumber && UserInRoom.numberRoulette != 0 && winNumber != 0)
                            {
                                int WinJetons = UserInRoom.miseRoulette * 3;
                                UserInRoom.GetClient().GetHabbo().Casino_Jetons += WinJetons;
                                UserInRoom.GetClient().GetHabbo().updateCasinoJetons();
                                User.GetClient().Shout("*Wins " + (WinJetons - UserInRoom.miseRoulette) + " chips in roulette " + UserInRoom.miseRoulette + " chips)*");
                            }
                            else if (red.Contains(UserInRoom.numberRoulette) && red.Contains(winNumber) && UserInRoom.numberRoulette != 0 && winNumber != 0 || !red.Contains(UserInRoom.numberRoulette) && !red.Contains(winNumber) && UserInRoom.numberRoulette != 0 && winNumber != 0)
                            {
                                int WinJetons = Convert.ToInt32(UserInRoom.miseRoulette * 1.5);
                                UserInRoom.GetClient().GetHabbo().Casino_Jetons += WinJetons;
                                UserInRoom.GetClient().GetHabbo().updateCasinoJetons();
                                User.GetClient().Shout("*Wins " + (WinJetons - UserInRoom.miseRoulette) + " chips in roulette (by betting " + UserInRoom.miseRoulette + " chips)*");
                            }
                            else
                            {
                                UserInRoom.GetClient().SendWhisper("You won nothing at roulette.");
                            }

                            UserInRoom.participateRoulette = false;
                            UserInRoom.numberRoulette = 0;
                            UserInRoom.miseRoulette = 0;
                        }
                    }

                    PlusEnvironment.RouletteTurning = false;
                    timer1.Stop();
                };
                timer1.Start();
                return;
            }
            else if(PlusEnvironment.RouletteEtat != 0)
            {
                User.GetClient().Shout("*Places his bets*");
                PlusEnvironment.RouletteEtat = 0;

                foreach (RoomUser UserInRoom in Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetUserList().ToList())
                {
                    if (UserInRoom == null || UserInRoom.IsBot || UserInRoom.GetClient() == null || UserInRoom.GetClient().GetHabbo() == null)
                        continue;

                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(UserInRoom.GetClient(), "roulette_casino;reset");
                }
                return;
            }
        }
    }
}