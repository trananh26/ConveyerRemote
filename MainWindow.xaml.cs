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

namespace CaptureWebcam
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            //the fps of the webcam
            int cameraFps = 30;

            ////init the camera
            //capture = new VideoCapture(1);

            //set the captured frame width and height (default 640x480)
            //capture.Set(CapProp.FrameWidth, 1024);
            //capture.Set(CapProp.FrameHeight, 768);

            DispatcherTimer TimerPing = new DispatcherTimer();
            TimerPing.Interval = TimeSpan.FromSeconds(5);
            TimerPing.Tick += TimerPing_Tick;
            //TimerPing.Start();

            DispatcherTimer TimerReadPLC = new DispatcherTimer();
            TimerReadPLC.Interval = TimeSpan.FromSeconds(0.1);
            TimerReadPLC.Tick += TimerReadPLC_Tick;
            TimerReadPLC.Start();

            //create a timer that refreshes the webcam feed
            timer = new System.Timers.Timer()
            {
                Interval = 1000 / cameraFps,
                Enabled = true
            };
            //timer.Elapsed += new ElapsedEventHandler(timer_Tick);
        }


        VideoCapture capture;
        FilterInfoCollection filterInfo;
        VideoCaptureDevice captureDevice;

        System.Timers.Timer timer;
        BLDatabase oBL = new BLDatabase();
        List<clsMaterial> lstMaterial = new List<clsMaterial>();
        private ActUtlType PLC = new ActUtlType();
        private string IP_PLC = "192.168.1.250";
        private string _oldQRCode;
        private int psWait;

        private void Connect_PLC()
        {
            //try
            //{
            //    PLC.ActLogicalStationNumber = 25;
            //    PLC.Open();
            //    PLC.SetDevice("M1", 1);

            //    MessageBox.Show("Kết nối PLC thành công", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            //}
            //catch (Exception)
            //{
            //    MessageBox.Show("Không kết nối được với PLC. Vui lòng kiểm tra lại kết nối", "Warning", MessageBoxButton.OK, MessageBoxImage.Information);
            //}
        }
        private void TimerPing_Tick(object sender, EventArgs e)
        {
            PingPLC();
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
        //    int SS1;
        //    PLC.GetDevice("M511", out SS1);
        //    int SS2;
        //    PLC.GetDevice("M512", out SS2);
        //    int SS3;
        //    PLC.GetDevice("M513", out SS3);
        //    if (SS1 == 1)
        //    {
        //        if (txtQRCode.Text == _oldQRCode || psWait == 1)
        //        {
        //            // hàng k có mã QRcode hoặc đọc lỗi
        //        }


        //    }

        //    if (SS2 == 1)
        //    {

        //    }

        //    if (SS3 == 1)
        //    {

        //    }

        }

        private void CheckMaterialInfor()
        {
            DataTable dt = new DataTable();
            if (dt.Rows.Count == 0)
            {
                //Xử lý chờ đẩy ra chỗ k có thông tin hàng
                psWait = 1;
            }
            else
            {
                lstMaterial.Clear();
                DataRow dr = dt.Rows[0];

                clsMaterial _material = new clsMaterial();
                _material.QRCode = dr["QRCode"].ToString();
                _material.ProductCode = dr["ProductCode"].ToString();
                _material.ProductName = dr["ProductName"].ToString();
                _material.ProductHeight = dr["ProductHeight"].ToString();

                ///insert lịch sử phân loại
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

                filterInfo = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                captureDevice = new VideoCaptureDevice(filterInfo[0].MonikerString);
                captureDevice.NewFrame += CaptureDevice_NewFrame;
                captureDevice.Start();
            }
            else
            {
                this.Close();
            }
        }

        private void CaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            this.Dispatcher.Invoke(() =>
            {
                Bitmap img = (Bitmap)eventArgs.Frame.Clone();

                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, (SendOrPostCallback)delegate
                {
                    IntPtr hBitmap = img.GetHbitmap();
                    System.Windows.Media.Imaging.BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        hBitmap,
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                    //DeleteObject(hBitmap);

                    img.Dispose();
                    GC.Collect();
                    feedImage.Source = bitmapSource;

                }, null);


                string qrcode = FindQrCodeInImage((Bitmap)eventArgs.Frame.Clone());

                //Image1.Source = (Bitmap)eventArgs.Frame.Clone();
                if (!string.IsNullOrEmpty(qrcode))
                {
                    //set the found text in the qr code in the ui
                    txtQRCode.Text = qrcode;
                    txtCheckTime.Text = $"Last scan: {DateTime.Now.ToLongDateString()} at {DateTime.Now.ToShortTimeString()}.";

                    if (qrcode != _oldQRCode)
                    {
                        CheckMaterialInfor();
                        _oldQRCode=qrcode;
                    }
                    //play a sound to indicate qr code found
                    var player_ok = new SoundPlayer(GetStreamFromResource("sound_ok.wav"));
                    player_ok.Play();

                    //hide the feed image
                    feedImage.Visibility = Visibility.Collapsed;
                }

            });
        }

        private bool CheckLiensce()
        {
            bool Result = true;
            DateTime LiesnceDate = new DateTime(2024, 02, 15, 23, 59, 59);
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

        //private async void timer_Tick(object sender, ElapsedEventArgs e)
        //{
        //    try
        //    {
        //        //there is a qr code image visible
        //        if (feedImage.Visibility == Visibility.Collapsed)
        //        {
        //            timer.Stop();

        //            //the delay time you want to display the qr code in the ui for
        //            await Task.Run(() => Task.Delay(2500));

        //            //set the image visibility
        //            this.Dispatcher.Invoke(() =>
        //            {
        //                feedImage.Visibility = Visibility.Visible;
        //                Image1.Visibility = Visibility.Collapsed;
        //            });

        //            timer.Start();
        //        }

        //        this.Dispatcher.Invoke(() =>
        //        {
        //            var mat1 = capture.QueryFrame();
        //            var mat2 = new Mat();

        //            //flip the image horizontally
        //            CvInvoke.Flip(mat1, mat2, FlipType.Horizontal);

        //            //convert the mat to a bitmap
        //            var bmp = mat2.ToImage<Bgr, byte>().ToBitmap();

        //            //copy the bitmap to a memorystream
        //            var ms = new MemoryStream();
        //            bmp.Save(ms, ImageFormat.Bmp);

        //            //display the image on the ui
        //            feedImage.Source = BitmapFrame.Create(ms);

        //            //try to find a qr code in the feed
        //            string qrcode = FindQrCodeInImage(bmp);

        //            if (!string.IsNullOrEmpty(qrcode))
        //            {
        //                //set the found text in the qr code in the ui
        //                txtQRCode.Text = qrcode;
        //                txtCheckTime.Text = $"Last scan: {DateTime.Now.ToLongDateString()} at {DateTime.Now.ToShortTimeString()}.";

        //                if (qrcode != _oldQRCode)
        //                {
        //                    CheckMaterialInfor();
        //                }
        //                //play a sound to indicate qr code found
        //                var player_ok = new SoundPlayer(GetStreamFromResource("sound_ok.wav"));
        //                player_ok.Play();

        //                //hide the feed image
        //                feedImage.Visibility = Visibility.Collapsed;
        //            }

        //        });
        //    }
        //    catch (Exception ee)
        //    {

        //        MessageBox.Show(ee.Message);
        //    }

        //}

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
