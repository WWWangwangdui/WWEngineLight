using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Data;
using System.ComponentModel;
using System.Drawing;

namespace WWEngineCC
{
    public class WWscene
    {
        private string Name;
        [Category("基本属性")]
        [DisplayName("场景名")]
        public string name
        {
            get
            {
                return Name;
            }
            set
            {
                Name = value;
            }
        }
        [Category("基本属性")]
        [DisplayName("场景路径")]
        public string path
        {
            get
            {
                return Name + ".WWscene";
            }
        }
        [Category("基本属性")]
        [DisplayName("场景绝对路径")]
        public string GlobalPath
        {
            get
            {
                return Path.Combine(WWDirector.WWProject.ScenesPath, path);
            }
        }

        private List<WWobject> objects = new List<WWobject>();
        [Category("场景成员")]
        [DisplayName("游戏对象列表")]
        public List<WWobject> Objects
        {
            get
            {
                return objects;
            }
        }

        [Category("基本属性")]
        [DisplayName("游戏左上角大小")]
        public PointF LeftTopPoint { get; set; } = new PointF(0, 0);
        [Category("基本属性")]
        [DisplayName("游戏窗口大小")]
        public SizeF WindowSize { get; set; } = new SizeF(900, 600);

        private int _nxtObjId = 1;
        private int _nxtModId = 1;
        [Browsable(false)]
        public int nxtObjId
        {
            get
            {
                return _nxtObjId;
            }
        }
        [Browsable(false)]
        public int nxtModId
        {
            get
            {
                return _nxtModId;
            }
        }
        public void addObjId()
        {
            _nxtObjId++;
        }
        public void addModId()
        {
            _nxtModId++;
        }

        private WWcamera camera = new WWcamera();

        [Category("场景运行中属性")]
        [DisplayName("相机")]
        public WWcamera WWcamera
        {
            get => camera;
            set => camera = value;
        }

        public WWobject WWaddObj(string name = "")
        {
            WWobject NEW = new WWobject();
            NEW.Name = name;
            objects.Add(NEW);
            NEW.WWaddMod("WWTransform");
            return NEW;
        }
        public static WWscene WWloadScene(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(WWscene), WWPluginCC.types.ToArray());
            FileStream fs = new FileStream(path, FileMode.Open);
            WWscene res = serializer.Deserialize(fs) as WWscene;
            fs.Close();
            return res;
        }
      
        public bool WWdelObj(WWobject obj)
        {
            if (objects.Contains(obj))
            {
                obj.WWkilled();
                objects.Remove(obj);
                return true;
            }
            return false;
        }
        public void WWinit()
        {
            foreach (var item in objects.ToArray())
            {
                item.WWinit(null);
            }
        }

        internal void WWupdate()
        {
            camera.WWupdate();
            foreach (var item in objects.ToArray())
            {
                if (item.activity)
                    item.WWupdate();
            }
        }

        internal void WWsleepUpdate()
        {
            WWcamera.WWupdate();
            foreach (var item in objects.ToArray())
            {
                if (item.activity)
                    item.WWsleepUpdate();
            }
        }
    }

    public class WWproj
    {
        private string Name;
        private string Path;
        [Category("基本属性")]
        [DisplayName("入口场景")]
        public string WWDefaultSceneName { get; set; }


        [Browsable(true)]
        [Category("基本属性")]
        [DisplayName("项目名")]
        public string name
        {
            get
            {
                return Name;
            }
            set
            {
                Name = value;
            }
        }
        [XmlIgnore()]
        [Browsable(true)]
        [Category("路径")]
        [DisplayName("项目根目录")]
        public string path
        {
            get
            {
                return Path;
            }
            set
            {
                Path = value;
            }
        }
        [Browsable(true)]
        [Category("路径")]
        [DisplayName("数据路径")]
        public string DataPath
        {
            get
            {
                return path + "Data\\";
            }
        }
        [Browsable(true)]
        [Category("路径")]
        [DisplayName("主程序路径")]
        public string BinPath
        {
            get
            {
                return path + "Bin\\";
            }
        }
        [Browsable(true)]
        [Category("路径")]
        [DisplayName("资源路径")]
        public string AssetsPath
        {
            get
            {
                return path + "Assets\\";
            }
        }
        [Browsable(true)]
        [Category("路径")]
        [DisplayName("存档路径")]
        public string SavePath
        {
            get
            {
                return path + "Save\\";
            }
        }
        [Browsable(true)]
        [Category("路径")]
        [DisplayName("场景路径路径")]
        public string ScenesPath
        {
            get
            {
                return path + "Scenes\\";
            }
        }
        [Browsable(true)]
        [Category("路径")]
        [DisplayName("项目文件路径")]
        public string ProjPath
        {
            get
            {
                return path + name + ".WWproj";
            }
        }

        private List<string> scenename = new List<string>();

        [Category("场景")]
        [DisplayName("场景名")]
        public List<string>Scenename
        {
            get
            {
                return scenename;
            }
        }

        [Category("场景")]
        [DisplayName("场景数")]
        public int SceneCount
        {
            get
            {
                return scenename.Count;
            }
        }


        [Browsable(false)]
        public string Recommended_name
        {
            get
            {
                for (int i = 1; i <= 5000; i++) 
                {
                    string tmp = "场景" + i.ToString();
                    if (!scenename.Contains(tmp)) return tmp;
                }
                return "";
            }
        }

        public string WWgetCorrectName(string _name)
        {
            string tmp = _name;
            int cnt = 1;
            while(!WWcheckName(tmp))
            {
                tmp = _name + cnt.ToString();
                cnt++;
            }
            return tmp;
        }

        public bool WWcheckName(string _name)
        {
            return !scenename.Contains(_name);
        }

        public static WWproj WWloadProj(string path)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(WWproj));
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
            WWproj res = (WWproj)xmlSerializer.Deserialize(fs);
            fs.Close();
            res.path = System.IO.Path.GetDirectoryName(path) + '\\';
            return res;
        }
        
    }
}
