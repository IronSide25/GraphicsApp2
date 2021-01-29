using GraphicsApp2.Render;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GraphicsApp2.Selection
{
    class EllipseSelection : ISelectionStrategy
    {
        Scene mainScene;
        Point startPos;
        Point endPos;
        public Rectangle selectionRectangle;
        int segmentIndex;
        Image image;

        public EllipseSelection(Scene _mainScene)
        {
            mainScene = _mainScene;
        }

        public void OnMouseDown(MouseEventArgs e)
        {
            startPos = e.Location;
        }

        public void OnMouseMove(MouseEventArgs e)
        {
            endPos = e.Location;
        }

        public bool OnMouseUp(MouseEventArgs e)
        {
            if (selectionRectangle.Width > 0 && selectionRectangle.Height > 0)
            {
                Bitmap src = (Bitmap)mainScene.GetRootImage();
                Bitmap target = new Bitmap(selectionRectangle.Width, selectionRectangle.Height);

                using (Graphics g = Graphics.FromImage(target))
                {
                    g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height), selectionRectangle, GraphicsUnit.Pixel);
                    g.Save();
                }

                Rectangle targetImageRect = new Rectangle(new Point(0, 0), selectionRectangle.Size);
                for (int y = 0; y < target.Height; y++)            
                    for (int x = 0; x < target.Width; x++)
                        if (!IsInsideEllipse(targetImageRect, new Point(x, y)))
                            target.SetPixel(x, y, Color.Transparent);

                int yStart = selectionRectangle.Location.Y;
                int yEnd = selectionRectangle.Location.Y + selectionRectangle.Height;
                int xStart = selectionRectangle.Location.X;
                int xEnd = selectionRectangle.Location.X + selectionRectangle.Width;
                if (yStart < 0)
                    yStart = 0;
                if (xStart < 0)
                    xStart = 0;
                if (yEnd > src.Height)
                    yEnd = src.Height;
                if (xEnd > src.Width)
                    xEnd = src.Width;

                /*for (int y = yStart; y < yEnd; y++)            
                    for (int x = xStart; x < xEnd; x++)
                        if (IsInsideEllipse(selectionRectangle, new Point(x, y)))
                            src.SetPixel(x, y, Color.White);*/

                Point p = new Point(selectionRectangle.Left, selectionRectangle.Top);
                Data.Segment segm = new Data.Segment(target, p);
                segm.selectionStrategy = this;
                mainScene.AddSegment(segm);
                image = segm.image;
                segmentIndex = mainScene.GetSegmentCount() - 1;              
            }
            return true;
        }

        public void OnSelectionPaint(PaintEventArgs e)
        {
            selectionRectangle = new Rectangle(
                    Math.Min(startPos.X, endPos.X),
                    Math.Min(startPos.Y, endPos.Y),
                    Math.Abs(startPos.X - endPos.X),
                    Math.Abs(startPos.Y - endPos.Y));
            e.Graphics.DrawEllipse(Pens.DarkCyan, selectionRectangle);
        }

        /*public void PaintSelection()
        {
            Image image = mainScene.GetSegmentAt(segmentIndex).image;
            using (Graphics g = Graphics.FromImage(image))
            {
                Size size = selectionRectangle.Size;
                size.Width -= 1;
                size.Height -= 1;
                g.DrawEllipse(Pens.White, new Rectangle(new Point(0, 0), size));
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
                Size size = selectionRectangle.Size;
                size.Width -= 1;
                size.Height -= 1;
                g.DrawEllipse(Pens.White, new Rectangle(new Point(0, 0), size));
                g.Save();
            }
            return selectionImage;
        }

        private bool IsInsideEllipse(Rectangle ellipse, Point location)
        {
            Point center = new Point(
                  ellipse.Left + (ellipse.Width / 2),
                  ellipse.Top + (ellipse.Height / 2));

            double _xRadius = ellipse.Width / 2;
            double _yRadius = ellipse.Height / 2;

            if (_xRadius <= 0.0 || _yRadius <= 0.0)
                return false;

            Point normalized = new Point(location.X - center.X, location.Y - center.Y);
            return ((double)(normalized.X * normalized.X)
                     / (_xRadius * _xRadius)) + ((double)(normalized.Y * normalized.Y) / (_yRadius * _yRadius)) <= 1.00;
        }

        public void SetIndex(int _index)
        {
            segmentIndex = _index;
        }
    }
}
