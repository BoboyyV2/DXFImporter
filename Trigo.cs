using System;
using System.Drawing;

namespace DXFImporter
{
    public static class Trigo
    {

        public static double DegreesToRadians(double angle)
        {
            return angle * Math.PI / 180.0;
        }

        /**
         * <summary>compute the value closest to the taget</summary>
         * <returns>the closest value to target between the 2 given number</returns>
         */
        static float ClosestTo(float lhs, float rhs, float target)
        {
            float deltaL = Math.Min(Math.Abs(lhs - target), Math.Abs(lhs - (360 + target)));
            float deltaR = Math.Min(Math.Abs(rhs - target), Math.Abs(rhs - (360 + target)));
            if (deltaL < deltaR)
            {
                return lhs;
            }
            return rhs;

        }




        /**
         * <summary>compute the angle closest to the targeted angle</summary>
         * <returns>the targeted angle idf it is in the arc, the closest limit otherwise</returns>
         * <param name="startAngle">the angle at which the arc start, use the same systeme as DrawArc : 0 is the same as radiant 0 but the angle go clockwise</param>
         * <param name="sweepAngle">the angle of the sweep (the arc start from startAngle and end at start + sweep angle).</param>
         * <param name="targetAngle">the targeted angle</param>
         */
        public static float GetMaxAngle(float startAngle, float sweepAngle, float targetAngle)
        {
            if (IsAngleOnArc(startAngle, sweepAngle, targetAngle))
            {
                return targetAngle;
            }
            float endAngle = (((startAngle + sweepAngle) % 360) + 360) % 360;
            return ClosestTo(startAngle, endAngle, targetAngle);
        }


        /**
         * <summary>compute if an angle is on the given arc</summary>
         * <returns>true if it is, false otherwise</returns>
         * <param name="startAngle">the angle at which the arc start, use the same systeme as DrawArc : 0 is the same as radiant 0 but the angle go clockwise</param>
         * <param name="sweepAngle">the angle of the sweep (the arc start from startAngle and end at start + sweep angle).</param>
         * <param name="targetAngle">the targeted angle</param>
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


        /**
        * <summary>compute the position of the point on a circle at a given angle</summary>
        * <returns>the position of the point</returns>
        * <param name="center"> the center of the circle</param>
        * <param name="radius">the radius of the circle</param>
        * <param name="angle">the angle in degres starting from 0pie and going clockwise</param>
        */
        public static PointF GetArcPoint(PointF center, float radius, float angle)
        {
            //convert to the same system as radiant
            angle = -angle;
            if (angle < 0)
            {
                angle += 360;
            }

            //convert to radiant
            double angleRadians = angle * Math.PI / 180.0;
            double cos = Math.Cos(angleRadians);
            double sin = Math.Sin(angleRadians);
            double x = center.X + radius * cos;
            double y = center.Y + radius * sin;

            return new PointF((float)x, (float)y);
        }


        /**
         * <summary>compute the limit coordinate of a circle or arc, this is meant to work for the 4 cardinal directions only, the rest is up to luck</summary>
         * <remarks>it is best to simply use the individual function </remarks>
         * <returns>a point on the arc/circle closest to the specified angle</returns>
         * <param name="center"> the center of the circle</param>
         * <param name="radius">the radius of the circle</param>
         * <param name="startAngle">the angle at which the arc start, use the same systeme as DrawArc : 0 is the same as radiant 0 but the angle go clockwise.<br></br>
         * for circle use whatever</param>
         * <param name="sweepAngle">the angle of the sweep (the arc start from startAngle and end at start + sweep angle).<br></br>
         * for circles use 360</param>
         * <param name="targetAngle">the targeted angle, this value should be :<br></br>
         *  0   for XMax<br></br>
         *  180 for XMin<br></br>
         *  90 for YMax<br></br>
         *  270  for YMin</param>
         */
        public static PointF GetRealLimit(PointF center, float radius, float startAngle, float sweepAngle, float targetAngle)
        {
            float closestAngle = GetMaxAngle(startAngle, sweepAngle, targetAngle);
            return GetArcPoint(center, radius, closestAngle);
        }
    }
}
