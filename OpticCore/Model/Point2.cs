using OpticalSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticCore.Model
{
    public class Point2
    {
        public double x;
        public double y;

        public Point2()
        {
            x = 0;
            y = 0;
        }

        public Point2(Vector3 vector3)
        {
            x = vector3.Z;
            y = vector3.Y;
        }

        public Point2(double x, double y) {
            this.x = x;
            this.y = y;
        }


    }
}
