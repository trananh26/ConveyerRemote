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
using LiveCharts;

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

            ShowPieReport();

            ShowLineReport();

        }

        /// <summary>
        /// Hiển thị biểu đồ dạng line
        /// </summary>
        private void ShowLineReport()
        {
            uc_ReportByTimeHeight.srTall.Title = "Cao";
            uc_ReportByTimeHeight.srTall.Values = new ChartValues<double> { 10, 5, 10, 8, 2, 2 };

            uc_ReportByTimeHeight.srMid.Title = "Trung bình";
            uc_ReportByTimeHeight.srMid.Values = new ChartValues<double> { 2, 6, 10, 11, 1, 0 };

            uc_ReportByTimeHeight.srShort.Title = "Thấp";
            uc_ReportByTimeHeight.srShort.Values = new ChartValues<double> { 4, 2, 1, 2, 3, 8 };

            uc_ReportByTimeHeight.srOther.Title = "Không có thông tin";
            uc_ReportByTimeHeight.srOther.Values = new ChartValues<double> { 6, 5, 10, 9, 3, 8 };

            uc_ReportByTimeHeight.Label.Labels = new[] {(DateTime.Now.Hour - 5).ToString(), (DateTime.Now.Hour - 4).ToString(), (DateTime.Now.Hour - 3).ToString(),
                (DateTime.Now.Hour-2).ToString(),(DateTime.Now.Hour-1).ToString(),DateTime.Now.Hour.ToString() };


            //Dữ liệu vận hành trong tuần
            uc_ReportByTimeMaterial.srTall.Title = "GALAXY Z FOLD 5";
            uc_ReportByTimeMaterial.srTall.Values = new ChartValues<double> { 20, 31, 36, 30, 38, 6 };// double.Parse(dtReport.Rows[0]["Complete"].ToString()) };

            uc_ReportByTimeMaterial.srMid.Title = "IPHONE 15 PROMAX";
            uc_ReportByTimeMaterial.srMid.Values = new ChartValues<double> { 3, 10, 9, 12, 16, 2 };// double.Parse(dtReport.Rows[0]["Cancel"].ToString()) };

            uc_ReportByTimeMaterial.srShort.Title = "SAMSUNG S24 ULTRA";
            uc_ReportByTimeMaterial.srShort.Values = new ChartValues<double> { 6, 5, 10, 15, 8, 9 };// double.Parse(dtReport.Rows[0]["Transfering"].ToString()) };

            uc_ReportByTimeMaterial.srOther.Title = "Không có thông tin";
            uc_ReportByTimeMaterial.srOther.Values = new ChartValues<double> { 3, 10, 9, 8, 5, 2 };// double.Parse(dtReport.Rows[0]["Transfering"].ToString()) };

            uc_ReportByTimeMaterial.Label.Labels = new[] {DateTime.Now.Date.AddDays(-5).ToString("dd/MM/yyyy"), DateTime.Now.Date.AddDays(-4).ToString("dd/MM/yyyy"), DateTime.Now.Date.AddDays(-3).ToString("dd/MM/yyyy"),
                DateTime.Now.Date.AddDays(-2).ToString("dd/MM/yyyy"),DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy"),DateTime.Now.Date.ToString("dd/MM/yyyy") };
        }

        /// <summary>
        /// Hiện biể đồ hình tròn
        /// </summary>
        private void ShowPieReport()
        {

            uc_ReportByMaterialType.Good.Title = "GALAXY Z FOLD 5";
            uc_ReportByMaterialType.Normal.Title = "IPHONE 15 PROMAX";
            uc_ReportByMaterialType.Other.Title = "SAMSUNG S24 ULTRA";
            uc_ReportByMaterialType.Warning.Title = "Không có thông tin";
        }

        private void btnSendReport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tính năng đang phát triển", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
