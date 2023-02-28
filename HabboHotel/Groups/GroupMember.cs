namespace Plus.HabboHotel.Groups
{
    public class GroupMember
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Look { get; set; }

        public GroupMember(int id, string username, string look)
        {
            Id = id;
            Username = username;
            Look = look;
        }
    }
}