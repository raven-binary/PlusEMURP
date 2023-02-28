namespace Plus.HabboHotel.Rooms.Chat.Styles
{
    public sealed class ChatStyle
    {
        public ChatStyle(int id, string name, string requiredRight)
        {
            Id = id;
            Name = name;
            RequiredRight = requiredRight;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string RequiredRight { get; set; }
    }
}