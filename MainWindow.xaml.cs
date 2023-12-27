using System;
using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Timers;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System.Media;
using System.Reflection;
using ZXing;
using ZXing.Common;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using ActUtlTypeLib;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Windows.Controls;
using System.Net.NetworkInformation;
using System.Windows.Threading;
using CaptureWebcam.Database;
using System.Data;
using CaptureWebcam.Common;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Configuration;
using Microsoft.Win32;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using ZXing.QrCode.Internal;
using LiveCharts;

namespace CaptureWebcam
{
    public partial class MainWindow : Window
    {
        DispatcherTimer Timer_CheckIP;
        public MainWindow()
        {
            InitializeComponent();

            //the fps of the webcam
            int cameraFps = 30;

            DispatcherTimer TimerPing = new DispatcherTimer();
            TimerPing.Interval = TimeSpan.FromSeconds(5);
            TimerPing.Tick += TimerPing_Tick;
            //TimerPing.Start();

            DispatcherTimer TimerReadPLC = new DispatcherTimer();
            TimerReadPLC.Interval = TimeSpan.FromSeconds(0.1);
            TimerReadPLC.Tick += TimerReadPLC_Tick;
            TimerReadPLC.Start();

            Timer_CheckIP = new DispatcherTimer();
            Timer_CheckIP.Interval = TimeSpan.FromSeconds(3);
            Timer_CheckIP.Tick += Timer_CheckIP_Tick;

            //create a timer that refreshes the webcam feed
            timer = new System.Timers.Timer()
            {
                Interval = 1000,
                Enabled = true
            };
            timer.Elapsed += new ElapsedEventHandler(timer_Tick);
        }


        //VideoCapture capture;
        FilterInfoCollection filterInfo;
        VideoCaptureDevice captureDevice;
        private BLDatabase oBL = new BLDatabase();
        System.Timers.Timer timer;
        List<clsMaterial> lstMaterial = new List<clsMaterial>();
        private ActUtlType PLC = new ActUtlType();
        private string IP_PLC = "192.168.1.250";
        private string _oldQRCode;
        private int psWait;

        private void Connect_PLC()
        {
            try
            {
                PLC.ActLogicalStationNumber = 10; //Simulation
                //PLC.ActLogicalStationNumber = 20;   //PLC thực
                PLC.Open();
                PLC.SetDevice("M1", 1);

                MessageBox.Show("Kết nối PLC thành công", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("Không kết nối được với PLC. Vui lòng kiểm tra lại kết nối", "Warning", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void TimerPing_Tick(object sender, EventArgs e)
        {
            //PingPLC();
        }

        private void PingPLC()
        {
            try
            {
                Ping PLCPing = new Ping();
                PingReply Reply = PLCPing.Send(IP_PLC);
                // check when the ping is not success
                if (Reply.Status != IPStatus.Success)
                {
                    //AlarmLog.LogAlarmToDatabase("04");
                    MessageBox.Show("Không kết nối được với PLC. Vui lòng kiểm tra lại kết nối", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }
            catch (Exception ex)
            {
                //AlarmLog.LogAlarmToDatabase("04");
                MessageBox.Show("Không kết nối được với PLC. Vui lòng kiểm tra lại kết nối", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void TimerReadPLC_Tick(object sender, EventArgs e)
        {
            try
            {
                ReadPLCParameter();
            }
            catch (Exception ee)
            {

                MessageBox.Show("Có lỗi xảy ra trong quá trình đọc dữ liệu.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        /// <summary>
        /// Thực hiện đọc ghi với PLC
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        /// Không có thông tin --> Đẩy ở đầu
        /// Có thông tin --> kệ mẹ n đẩy theo chiều cao
        private void ReadPLCParameter()
        {
            int SS1 = 0;
            PLC.GetDevice("M600", out SS1);
            int SS2;
            //PLC.GetDevice("M512", out SS2);
            int SS3;
            //PLC.GetDevice("M513", out SS3);
            if (SS1 == 1)
            {
                Timer_CheckIP.Start();

            }
        }

        //Tạo lệnh trong trường hợp không đọc được QR Code
        private void Timer_CheckIP_Tick(object sender, EventArgs e)
        {
            if (txtQRCode.Text == _oldQRCode || txtQRCode.Text == string.Empty)
            {
                PLC.SetDevice("M510", 1);
                psWait = 1;

                clsMaterial _material = new clsMaterial();
                _material.QRCode = "NoInfor";
                _material.ProductCode = "NoInfor";
                _material.ProductName = "NoInfor";
                _material.ProductHeight = "NoInfor";

                ///insert lịch sử phân loại
                ///
                string _commandID = DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + _material.ProductCode;
                oBL.InsertHistory(_material, _commandID);
            }

            Timer_CheckIP.Stop();
            GetPerforment();

            ///Dừng 0.5s rồi reset trạng thái
            Thread.Sleep(1500);
            PLC.SetDevice("M510", 0);
            PLC.SetDevice("M20", 0);
        }

        /// <summary>
        /// Tìm kiếm thông tin hàng vừa được đưa vào
        /// </summary>
        private void CheckMaterialInfor(string qrcode)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = oBL.GetMaterialInforByCode(qrcode);
                if (dt.Rows.Count == 0)
                {
                    PLC.SetDevice("M510", 1);
                    //Xử lý chờ đẩy ra chỗ k có thông tin hàng
                    psWait = 1;

                    clsMaterial _material = new clsMaterial();
                    _material.QRCode = qrcode;
                    _material.ProductCode = "NoInfor";
                    _material.ProductName = "NoInfor";
                    _material.ProductHeight = "NoInfor";

                    ///insert lịch sử phân loại
                    ///
                    string _commandID = DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + qrcode + "_" + _material.ProductCode;
                    oBL.InsertHistory(_material, _commandID);

                    Timer_CheckIP.Stop();
                }
                else
                {
                    PLC.SetDevice("M20", 1);

                    psWait = 0;

                    lstMaterial.Clear();
                    DataRow dr = dt.Rows[0];
                    dtg_MaterialInfor.ItemsSource = dt.DefaultView;
                    clsMaterial _material = new clsMaterial();
                    _material.QRCode = dr["QRCode"].ToString();
                    _material.ProductCode = dr["ProductCode"].ToString();
                    _material.ProductName = dr["ProductName"].ToString();
                    _material.ProductHeight = dr["ProductHeight"].ToString();

                    ///insert lịch sử phân loại
                    ///
                    string _commandID = DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + qrcode + "_" + _material.ProductCode;
                    oBL.InsertHistory(_material, _commandID);

                    Timer_CheckIP.Stop();
                }
                GetPerforment();

                ///Dừng 0.5s rồi reset trạng thái
                Thread.Sleep(1500);
                PLC.SetDevice("M510", 0);
                PLC.SetDevice("M20", 0);
            }
            catch (Exception)
            {

            }

        }


        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            wdHistory frm = new wdHistory();
            frm.ShowDialog();
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            wdReport frm = new wdReport();
            frm.ShowDialog();
        }

        private void btnControl_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tính năng đang phát triển. Vui lòng thử lại sau", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tính năng đang phát triển. Vui lòng thử lại sau", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CheckLiensce())
            {
                Connect_PLC();

                GetPerforment();

                filterInfo = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                captureDevice = new VideoCaptureDevice(filterInfo[1].MonikerString);
                captureDevice.NewFrame += CaptureDevice_NewFrame;
                captureDevice.Start();

                lblServer.Text = "  Server: " + ConfigurationSettings.AppSettings["DatabaseConnection"];
            }
            else
            {
                this.Close();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            PLC.Close();
            captureDevice.Stop();
        }

        /// <summary>
        /// Lấy kết quả chạy trong ngày
        /// </summary>
        private void GetPerforment()
        {
            DataTable dt = new DataTable();
            dt = oBL.GetPerformentToday();
            double _tall = 0; double _mid = 0; double _short = 0; double _noTallInfor = 0;
            double _z5 = 0; double _i15 = 0; double _s24 = 0; double _noPrdInfor = 0;

            foreach (DataRow dr in dt.Rows)
            {
                ///Theo chiều cao
                if (dr["ProductHeight"].ToString() == "Tall")
                {
                    _tall++;
                }
                else if (dr["ProductHeight"].ToString() == "Mid")
                {
                    _mid++;
                }
                else if (dr["ProductHeight"].ToString() == "Short")
                {
                    _short++;
                }
                else
                {
                    _noTallInfor++;
                }

                ///Theo loại hàng
                if (dr["ProductCode"].ToString() == "GALAXY Z FOLD 5")
                {
                    _z5++;
                }
                else if (dr["ProductCode"].ToString() == "IPHONE 15 PROMAX")
                {
                    _i15++;
                }
                else if (dr["ProductCode"].ToString() == "SAMSUNG S24 ULTRA")
                {
                    _s24++;
                }
                else
                {
                    _noPrdInfor++;
                }
            }

            uc_ReportByMaterialType.Good.Values = new ChartValues<double> { _z5 };
            uc_ReportByMaterialType.Normal.Values = new ChartValues<double> { _i15 };
            uc_ReportByMaterialType.Other.Values = new ChartValues<double> { _s24 };
            uc_ReportByMaterialType.Warning.Values = new ChartValues<double> { _noPrdInfor };

            uc_ReportByMaterialType.Good.Title = "GALAXY Z FOLD 5";
            uc_ReportByMaterialType.Normal.Title = "IPHONE 15 PROMAX";
            uc_ReportByMaterialType.Other.Title = "SAMSUNG S24 ULTRA";
            uc_ReportByMaterialType.Warning.Title = "Không có thông tin";


            uc_ReportByTall.Good.Values = new ChartValues<double> { _tall };
            uc_ReportByTall.Normal.Values = new ChartValues<double> { _mid };
            uc_ReportByTall.Other.Values = new ChartValues<double> { _short };
            uc_ReportByTall.Warning.Values = new ChartValues<double> { _noTallInfor };

            uc_ReportByTall.Good.Title = "CAO";
            uc_ReportByTall.Normal.Title = "TRUNG BÌNH";
            uc_ReportByTall.Other.Title = "THẤP";
            uc_ReportByTall.Warning.Title = "Không có thông tin";

            PLC.SetDevice("D200", int.Parse(_tall.ToString()));
            PLC.SetDevice("D210", int.Parse(_mid.ToString()));
            PLC.SetDevice("D220", int.Parse(_short.ToString()));
            PLC.SetDevice("D230", int.Parse(_noTallInfor.ToString()));

        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        /// <summary>
        /// Xử lý mở camera và đọc QR Code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void CaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    Bitmap img = (Bitmap)eventArgs.Frame.Clone();

                    IntPtr hBitmap = img.GetHbitmap();
                    System.Windows.Media.Imaging.BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        hBitmap,
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions()
                        );
                    feedImage.Source = bitmapSource;
                    DeleteObject(hBitmap);

                    var source = new BitmapLuminanceSource(img);
                    var bitmap = new BinaryBitmap(new HybridBinarizer(source));
                    var result = new MultiFormatReader().decode(bitmap);

                    //no qr code found in bitmap
                    if (result == null)
                    {

                    }
                    else
                    {
                        string qrcode = FindQrCodeInImage(img);

                        if (!string.IsNullOrEmpty(qrcode))
                        {
                            //set the found text in the qr code in the ui
                            txtQRCode.Text = qrcode;
                            txtCheckTime.Text = $"Last scan: {DateTime.Now.ToLongDateString()} at {DateTime.Now.ToShortTimeString()}.";

                            if (qrcode != _oldQRCode)
                            {
                                CheckMaterialInfor(qrcode);
                                _oldQRCode = qrcode;
                            }
                            //play a sound to indicate qr code found
                            var player_ok = new SoundPlayer(GetStreamFromResource("sound_ok.wav"));
                            player_ok.Play();
                        }
                    }

                });
            }
            catch (Exception)
            {

            }

        }

        /// <summary>
        /// Kiểm tra hạn sử dụng hệ thống -- Open tới cuối năm 2024
        /// </summary>
        /// <returns></returns>
        private bool CheckLiensce()
        {
            bool Result = true;
            DateTime LiesnceDate = new DateTime(2024, 12, 31, 23, 59, 59);
            DateTime dateTime = DateTime.Now;

            int x = (dateTime - LiesnceDate).Days;
            if (x > 0)
            {
                string str = "Phần mềm đã hết hạn bản quyền sử dụng. Vui lòng liên hệ nhà cung cấp để được hỗ trợ Email:trananh260697@gmail.com Phone:0962174807";
                if (MessageBox.Show(str, "Hết hạn bản quyền", MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    Result = false;
                }
            }
            return Result;
        }

        private void timer_Tick(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
                   {
                       lblDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "  ";
                   });
        }

        /// <summary>
        /// Thực hiện tìm mã QR từ ảnh chụp được
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        private string FindQrCodeInImage(Bitmap bmp)
        {

            //decode the bitmap and try to find a qr code
            var source = new BitmapLuminanceSource(bmp);
            var bitmap = new BinaryBitmap(new HybridBinarizer(source));
            var result = new MultiFormatReader().decode(bitmap);

            //no qr code found in bitmap
            if (result == null)
            {
                return null;
            }

            //create a new qr code image
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = 300,
                    Width = 300
                }
            };

            //write the result to the new qr code bmp image
            var qrcode = writer.Write(result.Text);

            //make the bmp transparent
            qrcode.MakeTransparent();

            //show the found qr code in the app
            var stream = new MemoryStream();
            qrcode.Save(stream, ImageFormat.Png);

            //display the new qr code in the ui
            Image1.Source = BitmapFrame.Create(stream);
            Image1.Visibility = Visibility.Visible;

            //and/or save the new qr code image to disk if needed
            try
            {
                //qrcode.Save($"qr_code_{DateTime.Now.ToString("yyyyMMddHHmmss")}.gif", ImageFormat.Gif);
            }
            catch
            {
                //handle disk write errors here
            }

            //return the found qr code text
            return result.Text;
        }


        private static Stream GetStreamFromResource(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(string.Format("{0}.Resources.{1}", assembly.GetName().Name, filename));
        }


    }
}
