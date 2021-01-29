using GraphicsApp2.Render;
using GraphicsApp2.Selection;
using System;
using System.Drawing;
using System.IO;

namespace GraphicsApp2.Data
{
    class Segment
    {
        class SegmentSerializable
        {
            public string image;
            public Point point;
            public string selectionType;
            public Point[] newPoints;
            public Rectangle rect;
        }
   
        public Image image;
        public Point position;
        public ISelectionStrategy selectionStrategy;
        public bool isHighlighted = true;

        public Segment(Image _image, Point _position)
        {
            image = _image;
            position = _position;
        }

        public Segment(string json)
        {
            SegmentSerializable segmSer = Newtonsoft.Json.JsonConvert.DeserializeObject<SegmentSerializable>(json);
            image = StringToImage(segmSer.image);
            position = segmSer.point;
            switch(segmSer.selectionType)
            {
                case "RectangleSelection":
                    {
                        selectionStrategy = new RectangleSelection(Scene.instance);
                        ((RectangleSelection)selectionStrategy).selectionRectangle = segmSer.rect;
                        break;
                    }
                case "EllipseSelection":
                    {
                        selectionStrategy = new EllipseSelection(Scene.instance);
                        ((EllipseSelection)selectionStrategy).selectionRectangle = segmSer.rect;
                        break;
                    }
                case "PolygonSelection":
                    {
                        selectionStrategy = new PolygonSelection(Scene.instance);
                        ((PolygonSelection)selectionStrategy).newPoints = segmSer.newPoints;
                        break;
                    }
            }
        }

        public string GetJSON()
        {
            SegmentSerializable segmSer = new SegmentSerializable();
            segmSer.image = ImageToString(image);
            segmSer.point = position;
            segmSer.selectionType = selectionStrategy.GetType().Name;

            switch (selectionStrategy.GetType().Name)
            {
                case "RectangleSelection":
                    {
                        segmSer.rect = ((RectangleSelection)selectionStrategy).selectionRectangle;
                        break;
                    }
                case "EllipseSelection":
                    {
                        segmSer.rect = ((EllipseSelection)selectionStrategy).selectionRectangle;
                        break;
                    }
                case "PolygonSelection":
                    {
                        segmSer.newPoints = ((PolygonSelection)selectionStrategy).newPoints;
                        break;
                    }
            }
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(segmSer);
            return jsonString;
        }

        string ImageToString(Image image)
        {
            if (image == null)
                return String.Empty;
            Bitmap bmp = (Bitmap)image;
            MemoryStream mem = new MemoryStream();
            bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Png);
            var bytes = mem.ToArray();
            return Convert.ToBase64String(bytes);
        }

        Image StringToImage(string base64String)
        {
            if (String.IsNullOrWhiteSpace(base64String))
                return null;

            var bytes = Convert.FromBase64String(base64String);
            var stream = new MemoryStream(bytes);
            return Image.FromStream(stream);
        }
    }
}
