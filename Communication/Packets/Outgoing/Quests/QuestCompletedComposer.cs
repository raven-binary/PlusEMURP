using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Quests;
using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Outgoing.Quests
{
    internal class QuestCompletedComposer : MessageComposer
    {
        public Habbo Habbo { get; }
        public Quest Quest { get; }

        public QuestCompletedComposer(GameClient session, Quest quest)
            : base(ServerPacketHeader.QuestCompletedMessageComposer)
        {
            Habbo = session.GetHabbo();
            Quest = quest;
        }

        public override void Compose(ServerPacket packet)
        {
            int amountInCat = PlusEnvironment.GetGame().GetQuestManager().GetAmountOfQuestsInCategory(Quest.Category);
            int number = Quest == null ? amountInCat : Quest.Number;
            int userProgress = Quest == null ? 0 : Habbo.GetQuestProgress(Quest.Id);

            packet.WriteString(Quest.Category);
            packet.WriteInteger(number); // Quest progress in this cat
            packet.WriteInteger((Quest.Name.Contains("xmas2012")) ? 1 : amountInCat); // Total quests in this cat
            packet.WriteInteger(Quest == null ? 3 : Quest.RewardType); // Reward type (1 = Snowflakes, 2 = Love hearts, 3 = Pixels, 4 = Seashells, everything else is pixels
            packet.WriteInteger(Quest == null ? 0 : Quest.Id); // Quest id
            packet.WriteBoolean(Quest == null ? false : Habbo.GetStats().QuestId == Quest.Id); // Quest started
            packet.WriteString(Quest == null ? string.Empty : Quest.ActionName);
            packet.WriteString(Quest == null ? string.Empty : Quest.DataBit);
            packet.WriteInteger(Quest == null ? 0 : Quest.Reward);
            packet.WriteString(Quest == null ? string.Empty : Quest.Name);
            packet.WriteInteger(userProgress); // Current progress
            packet.WriteInteger(Quest == null ? 0 : Quest.GoalData); // Target progress
            packet.WriteInteger(Quest == null ? 0 : Quest.TimeUnlock); // "Next quest available countdown" in seconds
            packet.WriteString("");
            packet.WriteString("");
            packet.WriteBoolean(true); // ?
            packet.WriteBoolean(true); // Activate next quest..
        }
    }
}