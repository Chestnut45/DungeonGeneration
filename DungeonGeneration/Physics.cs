﻿using System;
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
        int maxYvel = 4;
        int maxXvel = 2;
        public int c = 0;
        public int xc = 0;
        bool done = false;
        CollisionSide side;
        Rectangle fr;
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

            c++;
            if (!p.grounded && p.previouslyGrounded)
            {
                c = 1;
            }

            //Calculate vertical and horizontal velocities
            if (p.yvel != maxYvel && p.grounded == false && c % 4 == 0)
            {
                p.yvel += gravCoefficient;
            }

            xc++;
            if (p.prevxacc != p.xacc)
            {
                xc = 0;
            }

            if (p.xacc == 0)
            {
                if (p.xvel != 0)
                {
                    
                    if (p.xvel < 0)
                    {
                        if (p.grounded)
                        {
                            if (xc % 4 == 0)
                                p.xvel += 1;
                        } else
                        {
                            if (xc % 8 == 0)
                                p.xvel += 1;
                        }
                    } else
                    {
                        if (p.grounded)
                        {
                            if (xc % 4 == 0)
                                p.xvel -= 1;
                        }
                        else
                        {
                            if (xc % 8 == 0)
                                p.xvel -= 1;
                        }
                    }
                    
                }
            }

            if (p.grounded)
            {
                if (xc % 4 == 0)
                {
                    p.xvel += p.xacc;
                }
            } else
            {
                if (xc % 8 == 0)
                {
                    p.xvel += p.xacc;
                }
            }

            p.xvel = MathHelper.Clamp(p.xvel, -maxXvel, maxXvel);

            //Check for FUTURE collision
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

            for (int w = 0; w < collisionsToResolve.Count; w++)
            {
                side = CollisionHelperAABB.GetCollisionSide(p.boundingBox, collisionsToResolve[w].boundingBox, new Vector2(p.xvel, p.yvel));

                switch (side)
                {
                    case CollisionSide.Left:
                        while (fr.Intersects(collisionsToResolve[w].boundingBox))
                        {
                            p.xvel--;
                            p.Update();
                            fr = new Rectangle(p.boundingBox.X + p.xvel, p.boundingBox.Y + p.yvel, p.boundingBox.Width, p.boundingBox.Height);
                        }
                        break;
                    case CollisionSide.Right:
                        while (fr.Intersects(collisionsToResolve[w].boundingBox))
                        {
                            p.xvel++;
                            p.Update();
                            fr = new Rectangle(p.boundingBox.X + p.xvel, p.boundingBox.Y + p.yvel, p.boundingBox.Width, p.boundingBox.Height);
                        }
                        break;
                    case CollisionSide.Top:
                        while (fr.Intersects(collisionsToResolve[w].boundingBox))
                        {
                            p.yvel--;
                            p.Update();
                            fr = new Rectangle(p.boundingBox.X + p.xvel, p.boundingBox.Y + p.yvel, p.boundingBox.Width, p.boundingBox.Height);
                        }
                        break;
                    case CollisionSide.Bottom:
                        while (fr.Intersects(collisionsToResolve[w].boundingBox))
                        {
                            p.yvel++;
                            p.Update();
                            fr = new Rectangle(p.boundingBox.X + p.xvel, p.boundingBox.Y + p.yvel, p.boundingBox.Width, p.boundingBox.Height);
                        }
                        break;
                }
                
                for (int i = 0; i < walls.Count; i++)
                {
                    if (fr.Intersects(walls[i].boundingBox))
                    {
                        collisionsToResolve.RemoveAt(w);
                        walls[i].distanceToPlayer = (int)CollisionHelperAABB.GetDistanceSquared(p.boundingBox, walls[i].boundingBox);
                        collisionsToResolve.Add(walls[i]);
                        collisionsToResolve = collisionsToResolve.OrderBy(o => o.distanceToPlayer).ToList();
                        w = 0;
                    }
                }

                done = true;
                for (int i = 0; i < collisionsToResolve.Count; i++)
                {
                    if (fr.Intersects(collisionsToResolve[i].boundingBox))
                    {
                        done = false;
                    }
                }

                if (done)
                {
                    break;
                }
            }

            //Update player position
            p.y += p.yvel;
            p.x += p.xvel;

            //Update hitbox
            p.Update();

            //Loading zone collision
            if (p.currentRoom != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (p.currentRoom.loadingZones[i] != null)
                    {
                        if (p.boundingBox.Intersects(p.currentRoom.loadingZones[i]) && p.currentRoom.adjacentRooms[i] != null)
                        {
                            switch (i)
                            {
                                case 0:
                                    //Move to left room
                                    p.currentRoom = p.currentRoom.adjacentRooms[0];
                                    p.x = p.currentRoom.loadingZones[2].X - 8;
                                    p.y = p.currentRoom.loadingZones[2].Y;
                                    p.xvel = 0;
                                    p.yvel = 0;
                                    p.Update();
                                    break;
                                case 1:
                                    //Move to up room
                                    p.currentRoom = p.currentRoom.adjacentRooms[1];
                                    p.x = p.currentRoom.loadingZones[3].X + 8;
                                    p.y = p.currentRoom.loadingZones[3].Y - 8;
                                    p.xvel = 0;
                                    p.yvel = 0;
                                    p.Update();
                                    break;
                                case 2:
                                    //Move to right room
                                    p.currentRoom = p.currentRoom.adjacentRooms[2];
                                    p.x = p.currentRoom.loadingZones[0].X + 8;
                                    p.y = p.currentRoom.loadingZones[0].Y;
                                    p.xvel = 0;
                                    p.yvel = 0;
                                    p.Update();
                                    break;
                                case 3:
                                    //Move to down room
                                    p.currentRoom = p.currentRoom.adjacentRooms[3];
                                    p.x = p.currentRoom.loadingZones[1].X + 8;
                                    p.y = p.currentRoom.loadingZones[1].Y + 8;
                                    p.xvel = 0;
                                    p.yvel = 0;
                                    p.Update();
                                    break;
                            }
                        }
                    }
                }
            }

            //After player physics has been handled, set the previousBoundingBox Rectangle
            p.previousBoundingBox = p.boundingBox;
            p.previouslyGrounded = p.grounded;
            p.prevxacc = p.xacc;
        }
    }
}