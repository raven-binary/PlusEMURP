using System;

using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Groups;
using Plus.Database.Interfaces;
using System.Data;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorATM : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
        }

        public void OnRemove(GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (Session == null)
                return;

            RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (!Gamemap.TilesTouching(Item.GetX, Item.GetY, User.Coordinate.X, User.Coordinate.Y))
            {
                User.MoveToIfCanWalk(Item.SquareInFront);
                return;
            }

            DataRow ATM = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `atms` WHERE `atm_id` = @id LIMIT 1;");
                dbClient.AddParameter("id", Item.Id);
                ATM = dbClient.getRow();
            }

            if (ATM == null)
            {
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("INSERT INTO `atms` (`atm_id`) VALUES ('" + Item.Id + "')");// the main amount is on the "atms" table (amount |default|=1500)
                }
            }

            if (User.usingATM == true)
            {
                User.usingATM = false;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "ATM;disconnect;");
                return;
            }
            else
            {
                User.usingATM = true;
                Session.GetHabbo().UsingItem = Item.Id;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "ATM;connect;" + PlusEnvironment.ConvertToPrice(Session.GetHabbo().Banque) + ";" + PlusEnvironment.ConvertToPrice(Convert.ToInt32(ATM["amount"])) + ";" + Session.GetHabbo().RankId + ";" + Convert.ToString(Session.GetHabbo().Working));
                return;
            }
        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}