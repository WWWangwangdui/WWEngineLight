using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Data;
using WWEngineCC.Properties;
using System.Drawing;

namespace WWEngineCC
{
    static public class WWDirector
    {
        public static Form1 mainForm;
        private static WWscene scene = null;
        private static WWproj proj = null;
        private static Dictionary<int, WWobject> keyobjects = new Dictionary<int, WWobject>();
        private static Dictionary<int, WWmoduleBase> keymodules = new Dictionary<int, WWmoduleBase>();
        public static Dictionary<string, object> globleParams = new Dictionary<string, object>();
        public static WWproj WWProject
        {
            get
            {
                return proj;
            }
        }
        public static WWscene WWScene
        {
            get
            {
                return scene;
            }
        }
     
        public static int objRegist(WWobject obj)
        {
            if (scene == null) return -1;
            int id = scene.nxtObjId;
            scene.addObjId();
            keyobjects.Add(id, obj);
            return id;
        }
        public static int modRegist(WWmoduleBase mod)
        {
            if (scene == null) return -1;
            int id = scene.nxtModId;
            scene.addModId();
            keymodules.Add(id, mod);
            return id;
        }
        public static WWobject WWaddObj(string name = "")
        {
            if (scene == null) return null;
            return scene.WWaddObj(name);
        }

        public static void WWloadProj(string path)
        {
            if (proj != null)
            {
                WWcloseProj();
            }
            proj = WWproj.WWloadProj(path);
            if (Directory.Exists(WWDirector.WWProject.BinPath + "temp"))
            {
                Directory.Delete(WWProject.BinPath + "temp", true);
            }
            WWPluginCC.WWloadPlugins();
            WWasCtrl.WWloadAssets();
            if(proj.WWDefaultSceneName==null)
            {
                MessageBox.Show("游戏文件出现错误");
            }
            else
            {
                WWloadScene(proj.WWDefaultSceneName);
            }
        }
        public static void WWcloseProj()
        {
            if (proj == null) return;
            Program.playing = false;
            proj = null;
            WWcloseScene();
            WWPluginCC.WWunloadPlugins();
            keyobjects.Clear();
            keymodules.Clear();
        }
        static string randerbasestr = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ" + "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";//复杂字符
        static Random randstrrandom = new Random();
        static string randerbasebasestr = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static string getRandStr()
        {
            StringBuilder SB = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                SB.Append(randerbasestr.Substring(randstrrandom.Next(0, randerbasestr.Length), 1));
            }
            return SB.ToString();
        }
        public static string getRandBaseStr()
        {
            StringBuilder SB = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                SB.Append(randerbasebasestr.Substring(randstrrandom.Next(0, randerbasebasestr.Length), 1));
            }
            return SB.ToString();
        }
        public static string WWaddGlobleParam(object obj)
        {
            string name = getRandStr();
            while (!WWaddGlobleParam(name, obj))
            {
                name = getRandStr();
            }
            return name;
        }

        public static bool WWdelGlobleParam(string name)
        {
            if (globleParams.ContainsKey(name))
            {
                globleParams.Remove(name);
                return true;
            }
            return false;
        }

        public static bool WWaddGlobleParam(string name, object obj)
        {
            if (name == null) return false;
            if (globleParams.ContainsKey(name)) return false;
            globleParams.Add(name, obj);
            return true;
        }
        public static WWcamera WWcamera
        {
            get
            {
                if (scene == null) return null;
                return scene.WWcamera;
            }
        }

        public static string Curname { get; private set; }

        public static object WWgetGlobleParam(string name)
        {
            if (globleParams.ContainsKey(name)) return globleParams[name];
            return null;
        }

        public static void WWcloseScene()
        {
            scene = null;
            keyobjects.Clear();
            keymodules.Clear();
            globleParams.Clear();
        }

        public static void WWupdate()
        {
            if (scene == null) return;
            scene.WWupdate();
        }

        public static void WWsleepUpdate()
        {
            if (scene == null) return;
            scene.WWsleepUpdate();
        }



        delegate void LoadSceneInvokeS(string Msg);

        public static void WWloadScene(string name)
        {
            if (mainForm.InvokeRequired)
            {
                LoadSceneInvokeS f = new LoadSceneInvokeS(WWloadScene);
                mainForm.Invoke(f, name);
                return;
            }
            WWcloseScene();
            WWscene NEW = WWscene.WWloadScene(proj.ScenesPath + name + ".WWscene");
            scene = NEW;
            foreach (var item in scene.Objects)
            {
                item.WWregist();
                item.WWload();
            }
            NEW.WWinit();
            Curname = NEW.name;
            WWcamera.LeftTop = new PointF((int)scene.LeftTopPoint.X, (int)scene.LeftTopPoint.Y);
            mainForm.Size = new Size((int)scene.WindowSize.Width, (int)scene.WindowSize.Height);
        }

        public static void WWreLoadScene()
        {
            if (scene == null) return;
            WWloadScene(scene.path);
        }

        delegate void LoadSceneInvoke(int Msg); //代理

        public static void WWloadScene(int index)
        {
            if(mainForm.InvokeRequired)
            {
                LoadSceneInvoke f = new LoadSceneInvoke(WWloadScene);
                mainForm.Invoke(f, index);
                return;
            }
            WWcloseScene();
            string path = proj.ScenesPath + proj.Scenename[index] + ".WWscene";
            WWscene NEW = WWscene.WWloadScene(path);
            scene = NEW;
            foreach (var item in scene.Objects)
            {
                item.WWregist();
                item.WWload();
            }
            NEW.WWinit();
            Curname = NEW.name;
            WWcamera.LeftTop = new PointF((int)scene.LeftTopPoint.X, (int)scene.LeftTopPoint.Y);
            mainForm.Size = new Size((int)scene.WindowSize.Width, (int)scene.WindowSize.Height);
        }

        public static WWobject WWgetObj(int id)
        {
            if (keyobjects.Keys.Contains(id))
                return keyobjects[id];
            return null;
        }

        public static WWmoduleBase WWgetMod(int id)
        {
            if (keymodules.ContainsKey(id))
                return keymodules[id];
            return null;
        }

        public static void WWreName(int objId, string name)
        {
            WWobject obj = WWgetObj(objId);
            if (obj != null)
            {
                obj.Name = name;
            }
        }
        public static bool WWdelObj(int objId)
        {
            if (scene == null) return false;
            WWobject obj = null;
            if (keyobjects.ContainsKey(objId))
                obj = keyobjects[objId];
            if (obj == null) return false;
            if (obj.Parent != null)
            {
                return obj.Parent.WWdelObj(obj);
            }
            WWremoveObj(obj);
            return scene.WWdelObj(obj);
        }

        public static void WWremoveMod(WWmoduleBase mod)
        {
            if (keymodules.ContainsKey(mod.ModuleID))
                keymodules.Remove(mod.ModuleID);
        }

        public static void WWremoveObj(WWobject obj)
        {
            if (keyobjects.ContainsKey(obj.ID))
                keyobjects.Remove(obj.ID);
        }

        internal static bool WWdelMod(int id)
        {
            if (scene == null) return false;
            WWmoduleBase mod = null;
            if (keymodules.ContainsKey(id)) mod = keymodules[id];
            if (mod == null) return false;
            if (mod.Parent != null)
            {
                return mod.Parent.WWdelMod(mod);
            }
            return false;
        }

    }
}
