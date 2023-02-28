namespace Plus.HabboHotel.Rooms.Chat.Filter
{
    internal sealed class WordFilter
    {
        public WordFilter(string word, string replacement, bool strict, bool bannable)
        {
            Word = word;
            Replacement = replacement;
            IsStrict = strict;
            IsBannable = bannable;
        }

        public string Word { get; }

        public string Replacement { get; }

        public bool IsStrict { get; }

        public bool IsBannable { get; }
    }
}