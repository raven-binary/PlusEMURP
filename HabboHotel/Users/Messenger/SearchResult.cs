namespace Plus.HabboHotel.Users.Messenger
{
    public struct SearchResult
    {
        public int UserId;
        public string Username;
        public string Motto;
        public string Figure;
        public string LastOnline;

        public SearchResult(int userId, string username, string motto, string figure, string lastOnline)
        {
            UserId = userId;
            Username = username;
            Motto = motto;
            Figure = figure;
            LastOnline = lastOnline;
        }
    }
}