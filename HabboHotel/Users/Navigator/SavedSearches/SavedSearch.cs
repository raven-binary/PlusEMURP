namespace Plus.HabboHotel.Users.Navigator.SavedSearches
{
    public class SavedSearch
    {
        public SavedSearch(int id, string filter, string search)
        {
            Id = id;
            Filter = filter;
            Search = search;
        }

        public int Id { get; set; }

        public string Filter { get; set; }

        public string Search { get; set; }
    }
}