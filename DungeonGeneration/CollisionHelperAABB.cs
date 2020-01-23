using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace DungeonGeneration
{
    /// <summary>An enumeration of the possible sides at which 2D collision occurred.</summary>
    [Flags]
    public enum CollisionSide
    {
        /// <summary>No collision occurred.</summary>
        None = 0,
        /// <summary>Collision occurred at the top side.</summary>
        Top = 1,
        /// <summary>Collision occurred at the bottom side.</summary>
        Bottom = 2,
        /// <summary>Collision occurred at the left side.</summary>
        Left = 4,
        /// <summary>Collision occurred at the right side.</summary>
        Right = 8
    }

    /// <summary>A collection of helper methods for 2D collision detection and response.</summary>
    public static class CollisionHelperAABB
    {
        /// <summary>Calculates which side of a stationary object 
        /// a moving object has collided with.</summary>
        /// <param name="movingObjectPreviousHitbox">The moving object's previous hitbox,
        /// from the frame prior to when collision occurred.</param>
        /// <param name="stationaryObjectHitbox">The stationary object's hitbox.</param>
        /// <param name="movingObjectVelocity">The moving object's velocity 
        /// during the frame in which the collision occurred.</param>
        /// <returns>The side of the stationary object the moving object has collided with.</returns>
        public static CollisionSide GetCollisionSide(
            Rectangle movingObjectPreviousHitbox,
            Rectangle stationaryObjectHitbox,
            Vector2 movingObjectVelocity)
        {
            double cornerSlopeRise = 0;
            double cornerSlopeRun = 0;

            double velocitySlope = movingObjectVelocity.Y / movingObjectVelocity.X;

            //Stores what sides might have been collided with
            CollisionSide potentialCollisionSide = CollisionSide.None;

            if (movingObjectPreviousHitbox.Right <= stationaryObjectHitbox.Left)
            {
                //Did not collide with right side; might have collided with left side
                potentialCollisionSide |= CollisionSide.Left;

                cornerSlopeRun = stationaryObjectHitbox.Left - movingObjectPreviousHitbox.Right;

                if (movingObjectPreviousHitbox.Bottom <= stationaryObjectHitbox.Top)
                {
                    //Might have collided with top side
                    potentialCollisionSide |= CollisionSide.Top;
                    cornerSlopeRise = stationaryObjectHitbox.Top - movingObjectPreviousHitbox.Bottom;
                }
                else if (movingObjectPreviousHitbox.Top >= stationaryObjectHitbox.Bottom)
                {
                    //Might have collided with bottom side
                    potentialCollisionSide |= CollisionSide.Bottom;
                    cornerSlopeRise = stationaryObjectHitbox.Bottom - movingObjectPreviousHitbox.Top;
                }
                else
                {
                    //Did not collide with top side or bottom side or right side
                    return CollisionSide.Left;
                }
            }
            else if (movingObjectPreviousHitbox.Left >= stationaryObjectHitbox.Right)
            {
                //Did not collide with left side; might have collided with right side
                potentialCollisionSide |= CollisionSide.Right;

                cornerSlopeRun = movingObjectPreviousHitbox.Left - stationaryObjectHitbox.Right;

                if (movingObjectPreviousHitbox.Bottom <= stationaryObjectHitbox.Top)
                {
                    //Might have collided with top side
                    potentialCollisionSide |= CollisionSide.Top;
                    cornerSlopeRise = movingObjectPreviousHitbox.Bottom - stationaryObjectHitbox.Top;
                }
                else if (movingObjectPreviousHitbox.Top >= stationaryObjectHitbox.Bottom)
                {
                    //Might have collided with bottom side
                    potentialCollisionSide |= CollisionSide.Bottom;
                    cornerSlopeRise = movingObjectPreviousHitbox.Top - stationaryObjectHitbox.Bottom;
                }
                else
                {
                    //Did not collide with top side or bottom side or left side;
                    return CollisionSide.Right;
                }
            }
            else
            {
                //Did not collide with either left or right side; 
                //must be top side, bottom side, or none
                if (movingObjectPreviousHitbox.Bottom <= stationaryObjectHitbox.Top)
                    return CollisionSide.Top;
                else if (movingObjectPreviousHitbox.Top >= stationaryObjectHitbox.Bottom)
                    return CollisionSide.Bottom;
                else
                    //Previous hitbox of moving object was already colliding with stationary object
                    return CollisionSide.None;
            }

            //Corner case; might have collided with more than one side
            //Compare slopes to see which side was collided with
            return GetCollisionSideFromSlopeComparison(potentialCollisionSide,
                velocitySlope, cornerSlopeRise / cornerSlopeRun);
        }

        /// <summary>Gets which side of a stationary object was collided with by a moving object
        /// by comparing the slope of the moving object's velocity and the slope of the velocity 
        /// that would have caused the moving object to be touching corners with the stationary
        /// object.</summary>
        /// <param name="potentialSides">The potential two sides that the moving object might have
        /// collided with.</param>
        /// <param name="velocitySlope">The slope of the moving object's velocity.</param>
        /// <param name="nearestCornerSlope">The slope of the path from the closest corner of the
        /// moving object's previous hitbox to the closest corner of the stationary object's
        /// hitbox.</param>
        /// <returns>The side of the stationary object with which the moving object collided.
        /// </returns>
        static CollisionSide GetCollisionSideFromSlopeComparison(
            CollisionSide potentialSides, double velocitySlope, double nearestCornerSlope)
        {
            if ((potentialSides & CollisionSide.Top) == CollisionSide.Top)
            {
                if ((potentialSides & CollisionSide.Left) == CollisionSide.Left)
                    return velocitySlope < nearestCornerSlope ?
                        CollisionSide.Top : CollisionSide.Left;
                else if ((potentialSides & CollisionSide.Right) == CollisionSide.Right)
                    return velocitySlope > nearestCornerSlope ?
                        CollisionSide.Top : CollisionSide.Right;
            }
            else if ((potentialSides & CollisionSide.Bottom) == CollisionSide.Bottom)
            {
                if ((potentialSides & CollisionSide.Left) == CollisionSide.Left)
                    return velocitySlope > nearestCornerSlope ?
                        CollisionSide.Bottom : CollisionSide.Left;
                else if ((potentialSides & CollisionSide.Right) == CollisionSide.Right)
                    return velocitySlope < nearestCornerSlope ?
                        CollisionSide.Bottom : CollisionSide.Right;
            }
            return CollisionSide.None;
        }

        /// <summary>Returns a Vector2 storing the correct location of a moving object 
        /// after collision with a stationary object has been resolved.</summary>
        /// <param name="movingObjectHitbox">The hitbox of the moving object colliding with a
        /// stationary object.</param>
        /// <param name="stationaryObjectHitbox">The hitbox of the stationary object.</param>
        /// <param name="collisionSide">The side of the stationary object with which the moving
        /// object collided.</param>
        /// <returns>A Vector2 storing the corrected location of the moving object 
        /// after resolving its collision with the stationary object.</returns>
        public static Vector2 GetCorrectedLocation(Rectangle movingObjectHitbox,
            Rectangle stationaryObjectHitbox, CollisionSide collisionSide)
        {
            Vector2 correctedLocation = movingObjectHitbox.Location.ToVector2();
            switch (collisionSide)
            {
                case CollisionSide.Left:
                    correctedLocation.X = stationaryObjectHitbox.X - movingObjectHitbox.Width;
                    break;
                case CollisionSide.Right:
                    correctedLocation.X = stationaryObjectHitbox.X + stationaryObjectHitbox.Width;
                    break;
                case CollisionSide.Top:
                    correctedLocation.Y = stationaryObjectHitbox.Y - movingObjectHitbox.Height;
                    break;
                case CollisionSide.Bottom:
                    correctedLocation.Y = stationaryObjectHitbox.Y + stationaryObjectHitbox.Height;
                    break;
            }
            return correctedLocation;
        }

        /// <summary>Returns the distance between the centers of two Rectangles.</summary>
        /// <param name="firstRectangle">The first Rectangle to compare.</param>
        /// <param name="secondRectangle">The second Rectangle to compare.</param>
        /// <returns>The distance between the centers of the two Rectangles.</returns>
        public static float GetDistance(Rectangle firstRectangle, Rectangle secondRectangle)
        {
            Vector2 firstCenter = new Vector2(firstRectangle.X + firstRectangle.Width / 2f,
                firstRectangle.Y + firstRectangle.Height / 2f);
            Vector2 secondCenter = new Vector2(secondRectangle.X + secondRectangle.Width / 2f,
                secondRectangle.Y + secondRectangle.Height / 2f);
            return Vector2.Distance(firstCenter, secondCenter);
        }

        /// <summary>Returns the squared distance between the centers of two Rectangles.</summary>
        /// <param name="firstRectangle">The first Rectangle to compare.</param>
        /// <param name="secondRectangle">The second Rectangle to compare.</param>
        /// <returns>The squared distance between the centers of the two Rectangles.</returns>
        public static float GetDistanceSquared(Rectangle firstRectangle, Rectangle secondRectangle)
        {
            Vector2 firstCenter = new Vector2(firstRectangle.X + firstRectangle.Width / 2f,
                firstRectangle.Y + firstRectangle.Height / 2f);
            Vector2 secondCenter = new Vector2(secondRectangle.X + secondRectangle.Width / 2f,
                secondRectangle.Y + secondRectangle.Height / 2f);
            return Vector2.DistanceSquared(firstCenter, secondCenter);
        }
    }
}
