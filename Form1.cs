using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
namespace ImageThing
{
    public partial class Form1 : Form
    {
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("User32.dll")]
        public static extern Int32 SetForegroundWindow(int hWnd);   
        int inc;
        string del;
        double width;
        double height;
        int fileR = 0;
        string[] args;
        string[] files;
        Keys[] keyStuff;
        double oldWidth;
        double oldHeight;
        string chosenImg;
        bool fullS = false;
        double sizeScale = 0;
        bool deleteMode = false;
        OpenFileDialog browse = new OpenFileDialog();
        double resW = Screen.PrimaryScreen.WorkingArea.Width;
        double resH = Screen.PrimaryScreen.WorkingArea.Height;
        string[] keyConfig = System.IO.File.ReadAllLines(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\config.txt");
        public Form1()
        {
            InitializeComponent();
            pictureBox1.MouseDown += new MouseEventHandler(pictureBox1_MouseDown);
            pictureBox1.MouseUp += new MouseEventHandler(pictureBox1_MouseUp);
            this.KeyDown += new KeyEventHandler(pictureBox1_KeyDown);
            this.KeyUp += new KeyEventHandler(Form1_KeyUp);
            keyStuff = new Keys[keyConfig.Length];
            this.TransparencyKey = Color.Fuchsia;
            this.BackColor = Color.Fuchsia;
            for (int i = 0; i < keyConfig.Length; i++)
            {
                keyStuff[i] = (Keys)Enum.Parse(typeof(Keys), keyConfig[i].Substring(keyConfig[i].IndexOf(' ') + 1));
            }
            try
            {
                args = Environment.GetCommandLineArgs();
                chosenImg = args[1];
                loadDir();
                loadImage();
            }
            catch
            {
                browseImage();
                SetForegroundWindow(Handle.ToInt32());
            }
        }
        private void scaleImg(double sizeStuff)
        {
            oldWidth = this.Width;
            oldHeight = this.Height;
            sizeScale += sizeStuff;
            this.Width = Convert.ToInt32(width + (sizeScale * (width / height)));
            this.Height = Convert.ToInt32(height + sizeScale);
            pictureBox1.Width = this.Width;
            pictureBox1.Height = this.Height;
            this.Left += Convert.ToInt32((oldWidth - this.Width) / 2);
            this.Top += Convert.ToInt32((oldHeight - this.Height) / 2);
            pictureBox1.Top = 0;
            pictureBox1.Left = 0;
        }
        private void incMent(int dir, bool doDel)
        {
            if (doDel == true)
            {
                del = files[inc];
                files[inc] = "deleted";
            }
            while (1 > 0)
            {
                inc += dir;
                if (inc > files.Length - 1)
                {
                    inc = 0;
                }
                if (inc < 0)
                {
                    inc = files.Length - 1;
                }
                try
                {
                    pictureBox1.Image = Image.FromFile(files[inc]);
                    break;
                }
                catch
                {
                    if (doDel == true)
                    {
                        if (fileR > files.Length - 1)
                        {
                            pictureBox1.Image = null;
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            File.Delete(del);
                            this.Close();
                        }
                        fileR += 1;
                    }
                }
            }
            if (doDel == true)
            {
                loadImage();
                GC.WaitForPendingFinalizers();
                File.Delete(del);
                fileR = 0;
            }
        }
        private void loadImage()
        {
            sizeScale = 0;
            oldWidth = this.Width;
            oldHeight = this.Height;
            height = Image.FromFile(files[inc]).Height;
            width = Image.FromFile(files[inc]).Width;
            pictureBox1.Image = Image.FromFile(files[inc]);
            if (fullS == false)
            {
                if (width > resW || height > resH)
                {
                    if (width / height > resW / resH)
                    {
                        this.Width = Convert.ToInt32(resW);
                        pictureBox1.Width = Convert.ToInt32(resW);
                        this.Height = Convert.ToInt32(height / (width / resW));
                        pictureBox1.Height = Convert.ToInt32(height / (width / resW));
                    }
                    if (width / height <= resW / resH)
                    {
                        this.Height = Convert.ToInt32(resH);
                        pictureBox1.Height = Convert.ToInt32(resH);
                        this.Width = Convert.ToInt32(width / (height / resH));
                        pictureBox1.Width = Convert.ToInt32(width / (height / resH));
                    }
                }
                if (width <= resW && height <= resH)
                {
                    this.Height = Convert.ToInt32(height);
                    pictureBox1.Height = Convert.ToInt32(height);
                    this.Width = Convert.ToInt32(width);
                    pictureBox1.Width = Convert.ToInt32(width);
                }
                this.Left += Convert.ToInt32((oldWidth - this.Width) / 2);
                this.Top += Convert.ToInt32((oldHeight - this.Height) / 2);
            }
            else
            {
                if (width >= resW)
                {
                    this.Width = Convert.ToInt32(resW);
                }
                else
                {
                    this.Width = Convert.ToInt32(width);
                }
                if (height >= resH)
                {
                    this.Height = Convert.ToInt32(resH);
                }
                else
                {
                    this.Height = Convert.ToInt32(height);
                }
                if (width < resW && height < resH)
                {
                    this.Height = Convert.ToInt32(height);
                    this.Width = Convert.ToInt32(width);
                }
                pictureBox1.Width = Convert.ToInt32(width);
                pictureBox1.Height = Convert.ToInt32(height);
                this.CenterToScreen();
            }
            pictureBox1.Left = 0;
            pictureBox1.Top = 0;
            GC.Collect();
        }
        private void loadDir()
        {
            files = Directory.GetFiles(Path.GetDirectoryName(chosenImg));
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i] == chosenImg)
                {
                    inc = i;
                }
            }
        }
        private void browseImage()
        {
            browse.ShowDialog();
            chosenImg = browse.FileName;
            loadDir();
            loadImage();
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 0xA1, 0x2, 0);
            }
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.Close();
            }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == keyStuff[9] && fullS == false)
            {
                if (width / height > resW / resH)
                {
                    sizeScale = (resW - width) / (width / height);
                }
                else
                {
                    sizeScale = resH - height;
                }
                scaleImg(0);
            }
            if (e.KeyCode == keyStuff[10] && fullS == false && width < resW && height < resH)
            {
                sizeScale = 0;
                scaleImg(0);
            }
            if (e.KeyCode == keyStuff[2])
            {
                browseImage();
            }
            if (e.KeyCode == keyStuff[5])
            {
                if (fullS == false && (width > resW || height > resH))
                {
                    fullS = true;
                }
                else
                {
                    fullS = false;
                }
                loadImage();
            }
            if (e.KeyCode == keyStuff[6])
            {
                MessageBox.Show(keyStuff[0] + ": Increase Size\n" + keyStuff[1] + ": Decrease Size\n" + keyStuff[2] + ": Browse For an Image\n" + keyStuff[3] + ": Center Image\n" + keyStuff[4] + ": Delete Image\n" + keyStuff[5] + ": Fullscreen\n" + keyStuff[6] + ": Help Menu\n" + keyStuff[7] + ": Previous Image\n" + keyStuff[8] + ": Next Image\n" + keyStuff[9] + ": Maximize Image\n" + keyStuff[10] + ": Reset Image Size\n" + keyStuff[11] + ": Toggle Delete Mode\nLeft Click: Drag Window\nRight Click: Close");
            }
            if (e.KeyCode == keyStuff[3])
            {
                this.CenterToScreen();
            }
            if (e.KeyCode == keyStuff[11])
            {
                if (deleteMode == false)
                {
                    deleteMode = true;
                    MessageBox.Show("Delete Mode Enabled");
                }
                else
                {
                    deleteMode = false;
                    MessageBox.Show("Delete Mode Disabled");
                }
            }
            if (e.KeyCode == keyStuff[4])
            {
                if (deleteMode == true)
                {
                    incMent(1, true);
                }
                else
                {
                    MessageBox.Show("Press X to enable Delete Mode");
                }
            }
            if (e.KeyCode == keyStuff[8])
            {
                incMent(1, false);
                loadImage();
            }
            if (e.KeyCode == keyStuff[7])
            {
                incMent(-1, false);
                loadImage();
            }
        }
        private void pictureBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (fullS == false)
            {
                if (e.KeyCode == keyStuff[0] && sizeScale + width < resW && sizeScale + height < resH)
                {
                    scaleImg(4);
                }
                if (e.KeyCode == keyStuff[1] && width <= resW && height <= resH && width + sizeScale * (width / height) > 0 && height + sizeScale > 0)
                {
                    scaleImg(-4);
                }
            }
            if (fullS == true)
            {
                if (e.KeyCode == keyStuff[12] && height > resH)
                {
                    pictureBox1.Top -= 50;
                    if (pictureBox1.Top < height * -1 + resH)
                    {
                        pictureBox1.Top = Convert.ToInt32(height * -1 + resH);
                    }
                }
                if (e.KeyCode == keyStuff[13])
                {
                    pictureBox1.Top += 50;
                    if (pictureBox1.Top > 0)
                    {
                        pictureBox1.Top = 0;
                    }
                }
                if (e.KeyCode == keyStuff[14] && width > resW)
                {
                    pictureBox1.Left -= 50;
                    if (pictureBox1.Left < width * -1 + resW)
                    {
                        pictureBox1.Left = Convert.ToInt32(width * -1 + resW);
                    }
                }
                if (e.KeyCode == keyStuff[15])
                {
                    pictureBox1.Left += 50;
                    if (pictureBox1.Left > 0)
                    {
                        pictureBox1.Left = 0;
                    }
                }
            }
        }
    }
}