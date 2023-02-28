namespace Plus.HabboHotel.Items.Data.Moodlight
{
    public class MoodlightPreset
    {
        public bool BackgroundOnly;
        public string ColorCode;
        public int ColorIntensity;

        public MoodlightPreset(string colorCode, int colorIntensity, bool backgroundOnly)
        {
            ColorCode = colorCode;
            ColorIntensity = colorIntensity;
            BackgroundOnly = backgroundOnly;
        }
    }
}