using System.Windows.Forms;

namespace GraphicsApp2.Selection
{
    interface ISelectionStrategy
    {
        void OnMouseDown(MouseEventArgs e);
        void OnMouseMove(MouseEventArgs e);
        bool OnMouseUp(MouseEventArgs e);
        void OnSelectionPaint(PaintEventArgs e);
        void PaintSelection();
    }
}
