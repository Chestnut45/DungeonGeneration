using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGeneration
{
    public class Camera
    {
        Matrix viewMatrix;
        float scale = 5.0f;
        Vector2 position = Vector2.Zero;

        public Matrix ViewMatrix
        {
            get { return viewMatrix; }
        }

        public void Update(Player p, GraphicsDeviceManager g)
        {
            /*
            position.X = p.x - (g.GraphicsDevice.Viewport.Width / 2) / scale;
            position.Y = p.y - (g.GraphicsDevice.Viewport.Height / 2) / scale;
            
            if (p.currentRoom != null)
            {
                position.X = MathHelper.Clamp(position.X, 0, (p.currentRoom.map.GetLength(0)*8) - g.GraphicsDevice.Viewport.Width);
                position.Y = MathHelper.Clamp(position.Y, 0, (p.currentRoom.map.GetLength(1)*8) - g.GraphicsDevice.Viewport.Height);
            }
            */
            
            viewMatrix = Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) * Matrix.CreateScale(new Vector3(scale, scale, 0));
        }
    }
}
