using Microsoft.Xna.Framework;
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
        public Rectangle boundingBox = new Rectangle();

        public Wall(int x, int y, Texture2D t)
        {
            X = x;
            Y = y;
            texture = t;
            boundingBox = new Rectangle(x * 8, y * 8, texture.Width, texture.Height);
        }
    }
}
