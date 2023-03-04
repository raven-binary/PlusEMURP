using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.HabboHotel.Items.Interactor
{
	public class InteractorOneWayGate : IFurniInteractor
	{
		public void OnPlace(GameClient Session, Item Item)
		{
			Item.ExtraData = "0";
			if (Item.InteractingUser != 0)
			{
				RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Item.InteractingUser);
				if (User != null)
				{
					User.ClearMovement(true);
					User.UnlockWalking();
				}
				Item.InteractingUser = 0;
			}
		}

		public void OnRemove(GameClient Session, Item Item)
		{
			Item.ExtraData = "0";
			if (Item.InteractingUser != 0)
			{
				RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Item.InteractingUser);
				if (User != null)
				{
					User.ClearMovement(true);
					User.UnlockWalking();
				}
				Item.InteractingUser = 0;
			}
		}

		public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
		{
			if (Session == null || Session.GetRoleplay().Prison)
				return;

			RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
			if (Item.InteractingUser2 != User.UserId)
			{
				Item.InteractingUser2 = User.UserId;
			}
			if (User == null || Item.GetBaseItem().InteractionType != InteractionType.ONE_WAY_GATE)
			{
				return;
			}
			if (User.Coordinate != Item.SquareInFront && User.CanWalk)
			{
				User.MoveTo(Item.SquareInFront);
			}
			else
			{
				if (!Item.GetRoom().GetGameMap().ValidTile(Item.SquareBehind.X, Item.SquareBehind.Y) || !Item.GetRoom().GetGameMap().CanWalk(Item.SquareBehind.X, Item.SquareBehind.Y, false) || !Item.GetRoom().GetGameMap().SquareIsOpen(Item.SquareBehind.X, Item.SquareBehind.Y, false))
				{
					return;
				}
				if (User.LastInteraction - PlusEnvironment.GetUnixTimestamp() < 0.0 && User.InteractingGate && User.GateId == Item.Id)
				{
					User.InteractingGate = false;
					User.GateId = 0;
				}
				if (Item.GetRoom().GetGameMap().CanWalk(Item.SquareBehind.X, Item.SquareBehind.Y, User.AllowOverride) && Item.InteractingUser == 0)
				{
					User.InteractingGate = true;
					User.GateId = Item.Id;
					Item.InteractingUser = User.HabboId;
					User.CanWalk = false;
					if (User.IsWalking && (User.GoalX != Item.SquareInFront.X || User.GoalY != Item.SquareInFront.Y))
					{
						User.ClearMovement(true);
					}
					User.AllowOverride = true;
					User.MoveTo(Item.Coordinate);
					Item.RequestUpdate(4, true);
				}
			}
		}

		public void OnWiredTrigger(Item Item)
		{
		}
	}
}
