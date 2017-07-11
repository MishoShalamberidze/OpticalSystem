using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using OpticalSystem.Model;

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



        public List<Ray> Calc(Ray ray)
        {
            List<Ray> listRay = new List<Ray> { ray };
            for (int i = 0; i < m; i++)
            {
                double n12 = n[i] / n[i + 1];
                ray = i == Ia ? Asferika(n12, ray, AsfreikPolynom1,r[Ia]) : Sfero(n12, r[i], ray);
                ray.Center.Z = ray.Center.Z - d[i];
                listRay.Add(ray);
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
            double r0 = Math.Sqrt(Math.Pow(resp.Center.X,2)+ Math.Pow(resp.Center.Y, 2));
            double u1 = polynom.Diff(r0);
            double u2 = Math.Sqrt(u1 * u1 + 1);

            Vector3 N = u1 * resp.Center / r0 / u2;
            N.Z = -1 / u2;

            u1 = ray.Direction * N * n12;
            double l = -u1 - Math.Sqrt(u1 * u1 + 1 - n12 * n12);
            resp.Direction = n12 * ray.Direction + l * N;
            return resp;

            //surface function aracxadi saxit
            double Fa(Vector3 a)
            {
                double tr = Math.Sqrt(a.X * a.X + a.Y * a.Y);
                return a.Z - polynom.Calc(tr);
            }


        }
 
    }


    
}
