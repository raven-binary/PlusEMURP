using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Quests;
using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Outgoing.Quests
{
    internal class QuestStartedComposer : MessageComposer
    {
        public Habbo Habbo { get; }
        public Quest Quest { get; }

        public QuestStartedComposer(GameClient session, Quest quest)
            : base(ServerPacketHeader.QuestStartedMessageComposer)
        {
            Habbo = session.GetHabbo();
            Quest = quest;
        }

        public override void Compose(ServerPacket packet)
        {
            QuestListComposer.SerializeQuest(packet, Habbo, Quest, Quest.Category);
        }
    }
}