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
        public Room currentRoom, pr;
        public Room[] currentDungeon;
        public Texture2D texture;
        public int x = 0, y = 0, xvel = 0, yvel = 0, xacc = 0, yacc = 0;
        public Vector2 velocity = new Vector2(0);
        public Rectangle boundingBox = new Rectangle();
        public Rectangle previousBoundingBox = new Rectangle();
        public bool grounded = false;

        public Player()
        {
            //Construct a player -- What do we need?
        }

        public void Update()
        {
            //Update boundingBox
            boundingBox.X = x + 2;
            boundingBox.Y = y + 5;
            boundingBox.Width = texture.Width - 3;
            boundingBox.Height = texture.Height - 5;
        }
    }
}
