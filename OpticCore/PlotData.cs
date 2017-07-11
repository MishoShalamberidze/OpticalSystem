using OpticalSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticCore
{
    public class PlotData
    {
        public double[] x;
        public double[] y;
        public string title;
        public string type = "scatter";

        public PlotData(List<Vector3> vectorList, string title = "title")
        {
            this.x = vectorList.Select(v => v.Z).ToArray();
            this.y = vectorList.Select(v => v.Y).ToArray();
            this.title = title;
        }
    }
}
