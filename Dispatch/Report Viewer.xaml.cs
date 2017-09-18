using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Dispatch
{
    /// <summary>
    /// Interaction logic for Report_Viewer.xaml
    /// </summary>
    public partial class Report_Viewer : Window
    {
        public Report_Viewer()
        {
            InitializeComponent();
        }

        public Call Cal { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 =
                new Microsoft.Reporting.WinForms.ReportDataSource
                {
                    Name = "DatasetCall",
                    Value = new List<Call> {Cal }
                };

            RV.LocalReport.DataSources.Add(reportDataSource1);
            RV.LocalReport.ReportEmbeddedResource = "Dispatch.SingleCallReport.rdlc";
            RV.RefreshReport();
        }
    }
}
