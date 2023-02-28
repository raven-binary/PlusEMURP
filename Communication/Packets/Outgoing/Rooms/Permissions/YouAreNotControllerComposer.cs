﻿namespace Plus.Communication.Packets.Outgoing.Rooms.Permissions
{
    internal class YouAreNotControllerComposer : MessageComposer
    {
        public YouAreNotControllerComposer()
            : base(ServerPacketHeader.YouAreNotControllerMessageComposer)
        {
        }

        public override void Compose(ServerPacket packet)
        {
        }
    }
}