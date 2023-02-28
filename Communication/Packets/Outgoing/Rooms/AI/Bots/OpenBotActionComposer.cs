using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Rooms.AI.Bots
{
    internal class OpenBotActionComposer : MessageComposer
    {
        public int BotId { get; }
        public string BotName { get; }
        public int ActionId { get; }
        public string BotSpeech { get; }

        public OpenBotActionComposer(RoomUser botUser, int actionId, string botSpeech)
            : base(ServerPacketHeader.OpenBotActionMessageComposer)
        {
            BotId = botUser.BotData.Id;
            BotName = botUser.BotData.Name;
            ActionId = actionId;
            BotSpeech = botSpeech;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(BotId);
            packet.WriteInteger(ActionId);
            if (ActionId == 2)
                packet.WriteString(BotSpeech);
            else if (ActionId == 5)
                packet.WriteString(BotName);
        }
    }
}