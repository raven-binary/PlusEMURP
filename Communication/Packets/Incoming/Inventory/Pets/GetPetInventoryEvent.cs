using System.Collections.Generic;
using Plus.Communication.Packets.Outgoing.Inventory.Pets;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms.AI;

namespace Plus.Communication.Packets.Incoming.Inventory.Pets
{
    internal class GetPetInventoryEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session.GetHabbo().GetInventoryComponent() == null)
                return;

            ICollection<Pet> pets = session.GetHabbo().GetInventoryComponent().GetPets();
            session.SendPacket(new PetInventoryComposer(pets));
        }
    }
}