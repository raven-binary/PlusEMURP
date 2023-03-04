using System;

using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.HabboHotel.Pathfinding;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Database.Interfaces;
using System.Data;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorCannabis : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
        }

        public void OnRemove(GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (!Session.GetHabbo().CurrentRoom.CheckRights(Session, false, true))
                return;

            if (Item.InteractingUser != 0)
                return;

            RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (!Gamemap.TilesTouching(Item.GetX, Item.GetY, User.Coordinate.X, User.Coordinate.Y) && Item.GetBaseItem().SpriteId != 4614)
            {
                User.MoveToIfCanWalk(Item.SquareInFront);
                return;
            }

            User.SetRot(Pathfinding.Rotation.Calculate(User.Coordinate.X, User.Coordinate.Y, Item.GetX, Item.GetY), false);

            if (Session.GetHabbo().getCooldown("arroser_plante"))
            {
                Session.SendWhisper("Wait a minute...");
                return;
            }

            DataRow CannabisPlant = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM cannabis WHERE item_id = @item_id LIMIT 1");
                dbClient.AddParameter("item_id", Item.Id);
                CannabisPlant = dbClient.getRow();
            }

            if (CannabisPlant == null)
            {
                removeCannabisPlant(Session, Item);
                return;
            }

            if (Convert.ToDateTime(CannabisPlant["last_arrosage"]).Day == DateTime.Now.Day && Convert.ToDateTime(CannabisPlant["last_arrosage"]).Month == DateTime.Now.Month && Convert.ToDateTime(CannabisPlant["last_arrosage"]).Year == DateTime.Now.Year)
            {
                Session.SendWhisper("This cannabis plant has already been watered, come back tomorrow.");
                return;
            }

            Session.GetHabbo().addCooldown("arroser_plante", 6000);
            Item.InteractingUser = Session.GetHabbo().Id;
            User.Frozen = true;
            Session.GetHabbo().Effects().ApplyEffect(595);

            User.OnChat(User.LastBubble, "* Sees the condition of the cannabis plant * ", true);
            
            System.Timers.Timer timer1 = new System.Timers.Timer(2000);
            System.Timers.Timer timer2 = new System.Timers.Timer(3000);
            System.Timers.Timer timer3 = new System.Timers.Timer(5000);

            // ETAT
            timer1.Interval = 2000;
            timer1.Elapsed += delegate
            {
                if (Convert.ToDateTime(CannabisPlant["last_arrosage"]).AddDays(2) < DateTime.Now)
                {
                    Session.SendWhisper("The cannabis plant is dead because you didn't water it.");
                    removeCannabisPlant(Session, Item);
                    Session.GetHabbo().resetEffectEvent();
                    Item.InteractingUser = 0;
                    User.Frozen = false;
                    timer1.Stop();
                    timer2.Stop();
                    timer3.Stop();
                    return;
                }

                if(Convert.ToInt32(CannabisPlant["etat"]) == 4)
                {
                    Random maladieChance = new Random();
                    int maladieChanceNumber = maladieChance.Next(1, 7);

                    if (maladieChanceNumber == 3)
                    {
                        Session.SendWhisper("The cannabis plant died because it developed a disease.");
                        removeCannabisPlant(Session, Item);
                        Session.GetHabbo().resetEffectEvent();
                        Item.InteractingUser = 0;
                        User.Frozen = false;
                        timer1.Stop();
                        timer2.Stop();
                        timer3.Stop();
                        return;
                    }
                }

                if (Convert.ToInt32(CannabisPlant["etat"]) >= 2 && !Session.GetHabbo().CurrentRoom.GetRoomItemHandler().CheckIfItemOnCase(5273, Item.GetX, Item.GetY))
                {
                    Session.SendWhisper("The cannabis plant needs light to grow more.");
                    Session.GetHabbo().resetEffectEvent();
                    Item.InteractingUser = 0;
                    User.Frozen = false;
                    timer1.Stop();
                    timer2.Stop();
                    timer3.Stop();
                    return;
                }

                if (Convert.ToInt32(CannabisPlant["etat"]) < 3)
                {
                    Session.SendWhisper("The seed is doing fine at the moment.");
                }
                else if (Convert.ToInt32(CannabisPlant["etat"]) == 3)
                {
                    Item.ExtraData = "1";
                    Item.UpdateState();
                    Session.SendWhisper("The cannabis plant begins to grow.");
                }
                else if (Convert.ToInt32(CannabisPlant["etat"]) == 5)
                {
                    Item.ExtraData = "0";
                    Item.UpdateState();
                    Session.SendWhisper("The cannabis plant has done well.");
                }
                else if (Convert.ToInt32(CannabisPlant["etat"]) == 6)
                {
                    Item.ExtraData = "2";
                    Item.UpdateState();
                    Session.SendWhisper("The cannabis plant has started to flower.");
                }
                else if (Convert.ToInt32(CannabisPlant["etat"]) == 7)
                {
                    Session.SendWhisper("The plant is ready to harvest.");
                }
                else
                {
                    Session.SendWhisper("The plant is fine.");
                }
                timer1.Stop();
            };
            timer1.Start();

            // WATERING OR HARVESTING
            timer2.Interval = 3000;
            timer2.Elapsed += delegate
            {
                if (Convert.ToInt32(CannabisPlant["etat"]) == 7)
                {
                    User.OnChat(User.LastBubble, "* Harvest the heads of the cannabis plant * ", true);
                }
                else
                {
                    decimal needEau = 0;
                    if (Convert.ToInt32(CannabisPlant["etat"]) > 5)
                    {
                        needEau = Convert.ToDecimal(0.50);
                    }
                    else if (Convert.ToInt32(CannabisPlant["etat"]) > 3)
                    {
                        needEau = Convert.ToDecimal(0.40);
                    }
                    else if (Convert.ToInt32(CannabisPlant["etat"]) > 2)
                    {
                        needEau = Convert.ToDecimal(0.30);
                    }
                    else
                    {
                        needEau = Convert.ToDecimal(0.20);
                    }

                    if (Session.GetHabbo().Eau < needEau)
                    {
                        Session.SendWhisper("The cannabis plant needs " + needEau + " water to develop.");
                        Session.GetHabbo().resetEffectEvent();
                        User.Frozen = false;
                        timer2.Stop();
                        timer3.Stop();
                        Item.InteractingUser = 0;
                        return;
                    }

                    User.OnChat(User.LastBubble, "* Water the cannabis plant [-" + needEau + "] *", true);
                    Session.GetHabbo().Eau -= needEau;
                    Session.GetHabbo().updateEau();
                }
                timer2.Stop();
            };
            timer2.Start();

            // END OF WATERING OR HARVEST
            timer3.Interval = 5000;
            timer3.Elapsed += delegate
            {
                if (Convert.ToInt32(CannabisPlant["etat"]) == 7)
                {
                    User.GetClient().Shout("*Stop harvesting*");
                    Random weedRecolt = new Random();
                    int weedRecoltNumber = weedRecolt.Next(15, 25);
                    Session.GetHabbo().Weed += weedRecoltNumber;
                    Session.GetHabbo().updateWeed();
                    Session.SendWhisper("You harvested " + weedRecoltNumber + "g of cannabis from this stem.");
                    removeCannabisPlant(Session, Item);
                    Session.GetHabbo().resetEffectEvent();
                    Item.InteractingUser = 0;
                    User.Frozen = false;
                }
                else
                {
                    User.OnChat(User.LastBubble, "* Stops watering cannabis plants * ", true);
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE cannabis SET last_arrosage = NOW(), etat = etat + 1 WHERE item_id = @item_id");
                        dbClient.AddParameter("item_id", Item.Id);
                        dbClient.RunQuery();
                    }
                    Session.GetHabbo().resetEffectEvent();
                    Item.InteractingUser = 0;
                    User.Frozen = false;
                }
                timer3.Stop();
            };
            timer3.Start();
        }

        public void OnWiredTrigger(Item Item)
        {
        }

        public void removeCannabisPlant(GameClient Session, Item Item)
        {
            Session.GetHabbo().CurrentRoom.GetRoomItemHandler().RemoveFurniture(null, Item.Id);
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM items WHERE id = @item_id");
                dbClient.AddParameter("item_id", Item.Id);
                dbClient.RunQuery();
            }
        }
    }
}