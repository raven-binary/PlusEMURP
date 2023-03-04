using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plus;
using Fleck;
using Plus.HabboHotel.GameClients;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using System.Data;
using Plus.Communication.Packets.Incoming.Groups;
using Plus.Communication.Packets.Outgoing.Groups;

using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.GroupsRank;
using System.Drawing;
using System.Drawing.Imaging;
using WebHook;

namespace Bobba.HabboRoleplay.Web.Outgoing
{
    class TurfWebEvent : IWebEvent
    {
        public static void ClaimGang(Room CurrentRoom)
        {
            foreach (RoomUser UserInRoom in CurrentRoom.GetRoomUserManager().GetUserList().ToList())
            {
                if (UserInRoom == null || UserInRoom.IsBot || UserInRoom.GetClient() == null || UserInRoom.GetClient().GetHabbo() == null)
                    continue;

                Group Group = null;
                if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(UserInRoom.GetClient().GetHabbo().CurrentRoom.Group.Id, out Group))
                {
                    UserInRoom.GetClient().SendWhisper("An error occurred while changing turf flags! Report this to a Habbo Staff.");
                    return;
                }

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `groups` SET `colour1` = @colour1, `colour2` = @colour2 WHERE `id` =' " + Group.Id + "' LIMIT 1");
                    dbClient.AddParameter("colour1", PlusEnvironment.GetGangColors(CurrentRoom.Capture, "color_1"));
                    dbClient.AddParameter("colour2", PlusEnvironment.GetGangColors(CurrentRoom.Capture, "color_2"));
                    dbClient.RunQuery();
                }

                Group.Colour1 = PlusEnvironment.GetGangColors(CurrentRoom.Capture, "color_1");
                Group.Colour2 = PlusEnvironment.GetGangColors(CurrentRoom.Capture, "color_2");

                if (UserInRoom.GetClient().GetHabbo().CurrentRoom != null)
                {
                    foreach (Item Item in UserInRoom.GetClient().GetHabbo().CurrentRoom.GetRoomItemHandler().GetFloor.ToList())
                    {
                        if (Item == null || Item.GetBaseItem() == null)
                            continue;

                        if (Item.GetBaseItem().InteractionType != InteractionType.GUILD_ITEM)
                            continue;

                        UserInRoom.GetClient().GetHabbo().CurrentRoom.SendMessage(new ObjectUpdateComposer(Item, 0));
                        Item.UpdateState(false, true);
                    }
                }

                if (UserInRoom.GetClient().GetHabbo().Gang == UserInRoom.GetClient().GetHabbo().CurrentRoom.Capture)
                {
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(UserInRoom.GetClient(), "turf;show;" + UserInRoom.GetClient().GetHabbo().CurrentRoom.Name + ";Controlled by <b>" + UserInRoom.GetClient().GetHabbo().getNameOfThisGang(CurrentRoom.Capture) + "</b> for " + PlusEnvironment.GetTimeSince(UserInRoom.GetClient().GetHabbo().CurrentRoom.CaptureDate) + ";none");
                }
                else
                {
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(UserInRoom.GetClient(), "turf;show;" + UserInRoom.GetClient().GetHabbo().CurrentRoom.Name + ";Controlled by </b>" + UserInRoom.GetClient().GetHabbo().getNameOfThisGang(CurrentRoom.Capture) + "</b>;block");
                }

                System.Timers.Timer ClaimTimer = new System.Timers.Timer(100);
                ClaimTimer.Interval = 100;
                ClaimTimer.Elapsed += delegate
                {
                    if (UserInRoom.GetClient().GetHabbo().isInTurf)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(UserInRoom.GetClient(), "turf-controlled-time;Controlled by <b>" + UserInRoom.GetClient().GetHabbo().getNameOfThisGang(CurrentRoom.Capture) + "</b> for " + PlusEnvironment.GetTimeSince(UserInRoom.GetClient().GetHabbo().CurrentRoom.CaptureDate));
                        ClaimTimer.Start();
                    }
                    else
                    {
                        ClaimTimer.Stop();
                    }
                };
                ClaimTimer.Start();
            }
        }

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
                /*#region Claim Turf
                case "claim":
                    {
                        Room CurrentRoom = Client.GetHabbo().CurrentRoom;

                        if (CurrentRoom.Assault > 0)
                            return;

                        if (Client.GetHabbo().Gang == 0)
                        {
                            Client.SendWhisper("You must be in a gang to claim this turf");
                            return;
                        }

                        if (CurrentRoom.Capture == Client.GetHabbo().Gang)
                        {
                            Client.SendWhisper("Your gang already claimed this turf");
                            return;
                        }

                        if (CurrentRoom.Capture != 0)
                        {
                            PlusEnvironment.GetGame().GetClientManager().sendGangMsg(CurrentRoom.Capture, "Your turf is under attack!");
                            PlusEnvironment.GetGame().GetClientManager().sendGangMsg(CurrentRoom.Capture, "Your gang's turf (" + Client.GetHabbo().CurrentRoom.Name + ") is being assaulted by " + Client.GetHabbo().getNameOfGang());
                            PlusEnvironment.GetGame().GetClientManager().TurfAlert(CurrentRoom.Capture, "show");

                            PlusEnvironment.GetGame().GetClientManager().sendGangMsg(Client.GetHabbo().Gang, "Your gang is attempting to capture " + Client.GetHabbo().CurrentRoom.Name + " from " + Client.GetHabbo().getNameOfThisGang(CurrentRoom.Capture));
                        }
                        else
                        {
                            PlusEnvironment.GetGame().GetClientManager().sendGangMsg(Client.GetHabbo().Gang, "Your gang is attempting to capture " + Client.GetHabbo().CurrentRoom.Name);
                        }

                        CurrentRoom.Assault = Client.GetHabbo().Gang;
                        CurrentRoom.ClaimUserId = Client.GetHabbo().Id;

                        foreach (RoomUser UserInRoom in CurrentRoom.GetRoomUserManager().GetUserList().ToList())
                        {
                            if (UserInRoom == null || UserInRoom.IsBot || UserInRoom.GetClient() == null || UserInRoom.GetClient().GetHabbo() == null)
                                continue;

                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(UserInRoom.GetClient(), "turf-claim;" + Client.GetHabbo().getNameOfGang());
                        }

                        #region Claim Timer
                        System.Timers.Timer ClaimTimer = new System.Timers.Timer(500);
                        ClaimTimer.Interval = 500;
                        ClaimTimer.Elapsed += delegate
                        {
                            if (Client.GetHabbo().CurrentRoomId == CurrentRoom.Id)
                            {
                                foreach (RoomUser UserInRoom in CurrentRoom.GetRoomUserManager().GetUserList().ToList())
                                {
                                    if (UserInRoom == null || UserInRoom.IsBot || UserInRoom.GetClient() == null || UserInRoom.GetClient().GetHabbo() == null)
                                        continue;

                                    if (!PlusEnvironment.StopGangClaim.ContainsKey(Client.GetHabbo().CurrentRoomId))
                                    {
                                        if (CurrentRoom.Capture == 0)
                                        {
                                            CurrentRoom.ClaimPrecent += 1.7; //30 secs cuz turf don't had a gang
                                        }
                                        else
                                        {
                                            CurrentRoom.ClaimPrecent += 0.3; //3mins cuz turf had a gang
                                        }

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(UserInRoom.GetClient(), "turf-claim-progress;" + CurrentRoom.ClaimPrecent);
                                    }
                                    else
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(UserInRoom.GetClient(), "turf-claim-stop;" + CurrentRoom.ClaimPrecent);
                                    }

                                    if (CurrentRoom.ClaimPrecent >= 100)
                                    {
                                        CurrentRoom.ClaimPrecent = 0;

                                        int Y = 24;
                                         
                                        char[] alpha = Client.GetHabbo().getNameOfGang().ToCharArray();
                                        foreach (var c in alpha)
                                        {
                                            Y -= 1;
                                            Item Bomb = new Item(PlusEnvironment.GetRandomNumber(590000, 590090095), Client.GetHabbo().CurrentRoomId, 399996, "state	0	imageUrl	/client/swfz/c_images/abc/" + c + ".png	offsetX	-30	offsetY	-45	offsetZ	99999", 3, Y, 0, 0, 0, 0, 0, 0, string.Empty, CurrentRoom);
                                            if (CurrentRoom.GetRoomItemHandler().SetFloorItemByForce(null, Bomb, 3, Y, 0, true, false, true))
                                            {
                                                CurrentRoom.SendMessage(new ObjectUpdateComposer(Bomb, 0));
                                            }
                                        }

                                        if (CurrentRoom.Capture > 0)
                                        {
                                            PlusEnvironment.GetGame().GetClientManager().LiveFeed("<span class=\"green\">" + Client.GetHabbo().getNameOfGang() + "</span> captured <span class=\"blue\">" + CurrentRoom.Name + "</span> from <span class=\"red\">" + Client.GetHabbo().getNameOfThisGang(CurrentRoom.Capture) + "</span>");
                                            Webhook.SendWebhook("**" + Client.GetHabbo().getNameOfGang() + "** captured **" + CurrentRoom.Name + "** from **" + Client.GetHabbo().getNameOfThisGang(CurrentRoom.Capture) + "** :muscle:");
                                            PlusEnvironment.GetGame().GetClientManager().sendGangMsg(CurrentRoom.Capture, "Your gang's turf (" + Client.GetHabbo().CurrentRoom.Name + ") is being captured by " + Client.GetHabbo().getNameOfGang());
                                            PlusEnvironment.GetGame().GetClientManager().TurfAlert(CurrentRoom.Capture, "hide");
                                            PlusEnvironment.GetGame().GetClientManager().sendGangMsg(Client.GetHabbo().Gang, "Your gang has successfully captured " + Client.GetHabbo().CurrentRoom.Name + " from " + Client.GetHabbo().getNameOfThisGang(CurrentRoom.Capture));
                                        }
                                        else
                                        {
                                            PlusEnvironment.GetGame().GetClientManager().LiveFeed("<span class=\"green\">" + Client.GetHabbo().getNameOfGang() + "</span> captured <span class=\"blue\">" + CurrentRoom.Name + "</span>");
                                            Webhook.SendWebhook("**" + Client.GetHabbo().getNameOfGang() + "** captured **" + CurrentRoom.Name + "** :muscle:");
                                        }

                                        CurrentRoom.Assault = 0;
                                        CurrentRoom.Capture = Client.GetHabbo().Gang;

                                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                        {
                                            dbClient.SetQuery("UPDATE rooms SET capture = @capture WHERE `id` = @id LIMIT 1");
                                            dbClient.AddParameter("capture", Client.GetHabbo().Gang);
                                            dbClient.AddParameter("id", CurrentRoom.Id);
                                            dbClient.RunQuery();
                                        }

                                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                        {
                                            dbClient.SetQuery("UPDATE rooms SET capture_date = @capture_date WHERE id = @id LIMIT 1");
                                            dbClient.AddParameter("capture_date", DateTime.Now.ToUniversalTime());
                                            dbClient.AddParameter("id", Client.GetHabbo().CurrentRoomId);
                                            dbClient.RunQuery();
                                        }

                                        CurrentRoom.CaptureDate = DateTime.Now.ToUniversalTime();
                                        ClaimGang(CurrentRoom);

                                        ClaimTimer.Stop();
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                ClaimTimer.Stop();
                                if (CurrentRoom.Capture > 0)
                                {
                                    PlusEnvironment.GetGame().GetClientManager().sendGangMsg(CurrentRoom.Assault, "Your gang failed to capture " + CurrentRoom.Name + " from " + Client.GetHabbo().getNameOfThisGang(CurrentRoom.Capture));
                                    PlusEnvironment.GetGame().GetClientManager().sendGangMsg(CurrentRoom.Capture, "You have successfully defended your turf (" + CurrentRoom.Name + ") from " + Client.GetHabbo().getNameOfThisGang(CurrentRoom.Assault));
                                }
                                else
                                {
                                    PlusEnvironment.GetGame().GetClientManager().sendGangMsg(CurrentRoom.Assault, "Your gang failed to capture " + CurrentRoom.Name);
                                }
                                CurrentRoom.Assault = 0;
                                CurrentRoom.ClaimUserId = 0;
                                CurrentRoom.ClaimPrecent = 0;

                                if (CurrentRoom.Capture != 0)
                                {
                                    PlusEnvironment.GetGame().GetClientManager().TurfAlert(CurrentRoom.Capture, "hide");
                                }

                                foreach (RoomUser UserInRoom in CurrentRoom.GetRoomUserManager().GetUserList().ToList())
                                {
                                    if (UserInRoom == null || UserInRoom.IsBot || UserInRoom.GetClient() == null || UserInRoom.GetClient().GetHabbo() == null)
                                        continue;

                                    if (CurrentRoom.Capture == 0)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(UserInRoom.GetClient(), "turf;show;" + CurrentRoom.Group.Name + ";Uncontrolled");
                                    }
                                    else
                                    {
                                        if (UserInRoom.GetClient().GetHabbo().Gang == CurrentRoom.Capture)
                                        {
                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(UserInRoom.GetClient(), "turf;show;" + CurrentRoom.Group.Name + ";Controlled by <b>" + UserInRoom.GetClient().GetHabbo().getNameOfThisGang(CurrentRoom.Capture) + "</b> for " + PlusEnvironment.GetTimeSince(CurrentRoom.CaptureDate) + ";none");
                                        }
                                        else
                                        {
                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(UserInRoom.GetClient(), "turf;show;" + CurrentRoom.Group.Name + ";Controlled by <b>" + UserInRoom.GetClient().GetHabbo().getNameOfThisGang(CurrentRoom.Capture) + "</b>;block");
                                        }
                                    }
                                }
                                return;
                            }
                            ClaimTimer.Start();
                        };
                        ClaimTimer.Start();
                        #endregion
                    }
                    break;
                #endregion*/
            }
        }
    }
}