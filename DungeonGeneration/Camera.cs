using Microsoft.Xna.Framework;
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

        public Matrix ViewMatrix
        {
            get { return viewMatrix; }
        }

        public void Update()
        {
            viewMatrix = Matrix.CreateTranslation(new Vector3(0, 0, 0)) * Matrix.CreateScale(scale);
        }
    }
}
