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
using Plus.Communication.Packets.Outgoing.Rooms.Notifications;
using System.Data;
using System.Threading;

namespace Bobba.HabboRoleplay.Web.Outgoing
{
    class CardsWebEvent : IWebEvent
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
                #region Random
                case "random":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Client.GetHabbo().CardsWith);
                        if (TargetClient == null)
                            return;

                        Random rnd = new Random();
                        int Number = rnd.Next(1, 14);
                        var List = new List<string> { "clubs", "diamonds", "hearts", "spades" };
                        int index = rnd.Next(List.Count);
                        string Card = string.Empty;

                        if (Number == 1)
                        {
                            Card =  "ace_of_" + List[index];
                        }
                        else if (Number == 11)
                        {
                            Card = "jack_of_" + List[index];
                        }
                        else if (Number == 12)
                        {
                            Card = "queen_of_" + List[index];
                        }
                        else if (Number == 13)
                        {
                            Card = "king_of_" + List[index];
                        }
                        else
                        {
                            Card = Number + "_of_clubs";
                        }

                        TargetClient.GetHabbo().CurrentCard = Number;


                        Client.SendWhisper(TargetClient.GetHabbo().Username + " has 15 seconds to take their turn");
                        TargetClient.SendWhisper(TargetClient.GetHabbo().Username + " has 15 seconds to take their turn");

                        Random TokenRand = new Random();
                        int tokenNumber = TokenRand.Next(1600, 2894354);
                        TargetClient.GetHabbo().PlayToken = tokenNumber;

                        System.Timers.Timer timer1 = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(15));
                        timer1.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(15);
                        timer1.Elapsed += delegate
                        {
                            if (TargetClient.GetHabbo().CardsPlaying && TargetClient.GetHabbo().PlayToken == tokenNumber)
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(TargetClient, "cards", "pick");
                            }
                            timer1.Stop();
                        };
                        timer1.Start();

                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "current-card;" + Card);
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "current-card;" + Card);

                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "card-higher-button;0");
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "card-higher-button;1");

                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "card-table-status;Opponent's Turn");
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "card-table-status;Your Turn");
                    }
                    break;
                #endregion
                #region Higher
                case "higher":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (!Client.GetHabbo().CardsPlaying || !Client.GetHabbo().MyTurn)
                            return;

                        GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Client.GetHabbo().CardsWith);
                        if (TargetClient == null)
                            return;

                        Random rnd = new Random();
                        int Number = rnd.Next(1, 14);
                        var List = new List<string> { "clubs", "diamonds", "hearts", "spades" };
                        int index = rnd.Next(List.Count);
                        string Card = string.Empty;

                        if (Number == 1)
                        {
                            Card = "ace_of_" + List[index];
                        }
                        else if (Number == 11)
                        {
                            Card = "jack_of_" + List[index];
                        }
                        else if (Number == 12)
                        {
                            Card = "queen_of_" + List[index];
                        }
                        else if (Number == 13)
                        {
                            Card = "king_of_" + List[index];
                        }
                        else
                        {
                            Card = Number + "_of_clubs";
                        }

                        Client.SendWhisper(Client.GetHabbo().Username + " flips a " + Number);
                        TargetClient.SendWhisper(Client.GetHabbo().Username + " flips a " + Number);

                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "current-card;" + Card);
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "current-card;" + Card);

                        if (Number > Client.GetHabbo().CurrentCard)
                        {
                            Client.SendWhisper(TargetClient.GetHabbo().Username + " has 15 seconds to take their turn");
                            TargetClient.SendWhisper(TargetClient.GetHabbo().Username + " has 15 seconds to take their turn");

                            Random TokenRand = new Random();
                            int tokenNumber = TokenRand.Next(1600, 2894354);
                            TargetClient.GetHabbo().PlayToken = tokenNumber;

                            System.Timers.Timer timer1 = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(15));
                            timer1.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(15);
                            timer1.Elapsed += delegate
                            {
                                if (TargetClient.GetHabbo().CardsPlaying && TargetClient.GetHabbo().PlayToken == tokenNumber)
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(TargetClient, "cards", "pick");
                                }
                                timer1.Stop();
                            };
                            timer1.Start();

                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "card-higher-button;0");
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "card-higher-button;1");

                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "card-table-status;Opponent's Turn");
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "card-table-status;Your Turn");

                            Client.GetHabbo().CurrentCard = Number;
                            TargetClient.GetHabbo().CurrentCard = Number;

                            Client.GetHabbo().MyTurn = false;
                            TargetClient.GetHabbo().MyTurn = true;
                        }
                        else
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "cards", "end");
                        }
                    }
                    break;
                #endregion
                #region Lower
                case "lower":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (!Client.GetHabbo().CardsPlaying || !Client.GetHabbo().MyTurn)
                            return;

                        GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Client.GetHabbo().CardsWith);
                        if (TargetClient == null)
                            return;

                        Random rnd = new Random();
                        int Number = rnd.Next(1, 14);
                        var List = new List<string> { "clubs", "diamonds", "hearts", "spades" };
                        int index = rnd.Next(List.Count);
                        string Card = string.Empty;

                        if (Number == 1)
                        {
                            Card = "ace_of_" + List[index];
                        }
                        else if (Number == 11)
                        {
                            Card = "jack_of_" + List[index];
                        }
                        else if (Number == 12)
                        {
                            Card = "queen_of_" + List[index];
                        }
                        else if (Number == 13)
                        {
                            Card = "king_of_" + List[index];
                        }
                        else
                        {
                            Card = Number + "_of_clubs";
                        }

                        Client.SendWhisper(Client.GetHabbo().Username + " flips a " + Number);
                        TargetClient.SendWhisper(Client.GetHabbo().Username + " flips a " + Number);

                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "current-card;" + Card);
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "current-card;" + Card);

                        if (Number < Client.GetHabbo().CurrentCard)
                        {
                            Client.SendWhisper(TargetClient.GetHabbo().Username + " has 15 seconds to take their turn");
                            TargetClient.SendWhisper(TargetClient.GetHabbo().Username + " has 15 seconds to take their turn");

                            Random TokenRand = new Random();
                            int tokenNumber = TokenRand.Next(1600, 2894354);
                            TargetClient.GetHabbo().PlayToken = tokenNumber;

                            System.Timers.Timer timer1 = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(15));
                            timer1.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(15);
                            timer1.Elapsed += delegate
                            {
                                if (TargetClient.GetHabbo().CardsPlaying && TargetClient.GetHabbo().PlayToken == tokenNumber)
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(TargetClient, "cards", "pick");
                                }
                                timer1.Stop();
                            };
                            timer1.Start();

                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "card-higher-button;0");
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "card-higher-button;1");

                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "card-table-status;Opponent's Turn");
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "card-table-status;Your Turn");

                            Client.GetHabbo().CurrentCard = Number;
                            TargetClient.GetHabbo().CurrentCard = Number;

                            Client.GetHabbo().MyTurn = false;
                            TargetClient.GetHabbo().MyTurn = true;
                        }
                        else
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "cards", "end");
                        }
                    }
                    break;
                #endregion
                #region Random Pick
                case "pick":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Client.GetHabbo().CardsWith);
                        RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
                        if (TargetClient == null)
                            return;

                        Random rnd = new Random();
                        int Picked = rnd.Next(1, 3);

                        if (Picked == 1)
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "cards", "higher");
                        }
                        else
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "cards", "lower");
                        }

                    }
                    break;
                #endregion
                #region End Game
                case "end":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Client.GetHabbo().CardsWith);
                        RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
                        if (TargetClient == null)
                            return;

                        TargetUser.Say("wins the " + PlusEnvironment.ConvertToPrice(Client.GetHabbo().CardsOffer) + " dollars bet on Higher/Lower");

                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "card-higher-button;0");
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "card-higher-button;0");

                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "card-table-status;Loser... You lost $" + PlusEnvironment.ConvertToPrice(TargetClient.GetHabbo().CardsOffer) + " :(");
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "card-table-status;You won $" + PlusEnvironment.ConvertToPrice(Client.GetHabbo().CardsOffer) + "!");

                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "card-table-close;show");
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "card-table-close;show");

                        TargetClient.GetHabbo().Credits += Client.GetHabbo().CardsOffer + TargetClient.GetHabbo().CardsOffer;
                        TargetClient.SendMessage(new CreditBalanceComposer(TargetClient.GetHabbo().Credits));
                        TargetClient.GetHabbo().RPCache(3);

                        User.Freezed = false;
                        TargetUser.Freezed = false;

                        Client.GetHabbo().CardsPlaying = false;
                        Client.GetHabbo().CardsWith = null;
                        Client.GetHabbo().CardsOffer = 0;
                        Client.GetHabbo().MyTurn = false;
                        Client.GetHabbo().CurrentCard = 0;

                        TargetClient.GetHabbo().CardsPlaying = false;
                        TargetClient.GetHabbo().CardsWith = null;
                        TargetClient.GetHabbo().CardsOffer = 0;
                        TargetClient.GetHabbo().MyTurn = false;
                        TargetClient.GetHabbo().CurrentCard = 0;

                    }
                    break;
                #endregion
            }
        }
    }
}