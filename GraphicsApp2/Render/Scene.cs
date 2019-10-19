using GraphicsApp2.Data;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GraphicsApp2.Render
{
    class Scene
    {
        private readonly int width, height;
        private List<Segment> segments;
        public Image lastSceneImage;

        public Scene(int _width, int _height, TableLayoutPanel _previewPanel)
        {
            width = _width;
            height = _height;
            segments = new List<Segment>();
            lastSceneImage = new Bitmap(width, height);
        }

        public void AddSegment(Segment segment)
        {
            segments.Add(segment);
        }

        public void RemoveSegment(int index)
        {
            segments.RemoveAt(index);
        }

        public int GetSegmentCount()
        {
            return segments.Count;
        }

        public Segment GetSegmentAt(int index)
        {
            return segments[index];
        }

        public Image GetSceneImage()
        {
            Bitmap finalImage = new Bitmap(segments[0].image);
            using (Graphics grfx = Graphics.FromImage(finalImage))
            {
               for(int i = 1; i < segments.Count; i++)
                    grfx.DrawImage(segments[i].image, segments[i].position.X, segments[i].position.Y);

                //grfx.DrawImage(segments[1].image, segments[1].position.X, segments[1].position.Y);
                grfx.Save();
            }
            lastSceneImage = finalImage;
            return finalImage;
        }

        public List<Segment> GetSegments()
        {
            return segments;
        }

        public Image GetRootImage()
        {
            return segments[0].image;
        }        
    }
}
