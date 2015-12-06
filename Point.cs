using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StillBetterThanPHPLang {
    public class Point : ICloneable {
        public int x;
        public int y;
        public Point() {
            this.x = 0;
            this.y = 0;
        }
        public Point(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public object Clone() {
            return new Point(x, y);
        }
    }
}
