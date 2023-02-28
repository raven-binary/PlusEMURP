namespace Plus.HabboHotel.Navigator
{
    public class TopLevelItem
    {
        public TopLevelItem(int id, string searchCode, string filter, string localization)
        {
            Id = id;
            SearchCode = searchCode;
            Filter = filter;
            Localization = localization;
        }

        public int Id { get; set; }

        public string SearchCode { get; set; }

        public string Filter { get; set; }

        public string Localization { get; set; }
    }
}