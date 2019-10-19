using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
