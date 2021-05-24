using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace WWEngineCC
{
    static class Program
    {
        static Form1 mainForm;
        public static bool playing = true;
        private static Thread timer = null;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);;
            try
            {
                WWTime.init();
                timer = new Thread(new ThreadStart(WWTime.TimeCircle));
                timer.Start();
                WWPluginCC.WWinit();
                mainForm = new Form1();
                Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                onQuit();
                return;
            }
        }
        public static void update()
        {
            WWTime.WWupdate();
            WWkeyIO.WWupdate();
            if (playing)
            {
                WWDirector.WWupdate();
                WWRenderer.Render();
            }
            else
            {
                WWDirector.WWsleepUpdate();
                WWRenderer.Render();
            }
        }

        public static void onQuit()
        {
            timer.Abort();
        }
    }
}
