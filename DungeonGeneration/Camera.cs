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
        float scale = 6.0f;
        Vector2 position = Vector2.Zero;

        public Matrix ViewMatrix
        {
            get { return viewMatrix; }
        }

        public void Update()
        {
            viewMatrix = Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) * Matrix.CreateScale(scale);
        }
    }
}
