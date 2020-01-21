using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGeneration
{
    public class Wall
    {
        public int X, Y;
        public Texture2D texture;

        public Wall(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
