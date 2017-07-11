
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpticalSystem;
using OpticalSystem.Model;
using System.Diagnostics;
using System.Windows.Threading;
using OpticCore.Model;
using OpticCore;

//using OpticCore;
namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Random rnd = new Random();
        public Core core;
        public Draw drawHandler;
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        public TranslateTransform transform;
        Dictionary<double, Brush> brushes = new Dictionary<double, Brush>();

        List<Ray> lastRays = new List<Ray>();
        List<List<Ray>> answerRays = new List<List<Ray>>();


        List<Vector3> asfericPoints = new List<Vector3>();

        double[] kardinalH = new double[2];
        double[] kardinalF = new double[2];

        int drawRayIndex = -14;

        double screenZ;
        double startPointZ;


        public MainWindow()
        {

            InitializeComponent();
            InitialLensSystem();
            InitialOneLens();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0,1,200);
            Canvas1.MouseMove += Canvas1_MouseMove;
        }

        private void Canvas1_MouseMove(object sender, MouseEventArgs e)
        {
            var x = e.MouseDevice.GetPosition(Canvas1);
            Label1.Content = $"{x.X / Draw.k};   {x.Y / Draw.k}";
        }

        public void InitialLensSystem()
        {
            core = new Core();
            //draw objective mine
            /*
            core.Ia = 15;
            core.d = new double[] { 4.8, 1, 7.86, 13.5, 6.52, 0 };
            core.r = new double[] { 38.4, 105, 17.96, 13.4, 30.14, -41.8 };
            core.m = 6;
            const double n1 = 1.66;
            const double n2 = 1.4925;
            const double n3 = 1.599;            
            brushes.Add(n1, new SolidColorBrush(Color.FromArgb(30, 102, 160, 255)));
            brushes.Add(n2, new SolidColorBrush(Color.FromArgb(65, 102, 160, 255)));
            brushes.Add(n3, new SolidColorBrush(Color.FromArgb(100, 102, 160, 255)));
            core.n = new double[] { 1, n1, 1, n2, 1, n3, 1 };
            drawHandler = new Draw(25, 10);
            transform = new TranslateTransform(600, 0);
            */

            //draw erfle_objective

            //core.Ia = 14;
            //core.d = new double[] { 1.7, 15, 0.25, 7.60, 0.25, 13.8, 1.8, 0 };
            //core.r = new double[] { -56.01, 31.89, -31.89, 70.78, -70.78, 29.41, -34.42, -170.23 };
            //core.m = 8;
            //const double n1 = 1.6199;
            //const double n2 = 1.5163;             
            //brushes.Add(n1, new SolidColorBrush(Color.FromArgb(50, 102, 160, 255)));
            //brushes.Add(n2, new SolidColorBrush(Color.FromArgb(100, 102, 160, 255)));

            //transform = new TranslateTransform(500,2);

            //core.n = new double[] { 1, n1, n2, 1, n2, 1, n2, n1, 1 };

            //drawHandler = new Draw(33, 12);



            core.Ia = 1;
            core.d = new double[] { 2, 0 };
            core.r = new double[] { 10, -10, };
            core.m = 2;
            const double n1 = 1.6199;
            brushes = new Dictionary<double, Brush>();
            brushes.Add(n1, new SolidColorBrush(Color.FromArgb(50, 102, 160, 255)));
            transform = new TranslateTransform(850, 0);
            core.n = new double[] { 1, n1, 1 };
            drawHandler = new Draw(8, 40);

        }

        public void InitialOneLens(double d1 = 0)
        {

            bool drawKardinalPlane = false;
            bool drawEiconials = false;
            bool drawMainRays = false;
            bool drawRandomCircle = false;
            bool drawLens = true;
            bool drawFocusLine = false;
            bool drawScreenLine = false;
            bool drawSimpleRays = true;
            bool drawAnimationAsfericCalc = false;
            bool drawSquare = false;


            if (core.Ia <= core.m - 1)
            {
                core.AsfreikPolynom1 = core.AsfericCoeficients(core.r[0], core.r[1], core.n[1], core.d[0], Draw.h / 2, 20, 15, ref asfericPoints);
            }

            #region kardianl point

            double[] f = new double[2];
            var kardianls = core.GetKardinals(out f);
            kardinalH = kardianls;
            kardinalF = f;
            //Draw.addText(transform.X - kardianls[0] * Draw.k + 5, 200, "H1", Colors.Blue, Canvas1);
            //Draw.addText(transform.X + (-kardianls[1] + core.d.Sum()) * Draw.k, 200, "H2", Colors.Blue, Canvas1);
            Draw.addText(100, 5, "Optical line", Colors.Black, Canvas1);
            //Draw.addText(10, 5, "Start", Colors.Black, Canvas1);

            if (drawKardinalPlane)
            {
                var h1Line = drawHandler.ScreenLine(core, -kardianls[0], transform, Colors.Black, 1);
                h1Line.RenderTransform = transform;
                Canvas1.Children.Add(h1Line);
                var h2Line = drawHandler.ScreenLine(core, core.d.Sum() + kardianls[1], transform, Colors.Black, 1);
                h1Line.RenderTransform = transform;
                Canvas1.Children.Add(h2Line);
            }
            double focus = core.getFs(false);
            startPointZ = -20;
            //Draw.addText(20*Draw.k+transform.X,2 , "focus line", Colors.Black, Canvas1);            
            //found fouces screen by kardinal
            screenZ = kardianls[1] + 1 / (1 / f[1] + 1 / (startPointZ + kardianls[0])) + core.d.Sum();
            Print();
            #endregion kardinal point            
            // simple rays
            Dictionary<double, double> focuses = new Dictionary<double, double>();
            Dictionary<double, double> screens = new Dictionary<double, double>();
            if (drawSimpleRays || drawEiconials)
            {
                for (double h = -Draw.h/2+0.2; h <= Draw.h /2-0.2; h += 0.3)
                {
                    Ray ray2 = new Ray(0, h+3, startPointZ, 0, h+0.35, 0);
                    ray2.Direction = (ray2.Direction - ray2.Center).Normal;
                    var ans = GetAns(core, ray2);
                    answerRays.Add(ans);
                    lastRays.Add(ans.LastOrDefault());
                    var focust = ans[2].Center - ans[2].Center.Y / ans[2].Direction.Normal.Y * ans[2].Direction.Normal;
                    var y = ans[2].Center.Y + (20 - ans[2].Center.Z) / ans[2].Direction.Normal.Z * ans[2].Direction.Normal.Y;
                   // screens.Add(ans[2].Center.Z, y);
                    focust.Z -= core.d[0];
                    focuses.Add(ans[2].Center.Y, focust.Z);
                    if (drawSimpleRays)
                    {
                        var pol1 = drawHandler.DrawRay(ans);
                        pol1.Stroke = Brushes.Red;
                        pol1.RenderTransform = transform;
                        // surface.Fill = Brushes.LightBlue;
                        pol1.StrokeThickness = 1;
                        Canvas1.Children.Add(pol1);
                    }
                }
                var ttt = focuses;
                var sss = screens;
                // write output in file
                //foreach (var foc in focuses)
                //   Debug.WriteLine(foc);
                List<double> screenPointsY = new List<double>();
                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"D:\WriteLines2.txt"))
                {
                    foreach (KeyValuePair<double, double> foc in focuses)
                        file.WriteLine($"{foc.Key}; {foc.Value}");
                }

            }

            // draw random circle
            if (drawRandomCircle)
            {

                List<Vector3> screens2 = new List<Vector3>();
                var startPoint = new Vector3(0, 0, -25);
                for (int i = 0; i < 5000; i++)
                {
                    Ray ray2 = new Ray(startPoint, GetRandom(4));
                    ray2.Direction = (ray2.Direction - ray2.Center).Normal;

                    var ans = GetAns(core, ray2);
                    answerRays.Add(ans);
                    lastRays.Add(ans.LastOrDefault());
                    //var focus = ans[2].Center - ans[2].Center.Y / ans[2].Direction.Normal.Y * ans[2].Direction.Normal;
                    //focus.Z -= core.d[0];
                    //focuses.Add(ans[2].Center.Y, focus.Z);
                    var screenPoint = ans[2].Center + (25 - ans[2].Center.Z) / ans[2].Direction.Normal.Z * ans[2].Direction.Normal;
                    screens2.Add(screenPoint);                    
                    var pol = drawHandler.DrawRay(ans);
                    pol.Stroke = Brushes.Red;
                    // surface.Fill = Brushes.LightBlue;
                    pol.StrokeThickness = 1;
                    pol.RenderTransform = transform;
                    Canvas1.Children.Add(pol);
                }

                Draw.DrawScreenPoints(screens2, Canvas1, 150, new TranslateTransform(600, 0));
            }

            void Print()
            {
                int fontSize = 20;
                string r = "R = ";
                foreach(var R in core.r)
                {
                    r += $"{R}; ";
                }
                labelRadius.Content = r;
                labelRadius.FontSize = fontSize;

                string n = "n = ";
                foreach (var N in core.n)
                {
                    if (N == 1) continue;
                    n += $"{N}; ";
                }
                labelN.Content = n;
                labelN.FontSize = fontSize;

                string d = "d = ";
                foreach (var D in core.d)
                {
                    if (D == 0) continue;
                    d += $"{D}; ";
                }
                labelDimension.Content = d;
                labelDimension.FontSize = fontSize;

                labelFocus.Content = $"focus = {focus}";
                labelFocus.FontSize = fontSize;

                if (core.AsfreikPolynom1!= null)
                {
                    string polin = "Asferic Koef: ";
                    int index = 0;
                    foreach(var item in core.AsfreikPolynom1.Members)
                    {
                        index++;
                        polin += $"C{index} = {item.k}; "; 
                    }
                    labelPolynom.Content = polin;
                    labelPolynom.FontSize = fontSize;
                }

                string kardinal = "";
                var h1 = String.Format("{0:0.##}", -kardinalH[0]);
                var h2 = String.Format("{0:0.##}", -kardinalH[1]);
                var f1 = String.Format("{0:0.##}", kardinalF[0]);
                var f2 = String.Format("{0:0.##}", kardinalF[1]);
                kardinal = $"H1 = {h1}; H2 = {h2} ; F1 = {f1}; F2 = {f2};";
                labelKardinal.Content = kardinal;
                labelKardinal.FontSize = fontSize;

            }

            if (drawMainRays)
            {
                var crossedPoints = new List<Vector3>();
                var screenPoints = new List<Vector3>();
                var startpoints = new List<Vector3>();
                double dh = .023;
                int i = -1;
                for (double objH = -1; objH < -0.1; objH += 0.11)
                {
                    i++;
                    var startPoint = new Vector3(0, objH, startPointZ);
                    startpoints.Add(new Vector3(startPointZ, objH, 0));
                    double dzMin = 50;
                    double screenY = 0;
                    for (double h = -Draw.h / 4 + dh; h < Draw.h / 4; h += 0.33)
                    {
                        var direction1 = (new Vector3(0, h, 0)) - startPoint;
                        direction1.Normalize();
                        var ray1 = new Ray(startPoint, direction1);
                        var ans1 = core.Calc(ray1);
                        var direction2 = (new Vector3(0, h + dh, 0)) - startPoint;
                        direction2.Normalize();
                        var ray2 = new Ray(startPoint, direction2);
                        var ans2 = core.Calc(ray2);
                        var croosP = Ray.CroosPoint(ans1.LastOrDefault(), ans2.LastOrDefault());
                        var ans2Last = ans2.LastOrDefault();
                        if (Math.Abs(croosP.Z - startPointZ) < dzMin)
                        {
                            dzMin = Math.Abs(croosP.Z - startPointZ);
                            screenY = ans2Last.Center.Y + (screenZ - ans2Last.Center.Z) / ans2Last.Direction.Normal.Z * ans2Last.Direction.Normal.Y;
                        }
                        crossedPoints.Add(croosP);

                        var pol2 = drawHandler.DrawRay(ans1);
                        pol2.Stroke = Brushes.Red;
                        pol2.StrokeThickness = 1;
                        pol2.RenderTransform = transform;
                        Canvas1.Children.Add(pol2);
                    }

                    screenPoints.Add(new Vector3(screenZ, screenY));
                }

                var rayT = new Ray(0, 0, startPointZ, 0, 0.2, 0);
                rayT.Direction = (rayT.Direction - rayT.Center).Normal;
                var pol = drawHandler.DrawRay(core.Calc(rayT));
                pol.Stroke = Brushes.Red;
                pol.StrokeThickness = 1;
                pol.RenderTransform = transform;
                Canvas1.Children.Add(pol);

                Draw.DrawScreenPoints(screenPoints, Canvas1, 0, transform);
                Draw.DrawScreenPoints(startpoints, Canvas1, 0, transform);


                var t = crossedPoints.Select(v => new Vector3(v.Z, v.Y, 0)).ToList();
                Draw.DrawScreenPoints(t, Canvas1, 0, transform);
            }


            if (drawFocusLine)
            {
                Canvas1.Children.Add(drawHandler.ScreenLine(core, focus, transform));
                Canvas1.Children.Add(drawHandler.ScreenLine(core, Math.Abs(kardinalH[0]) - kardinalF[0], transform));
            }

            if (drawScreenLine)
            {
                Canvas1.Children.Add(drawHandler.ScreenLine(core, core.getFs(), transform, Colors.Black, 1));
            }

            if (drawLens) DrawLens();

            if (drawEiconials)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    drawHandler.DrawEiqonial(answerRays, core, ref Canvas1, 0, transform, 5);
                }));
            }
            if (drawAnimationAsfericCalc)
            {
                DrawSferoSurface1();
                dispatcherTimer.Start();
            }
            if (drawSquare)
            {
                drawSquarePoint();
            }

        }


        public void DrawLens()
        {
            List<Polyline> surfaces = drawHandler.DrawSystemLens(core, brushes);


            foreach (var surface in surfaces)
            {
                surface.Stroke = Brushes.Black;
                //surface.Fill = Brushes.LightBlue;
                surface.StrokeThickness = 1;
                surface.RenderTransform = transform;
                Canvas1.Children.Add(surface);
            }

        }


        public void DrawSferoSurface1()
        {
            List<Vector3> polynomPoints = new List<Vector3>();
            //draw first sfero surface
            for (double y = -Draw.h / 2; y < Draw.h / 2; y += .01)
            {
                double x = drawHandler.CalcZ(core.r[0], y, 0);
                polynomPoints.Add(new Vector3(x*2, y * 2, 0));
            }
        }
        public void DrawSferoSurface2()
        {
            List<Vector3> polynomPoints = new List<Vector3>();
            //draw first sfero surface
            for (double y = -Draw.h / 2; y < Draw.h / 2; y += .01)
            {
                double x = drawHandler.CalcZ(core.r[0], y, 0);
                polynomPoints.Add(new Vector3(2*core.d[0]-x * 2, y * 2, 0));
            }
            Draw.DrawScreenPoints(polynomPoints, Canvas1, Draw.k, transform, Colors.Black, 2);

        }

        public void DrawAsfericSurface()
        {
            List<Vector3> polynomPoints = new List<Vector3>();
            for (double y = -Draw.h *2.5; y < Draw.h*2.5 ; y += .05)
            {
                polynomPoints.Add(new Vector3((core.AsfreikPolynom1.Calc(y) + core.d[0])*2, y * 2, 0));
            }
            Draw.DrawScreenPoints(polynomPoints, Canvas1, Draw.k, transform, Colors.Black, 2);
        }


        public void DrawByIndex(int i)
        {
            Ray ray2 = new Ray();
            ray2.Center = new Vector3(0, 0, -20);
            double y = -Draw.h / 30 * i;
            var directionPoint = new Vector3(0, y, drawHandler.CalcZ(core.r[0], y, 0));
            ray2.Direction = (directionPoint - ray2.Center).Normal;
            var ans = GetAns(core, ray2);
            var pol1 = drawHandler.DrawRay(ans);
            pol1.Stroke = Brushes.Red;
            pol1.RenderTransform = transform;
            pol1.StrokeThickness = 1;
            Canvas1.Children.Add(pol1);
            Draw.DrawPoint(new Vector3(ans[2].Center.Z, ans[2].Center.Y * 2, 0), Canvas1, Draw.k, transform, Colors.Black, 5);

        }

        private List<Vector3> GetSquareStartPoints()
        {
            double a = 4;
            double b = 4;
            double n = 10;
            double dx = 0.03;
            double z = -30;



            List<Vector3> startPoints = new List<Vector3>();
            for(double y = -a/2; y<=a/2; y += a / n)
            {
                for(double x= -b/2; x<=b/2; x += dx)
                {
                    startPoints.Add(new Vector3(x, y, z));
                    startPoints.Add(new Vector3(y, x, z));
                }
            }
            return startPoints;
        }

        public void drawSquarePoint()
        {
            var startPoints = GetSquareStartPoints();
            
            List<Vector3> screenPoints = new List<Vector3>();
            List<double> alpaList = new List<double>();
            /*focus*/
            Ray ray1 = new Ray(0, 0, -30, 0, .02, 0);
            ray1.Direction = (ray1.Direction - ray1.Center).Normal;
            var ans2 = core.Calc(ray1);
            var lastRay2 = ans2.LastOrDefault();
            var focus = lastRay2.Center - lastRay2.Center.Y / lastRay2.Direction.Normal.Y * lastRay2.Direction.Normal;
            double focusZ =  focus.Z;
            /*end focus*/

            
            double screenZ = focusZ;
            foreach (Vector3 startPoint in startPoints)
            {
                double tempZ = startPoint.Z;
                startPoint.Z = 0;
                double length = startPoint.Length;
                double alpa =  Math.Atan(startPoint.X / startPoint.Y);
                if (startPoint.X < 0) alpa += Math.PI;
                alpaList.Add(alpa);
                Ray ray = new Ray(0,length,tempZ,0,length,0);
                ray.Direction -= ray.Center;
                ray.Direction.Normalize();


                var ans = core.Calc(ray);
                var lastRay = ans.LastOrDefault();
                var y = lastRay.Center.Y + (screenZ - lastRay.Center.Z) / lastRay.Direction.Normal.Z * lastRay.Direction.Normal.Y;
                var screenPoint = new Vector3(Math.Cos(alpa) * y, Math.Sin(alpa) * y, screenZ);
                screenPoints.Add(screenPoint);
            }
            var t1 = alpaList.Min();
            var t2 = alpaList.Max();
            var t = screenPoints;
            Draw.DrawScreenPoints(screenPoints, Canvas1, 250, transform, Colors.Black, 2);
        }



        public void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            DrawByIndex(++drawRayIndex);
            if (drawRayIndex > 14)
            {
                DrawAsfericSurface();
                dispatcherTimer.Stop();
                string polin = "Asferic Koef: ";
                int index = 0;
                foreach (var item in core.AsfreikPolynom1.Members)
                {
                    index++;
                    polin += $"C{index} = {item.k}; ";
                }
                labelPolynom.Content = polin;
                labelPolynom.FontSize = 20;

                string kardinal = "";
                var h1 = String.Format("{0:0.##}", -kardinalH[0]);
                var h2 = String.Format("{0:0.##}", -kardinalH[1]);
                var f1 = String.Format("{0:0.##}", kardinalF[0]);
                var f2 = String.Format("{0:0.##}", kardinalF[1]);
                kardinal = $"H1 = {h1}; H2 = {h2} ; F1 = {f1}; F2 = {f2};";
                labelKardinal.Content = kardinal;
                labelKardinal.FontSize = 20;
            }
        }
        private void DrawGrap()
        {
            SolidColorBrush myBrush = new SolidColorBrush();
            myBrush.Color = Colors.Black;
            Line centerLine = new Line();
            centerLine.Stroke = myBrush;
            centerLine.Width = Border1.Width;
            centerLine.StrokeThickness = 1;
            centerLine.X1 = -Border1.Width / 2;
            centerLine.Y1 = 0;
            centerLine.X2 = Border1.Width / 4;
            centerLine.Y2 = 0;
            Canvas1.Children.Add(centerLine);
        }

        private static Vector3 GetRandom(double radius)
        {
            double fi = rnd.NextDouble() * 2 * Math.PI;
            radius *= rnd.NextDouble();
            double x = Math.Cos(fi) * radius;
            double y = Math.Sin(fi) * radius;
            return new Vector3(x, y, 0);
        }

        private static List<Ray> GetAns(Core core, Ray ray2)
        {
            return core.Calc(ray2);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            List<Vector3> vl = new List<Vector3>();
            for(int i=0; i < 500; i++)
            {
                vl.Add(new Vector3(0, rnd.NextDouble()*10-5, i));
            }

            var plot = new PlotData(vl, "misho");
            Window2 win2 = new Window2(plot);
            win2.Show();
            ////polyline.RenderTransform = new TranslateTransform(200, 200);
            //Canvas1.Children.Clear();
            //tempd1 += 0.1;
            //InitialOneLens(tempd1);
        }

    }
}
