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
        int maxYvel = 3;
        int maxXvel = 2;
        CollisionSide side;
        Rectangle fr;
        Vector2 cp;
        List<Wall> collisionsToResolve = new List<Wall>();

        public void Update(Player p, List<Wall> walls)
        {
            //Calculate if you're on the ground lol
            fr = new Rectangle(p.boundingBox.X, p.boundingBox.Y + 1, p.boundingBox.Width, p.boundingBox.Height);
            p.grounded = false;
            for (int i = 0; i < walls.Count; i++)
            {
                if (fr.Intersects(walls[i].boundingBox))
                {
                    p.grounded = true;
                }
            }

            //Calculate vertical and horizontal velocities
            if (p.yvel != maxYvel && p.grounded == false)
            {
                p.yvel += gravCoefficient;
            }

            if (p.xacc == 0)
            {
                if (p.xvel != 0)
                {
                    
                    if (p.xvel < 0)
                    {
                        p.xvel += 1;
                    } else
                    {
                        p.xvel -= 1;
                    }
                    
                }
            }
            p.xvel += p.xacc;
            p.xvel = MathHelper.Clamp(p.xvel, -maxXvel, maxXvel);

            //Check for FUTURE collision
            //AS OF RIGHT NOW, THIS ONLY WORKS IF THE PLAYER DOES NOT MOVE FASTER THAN 1PX IN 1 FRAME, TODO: UPDATE THIS
            collisionsToResolve.Clear();
            fr = new Rectangle(p.boundingBox.X + p.xvel, p.boundingBox.Y + p.yvel, p.boundingBox.Width, p.boundingBox.Height);
            for (int i = 0; i < walls.Count; i++)
            {
                if (fr.Intersects(walls[i].boundingBox))
                {
                    walls[i].distanceToPlayer = (int)CollisionHelperAABB.GetDistanceSquared(fr, walls[i].boundingBox);
                    collisionsToResolve.Add(walls[i]);
                }
            }

            collisionsToResolve = collisionsToResolve.OrderBy(o => o.distanceToPlayer).ToList();

            foreach (Wall w in collisionsToResolve)
            {
                side = CollisionHelperAABB.GetCollisionSide(p.boundingBox, w.boundingBox, new Vector2(p.xvel, p.yvel));
                
                switch (side)
                {
                    case CollisionSide.Top:
                        p.yvel = 0;
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

            //Actually move player to where they need to be

            p.y += p.yvel;
            p.x += p.xvel;

            p.Update();

            //After player physics has been handled, set the previousBoundingBox Rectangle
            p.previousBoundingBox = p.boundingBox;
        }
    }
}