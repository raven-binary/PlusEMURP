using System.Collections.Generic;
using Plus.HabboHotel.Catalog;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class CatalogIndexComposer : MessageComposer
    {
        public Habbo Habbo { get; }
        public ICollection<CatalogPage> Pages { get; }

        public CatalogIndexComposer(GameClient sesion, ICollection<CatalogPage> pages)
            : base(ServerPacketHeader.CatalogIndexMessageComposer)
        {
            Habbo = sesion.GetHabbo();
            Pages = pages;
        }

        public void WriteRootIndex(Habbo habbo, ICollection<CatalogPage> pages, ServerPacket packet)
        {
            packet.WriteBoolean(true);
            packet.WriteInteger(0);
            packet.WriteInteger(-1);
            packet.WriteString("root");
            packet.WriteString(string.Empty);
            packet.WriteInteger(0);
            packet.WriteInteger(CalcTreeSize(habbo, pages, -1));
        }

        public void WriteNodeIndex(CatalogPage page, int treeSize, ServerPacket packet)
        {
            packet.WriteBoolean(page.Visible);
            packet.WriteInteger(page.Icon);
            packet.WriteInteger(-1);
            packet.WriteString(page.PageLink);
            packet.WriteString(page.Caption);
            packet.WriteInteger(0);
            packet.WriteInteger(treeSize);
        }

        public void WritePage(CatalogPage page, int treeSize, ServerPacket packet)
        {
            packet.WriteBoolean(page.Visible);
            packet.WriteInteger(page.Icon);
            packet.WriteInteger(page.Id);
            packet.WriteString(page.PageLink);
            packet.WriteString(page.Caption);

            packet.WriteInteger(page.ItemOffers.Count);
            foreach (int i in page.ItemOffers.Keys)
            {
                packet.WriteInteger(i);
            }

            packet.WriteInteger(treeSize);
        }

        public int CalcTreeSize(Habbo habbo, ICollection<CatalogPage> pages, int parentId)
        {
            int i = 0;
            foreach (CatalogPage page in pages)
            {
                if (page.MinimumRank > habbo.Rank || (page.MinimumVip > habbo.VipRank && habbo.Rank == 1) || page.ParentId != parentId)
                    continue;

                if (page.ParentId == parentId)
                    i++;
            }

            return i;
        }

        public override void Compose(ServerPacket packet)
        {
            WriteRootIndex(Habbo, Pages, packet);

            foreach (CatalogPage parent in Pages)
            {
                if (parent.ParentId != -1 || parent.MinimumRank > Habbo.Rank || (parent.MinimumVip > Habbo.VipRank && Habbo.Rank == 1))
                    continue;

                WritePage(parent, CalcTreeSize(Habbo, Pages, parent.Id), packet);

                foreach (CatalogPage child in Pages)
                {
                    if (child.ParentId != parent.Id || child.MinimumRank > Habbo.Rank || (child.MinimumVip > Habbo.VipRank && Habbo.Rank == 1))
                        continue;

                    if (child.Enabled)
                        WritePage(child, CalcTreeSize(Habbo, Pages, child.Id), packet);
                    else
                        WriteNodeIndex(child, CalcTreeSize(Habbo, Pages, child.Id), packet);

                    foreach (CatalogPage subChild in Pages)
                    {
                        if (subChild.ParentId != child.Id || subChild.MinimumRank > Habbo.Rank)
                            continue;

                        WritePage(subChild, 0, packet);
                    }
                }
            }

            packet.WriteBoolean(false);
            packet.WriteString("NORMAL");
        }
    }
}