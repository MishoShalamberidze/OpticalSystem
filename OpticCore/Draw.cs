using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Shapes;
using OpticalSystem.Model;
using Point = System.Windows.Point;
using System.Windows.Media;
using System.Windows.Controls;

namespace OpticalSystem
{
    public class Draw
    {
        public  static double h;
        public static double k;
        public Draw(double h, double k)
        {
            Draw.h = h;
            Draw.k = k;
        }
        public List<Polyline> DrawSystemLens(Core core, Dictionary<double, Brush> brushes = null)
        {
            List<Polyline> polylines = new List<Polyline>();
            double distance = 0;
            for (int i = 0; i < core.m; i++)
            {
                if (core.n[i + 1] != 1)
                {
                    List<Point> surfacePoints1, surfacePoints2;
                    Polynom polynom;
                    polynom = i == core.Ia ? core.AsfreikPolynom1 : null;
                    surfacePoints1 = getSimetricPoints(core.r[i], distance, polynom);
                    polynom = i + 1 == core.Ia ? core.AsfreikPolynom1 : null;
                    surfacePoints2 = getSimetricPoints(core.r[i + 1], distance + core.d[i], polynom);
                    surfacePoints2.Reverse();
                    surfacePoints1.AddRange(surfacePoints2);
                    var polyline = new Polyline();
                    surfacePoints1.ForEach((p) =>
                    {
                        polyline.Points.Add(p);
                    });
                    polyline.Points.Add(surfacePoints1.FirstOrDefault());
                    if (brushes != null)
                    {
                        var brush = brushes.FirstOrDefault(x => x.Key == core.n[i + 1]).Value;
                        polyline.Fill = brush;
                    }
                    polylines.Add(polyline);
                }
                distance += core.d[i];
            }
            return polylines;

        }

        public Polyline DrawRay(List<Ray> ray)
        {
            var rayPoints = ray.Select(x => x.Center).ToList();
            var lastPoint = ray.LastOrDefault().Center + ray.LastOrDefault().Direction * 500;
            rayPoints.Add(lastPoint);
            var points = Vector3ListToPoints(rayPoints);

            var polyline = new Polyline();
            foreach (var p in points)
            {
                polyline.Points.Add(p);
            }
            //surface.Fill = Brushes.LightBlue;
            polyline.Stroke = Brushes.Black;
            polyline.StrokeThickness = 1;
            return polyline;
        }

        private List<Point> getSimetricPoints(double r, double d, Polynom polynom = null)
        {
            List<Vector3> vectors = polynom != null ? SurfacePoints(polynom) : SurfacePoints(r);
            vectors.ForEach(x => x.Z += d);
            List<Vector3> t1 = new List<Vector3>();
            vectors.Reverse();
            foreach (var v in vectors)
            {
                t1.Add(new Vector3(v.X, -v.Y, v.Z));
            }
            vectors.Reverse();
            t1.AddRange(vectors);
            var t = Vector3ListToPoints(t1);
            return t;
        }

        private Point Vector3ToPoint(Vector3 v)
        {
            return new Point((int)v.Z, (int)v.Y);
        }

        private List<Point> Vector3ListToPoints(List<Vector3> vector3List)
        {
            var res = new List<Point>();
            foreach (var v in vector3List)
            {
                var p = Vector3ToPoint(v * k);
                if (!res.Contains(p)) res.Add(p);
            }
            return res;
        }

        //sphero
        private List<Vector3> SurfacePoints(double r)
        {
            List<Vector3> result = new List<Vector3>();

            double x = 0;
            for (double y = 0; y <= h / 2; y += 0.1)
            {
                result.Add(new Vector3(x, y, CalcZ(r, x, y)));
            }

            return result;
        }
        //aspheric
        private List<Vector3> SurfacePoints(Polynom polynom)
        {
            List<Vector3> result = new List<Vector3>();
            double x = 0;
            for (double y = 0; y <= h / 2; y += 0.1)
            {
                result.Add(new Vector3(x, y, polynom.Calc(y)));
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
        public double CalcZ(double r, double x, double y)
        {
            return r - Math.Sign(r) * Math.Sqrt(r * r - x * x - y * y);
        }

        public Line ScreenLine(Core core, double z, TranslateTransform transform = null, Color? color = null, int? thickness = null)
        {
            SolidColorBrush myBrush = new SolidColorBrush();
            myBrush.Color =color?? Colors.DarkOliveGreen;
            Line screen = new Line();
            screen.X1 = z * k;
            screen.X2 = z * k;
            screen.Y1 = -h * k*0.8;
            screen.Y2 = h * k*0.8;
            screen.Stroke = myBrush;
            screen.StrokeThickness = thickness?? 2;
            if (transform != null) screen.RenderTransform = transform;
            return screen;
        }

        public Polyline DrawChart(List<Vector3> points)
        {
            return null;
        }

        public void DrawEiqonial(List<List<Ray>> ans, Core core, ref Canvas canvas1, double t, TranslateTransform transform=null, double dt = 3)
        {            
            double j = 0;
                     
            while (j < 7000)
            {
                Polyline eiqonial = new Polyline();
                eiqonial.StrokeThickness = 1;
                eiqonial.Stroke = Brushes.Green;
                if (transform != null) eiqonial.RenderTransform = transform;
                canvas1.Children.Add(eiqonial);
                j += dt;
                ans.ForEach((ray) =>
                {
                    var p = RayInMoment(core, ray, j);
                    eiqonial.Points.Add(new Point(p.Z*k, p.Y*k));
                });

                canvas1.InvalidateVisual();
                eiqonial.InvalidateVisual();
            }
        }

        public Vector3 RayInMoment(Core core,List<Ray> rays,double dt)
        {
            const double v = 0.1;
            int rayLength = rays.Count;
            double t = 0;
            int i = 0;
            double dv = 0;
            while (t<dt)
            {
                dv = v / core.n[i];                
                var t1 = i== rayLength-1 ? 0 : (rays[i + 1].Center - rays[i].Center).Length / dv;
                if(t1+t<dt && i+1< rayLength)
                {
                    i++;
                    t += t1;
                }
                else
                {
                    break;
                }                
            }
            dt -= t;
            return rays[i].Center + dt * dv * rays[i].Direction;
        }

        public static void DrawScreenPoints(List<Vector3> points, Canvas screenCanvas, double kk = 0, TranslateTransform transform = null, Color? color = null, int? radius = null)
        {

            int dotSize = radius ?? 2;
            kk = kk == 0 ? Draw.k : kk;   
            points.ForEach((vector) =>
            {
                transform = transform ?? new TranslateTransform(0, 0);
                vector *= kk;
                Ellipse currentDot = new Ellipse();
                currentDot.Stroke = new SolidColorBrush(color?? Colors.Green);
                currentDot.StrokeThickness = 1;
                Canvas.SetZIndex(currentDot, 10);
                currentDot.Height = dotSize;
                currentDot.Width = dotSize;
                currentDot.Fill = new SolidColorBrush(Colors.Green);
                if (vector.Y > 0) vector.Y /= 2;
                if (vector.X > 0) vector.X /= 2;
                currentDot.Margin = new System.Windows.Thickness(vector.X, vector.Y, 0, 0);
                currentDot.RenderTransform = transform;
                screenCanvas.Children.Add(currentDot);
            });
        }

        public static void DrawPoint(Vector3 vector, Canvas screenCanvas, double kk = 0, TranslateTransform transform = null, Color? color = null, int? radius = null)
        {
            int dotSize = radius ?? 2;
            kk = kk == 0 ? Draw.k : kk;

            transform = transform ?? new TranslateTransform(0, 0);
            vector *= kk;
            Ellipse currentDot = new Ellipse();
            currentDot.Stroke = new SolidColorBrush(color ?? Colors.Green);
            currentDot.StrokeThickness = 1;
            Canvas.SetZIndex(currentDot, 10);
            currentDot.Height = dotSize;
            currentDot.Width = dotSize;
            currentDot.Fill = new SolidColorBrush(Colors.Green);
            if (vector.Y > 0) vector.Y /= 2;
            if (vector.X > 0) vector.X /= 1;
            currentDot.Margin = new System.Windows.Thickness(vector.X, vector.Y, 0, 0);
            currentDot.RenderTransform = transform;
            screenCanvas.Children.Add(currentDot);
        }

        public static void addText(double x, double y, string text, Color color, Canvas canvasObj)
        {
            TextBlock textBlock = new TextBlock();            
            textBlock.Foreground = new SolidColorBrush(color);
            textBlock.Text = text;
            Canvas.SetLeft(textBlock, x);
            Canvas.SetTop(textBlock, y);
            canvasObj.Children.Add(textBlock);
        }
    }
}
