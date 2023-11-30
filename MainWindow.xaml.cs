﻿using System;
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

namespace CaptureWebcam
{
    public partial class MainWindow : Window
    {
        VideoCapture capture;
        Timer timer;

        public MainWindow()
        {
            InitializeComponent();

            //the fps of the webcam
            int cameraFps = 30;

            //init the camera
            capture = new VideoCapture();
            
            //set the captured frame width and height (default 640x480)
            capture.Set(CapProp.FrameWidth, 1024);
            capture.Set(CapProp.FrameHeight, 768);            

            //create a timer that refreshes the webcam feed
            timer = new Timer()
            {
                Interval = 1000 / cameraFps,
                Enabled = true
            };
            //timer.Elapsed += new ElapsedEventHandler(timer_Tick);
        }


        private async void timer_Tick(object sender, ElapsedEventArgs e)
        {
            try
            {
                //there is a qr code image visible
                if (feedImage.Visibility == Visibility.Collapsed)
                {
                    timer.Stop();

                    //the delay time you want to display the qr code in the ui for
                    await Task.Run(() => Task.Delay(2500));

                    //set the image visibility
                    this.Dispatcher.Invoke(() =>
                    {
                        feedImage.Visibility = Visibility.Visible;
                        Image1.Visibility = Visibility.Collapsed;
                    });

                    timer.Start();
                }

                this.Dispatcher.Invoke(() =>
                {
                    var mat1 = capture.QueryFrame();
                    var mat2 = new Mat();

                    //flip the image horizontally
                    CvInvoke.Flip(mat1, mat2, FlipType.Horizontal);

                    //convert the mat to a bitmap
                    var bmp = mat2.ToImage<Bgr, byte>().ToBitmap();

                    //copy the bitmap to a memorystream
                    var ms = new MemoryStream();
                    bmp.Save(ms, ImageFormat.Bmp);

                    //display the image on the ui
                    feedImage.Source = BitmapFrame.Create(ms);

                    //try to find a qr code in the feed
                    string qrcode = FindQrCodeInImage(bmp);

                    if (!string.IsNullOrEmpty(qrcode))
                    {
                        //set the found text in the qr code in the ui
                        txtQRCode.Text = qrcode;
                        txtCheckTime.Text = $"Last scan: {DateTime.Now.ToLongDateString()} at {DateTime.Now.ToShortTimeString()}.";

                        //play a sound to indicate qr code found
                        var player_ok = new SoundPlayer(GetStreamFromResource("sound_ok.wav"));
                        player_ok.Play();

                        //hide the feed image
                        feedImage.Visibility = Visibility.Collapsed;
                    }
                });
            }
            catch (Exception ee)
            {

                MessageBox.Show(ee.Message);
            }
           
        }


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

        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDetail_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnControl_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
