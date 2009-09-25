using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            tableLayoutPanel1.RowCount = 0;
            UpdateTable();
        }

        Color[] clrs = new Color[]{
            Color.Red,
            Color.Purple,
            Color.SaddleBrown,
            Color.Blue,
            Color.White,
            Color.Black,
            Color.Bisque,
            Color.Aquamarine,
            Color.DarkSalmon,
            Color.Indigo,
            Color.LightYellow,
            Color.Moccasin,
            Color.Pink,
            Color.SkyBlue,
            Color.Wheat
        };

        private void UpdateTable()
        {
            int oldCount = tableLayoutPanel1.RowCount;
            int cellHeight = (int)(((float)tableLayoutPanel1.Width / 2) * (1f + (float)trackBar1.Value / 10f));
            int count = 1;
            int firstHeight = (int)((float)cellHeight * ((float)trackBar2.Value / 100f));
            if (firstHeight < tableLayoutPanel1.Height)
            {
                count++;
                while (firstHeight + (count-1) * cellHeight < tableLayoutPanel1.Height)
                {
                    count++;
                }
            }
            label1.Text = count.ToString();
            while (count < tableLayoutPanel1.RowStyles.Count)
            {
                tableLayoutPanel1.RowStyles.RemoveAt(tableLayoutPanel1.RowStyles.Count - 1);
            }
            while (count > tableLayoutPanel1.RowStyles.Count)
            {
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, cellHeight));
            }
            tableLayoutPanel1.RowStyles[count - 1].SizeType = SizeType.AutoSize;
            tableLayoutPanel1.RowStyles[count - 1].Height = 0;
            tableLayoutPanel1.RowCount = tableLayoutPanel1.RowStyles.Count;
            if (oldCount != count)
            {
                tableLayoutPanel1.Controls.Clear();
                tableLayoutPanel1.Controls.Add(GetNewPanel(0), 0, 0);
                tableLayoutPanel1.Controls.Add(GetNewPanel(tableLayoutPanel1.RowStyles.Count - 1), 0, tableLayoutPanel1.RowStyles.Count - 1);
            }
            tableLayoutPanel1.RowStyles[0].SizeType = SizeType.Absolute;
            tableLayoutPanel1.RowStyles[0].Height = firstHeight;
            for (int i = 1; i < tableLayoutPanel1.RowStyles.Count - 1; i++)
            {
                if (oldCount != count)
                {
                    tableLayoutPanel1.Controls.Add(GetNewPanel(i), 0, i);
                }
                tableLayoutPanel1.RowStyles[i].SizeType = SizeType.Absolute;
                tableLayoutPanel1.RowStyles[i].Height = cellHeight;
            }
        }

        private void tableLayoutPanel1_SizeChanged(object sender, EventArgs e)
        {
            UpdateTable();
        }

        private Control GetNewPanel(int index)
        {
            int i = index;
            Panel pnl = new Panel();
            while (i >= clrs.Length)
            {
                i -= clrs.Length;
            }
            pnl.BackColor = clrs[i];
            pnl.Padding = new Padding(0);
            pnl.Margin = new Padding(0);
            pnl.Dock = DockStyle.Fill;
            pnl.MinimumSize = new Size();
            return pnl;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            UpdateTable();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            UpdateTable();
        }
    }
}
