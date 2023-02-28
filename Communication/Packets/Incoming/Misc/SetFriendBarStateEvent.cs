using Plus.Communication.Packets.Outgoing.Sound;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users.Messenger.FriendBar;

namespace Plus.Communication.Packets.Incoming.Misc
{
    internal class SetFriendBarStateEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null)
                return;

            session.GetHabbo().FriendBarState = FriendBarStateUtility.GetEnum(packet.PopInt());
            session.SendPacket(new SoundSettingsComposer(session.GetHabbo().ClientVolume, session.GetHabbo().ChatPreference, session.GetHabbo().AllowMessengerInvites, session.GetHabbo().FocusPreference, FriendBarStateUtility.GetInt(session.GetHabbo().FriendBarState)));
        }
    }
}