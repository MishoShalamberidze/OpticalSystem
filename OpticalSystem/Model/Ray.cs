using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace OpticalSystem
{
    public class Ray
    {
        public Vector3 Center;

        public Vector3 Direction;

        public Ray(double x, double y, double z, double sx, double sy, double sz)
        {
            Center = new Vector3(x, y, z);
            Direction = new Vector3(sx, sy, sz);
        }

        public Ray(Vector3 center, Vector3 direction)
        {
            this.Center =new Vector3(center.X,center.Y,center.Z);
            this.Direction = new Vector3(direction.X,direction.Y,direction.Y);
        }

        public Ray()
        {

        }
    }
}
