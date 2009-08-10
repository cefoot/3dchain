using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Chain3D
{

    public delegate void Draw(Graphics gra0,Rectangle area);

    public partial class PaintArea : UserControl
    {
        public PaintArea()
        {
            InitializeComponent();
        }

        List<Draw> _drawList = new List<Draw>();

        public Draw DrawSpecial
        {
            set
            {
                if (value != null)
                {
                    _drawList.Add(value);
                }
            }
        }

        public void ClearDrawList()
        {
            _drawList.Clear();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            foreach (var curDraw in _drawList)
            {
                if (curDraw != null)
                {
                    curDraw(e.Graphics, e.ClipRectangle);
                }
            }
        }
    }
}
