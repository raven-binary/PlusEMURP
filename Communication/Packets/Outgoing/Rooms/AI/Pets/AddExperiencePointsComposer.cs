namespace Plus.Communication.Packets.Outgoing.Rooms.AI.Pets
{
    internal class AddExperiencePointsComposer : MessageComposer
    {
        public int PetId { get; }
        public int VirtualId { get; }
        public int Amount { get; }

        public AddExperiencePointsComposer(int petId, int virtualId, int amount)
            : base(ServerPacketHeader.AddExperiencePointsMessageComposer)
        {
            PetId = petId;
            VirtualId = virtualId;
            Amount = amount;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(PetId);
            packet.WriteInteger(VirtualId);
            packet.WriteInteger(Amount);
        }
    }
}