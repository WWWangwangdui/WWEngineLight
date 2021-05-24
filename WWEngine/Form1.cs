using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WWEngineCC
{
    public partial class Form1 : Form
    {
        private static bool Lock = false;
        public Form1()
        {
            InitializeComponent();
            WWPluginCC.RegistMod();
            WWRenderer.WWinit();
            WWDirector.mainForm = this;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            WWkeyIO.Target = this;
            string path = "";
            string[] paths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.WWproj");
            if(paths.Length!=1)
            {
                MessageBox.Show("游戏配置文件有误");
                Close();
                return;
            }
            path = paths[0];
            if (File.Exists(path))
            {
                try
                {
                    WWDirector.WWloadProj(path);
                    if(WWDirector.WWProject!=null)
                    {
                        this.Text = WWDirector.WWProject.name;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Close();
                }
            }
            else
            {
                MessageBox.Show("未找到游戏配置文件");
                Close();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (!Lock)
            {
                Lock = true;
                WWRenderer.WWsetHwnd((uint)this.Handle, this.Size.Width, this.Size.Height);
                WWRenderer.WWstartDraw();
                WWRenderer.drawing = true;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.onQuit();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            WWRenderer.WWsetSize(Width, Height);
        }
    }
}
