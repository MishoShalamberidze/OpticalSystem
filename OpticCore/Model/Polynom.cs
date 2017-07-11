using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpticalSystem.Model
{
    public class Polynom
    {
        public List<PolynomItem> Members;

        public Polynom() {}

        public Polynom(params PolynomItem[] items)
        {
            this.Members.AddRange(items);
        }

        public Polynom(params double[] kp)
        {
            this.Members = new List<PolynomItem>();
            if(kp.Length % 2 != 0) throw new Exception("polynom initial error(even array), please enter k1,p1,k2,p2 ...");
            for (int i = 0; i < kp.Length; i+=2)
            {
                Members.Add(new PolynomItem(kp[i],kp[i+1]));
            }
        }
        
        public double Calc(double x)
        {
            return Members.Sum(member => member.k * Math.Pow(x, member.p));
        }

        public double Diff(double x)
        {
            //var h = 0.1;
            //var s2 = (Calc(x + h) - Calc(x-h)) / 2/h;            
            return Members.Sum(member => member.k * member.p * Math.Pow(x, member.p - 1));
        }

        public class PolynomItem
        {
            public double k;
            public double p;

            public PolynomItem(double k, double p)
            {
                this.k = k;
                this.p = p;
            }
        }
    }

    
}
