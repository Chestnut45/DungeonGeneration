using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGeneration
{
    public class Player
    {
        public Room currentRoom;
        public Room[] currentDungeon;
        public Texture2D texture;
        public int x, y, xvel, yvel;
        public Rectangle boundingBox = new Rectangle();

        public Player()
        {
            //Construct a player -- What do we need?
        }

        public void Update()
        {
            boundingBox.X = x * 8;
            boundingBox.Y = y * 8;
            boundingBox.Width = texture.Width;
            boundingBox.Height = texture.Height;
        }
    }
}
