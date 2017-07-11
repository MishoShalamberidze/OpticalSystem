using System;
using System.Collections.Generic;
using System.Text;

namespace OpticalSystem
{
    public class Vector3
    {
        public double X;
        public double Y;
        public double Z;

        public Vector3(double x = 0, double y = 0, double z = 0)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public void Initial(double x=0, double y=0, double z=0)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public void Initial(Vector3 a)
        {
            this.Initial(a.X,a.Y,a.Z);
        }
        #region operators

        public static Vector3 operator *(double k, Vector3 a)
        {
            return new Vector3(a.X * k, a.Y * k, a.Z * k);
        }
        
        public static Vector3 operator *(Vector3 a, double k) => k * a;
        public static Vector3 operator /(Vector3 a, double k) => a * (1 / k);
        
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3 operator -(Vector3 a)
        {
            return -1 * a;
        }

        public static Vector3 operator -(Vector3 a, Vector3 b) => a + (-b);

        public static double operator *(Vector3 a, Vector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }
        #endregion operator 

        public Vector3 Normal => this / this.Length;

        public void Normalize()
        {
            var a = this.Normal;
            X = a.X;
            Y = a.Y;
            Z = a.Z;
        }
        public double Length
        {
            get { return Math.Sqrt(X * X + Y * Y + Z * Z); }
            set
            {
                Initial(this.Normal*value);
            }
        }
    }
}
