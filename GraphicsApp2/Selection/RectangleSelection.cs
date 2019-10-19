using GraphicsApp2.Render;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphicsApp2.Selection
{
    class RectangleSelection : ISelectionStrategy
    {
        Scene mainScene;
        Point startPos;
        Point endPos;
        Rectangle selectionRectangle;
        int segmentIndex;

        public RectangleSelection(Scene _mainScene)
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
            if(selectionRectangle.Width > 0 && selectionRectangle.Height > 0)
            {
                Bitmap src = (Bitmap)mainScene.GetRootImage();
                Bitmap target = new Bitmap(selectionRectangle.Width, selectionRectangle.Height);

                using (Graphics g = Graphics.FromImage(target))
                {
                    g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height), selectionRectangle, GraphicsUnit.Pixel);
                    g.Save();
                }

                using (Graphics g = Graphics.FromImage(src))
                {
                    g.FillRectangle(Brushes.White, selectionRectangle);
                    g.Save();
                }
                Point p = new Point(selectionRectangle.Left, selectionRectangle.Top);
                Data.Segment segm = new Data.Segment(target, p);
                segm.selectionStrategy = this;
                mainScene.AddSegment(segm);
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
            e.Graphics.DrawRectangle(Pens.DarkCyan, selectionRectangle);
        }

        public void PaintSelection()
        {
            Image image = mainScene.GetSegmentAt(segmentIndex).image;
            Console.WriteLine(this.GetHashCode());
            using (Graphics g = Graphics.FromImage(image))
            {
                g.DrawRectangle(Pens.White, new Rectangle(0, 0, image.Width-1, image.Height-1));
                g.Save();
            }
        }
    }
}
