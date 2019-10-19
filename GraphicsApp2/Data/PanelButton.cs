using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphicsApp2.Data
{
    class PanelButton : Button
    {
        public object Object { get; set; }

        public int Index { get; set; }
    }
}
