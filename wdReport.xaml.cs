using Aspose.Cells;
using CaptureWebcam.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
using Aspose.Cells;
using Microsoft.Win32;

namespace CaptureWebcam
{
    /// <summary>
    /// Interaction logic for wdReport.xaml
    /// </summary>
    public partial class wdReport : Window
    {
        public wdReport()
        {
            InitializeComponent();
        }
        DataSet _dtReport = new DataSet();
        BLDatabase oBL = new BLDatabase();

       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _dtReport = oBL.GetHistoryForReport();
        }

        private void btnSendReport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tính năng đang phát triển", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
