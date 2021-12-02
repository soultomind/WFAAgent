using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Win32
{
    public class POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public static implicit operator Point(POINT point)
        {
            return new Point(point.X, point.Y);
        }

        public static implicit operator POINT(Point point)
        {
            return new POINT(point.X, point.Y);
        }
    }
}
