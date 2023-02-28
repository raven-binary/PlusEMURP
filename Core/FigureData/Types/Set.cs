using System.Collections.Generic;

namespace Plus.Core.FigureData.Types
{
    internal class Set
    {
        public int Id { get; set; }
        public string Gender { get; set; }
        public int ClubLevel { get; set; }
        public bool Colorable { get; set; }
        public bool Selectable { get; set; }
        public bool PreSelectable { get; set; }
        public Dictionary<string, Part> Parts { get; set; }

        public Set(int id, string gender, int clubLevel, bool colorable, bool selectable, bool preSelectable)
        {
            Id = id;
            Gender = gender;
            ClubLevel = clubLevel;
            Colorable = colorable;
            Selectable = selectable;
            PreSelectable = preSelectable;

            Parts = new Dictionary<string, Part>();
        }
    }
}