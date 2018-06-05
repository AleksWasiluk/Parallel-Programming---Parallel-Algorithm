using System;

namespace lab03
{
    internal class Location
    {
        int x, y;
        public Location(int _x,int _y)
        {
            x = _x;
            y = _y;
        }
        public Location(Location l)
        {
            x = l.x;
            y = l.y;
        }
        public int X { get { return x; } set { x = value; } }
        public int Y { get { return y; } set { y = value; } }
        public static double Distance(Location l1, Location l2)
        {
            int x2 = l1.x - l2.x;
            x2 *= x2;

            int y2 = l1.y - l2.y;
            y2 *= y2;

            return Math.Sqrt(x2 + y2);
        }
    }
}