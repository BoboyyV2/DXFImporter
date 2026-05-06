using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace DXFImporter
{
    public static class Trigo
    {


        /**
         * <summary>compute the value closest to the taget</summary>
         * <returns>the closest value to target between the 2 given number</returns>
         */
        static float ClosestTo(float lhs, float rhs, float target)
        {
            float deltaL = Math.Min(Math.Abs(lhs - target),Math.Abs(lhs - (360 + target) ) );
            float deltaR = Math.Min(Math.Abs(rhs - target), Math.Abs(rhs - (360 + target)));
            if (deltaL < deltaR)
            {
                return lhs;
            }
            return rhs;

        }

        


        /**
         * <summary>compute the angle with maximum X coordinates of an arc</summary>
         * <returns>the angle with max X coordinates</returns>
         * <param name="startAngle">the angle at which the arc start, use the same systeme as DrawArc : 0 is the same as radiant 0 but the angle go clockwise</param>
         * <param name="sweepAngle">the angle of the sweep (the arc start from startAngle and end at start + sweep angle).</param>
         */
        public static float GetMaxAngle(float startAngle, float sweepAngle, float targetAngle)
        {
            if (IsAngleOnArc(startAngle, sweepAngle, targetAngle))
            {
                return targetAngle;
            }
            float endAngle =  ( ( (startAngle + sweepAngle) % 360) + 360) % 360;
            return ClosestTo(startAngle, endAngle, targetAngle);
        }

        
        /**
         * <summary>compute if an angle is on the given arc</summary>
         * <returns>true if it is, false otherwise</returns>
         */
        public static bool IsAngleOnArc(float startAngle, float sweepAngle, float targetAngle)
        {
            if (sweepAngle >= 360) //tour complet ou plus
            {
                return true; 
            }
            //normalise l'angle
            targetAngle = (targetAngle % 360 + 360) % 360;//positive

            ///direction
            bool isClockwise = sweepAngle > 0;

            float endAngle = startAngle + sweepAngle;
            endAngle = (endAngle % 360 + 360) % 360;

            if (isClockwise)
            {
                //clockwise  si on cross 0
                if (startAngle >= endAngle)
                {
                    //
                    return (targetAngle >= startAngle || targetAngle <= endAngle);
                }
                return (targetAngle >= startAngle && targetAngle <= endAngle);
            }
            //counter clockwise
            // cross 0
            if (startAngle <= endAngle)
            {
                //
                return (targetAngle <= startAngle || targetAngle >= endAngle);
            }
            return (targetAngle <= startAngle && targetAngle >= endAngle);

        }

        public static PointF GetArcPoint(PointF center,  float radius, float angle)
        {
            angle = -angle;
            if(angle < 0)
            {
                angle += 360;
            }

            //convert enradians
            double angleRadians = angle * Math.PI / 180.0;
            double cos = Math.Cos(angleRadians);
            double sin = Math.Sin(angleRadians);
            double x = center.X + radius * cos;
            double y = center.Y + radius * sin;

            return new PointF((float)x, (float)y);
        }
        


    }
}
