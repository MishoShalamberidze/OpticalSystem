using OpticCore;
using OpticCore.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public PlotData plotData;
        public Window2(PlotData plotData)
        {
            this.plotData = plotData;            
            InitializeComponent();
            string curDir = Directory.GetCurrentDirectory();
            this.WebView1.Source = new Uri($"file:///{curDir}/html/index.html");
            this.WebView1.LoadCompleted += WebView1_LoadCompleted;
        }

        private void WebView1_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            var json = new JavaScriptSerializer().Serialize(plotData);
            this.WebView1.InvokeScript("drawPlot", json);
        }

    }
}
