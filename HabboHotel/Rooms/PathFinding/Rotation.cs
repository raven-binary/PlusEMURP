namespace Plus.HabboHotel.Rooms.PathFinding
{
    public static class Rotation
    {
        public static int Calculate(int x1, int y1, int x2, int y2)
        {
            int rotation = 0;

            if (x1 > x2 && y1 > y2)
                rotation = 7;

            else if (x1 < x2 && y1 < y2)
                rotation = 3;

            else if (x1 > x2 && y1 < y2)
                rotation = 5;

            else if (x1 < x2 && y1 > y2)
                rotation = 1;

            else if (x1 > x2)
                rotation = 6;

            else if (x1 < x2)
                rotation = 2;

            else if (y1 < y2)
                rotation = 4;

            else if (y1 > y2)
                rotation = 0;

            return rotation;
        }

        public static int Calculate(int x1, int y1, int x2, int y2, bool moonwalk)
        {
            int rot = Calculate(x1, y1, x2, y2);

            if (!moonwalk)
                return rot;

            return RotationInverse(rot);
        }

        public static int RotationInverse(int rot)
        {
            if (rot > 3)
                rot = rot - 4;
            else
                rot = rot + 4;

            return rot;
        }
    }
}