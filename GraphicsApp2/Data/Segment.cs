using GraphicsApp2.Selection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsApp2.Data
{
    class Segment
    {
        public Image image;
        public Point position;
        public ISelectionStrategy selectionStrategy;

        public Segment(Image _image, Point _position)
        {
            image = _image;
            position = _position;
        }
    }
}
