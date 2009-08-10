using System;
using System.Collections.Generic;
using System.Text;

namespace Chain3D
{
    public class Location
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }

        private Location(int x, int y, int z, bool noLookup)
        {
            X = x;
            Y = y;
            Z = z;
            if (!noLookup && (X > MainFrm.Instance.Xmax || X < 0))
            {
                MainFrm.Instance.GameOver("X Coordinate is out of Gamearea", new Location(x, y, z, true));
            }
            if (!noLookup && (Y > MainFrm.Instance.Ymax || Y < 0))
            {
                MainFrm.Instance.GameOver("Y Coordinate is out of Gamearea", new Location(x, y, z, true));
            }
            if (!noLookup && (Z > MainFrm.Instance.Zmax || Z < 0))
            {
                MainFrm.Instance.GameOver("Z Coordinate is out of Gamearea", new Location(x, y, z, true));
            }
        }

        public Location(int x, int y, int z)
            : this(x, y, z, false)
        {
        }

        public override bool Equals(object obj)
        {
            if (obj is Location)
            {
                Location loc = obj as Location;
                if (loc.X == this.X && loc.Y == this.Y && loc.Z == this.Z)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
