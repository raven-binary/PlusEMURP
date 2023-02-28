namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.YouTubeTelevisions
{
    internal class GetYouTubeVideoComposer : MessageComposer
    {
        public int ItemId { get; }
        public string YouTubeVideo { get; }

        public GetYouTubeVideoComposer(int itemId, string youTubeVideo)
            : base(ServerPacketHeader.GetYouTubeVideoMessageComposer)
        {
            ItemId = itemId;
            YouTubeVideo = youTubeVideo;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(ItemId);
            packet.WriteString(YouTubeVideo); //"9Ht5RZpzPqw");
            packet.WriteInteger(0); //Start seconds
            packet.WriteInteger(0); //End seconds
            packet.WriteInteger(0); //State
        }
    }
}