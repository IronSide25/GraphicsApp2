using GraphicsApp2.Render;
using GraphicsApp2.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GraphicsApp2.Selection;

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
                buttonHighlight.Text = "Highlight" + i;
                buttonHighlight.Index = i;
                buttonHighlight.Click += new EventHandler(buttonHighlight_Click);

                PanelButton buttonRemove = new PanelButton();
                buttonRemove.Text = "Remove";
                buttonRemove.Index = i;
                buttonRemove.Click += new EventHandler(buttonRemove_Click);

                Panel buttonsPanel = new Panel();
                buttonsPanel.Size = new Size(256, 256);
                buttonsPanel.Controls.Add(buttonHighlight);
                buttonsPanel.Controls.Add(buttonRemove);
                buttonRemove.Location = new Point(0, 50);
                previewPanel.Controls.Add(pictureBox, 0, i - 1);
                previewPanel.Controls.Add(buttonsPanel, 1, i - 1);
            }
            previewPanel.Refresh();
        }

        void buttonHighlight_Click(object sender, EventArgs e)
        {
            PanelButton pb = (sender as PanelButton);
            mainScene.GetSegmentAt(pb.Index).selectionStrategy.PaintSelection();
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
    }
}
