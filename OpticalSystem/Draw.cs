using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using OpticalSystem.Model;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OpticalSystem
{
    public  class Draw
    {
        public double h = 100;

        public Polyline GetPolyline(double r)
        {
            Polyline polyline = new Polyline();
            List<Vector3> points = SurfacePoints(r);
            points.Reverse();
            foreach (var point in points)
            {
                var p = new Point((int)point.Z,(int)point.Y);
            }

        }

        
        
        //sphero
        public List<Vector3> SurfacePoints(double r)
        {
            List<Vector3> result = new List<Vector3>();

            double x = 0;
            for (double y = 0; y <= h/2; y += 0.1)
            {
                result.Add(new Vector3(x,y, CalcZ(r,x,y)));
            }
            
            return result;
        }
        //aspheric
        public List<Vector3> SurfacePoints(Polynom polynom, double r)
        {
            List<Vector3> result = new List<Vector3>();
            double x = 0;
            for (double y = 0; y <= h; y += 0.1)
            {
                
            }
            return result;
        }
        
        //asperic
        private double CalcZ(Vector3 a, Polynom polynom)
        {
            double tr = Math.Sqrt(a.X * a.X + a.Y * a.Y);
            return a.Z - polynom.Calc(tr);
        }

        // sphero
        private double CalcZ(double r, Vector3 v)
        {
            return CalcZ(r, v.X, v.Y);
        }
        private double CalcZ(double r, double x, double y)
        {
            return r - Math.Sign(r) * Math.Sqrt(r * r - x * x - y * y);
        }
    }
}
