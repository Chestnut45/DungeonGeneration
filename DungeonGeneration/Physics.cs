using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace DungeonGeneration
{
    public class Physics
    {
        int gravCoefficient = 1;
        int maxYvel = 1;
        CollisionSide side;
        Rectangle fr;

        public void Update(Player p, List<Wall> walls)
        {
            //Calculate vertical and horizontal velocities
            if (p.yvel != maxYvel && p.grounded == false)
            {
                p.yvel += gravCoefficient;
            }

            //Check for FUTURE collision
            //AS OF RIGHT NOW, THIS ONLY WORKS IF THE PLAYER DOES NOT MOVE FASTER THAN 1PX IN 1 FRAME, TODO: UPDATE THIS
            //TODO: ADD TO LIST OF COLLISIONS TO RESOLVE, ORDER BY DISTANCE FROM PLAYER. IMPORTANT!
            fr = new Rectangle(p.boundingBox.X + p.xvel, p.boundingBox.Y + p.yvel, p.boundingBox.Width, p.boundingBox.Height);
            for (int i = 0; i < walls.Count; i++)
            {
                if (fr.Intersects(walls[i].boundingBox))
                {
                    side = CollisionHelperAABB.GetCollisionSide(p.boundingBox, walls[i].boundingBox, new Vector2(p.xvel, p.yvel));
                    switch (side)
                    {
                        case CollisionSide.Top:
                            p.yvel = 0;
                            p.grounded = true;
                            break;
                        case CollisionSide.Bottom:
                            p.yvel = 0;
                            break;
                        case CollisionSide.Left:
                            p.xvel = 0;
                            break;
                        case CollisionSide.Right:
                            p.xvel = 0;
                            break;
                    }
                }
            }

            //Actually move player to where they need to be

            p.y += p.yvel;
            p.x += p.xvel;

            p.Update();

            //After player physics has been handled, set the previousBoundingBox Rectangle
            p.previousBoundingBox = p.boundingBox;
        }
    }
}