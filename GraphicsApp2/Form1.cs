using GraphicsApp2.Render;
using GraphicsApp2.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GraphicsApp2.Selection;
using System.IO;
using System.Text;

namespace GraphicsApp2
{
    public partial class Form1 : Form
    {
        Scene mainScene;
        bool isSelecting = false;
        ISelectionStrategy selectionStrategy;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.InitialImage = null;
            previewPanel.AutoScroll = true;
            mainScene = new Scene(pictureBox1.Width, pictureBox1.Height, previewPanel);
            Image image1 = Image.FromFile("D:\\DATA\\Programming\\Grafika\\Grafika2\\Image1.jpg");
            mainScene.AddSegment(new Segment(image1, new Point(0, 0)));
            pictureBox1.Image = mainScene.GetSceneImage();
            selectionStrategy = new RectangleSelection(mainScene);
            RefreshPreviewPanel();
        }
   
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            isSelecting = true;
            selectionStrategy.OnMouseDown(e);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if(isSelecting)
            {
                selectionStrategy.OnMouseMove(e);                
                pictureBox1.Invalidate();
            }           
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {          
            if (selectionStrategy.OnMouseUp(e))
            {
                isSelecting = false;
                pictureBox1.Image = mainScene.GetSceneImage();
                if (selectionStrategy is RectangleSelection)
                    selectionStrategy = new RectangleSelection(mainScene);
                else if (selectionStrategy is EllipseSelection)
                    selectionStrategy = new EllipseSelection(mainScene);
                else if (selectionStrategy is PolygonSelection)
                    selectionStrategy = new PolygonSelection(mainScene);
                RefreshPreviewPanel();
            }
            if (selectionStrategy is ISelectionStrategy)
                pictureBox1.Invalidate();
        }

        private void pictureBox1_OnPaint(object sender, PaintEventArgs e)
        {
            if (isSelecting)
            {
                selectionStrategy.OnSelectionPaint(e);                
            }           
        }

        private void rectangleButton_Click(object sender, System.EventArgs e)
        {
            selectionStrategy = new RectangleSelection(mainScene);
        }

        private void ellipseButton_Click(object sender, System.EventArgs e)
        {
            selectionStrategy = new EllipseSelection(mainScene);
        }

        private void polygonButton_Click(object sender, System.EventArgs e)
        {
            selectionStrategy = new PolygonSelection(mainScene);
        }


       

        void buttonHighlight_Click(object sender, EventArgs e)
        {
            PanelButton pb = (sender as PanelButton);
            Segment segm = mainScene.GetSegmentAt(pb.Index);
            //mainScene.GetSegmentAt(pb.Index).selectionStrategy.PaintSelection();
            segm.isHighlighted = !segm.isHighlighted;
            pictureBox1.Image = mainScene.GetSceneImage();
            previewPanel.Refresh();

        }

        void buttonRemove_Click(object sender, EventArgs e)
        {
            PanelButton pb = (sender as PanelButton);
            mainScene.RemoveSegment(pb.Index);
            RefreshPreviewPanel();
            pictureBox1.Image = mainScene.GetSceneImage();
        }

        void buttonSave_Click(object sender, EventArgs e)
        {
            PanelButton pb = (sender as PanelButton);
            Image imageToSave = mainScene.GetSegmentAt(pb.Index).image;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Png Image|*.png";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                switch (saveFileDialog1.FilterIndex)
                {
                    case 1:
                        imageToSave.Save(fs,
                          System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;

                    case 2:
                        imageToSave.Save(fs,
                          System.Drawing.Imaging.ImageFormat.Bmp);
                        break;

                    case 3:
                        imageToSave.Save(fs,
                          System.Drawing.Imaging.ImageFormat.Png);
                        break;
                }

                fs.Close();
            }
        }

        public void buttonSaveSegment_Click(object sender, EventArgs e)
        {
            PanelButton pb = (sender as PanelButton);
            Segment segmentToSave = mainScene.GetSegmentAt(pb.Index);

            string jsonString = segmentToSave.GetJSON();
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Segment Image|*.segment";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                byte[] info = new UTF8Encoding(true).GetBytes(jsonString);
                fs.Write(info, 0, info.Length);
                fs.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    string filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        string fileContent = reader.ReadToEnd();
                        Segment segm = new Segment(fileContent);
                        mainScene.AddSegment(segm);
                        segm.selectionStrategy.SetIndex(mainScene.GetSegmentCount() - 1);
                        pictureBox1.Image = mainScene.GetSceneImage();
                        RefreshPreviewPanel();
                    }
                }
            }
        }


        public void RefreshPreviewPanel()
        {
            previewPanel.Controls.Clear();
            previewPanel.RowCount = 2;
            previewPanel.ColumnCount = 2;
            List<Segment> segments = mainScene.GetSegments();
            for (int i = 1; i < segments.Count; i++)
            {
                PictureBox pictureBox = new PictureBox();
                pictureBox.Size = new Size(256, 256);
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox.Image = segments[i].image;

                PanelButton buttonHighlight = new PanelButton();
                buttonHighlight.Size = new Size(128, 32);
                buttonHighlight.Text = "Highlight " + i;
                buttonHighlight.Index = i;
                buttonHighlight.Click += new EventHandler(buttonHighlight_Click);

                PanelButton buttonRemove = new PanelButton();
                buttonRemove.Size = new Size(128, 32);
                buttonRemove.Text = "Remove " + i;
                buttonRemove.Index = i;
                buttonRemove.Click += new EventHandler(buttonRemove_Click);

                PanelButton buttonSaveImage = new PanelButton();
                buttonSaveImage.Size = new Size(128, 32);
                buttonSaveImage.Text = "Save Image " + i;
                buttonSaveImage.Index = i;
                buttonSaveImage.Click += new EventHandler(buttonSave_Click);

                PanelButton buttonSaveSegment = new PanelButton();
                buttonSaveSegment.Size = new Size(128, 32);
                buttonSaveSegment.Text = "Save Segment " + i;
                buttonSaveSegment.Index = i;
                buttonSaveSegment.Click += new EventHandler(buttonSaveSegment_Click);

                Panel buttonsPanel = new Panel();
                buttonsPanel.Size = new Size(256, 256);
                buttonsPanel.Controls.Add(buttonHighlight);
                buttonsPanel.Controls.Add(buttonRemove);
                buttonsPanel.Controls.Add(buttonSaveImage);
                buttonsPanel.Controls.Add(buttonSaveSegment);
                buttonRemove.Location = new Point(0, 50);
                buttonSaveImage.Location = new Point(0, 100);
                buttonSaveSegment.Location = new Point(0, 150);
                previewPanel.Controls.Add(pictureBox, 0, i - 1);
                previewPanel.Controls.Add(buttonsPanel, 1, i - 1);
            }
            previewPanel.Refresh();
        }
    }
}
