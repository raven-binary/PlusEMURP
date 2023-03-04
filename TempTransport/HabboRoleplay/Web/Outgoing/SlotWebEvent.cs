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

namespace Bobba.HabboRoleplay.Web.Outgoing
{
    class SlotWebEvent : IWebEvent
    {
        /// <summary>
        /// Executes socket data.
        /// </summary>
        /// <param name="Client"></param>
        /// <param name="Data"></param>
        /// <param name="Socket"></param>
        public void Execute(GameClient Client, string Data, IWebSocketConnection Socket)
        {

            if (!PlusEnvironment.GetGame().GetWebEventManager().SocketReady(Client, true) || !PlusEnvironment.GetGame().GetWebEventManager().SocketReady(Socket))
                return;


            string Action = (Data.Contains(',') ? Data.Split(',')[0] : Data);

            switch (Action)
            {
                #region slot
                case "slot":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (!User.connectedToSlot)
                            return;

                        if (User.isSlot)
                        {
                            Client.SendWhisper("Du spielst schon!");
                            return;
                        }

                        if (Client.GetHabbo().Casino_Jetons <= 0)
                        {
                            Client.SendWhisper("Du benötigst einen Casino-Chip, um spielen zu können.");
                            return;
                        }

                        if (User.participateRoulette == true)
                        {
                            Client.SendWhisper("Bitte warte auf die Roulette - Ziehung, bevor du den Spielautomaten spielen können.");
                            return;
                        }

                        User.isSlot = true;
                        User.Frozen = true;
                        Random winChance = new Random();
                        int winNumber = winChance.Next(1, 20);
                        int jackpotNumber = winChance.Next(1, 250000);

                        Client.GetHabbo().Casino_Jetons -= 1;
                        Client.GetHabbo().updateCasinoJetons();
                        User.GetClient().Shout("*Wirft einen Chip und startet den Spielautomaten*");
                        int Win;

                        #region win
                        if (jackpotNumber == 241683)
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "slot_machine;spin;" + Client.GetHabbo().Casino_Jetons + ";2;2;2");
                            Win = 300;
                        }
                        else if (winNumber == 3 || winNumber == 7)
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "slot_machine;spin;" + Client.GetHabbo().Casino_Jetons + ";5;5;5");
                            Win = 2;
                        }
                        else if (winNumber == 5)
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "slot_machine;spin;" + Client.GetHabbo().Casino_Jetons + ";4;4;4");
                            Win = 5;
                        }
                        else if (winNumber == 9)
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "slot_machine;spin;" + Client.GetHabbo().Casino_Jetons + ";1;1;1");
                            Win = 4;
                        }
                        else if (winNumber == 14 || winNumber == 17 || winNumber == 19)
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "slot_machine;spin;" + Client.GetHabbo().Casino_Jetons + ";3;3;3");
                            Win = 1;
                        }
                        else
                        {
                            int number1 = winChance.Next(1, 6);
                            int number2 = winChance.Next(1, 6);
                            int number3 = winChance.Next(1, 6);

                            if (number3 == number1 && number3 == number2)
                            {
                                if (number3 == 4)
                                {
                                    number3 = 2;
                                }
                                else
                                {
                                    number3 = 4;
                                }
                            }

                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "slot_machine;spin;" + Client.GetHabbo().Casino_Jetons + ";" + number1 + ";" + number2 + ";" + number3);
                            Win = 0;
                        }

                        #endregion

                        System.Timers.Timer timer2 = new System.Timers.Timer(5000);
                        timer2.Interval = 5000;
                        timer2.Elapsed += delegate
                        {
                            if (User.isSlot == true)
                            {
                                if (Win == 300)
                                {
                                    User.GetClient().Shout("*Gewinnt den Jackpot von 300 Chips*");
                                    Client.GetHabbo().Casino_Jetons += 300;
                                    Client.GetHabbo().updateCasinoJetons();
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "slot_machine;connect;" + Client.GetHabbo().Casino_Jetons);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "slot_machine;jackpot");
                                }
                                else if (Win > 0)
                                {
                                    User.GetClient().Shout("*Gewinnt " + Win + " Chip(s)*");
                                    Client.GetHabbo().Casino_Jetons += Win;
                                    Client.GetHabbo().updateCasinoJetons();
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "slot_machine;connect;" + Client.GetHabbo().Casino_Jetons);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "slot_machine;win");
                                }
                                else if (Win == 0)
                                {
                                    User.GetClient().Shout("*Hat verloren*");
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "slot_machine;loose");
                                }
                                User.Frozen = false;
                                User.isSlot = false;
                            }
                            timer2.Stop();
                        };
                        timer2.Start();

                        break;
                    }
                    #endregion
            }
        }
    }
}