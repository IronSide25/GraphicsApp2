using GraphicsApp2.Render;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GraphicsApp2.Selection
{
    class PolygonSelection : ISelectionStrategy
    {
        const float minDistanceOffset = 5;
        List<Point> points;
        Scene mainScene;
        public Point[] newPoints;
        int segmentIndex;


        public PolygonSelection(Scene _mainScene)
        {
            points = new List<Point>();
            mainScene = _mainScene;
        }

        public void OnMouseDown(MouseEventArgs e)
        {
            points.Add(e.Location);
        }

        public void OnMouseMove(MouseEventArgs e)
        {
        }

        public bool OnMouseUp(MouseEventArgs e)
        {
            if (points.Count > 1)
            {
                int x1 = points[0].X;
                int y1 = points[0].Y;
                int x2 = points[points.Count - 1].X;
                int y2 = points[points.Count - 1].Y;

                float distSqr = ((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
                if (distSqr < minDistanceOffset * minDistanceOffset)
                {                  
                    CutElement();
                    points.Clear();
                    return true;
                }
            }                      
            return false;
        }

        public void OnSelectionPaint(PaintEventArgs e)
        {
            if (points.Count > 1)
                for (int i = 1; i < points.Count; i++)
                {
                    e.Graphics.DrawLine(Pens.DarkCyan, points[i - 1], points[i]);
                }
        }

        /*public void PaintSelection()
        {
            using (Graphics g = Graphics.FromImage(mainScene.GetSegmentAt(segmentIndex).image))
            {
                for (int i = 1; i < newPoints.Length; i++)
                {
                    g.DrawLine(Pens.White, newPoints[i - 1], newPoints[i]);
                }
                g.Save();
            }
        }*/

        public Image SelectionImage(int ind)
        {
            Image image = mainScene.GetSegmentAt(ind).image;
            Bitmap selectionImage = new Bitmap(image);
            using (Graphics g = Graphics.FromImage(selectionImage))
            {
                g.Clear(Color.Transparent);
                for (int i = 1; i < newPoints.Length; i++)
                {
                    g.DrawLine(Pens.White, newPoints[i - 1], newPoints[i]);
                }
                g.Save();
            }
            return selectionImage;
        }

        private void CutElement()
        {
            Bitmap src = (Bitmap)mainScene.GetRootImage();
            int leftX = src.Width;
            int rightX = 0;
            int topY = src.Height;
            int bottomY = 0;

            foreach(Point po in points)
            {
                if (po.X < leftX)
                    leftX = po.X;
                if (po.X > rightX)
                    rightX = po.X;
                if (po.Y < topY)
                    topY = po.Y;
                if (po.Y > bottomY)
                    bottomY = po.Y;
            }

            Rectangle selectionRectangle = new Rectangle(leftX, topY, rightX - leftX, bottomY - topY);
            Bitmap target = new Bitmap(selectionRectangle.Width, selectionRectangle.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height), selectionRectangle, GraphicsUnit.Pixel);
                g.Save();
            }

            newPoints = new Point[points.Count];
            for (int i = 0; i < points.Count; i++)
                newPoints[i] = new Point(points[i].X - selectionRectangle.Location.X, points[i].Y - selectionRectangle.Location.Y);

            for (int y = 0; y < target.Height; y++)
                for (int x = 0; x < target.Width; x++)
                    if (!IsInPolygon(newPoints, new Point(x, y)))
                        target.SetPixel(x, y, Color.Transparent);

            int yStart = selectionRectangle.Location.Y;
            int yEnd = selectionRectangle.Location.Y + selectionRectangle.Height;
            int xStart = selectionRectangle.Location.X;
            int xEnd = selectionRectangle.Location.X + selectionRectangle.Width;

            /*for (int y = yStart; y < yEnd; y++)
                for (int x = xStart; x < xEnd; x++)
                    if (IsInPolygon(points.ToArray(), new Point(x, y)))
                        src.SetPixel(x, y, Color.White);*/

            Point p = new Point(selectionRectangle.Left, selectionRectangle.Top);
            Data.Segment segm = new Data.Segment(target, p);
            segm.selectionStrategy = this;
            mainScene.AddSegment(segm);
            segmentIndex = mainScene.GetSegmentCount() - 1; 
        }

        public static bool IsInPolygon(Point[] poly, Point p)
        {
            Point p1, p2;
            bool inside = false;
            if (poly.Length < 3)
            {
                return inside;                
            }
            Point oldPoint = new Point(poly[poly.Length - 1].X, poly[poly.Length - 1].Y);
            for (int i = 0; i < poly.Length; i++)
            {
                Point newPoint = new Point(poly[i].X, poly[i].Y);
                if (newPoint.X > oldPoint.X)
                {
                    p1 = oldPoint;
                    p2 = newPoint;
                }
                else
                {
                    p1 = newPoint;
                    p2 = oldPoint;
                }
                if ((newPoint.X < p.X) == (p.X <= oldPoint.X) && (p.Y - p1.Y) * (p2.X - p1.X) < (p2.Y - p1.Y) * (p.X - p1.X))
                {
                    inside = !inside;
                }
                oldPoint = newPoint;
            }
            return inside;
        }

        public void SetIndex(int _index)
        {
            segmentIndex = _index;
        }
    }
}
