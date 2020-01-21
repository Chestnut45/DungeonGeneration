using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGeneration
{
    public class Drop
    {
        public enum dropType
        {
            key
        }

        public int X, Y;
        public Texture2D texture;
        public dropType drop;

        public Drop(int x, int y, dropType Drop)
        {
            X = x;
            Y = y;
            drop = Drop;
        }
    }
}
