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
            this.Center = new Vector3(center.X,center.Y,center.Z);
            this.Direction = new Vector3(direction.X,direction.Y,direction.Z);
        }

        public Ray(Vector3 center,double alpa)
        {
            this.Center = new Vector3(center.X, center.Y, center.Z);
            this.Direction = new Vector3(0, Math.Sin(alpa), Math.Cos(alpa));
        }

        public Ray()
        {

        }

        public static Vector3 CroosPoint(Ray a, Ray b)
        {
            double dz = b.Center.Z - a.Center.Z;
            double dy = b.Center.Y - a.Center.Y;
            double det = b.Direction.Z * a.Direction.Y - b.Direction.Y * a.Direction.Z;
            double u = (dy * b.Direction.Z - dz * b.Direction.Y) / det;
            double v = (dy * a.Direction.Z - dz * a.Direction.Y) / det;
            return a.Center + u * a.Direction;
        }
    }
}
