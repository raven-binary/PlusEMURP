namespace Plus.HabboHotel.Misc
{

    internal struct Figure
    {

        internal string Part;


        internal string PartId;


        internal string Gender;


        internal string Colorable;


        public Figure(string part, string partId, string gender, string colorable)
        {
            Part = part;
            PartId = partId;
            Gender = gender;
            Colorable = colorable;
        }
    }
}