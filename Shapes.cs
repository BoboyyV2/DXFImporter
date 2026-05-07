/* Author: Evren DAGLIOGLU
 * E-mail: evrenda@yahoo.com
 * This software is copyrighted to the author himself. It can be used freely for educational purposes.
 * For commercial usage written consent of the author must be taken and a reference to the author should be provided. 
 * No responsibility will be taken for any loss or damage that will occur as a result of the usage of this code. 
 * 
 * Please feel free to inform me about any bugs, problems, ideas etc.
*/

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using static System.Windows.Forms.AxHost;

namespace DXFImporter
{
	#region Shape class - abstract
	public abstract class Shape
	{

		protected Color contourColor;
		protected Color fillColor;
		protected float lineWidth;

		public int shapeIdentifier;
		public int rotation;
		public bool highlighted;

		public static PointF OriginOffset = new PointF();
		public static PointF PartOffset = new PointF();
		public static RectangleF Dimension = new RectangleF();

		public static PointF getOffsetedPosition(PointF point)
		{
			float x = point.X - (float)(PartOffset.X - OriginOffset.X);
			float y = point.Y + (float)(PartOffset.Y + OriginOffset.Y);


            return new PointF(x, y);
		}
        public static float getOffsetedPositionX(float x)
        {
            return (x - (float)(PartOffset.X - OriginOffset.X));
        }
        public static float getOffsetedPositionX(int x)
        {
            return (x - (float)(PartOffset.X - OriginOffset.X));
        }

        public static float getOffsetedPositionY(float y)
        {
            return (y + (float)(PartOffset.Y + OriginOffset.Y));
        }
        public static float getOffsetedPositionY(int y)
        {
            return (y + (float)(PartOffset.Y + OriginOffset.Y));
        }

		public void DrawOffsetedRectangle(ref Graphics g, ref Pen pen, float Xstart, float Ystart, float Xend, float Yend)
		{
			DrawOffsetedLine(ref g, ref pen, getOffsetedPositionY(Xstart), getOffsetedPositionY(Ystart),
								     getOffsetedPositionY(Xend), getOffsetedPositionY(Ystart));

            DrawOffsetedLine(ref g, ref pen, getOffsetedPositionY(Xend), getOffsetedPositionY(Ystart),
                                     getOffsetedPositionY(Xend), getOffsetedPositionY(Yend));

            DrawOffsetedLine(ref g, ref pen, getOffsetedPositionY(Xend), getOffsetedPositionY(Yend),
                                     getOffsetedPositionY(Xstart), getOffsetedPositionY(Yend));

            DrawOffsetedLine(ref g, ref pen, getOffsetedPositionY(Xstart), getOffsetedPositionY(Yend),
                                     getOffsetedPositionY(Xstart), getOffsetedPositionY(Ystart));
        }

		public void DrawOffsetedLine(ref Graphics g, ref Pen pen, float Xstart, float Ystart, float Xend, float Yend)
		{
            g.DrawLine(pen, getOffsetedPositionX(Xstart), getOffsetedPositionY(Ystart),
                            getOffsetedPositionX(Xend),   getOffsetedPositionY(Yend) );
        }

        public void DrawOffsetedEllipse(ref Graphics g, ref Pen pen, PointF centerPoint, float radius)
        {
            PointF pos = getOffsetedPosition(centerPoint);//get the value corresponding to our workzone
            g.DrawEllipse(pen, pos.X - radius, pos.Y - radius, (radius * 2) - pen.Width, (radius * 2) - pen.Width);
        }

        public void DrawOffsetedArc(ref Graphics g, ref Pen pen, PointF centerPoint, float radius, float startAngle, float tempAngle)
        {
            PointF pos = getOffsetedPosition(centerPoint);//get the value corresponding to our workzone

            //g.DrawArc (pen, (float)centerPoint.X - (float) radius, (float)centerPoint.Y - (float)radius, (float)radius*2, (float)radius*2,
			//           - (float) startAngle, -360 + (float) startAngle - (float)sweepAngle);
            g.DrawArc(pen, pos.X - radius , pos.Y - radius, (radius * 2) - pen.Width, (radius * 2) - pen.Width, -startAngle, tempAngle);

        }


        public abstract Color AccessContourColor
		{
			get;
			set;
		}

		public abstract Color AccessFillColor
		{
			get;
			set;
		}

		public abstract float AccessLineWidth
		{
			get;
			set;
		}

		public abstract int AccessRotation
		{
			get;
			set;
		}

		public abstract void Draw(Pen pen, Graphics g);
		public abstract bool Highlight(Pen pen, Graphics g, PointF point);
		
	}
	#endregion

	#region Line class
	public class Line : DXFImporter.Shape
	{
		protected PointF startPoint;
		protected PointF endPoint;

		public Line (PointF start, PointF end, Color color, float w)
		{
			startPoint = new PointF(start.X + (int)Math.Round(OriginOffset.X - PartOffset.X),
								   start.Y + (int)Math.Round(OriginOffset.Y - PartOffset.Y) );
			endPoint = new PointF(end.X + (int)Math.Round(OriginOffset.X - PartOffset.X),
                                   end.Y + (int)Math.Round(OriginOffset.Y - PartOffset.Y));
            contourColor = color;
			lineWidth = w;
			shapeIdentifier = 1;
			rotation= 0;
			
		}

		public Line()
		{

		}


		public override Color AccessContourColor
		{
			get
			{
				return contourColor;
			}
			set
			{
				contourColor = value;
			}
		}

		public override Color AccessFillColor
		{
			get
			{
				return fillColor;
			}
			set
			{
				fillColor = value;
			}
		}

		public override float AccessLineWidth
		{
			get
			{
				return lineWidth;
			}
			set
			{
				lineWidth = value;
			}

		}

		public override int AccessRotation
		{
			get
			{
				return rotation;
			}
			set
			{
				rotation = value;
			}
		}

		public override void Draw (Pen pen, Graphics g)
		{
			if (highlighted)
			{
				pen.Color = Color.Red;
				highlighted = false;
			}

			DrawOffsetedLine(ref g, ref pen, startPoint.X, startPoint.Y, endPoint.X, endPoint.Y	);
			//g.DrawLine(pen, getOffsetedPosition(startPoint), getOffsetedPosition(endPoint) );
		}

		

		public virtual PointF GetStartPoint
		{
			get
			{
				return startPoint;
			}

		}

		public virtual PointF GetEndPoint
		{
			get
			{
				return endPoint;
			}
		}

		public override bool Highlight(Pen pen, Graphics g, PointF point)
		{
			GraphicsPath areaPath;
			Pen areaPen;
			Region areaRegion;

			// Create path which contains wide line
			// for easy mouse selection
			areaPath = new GraphicsPath();
			areaPen = new Pen(Color.Red, 7);

			areaPath.AddLine(GetStartPoint.X, GetStartPoint.Y , GetEndPoint.X, GetEndPoint.Y);
			// startPoint and EndPoint are class members of type Point
			areaPath.Widen(areaPen);

			// Create region from the path
			areaRegion = new Region(areaPath);
					
			if (areaRegion.IsVisible(point) == true)
			{				
				//g.DrawLine(pen, GetStartPoint, GetEndPoint);

				//g.DrawLine(pen, GetStartPoint.X, GetStartPoint.Y , GetEndPoint.X, GetEndPoint.Y);
						
				areaPath.Dispose();
				areaPen.Dispose();
				areaRegion.Dispose();

				return true;
			}

			return false;
		}


		public bool Highlight(Pen pen, Graphics g, PointF point, double scale)
		{
			GraphicsPath areaPath;
			Pen areaPen;
			Region areaRegion;

			// Create path which contains wide line
			// for easy mouse selection
			areaPath = new GraphicsPath();
			areaPen = new Pen(Color.Red, 7);

			areaPath.AddLine((float)GetStartPoint.X* (float)scale, (float)GetStartPoint.Y * (float)scale, (float)GetEndPoint.X* (float)scale, (float)GetEndPoint.Y* (float)scale);
			// startPoint and EndPoint are class members of type Point
			areaPath.Widen(areaPen);

			// Create region from the path
			try
			{
				areaRegion = new Region(areaPath);
			}
			catch
			{
				return false;
			}
					
			if (areaRegion.IsVisible(point) == true)
			{				
				//g.DrawLine(pen, GetStartPoint, GetEndPoint);

				//g.DrawLine(pen, (float)GetStartPoint.X* (float)scale, (float)GetStartPoint.Y * (float)scale, (float)GetEndPoint.X* (float)scale, (float)GetEndPoint.Y* (float)scale);
						
				areaPath.Dispose();
				areaPen.Dispose();
				areaRegion.Dispose();

				return true;
			}

			areaPath.Dispose();
			areaPen.Dispose();
			areaRegion.Dispose();

			return false;
		}
	}
	#endregion

	#region Rectangle class
	public class rectangle : DXFImporter.Line
	{
		public rectangle (PointF start, PointF end, Color color, Color fill, float w, int angle)
		{
			startPoint = start;
			endPoint = end;
			contourColor = color;
			fillColor = fill;
			lineWidth = w;
			shapeIdentifier = 2;
			rotation = angle;
		
		}

		public override void Draw (Pen pen, Graphics g)
		{

			if (AccessRotation != 0)
			{
				DrawRotatedRectangle (pen, g);
				return;
			}
                 
			DrawOffsetedRectangle(ref g, ref pen, GetStartPoint.X, GetStartPoint.Y, GetEndPoint.X, GetEndPoint.Y);

			return;			
		}

		private void DrawRotatedRectangle(Pen pen, Graphics g)
		{
			if (highlighted)
			{
				pen.Color = Color.Red;
				highlighted = false;
			}

			PointF P1 = GetStartPoint;
			PointF P2 = GetEndPoint;

			PointF P3 = new PointF (P2.X, P1.Y);
			PointF P4 = new PointF (P1.X, P2.Y);


			PointF center = new PointF ( P1.X + (P3.X - P1.X)/2, P1.Y + (P4.Y - P1.Y)/2);

			int angle = AccessRotation;

			if (angle != 0)
			{
						
				P1 = CalculateRotatedNewPoint(P1, center, angle);	//Top left
				P3 = CalculateRotatedNewPoint(P3, center, angle);	//Bottom right

				P2 = CalculateRotatedNewPoint(P2, center, angle);	//Top right
				P4 = CalculateRotatedNewPoint(P4, center, angle);   //Bottom left


				DrawOffsetedRectangle(ref g, ref pen, P1.X, P1.Y, P2.X, P2.Y);

				return;

			}

		}

		private PointF CalculateRotatedNewPoint(PointF P, PointF center, int angle)
		{
			double angleRad = angle * 1 / 57.2957;
	
			PointF tempPoint = new PointF (P.X - center.X, P.Y - center.Y);

			double radius = Math.Sqrt ( (tempPoint.X * tempPoint.X) + (tempPoint.Y * tempPoint.Y) );


			double radiant1 = Math.Acos(tempPoint.X / radius);

			if (tempPoint.X < 0 && tempPoint.Y < 0)
				radiant1 = - radiant1;

			if (tempPoint.X > 0 && tempPoint.Y < 0)
				radiant1 = - radiant1;
			
			double radiant2 = Math.Asin(tempPoint.Y / radius);

			radiant1 = radiant1 + angleRad;
			radiant2 = radiant2 + angleRad;

			double temp;
			temp = radius * Math.Cos(radiant1);
			P.X = (int) temp + center.X;

			

			temp = radius * Math.Sin(radiant1);
			P.Y = (int) temp + center.Y;

			
			return P;
		}

		/*		public override bool Highlight(Pen pen, Graphics g, Point point)
				{
					Point check1 = GetStartPoint;
					Point check2 = GetEndPoint;
					
					Point check1a = new Point (check2.X, check1.Y);

					Point check2a = new Point (check1.X, check2.Y);

					Point center = new Point ( check1.X + (check1a.X - check1.X)/2, check1.Y + (check2a.Y - check1.Y)/2);

					check1 = CalculateRotatedNewPoint (check1, center, AccessRotation);

					check2 = CalculateRotatedNewPoint (check2, center, AccessRotation);
					
					check1a = CalculateRotatedNewPoint (check1a, center, AccessRotation);

					check2a = CalculateRotatedNewPoint (check2a, center, AccessRotation);


					GraphicsPath areaPath;
					Pen areaPen;
					Region areaRegion;

					// Create path which contains wide line
					// for easy mouse selection
					areaPath = new GraphicsPath();
					areaPen = new Pen(Color.Red, 2);

					areaPath.AddLine(check1, check1a);
					areaPath.AddLine(check1a, check2);
					areaPath.AddLine(check2, check2a);
					areaPath.AddLine(check2a, check1);

					areaPath.CloseFigure();

					// startPoint and EndPoint are class members of type Point
					areaPath.Widen(areaPen);

					// Create region from the path
					areaRegion = new Region(areaPath);

					RectangleF arect = areaRegion.GetBounds(g);
					
					if (arect.Contains(point))
					{
				
						g.FillPath(Brushes.DarkOrange, areaPath);
						
						//g.FillRectangle(Brushes.Green, arect);

						areaPath.Dispose();
						areaPen.Dispose();
						areaRegion.Dispose();


						return true;

					}
			
					return false;
				}
		
		*/
		/*		public override bool Highlight(Pen pen, Graphics g, Point point)
				{
					Point P1 = GetStartPoint;
					Point P2 = GetEndPoint;

					Point P3 = new Point (P2.X, P1.Y);
					Point P4 = new Point (P1.X, P2.Y);

					if (AccessRotation != 0)
					{
						Point center = new Point ( P1.X + (P3.X - P1.X)/2, P1.Y + (P4.Y - P1.Y)/2);

						P1 = CalculateRotatedNewPoint(P1, center, AccessRotation);
						P2 = CalculateRotatedNewPoint(P2, center, AccessRotation);
						P3 = CalculateRotatedNewPoint(P3, center, AccessRotation);
						P4 = CalculateRotatedNewPoint(P4, center, AccessRotation);
					}

					int maxX = Math.Max(P1.X, P2.X);
					maxX = Math.Max(maxX, P3.X);
					maxX = Math.Max(maxX, P4.X);

					int minX = Math.Min(P1.X, P2.X);
					minX = Math.Min(minX, P3.X);
					minX = Math.Min(minX, P4.X);

					int maxY = Math.Max(P1.Y, P2.Y);
					maxY = Math.Max(maxY, P3.Y);
					maxY = Math.Max(maxY, P4.Y);

					int minY = Math.Min(P1.Y, P2.Y);
					minY = Math.Min(minY, P3.Y);
					minY = Math.Min(minY, P4.Y);

					if (point.X > minX && point.X < maxX && point.Y > minY && point.Y < maxY)
					{
						pen.Color = Color.LightGreen;

						Draw(pen, g);

						return true;
					}

					return false;
				}
		
		*/

		public override bool Highlight(Pen pen, Graphics g, PointF point)
		{
			PointF P1 = GetStartPoint;
			PointF P2 = GetEndPoint;

			PointF P3 = new PointF (P2.X, P1.Y);
			PointF P4 = new PointF (P1.X, P2.Y);

			if (AccessRotation != 0)
			{
				PointF bottom = new PointF (0, 0);
				PointF top = new PointF (0, 0);
				PointF left = new PointF (0, 0);
				PointF right = new PointF (0, 0);

				PointF center = new PointF ( P1.X + (P3.X - P1.X)/2, P1.Y + (P4.Y - P1.Y)/2);

				P1 = CalculateRotatedNewPoint(P1, center, AccessRotation);
				P2 = CalculateRotatedNewPoint(P2, center, AccessRotation);
				P3 = CalculateRotatedNewPoint(P3, center, AccessRotation);
				P4 = CalculateRotatedNewPoint(P4, center, AccessRotation);

				float maxX = Math.Max(P1.X, P2.X);
				maxX = Math.Max(maxX, P3.X);
				maxX = Math.Max(maxX, P4.X);

				if (maxX == P1.X)
					right = P1;
				if (maxX == P2.X)
					right = P2;
				if (maxX == P3.X)
					right = P3;
				if (maxX == P4.X)
					right = P4;

				float minX = Math.Min(P1.X, P2.X);
				minX = Math.Min(minX, P3.X);
				minX = Math.Min(minX, P4.X);


				if (minX == P1.X)
					left = P1;
				if (minX == P2.X)
					left = P2;
				if (minX == P3.X)
					left = P3;
				if (minX == P4.X)
					left = P4;

				
				float maxY = Math.Max(P1.Y, P2.Y);
				maxY = Math.Max(maxY, P3.Y);
				maxY = Math.Max(maxY, P4.Y);


				if (maxY == P1.Y)
					bottom = P1;
				if (maxY == P2.Y)
					bottom = P2;
				if (maxY == P3.Y)
					bottom = P3;
				if (maxY == P4.Y)
					bottom = P4;

				            
				float minY = Math.Min(P1.Y, P2.Y);
				minY = Math.Min(minY, P3.Y);
				minY = Math.Min(minY, P4.Y);


				if (minY == P1.Y)
					top = P1;
				if (minY == P2.Y)
					top = P2;
				if (minY == P3.Y)									
					top = P3;
				if (minY == P4.Y)
					top = P4;

                
				double c1 = checkPosition (left, top, point);
				double c2 = checkPosition (right, top, point);
				double c3 = checkPosition (right, bottom, point);
				double c4 = checkPosition (left, bottom, point);

				if ( (c1 > 0 && c2 > 0 && c3 < 0 && c4 < 0) )
				{
                    
					pen.Color = Color.LightGreen;

					Draw(pen, g);
					
					return true;
				}										
			}
			else
			{
				float maxX = Math.Max(P1.X, P2.X);
				maxX = Math.Max(maxX, P3.X);
				maxX = Math.Max(maxX, P4.X);

                float minX = Math.Min(P1.X, P2.X);
				minX = Math.Min(minX, P3.X);
				minX = Math.Min(minX, P4.X);

                float maxY = Math.Max(P1.Y, P2.Y);
				maxY = Math.Max(maxY, P3.Y);
				maxY = Math.Max(maxY, P4.Y);

                float minY = Math.Min(P1.Y, P2.Y);
				minY = Math.Min(minY, P3.Y);
				minY = Math.Min(minY, P4.Y);


				if (point.X > minX && point.X < maxX && point.Y > minY && point.Y < maxY)
				{
					pen.Color = Color.LightGreen;
					//	pen.Width = 1;

					Draw(pen, g);

					return true;
				}
			}

			return false;
		}

		private double checkPosition(PointF P1, PointF P2, PointF current)
		{
			double m = (double)(P2.Y - P1.Y)/(P2.X - P1.X);
			return ((current.Y - P1.Y) - (m * (current.X - P1.X)));			
		}
		
	}
	#endregion 

	#region Circle Class
	public class circle : DXFImporter.Shape
	{
		private PointF centerPoint;
		private double radius;

		public circle (PointF center, double r, Color color1, Color color2, float w)
		{
			centerPoint = center;
			radius = r;
			contourColor = color1;
			fillColor = color2;
			lineWidth = w;
			shapeIdentifier = 3;
			rotation= 0;
		}

		public override Color AccessContourColor
		{
			get
			{
				return contourColor;
			}
			set
			{
				contourColor = value;
			}
		}

		public override Color AccessFillColor
		{
			get
			{
				return fillColor;
			}
			set
			{
				fillColor = value;
			}
		}

		public override float AccessLineWidth
		{
			get
			{
				return lineWidth;
			}
			set
			{
				lineWidth = value;
			}

		}

		public override int AccessRotation
		{
			get
			{
				return rotation;
			}
			set
			{
				rotation = value;
			}
		}


		public PointF AccessCenterPoint
		{
			get
			{
				return centerPoint;
			}
			set
			{
				centerPoint = value;
			}
		}

		public double AccessRadius
		{
			get
			{
				return radius;
			}
		}

		public override void Draw (Pen pen, Graphics g)
		{
			if (highlighted)
			{
				pen.Color = Color.Red;
				highlighted = false;
			}

			DrawOffsetedEllipse(ref g, ref pen, centerPoint, (float)radius);
		}

		/*public void Draw (Pen pen, Graphics g, double scale)
		{
			if (highlighted)
			{
				pen.Color = Color.Red;
				highlighted = false;
			}

			g.DrawEllipse(pen, (float) centerPoint.X - (float) radius, (float)centerPoint.Y - (float)radius, (float)radius*2, (float)radius*2 );
		}*/

		/*		public override bool Highlight(Pen pen, Graphics g, Point point)
				{
					Point circular = AccessCenterPoint;
					int abc = (int) AccessRadius;
	
					GraphicsPath areaPath;
					Pen areaPen;
					Region areaRegion;

					// Create path which contains wide line
					// for easy mouse selection
					areaPath = new GraphicsPath();
					areaPen = new Pen(Color.Red, 4);
					areaPath.AddEllipse (circular.X - abc, circular.Y - abc, abc * 2, abc*2);
					// startPoint and EndPoint are class members of type Point
					areaPath.Widen(areaPen);

					areaRegion = new Region(areaPath);

					RectangleF arect = areaRegion.GetBounds(g);
					
					if (arect.Contains(point))
					{
				
						g.FillPath(Brushes.DarkOrange, areaPath);
						
						//g.FillRectangle(Brushes.Green, arect);

						areaPath.Dispose();
						areaPen.Dispose();
						areaRegion.Dispose();

		
						return true;

					}
					return false;
				}
		
		*/

		public override bool Highlight(Pen pen, Graphics g, PointF point)
		{
			PointF center = AccessCenterPoint;
			float rad = (float)AccessRadius;

            float check1y = center.Y - rad;
            float check2y = center.Y + rad;
            float check3x = center.X + rad;
            float check4x = center.X - rad;

			double result = (point.X - center.X)*(point.X - center.X) + (point.Y - center.Y)*(point.Y - center.Y) - radius*radius;

			if (result < 0)
			{
				//pen.Color = Color.Red;
				

				//g.DrawEllipse(pen, centerPoint.X - (int) radius, centerPoint.Y - (int)radius, (int)radius*2, (int)radius*2);

				return true;
			}


            
			return false;
		}

		public bool Highlight(Pen pen, Graphics g, PointF point, double scale)
		{
			PointF center = AccessCenterPoint;
			int rad = (int) AccessRadius;

			float check1y = center.Y - rad;
            float check2y = center.Y + rad;
            float check3x = center.X + rad;
            float check4x = center.X - rad;

			double result = (point.X - center.X*(float)scale)*(point.X - center.X*(float)scale) + (point.Y - center.Y*(float)scale)*(point.Y - center.Y*(float)scale) - radius*radius*(float)scale*(float)scale;

			if (result < 0)
			{
				//pen.Color = Color.Red;
				

				//g.DrawEllipse(pen, (float)centerPoint.X*(float)scale - (float) radius*(float)scale, (float)centerPoint.Y*(float)scale - (float)radius*(float)scale, (float)radius*2*(float)scale, (float)radius*2*(float)scale);

				return true;
			}


            
			return false;
		}
        
	}
	#endregion

	#region Freehand Class - Not Completed Yet
	public class FreehandTool : DXFImporter.Shape
	{
		private ArrayList linePoint;

		public FreehandTool (ArrayList points, Color color, float w)
		{
			
			contourColor = color;
			lineWidth = w;
			shapeIdentifier = 4;
			rotation= 0;
			linePoint = points;

		}

		public override Color AccessContourColor
		{
			get
			{
				return contourColor;
			}
			set
			{
				contourColor = value;
			}
			
		}

		public override Color AccessFillColor
		{
			get
			{
				return fillColor;
			}
			set
			{
				fillColor = value;
			}
		}


		public override float AccessLineWidth
		{
			get
			{
				return lineWidth;
			}
			set
			{
				lineWidth = value;
			}
		}

		public override int AccessRotation
		{
			get
			{
				return rotation;
			}
			set
			{
				rotation = value;
			}
		}

		public override void Draw(Pen pen, Graphics g)
		{			

		}

		public override bool Highlight(Pen pen, Graphics g, PointF point)
		{
			return false;
		}


	}
	#endregion

	#region Polyline Class

	public class polyline : DXFImporter.Shape
	{
		private ArrayList listOfLines;

		public polyline (Color color, float w)
		{
			listOfLines = new ArrayList();

			contourColor = color;
			lineWidth = w;
		}

		public override Color AccessContourColor
		{
			get
			{
				return contourColor;
			}
			set
			{
				contourColor = value;
			}
		}

		public override Color AccessFillColor
		{
			get
			{
				return fillColor;
			}
			set
			{
				fillColor = value;
			}
		}


		public override float AccessLineWidth
		{
			get
			{
				return lineWidth;
			}
			set
			{
				lineWidth = value;
			}

		}

		public override int AccessRotation
		{
			get
			{
				return rotation;
			}
			set
			{
				rotation = value;
			}
		}

		public override void Draw(Pen pen, Graphics g)
		{

			foreach (Line obj in listOfLines)
			{
				obj.Draw(pen, g);
			}

		}

		public void Draw(Pen pen, Graphics g, double scale)
		{
			if (highlighted)
			{
				pen.Color = Color.Red;
				highlighted = false;
			}

			foreach (Line obj in listOfLines)
			{
				obj.Draw(pen, g);
			}

		}

		public override bool Highlight(Pen pen, Graphics g, PointF point)
		{
			foreach (Line obj in listOfLines)
			{
				//obj.Draw(pen, g);

				if (obj.Highlight(pen, g, point))
				{
					//pen.Color = Color.Red;
					//Draw(pen, g);
					return true;
				}
			}
			


			return false;
		}

		public bool Highlight(Pen pen, Graphics g, PointF point, double scale)
		{
			foreach (Line obj in listOfLines)
			{
				//obj.Draw(pen, g);

				if (obj.Highlight(pen, g, point, scale))
				{
					//pen.Color = Color.Red;
					//Draw(pen, g, scale);
					return true;
				}
			}
			


			return false;
		}

		public void AppendLine (Line theLine)
		{
			listOfLines.Add(theLine);
		}
        
	}

	#endregion

	#region Arc Class

	public class arc : DXFImporter.Shape
	{
		private PointF centerPoint;
		private double radius;

		private double startAngle;
		private double sweepAngle;

		public arc (Point center, double r, double startangle, double sweepangle,Color color1, Color color2, float w)
		{
			centerPoint = center;
			radius = r;
			startAngle = startangle;
			sweepAngle = sweepangle;
			contourColor = color1;
			fillColor = color2;
			lineWidth = w;
			shapeIdentifier = 3;
			rotation= 0;
		}

		public double AccessStartAngle
		{
			get
			{
				return startAngle;
			}
			
		}

		public double AccessSweepAngle
		{
			get
			{
				return sweepAngle;
			}
		}

		public override Color AccessContourColor
		{
			get
			{
				return contourColor;
			}
			set
			{
				contourColor = value;
			}
		}

		public override Color AccessFillColor
		{
			get
			{
				return fillColor;
			}
			set
			{
				fillColor = value;
			}
		}

		public override float AccessLineWidth
		{
			get
			{
				return lineWidth;
			}
			set
			{
				lineWidth = value;
			}

		}

		public override int AccessRotation
		{
			get
			{
				return rotation;
			}
			set
			{
				rotation = value;
			}
		}


		public PointF AccessCenterPoint
		{
			get
			{
				return centerPoint;
			}
			set
			{
				centerPoint = value;
			}
		}

		public double AccessRadius
		{
			get
			{
				return radius;
			}
		}

		public override void Draw (Pen pen, Graphics g)
		{
			if (highlighted)
			{
				pen.Color = Color.Red;
				highlighted = false;
			}

			g.DrawArc (pen, (float)centerPoint.X - (float) radius, (float)centerPoint.Y - (float)radius, (float)radius*2, (float)radius*2, - (float) startAngle, -360 + (float) startAngle - (float)sweepAngle);
		}

		public void Draw (Pen pen, Graphics g, double scale)
		{
			//g.DrawEllipse(pen, (float) centerPoint.X* (float)scale - (float) radius* (float)scale, (float)centerPoint.Y * (float)scale - (float)radius* (float)scale, (float)radius*2* (float)scale, (float)radius*2* (float)scale);

			

			float tempAngle = 0;
			
		

			
			if (sweepAngle < startAngle)
			{
				tempAngle = -360 + (float) startAngle - (float) sweepAngle;

			}
				/*else if (startAngle > 180 && sweepAngle > 180)
				{
					tempAngle = startAngle - sweepAngle;
				
				
				}*/
			else
				tempAngle = (float) startAngle - (float) sweepAngle;

			
			DrawOffsetedArc(ref g, ref pen, centerPoint, (float)radius, (float)startAngle, tempAngle);
		}
		
		public override bool Highlight(Pen pen, Graphics g, PointF point)
		{
			PointF center = AccessCenterPoint;
            float rad = (float) AccessRadius;

            float check1y = center.Y - rad;
            float check2y = center.Y + rad;
            float check3x = center.X + rad;
            float check4x = center.X - rad;

			double result = (point.X - center.X)*(point.X - center.X) + (point.Y - center.Y)*(point.Y - center.Y) - radius*radius;

			if (result < 0)
			{
				//pen.Color = Color.Yellow;
				

				//g.DrawEllipse(pen, centerPoint.X - (int) radius, centerPoint.Y - (int)radius, (int)radius*2, (int)radius*2);

				return true;
			}
            
			return false;
		}

		public bool Highlight(Pen pen, Graphics g, PointF point, double scale)
		{
			PointF center = AccessCenterPoint;
			float rad = (float) AccessRadius;

            float check1y = center.Y - rad;
            float check2y = center.Y + rad;
            float check3x = center.X + rad;
            float check4x = center.X - rad;

			double result = (point.X - center.X*(float)scale)*(point.X - center.X*(float)scale) + (point.Y - center.Y*(float)scale)*(point.Y - center.Y*(float)scale) - radius*radius*(float)scale*(float)scale;

			if (result < 0)
			{
				//pen.Color = Color.Yellow;
				

				float tempAngle = 0;
				if (sweepAngle<startAngle)
					tempAngle = -360 + (float) startAngle - (float) sweepAngle;
				else
					tempAngle = (float) startAngle - (float) sweepAngle;


				//g.DrawArc(pen, (float)centerPoint.X*(float)scale - (float) radius*(float)scale, (float)centerPoint.Y*(float)scale - (float)radius*(float)scale, (float)radius*2*(float)scale, (float)radius*2*(float)scale, -startAngle, tempAngle);

				return true;
			}
            
			return false;
		}
	}

	#endregion
}
