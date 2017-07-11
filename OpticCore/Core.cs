using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using OpticalSystem.Model;
using System.Linq;

namespace OpticalSystem
{
    public class Core
    {
        public int m;
        //radius
        public double[] r;

        //distance between surface
        public double[] d;

        // gardatexis machvenebeli 
        public double[] n;

        //asferic type index
        public int Ia;

        public Polynom AsfreikPolynom1;

        public double screenZ = 20;



        public List<Ray> Calc(Ray ray)
        {
            List<Ray> listRay = new List<Ray> { ray };
            double distance = 0;
            for (int i = 0; i < m; i++)
            {
                distance += d[i];
                double n12 = n[i] / n[i + 1];
                ray = i == Ia ? Asferika(n12, ray, AsfreikPolynom1, r[Ia]) : Sfero(n12, r[i], ray);
                ray.Center.Z = ray.Center.Z - d[i];
                var tempRay = new Ray(ray.Center, ray.Direction);
                tempRay.Center.Z += distance;
                listRay.Add(tempRay);
            }
            return listRay;
        }



        Ray Sfero(double n12, double r0, Ray ray)
        {
            double t1 = -ray.Direction.X * ray.Center.X - ray.Direction.Y * ray.Center.Y + ray.Direction.Z * (r0 - ray.Center.Z);
            double t2 = Math.Pow(ray.Center.X, 2) + Math.Pow(ray.Center.Y, 2) + Math.Pow(r0 - ray.Center.Z, 2);

            double t = t1 - Math.Sign(r0) * Math.Sqrt(t1 * t1 - t2 + r0 * r0);

            Ray resp = new Ray();
            resp.Center = ray.Center + t * ray.Direction;

            Vector3 N = resp.Center / r0;
            N.Z--;

            double u1 = ray.Direction * N * n12;

            double l = -u1 - Math.Sqrt(u1 * u1 + 1 - n12 * n12);
            resp.Direction = n12 * ray.Direction + l * N;

            return resp;
        }

        Ray Asferika(double n12, Ray ray, Polynom polynom, double ra)
        {
            double eps = 0.01;
            double dh = 0.01;
            double t0 = -ray.Center.Z / ray.Direction.Z;
            double t1 = 0;
            double u = 1;

            while (Math.Abs(u) > eps)
            {
                double q1 = Fa(ray.Center + t0 * ray.Direction);
                double tt = t0 + dh;
                double q2 = Fa(ray.Center + tt * ray.Direction);
                u = q1 * dh / (q2 - q1);
                t1 = t0 - u;
                t0 = t1;
            }

            Ray resp = new Ray();
            resp.Center = ray.Center + t1 * ray.Direction;
            double r0 = Math.Sqrt(Math.Pow(resp.Center.X, 2) + Math.Pow(resp.Center.Y, 2));
            double u1 = polynom.Diff(r0);
            double u2 = Math.Sqrt(u1 * u1 + 1);

            Vector3 N = u1 * resp.Center / r0 / u2;
            N.Z = -1 / u2;

            u1 = ray.Direction * N * n12;
            double l = -u1 - Math.Sqrt(u1 * u1 + 1 - n12 * n12);
            resp.Direction = n12 * ray.Direction + l * N;
            return resp;

            //surface implicit function
            double Fa(Vector3 a)
            {
                double tr = Math.Sqrt(a.X * a.X + a.Y * a.Y);
                return a.Z - polynom.Calc(tr);
            }

        }

        public Polynom AsfericCoeficients(double r1, double r2, double n, double d, double h, double distance,int pointQuantity, ref List<Vector3> asfericPoints)
        {
            const int len = 100;
            double[] x = new double[len];
            double[] y = new double[len];
            double[] z = new double[len];
            double[] c = new double[4];
            double r = r1;
            r2 *= -1;
            double f = 1 / ((n - 1) * (1 / r1 + 1 / r2) - Math.Pow(n - 1, 2) * d / (n * r1 * r2));
            double h1 = f * d * (n - 1) / n / r2;
            double h2 = f * d * (n - 1) / n / r1;
            double s1 = distance;
            double s2 = 1 / (1 / f - 1 / (s1 + h1)) - h2;
            double yMax = h;
                                   
            double wMax = FS(yMax / r1);
            double w0 = .01;
            double dw = (wMax - w0) / pointQuantity;

            int index = 0;
            
            for (double w = w0; w <= wMax+dw; w += dw)
            {
                index++;
                double xt, zt;
                Asfer(w, out zt, out xt);
                asfericPoints.Add(new Vector3(zt, xt,0));
                x[index] = xt;
                z[index] = zt;
                var tr = Math.Sqrt(xt * xt + zt * zt);
            }
            double t8 = 0, t10 = 0, t12 = 0, t14 = 0, t16 = 0;
            double[] v = new double[4];            
            for (int i = 0; i <= index; i++)
            {
                y[i] = z[i] - Math.Pow(x[i], 2) / r2 / 2;
                double u = x[i];
                t8 +=  Math.Pow(u, 8);
                t10 += Math.Pow(u, 10);
                t12 += Math.Pow(u, 12);
                t14 += Math.Pow(u, 14);
                t16 += Math.Pow(u, 16);

                v[1] += y[i] * Math.Pow(u, 4);
                v[2] += y[i] * Math.Pow(u, 6);
                v[3] += y[i] * Math.Pow(u, 8);
            }            
            double[,] a1 = new double[3, 3] { 
                { t8,t10,t12},
                { t10,t12,t14},
                { t12,t14,t16}
            };

            double[] b = new double[5];
            Sys(a1, v,out b);
            b[0] = 1 / r2 / 2;
            Polynom pol1 = new Polynom(-b[0], 2, -b[1], 4,-b[2], 6, -b[3], 8);
            return pol1;

            double Pol(double X)
            {
                return c[0] * Math.Pow(X, 2) + c[1] * Math.Pow(X, 4) + c[2] * Math.Pow(X, 6) + c[3] * Math.Pow(X, 8);
            }
            double FS(double X)
            {
                return Math.Atan(X / Math.Sqrt(1 - X * X));
            }

            void Asfer(double w, out double Z, out double X)
            {
                var rrr = s1;
                double fi = Math.Atan(Math.Sin(w) / (s1 / r + 1 - Math.Cos(w)));
                double t1 = (s1 + r) * Math.Sin(fi) / n / r;
                double al = FS(t1) - w;
                double l = s1 + s2 + n * d;
                double z1 = d - r * (1 - Math.Cos(w));
                double k = -Math.Tan(al);
                double k1 = n / Math.Cos(al);
                double b1 = l - r * Math.Sin(w) / Math.Sin(fi) - z1 * n / Math.Cos(al);
                double B = r * Math.Sin(w) - k * z1;
                double v1 = k * k - k1 * k1 + 1;
                double v2 = k * B - k1 * b1 + s2;
                double v3 = s2 * s2 - b1 * b1 + B * B;
                double disk = v2 * v2 - v1 * v3;
                Z = (-v2 - Math.Sqrt(disk)) / v1;
                X = k * Z + B;
            }

            void Sys(double[,] g, double[] H, out double[] V)
            {                
                double d0 = g[0, 0] * (g[1, 1] * g[2, 2] - g[1, 2] * g[2, 1]) - g[0, 1] * (g[1, 0] * g[2, 2] - g[1, 2] * g[2, 0]) + g[0, 2] * (g[1, 0] * g[2, 1] - g[1, 1] * g[2, 0]);
                double d1 = H[1] * (g[1, 1] * g[2, 2] - g[1, 2] * g[2, 1]) - g[0, 1] * (H[2] * g[2, 2] - g[1, 2] * H[3]) + g[0, 2] * (H[2] * g[2, 1] - g[1, 1] * H[3]);
                double d2 = g[0, 0] * (H[2] * g[2, 2] - g[1, 2] * H[3]) - H[1] * (g[1, 0] * g[2, 2] - g[1, 2] * g[2, 0]) + g[0, 2] * (g[1, 0] * H[3] - H[2] * g[2, 0]);
                double d3 = g[0, 0] * (g[1, 1] * H[3] - H[2] * g[2, 1]) - g[0, 1] * (g[1, 0] * H[3] - H[2] * g[2, 0]) + H[1] * (g[1, 0] * g[2, 1] - g[1, 1] * g[2, 0]);
                V = new double[4];
                V[1] = d1 / d0;
                V[2] = d2 / d0;
                V[3] = d3 / d0;
            }


        }


        public double[] GetKardinals(out double[] f)
        {
            double[] hh = new double[2];
            Reverse();
            f = new double[2];
            hh[0] = H2(out f[0]);
            Reverse();
            hh[1] = H2(out f[1]);
            return hh;
        }
        public double H2(out double f)
        {            
            Ray ray1 = new Ray(0, 0.01, -20, 0, 0.01, 0);
            ray1.Direction = (ray1.Direction - ray1.Center).Normal;
            double H2;
            double sumD = d.Sum();
            var ans = Calc(ray1);
            var lastRay = ans.LastOrDefault();
            var focus = lastRay.Center - lastRay.Center.Y / lastRay.Direction.Normal.Y * lastRay.Direction.Normal;
            double foucsZ = focus.Z - sumD;
            double tgAlfa = lastRay.Center.Y / foucsZ;
            H2 = foucsZ - ray1.Center.Y / tgAlfa;
            f = foucsZ - H2;
            return H2;
        }

        public double getFs(bool isAbsoulte = false)
        {
            Ray ray1 = new Ray(0, .02, -20, 0, .02, 0);
            ray1.Direction = (ray1.Direction - ray1.Center).Normal;
            var ans = Calc(ray1);
            var lastRay = ans.LastOrDefault();
            var focus = lastRay.Center - lastRay.Center.Y / lastRay.Direction.Normal.Y * lastRay.Direction.Normal;
            double focusZ = focus.Z;
            if (isAbsoulte) focusZ -= this.d.Sum();
            return focusZ;
        }

        public void Reverse()
        {
            double[] r1 = new double[this.r.Length];
            for (int i = 0; i < this.r.Length; i++)
                r1[i] = -this.r[r.Length - i - 1];
            this.r = r1;

            double[] n1 = new double[this.n.Length];
            for (int i = 0; i < this.n.Length; i++)
                n1[i] = this.n[n.Length - i - 1];
            this.n = n1;

            double[] d1 = new double[this.d.Length];
            for (int i = 0; i < this.d.Length - 1; i++)
                d1[i] = this.d[this.d.Length - 2 - i];
            d1[this.d.Length - 1] = 0;
            this.d = d1;
        }

    }



}
