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
        [DllImportAttribute("user32.dll")] public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")] public static extern bool ReleaseCapture();
        double resH = Screen.PrimaryScreen.WorkingArea.Height;
        double resW = Screen.PrimaryScreen.WorkingArea.Width;
        double height;
        double width;
        string test;
        int index;
        int inc;
        string del;
        string[] args;
        string[] files;
        int fileR = 0;
        bool fullS = false;
        bool deleteMode = false;
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
                if (files[inc].EndsWith(".jpg") || files[inc].EndsWith(".png") || files[inc].EndsWith(".gif") || files[inc].EndsWith(".ico"))
                {
                    break;
                }
                else
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
            height = Image.FromFile(files[inc]).Height;
            width = Image.FromFile(files[inc]).Width;
            pictureBox1.Image = Image.FromFile(files[inc]);
            if (fullS == false)
            {
                if (width > resW)
                {
                    this.Width = Convert.ToInt32(resW);
                    pictureBox1.Width = Convert.ToInt32(resW);
                    this.Height = Convert.ToInt32(height / (width / resW));
                    pictureBox1.Height = Convert.ToInt32(height / (width / resW));
                }
                if (height > resH)
                {
                    this.Height = Convert.ToInt32(resH);
                    pictureBox1.Height = Convert.ToInt32(resH);
                    this.Width = Convert.ToInt32(width / (height / resH));
                    pictureBox1.Width = Convert.ToInt32(width / (height / resH));
                }
                if (width > resW && width / height > resW / resH)
                {
                    this.Width = Convert.ToInt32(resW);
                    pictureBox1.Width = Convert.ToInt32(resW);
                    this.Height = Convert.ToInt32(height / (width / resW));
                    pictureBox1.Height = Convert.ToInt32(height / (width / resW));
                }
                if (width <= resW && height <= resH)
                {
                    this.Height = Convert.ToInt32(height);
                    pictureBox1.Height = Convert.ToInt32(height);
                    this.Width = Convert.ToInt32(width);
                    pictureBox1.Width = Convert.ToInt32(width);
                }
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
        public Form1()
        {
            InitializeComponent();
            pictureBox1.MouseDown += new MouseEventHandler(pictureBox1_MouseDown);
            pictureBox1.MouseUp += new MouseEventHandler(pictureBox1_MouseUp);
            this.KeyDown += new KeyEventHandler(pictureBox1_KeyDown);
            this.KeyUp += new KeyEventHandler(Form1_KeyUp);
            args = Environment.GetCommandLineArgs();
            test = args[1];
            index = test.LastIndexOf("\\");
            test = test.Substring(0, index);
            files = Directory.GetFiles(test, "*");
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i] == args[1])
                {
                    inc = i;
                }
            }
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
            if (e.KeyCode == Keys.F)
            {
                if (fullS == false)
                {
                    fullS = true;
                }
                else
                {
                    fullS = false;
                }
                loadImage();
            }
            if (e.KeyCode == Keys.H)
            {
                MessageBox.Show("H: Help Menu \nJ: Previous Image \nK: Next Image \nX: Toggle Delete Mode \nD: Delete Image \nC: Center Image \nF: Fullscreen \nRight Click: Close \nLeft Click: Drag Window");
            }
            if (e.KeyCode == Keys.C)
            {
                this.CenterToScreen();
            }
            if (e.KeyCode == Keys.X)
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
            if (e.KeyCode == Keys.D)
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
            if (e.KeyCode == Keys.K)
            {
                incMent(1, false);
                loadImage();
            }
            if (e.KeyCode == Keys.J)
            {
                incMent(-1, false);
                loadImage();
            }
        }
        private void pictureBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (fullS == true)
            {
                if (e.KeyCode == Keys.Down && height > resH)
                {
                    pictureBox1.Top -= 50;
                    if (pictureBox1.Top < height * -1 + resH)
                    {
                        pictureBox1.Top = Convert.ToInt32(height * -1 + resH);
                    }
                }
                if (e.KeyCode == Keys.Up)
                {
                    pictureBox1.Top += 50;
                    if (pictureBox1.Top > 0)
                    {
                        pictureBox1.Top = 0;
                    }
                }
                if (e.KeyCode == Keys.Right && width > resW)
                {
                    pictureBox1.Left -= 50;
                    if (pictureBox1.Left < width * -1 + resW)
                    {
                        pictureBox1.Left = Convert.ToInt32(width * -1 + resW);
                    }
                }
                if (e.KeyCode == Keys.Left)
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