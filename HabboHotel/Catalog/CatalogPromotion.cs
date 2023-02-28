namespace Plus.HabboHotel.Catalog
{
    public class CatalogPromotion
    {
        public int Id { get; }
        public string Title { get; }
        public string Image { get; }
        public int Unknown { get; }
        public string PageLink { get; }
        public int ParentId { get; }

        public CatalogPromotion(int id, string title, string image, int unknown, string pageLink, int parentId)
        {
            Id = id;
            Title = title;
            Image = image;
            Unknown = unknown;
            PageLink = pageLink;
            ParentId = parentId;
        }
    }
}