using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGeneration
{
    public class Room
    {
        public int mx, my, id;
        public int[,] map = new int[32, 18];
        public bool[] exits = new bool[4];
        public int[] exitLocation = new int[3];
        public Room[] adjacentRooms = new Room[4];
        public Room(int mapx, int mapy)
        {
            mx = mapx;
            my = mapy;
        }
    }
}
