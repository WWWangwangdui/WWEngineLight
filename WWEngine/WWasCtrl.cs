using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace WWEngineCC
{
    public static class WWasCtrl
    {
        private static Dictionary<string, WWassetBase> Name_Asset = new Dictionary<string, WWassetBase>();
        private static Dictionary<int, WWassetBase> ID_Asset = new Dictionary<int, WWassetBase>();
        public static WWassetBase WWgetAsset(string name)
        {
            if (Name_Asset.ContainsKey(name)) return Name_Asset[name];
            return null;
        }

        public static void WWloadAssets()
        {
            ID_Asset.Clear();
            Name_Asset.Clear();
            string path = WWDirector.WWProject.AssetsPath;
            if (!Directory.Exists(path)) return;
            XmlSerializer xml = new XmlSerializer(typeof(AsBitmap));
            List<string> errors = new List<string>();
            foreach (string item in Directory.GetFiles(path,"*.WWBitmap",SearchOption.AllDirectories))
            {
                try
                {
                    using (FileStream fs = new FileStream(item, FileMode.Open))
                    {
                        AsBitmap NEW = xml.Deserialize(fs) as AsBitmap;
                        ID_Asset.Add(NEW.AssetID, NEW);
                        fs.Close();
                    }
                }
                catch
                {
                    errors.Add(item);
                }
            }
            xml = new XmlSerializer(typeof(AsAnimation));
            foreach (string item in Directory.GetFiles(path, "*.WWAnimation", SearchOption.AllDirectories))
            {
                try
                {
                    using (FileStream fs = new FileStream(item, FileMode.Open))
                    {
                        AsAnimation NEW = xml.Deserialize(fs) as AsAnimation;
                        ID_Asset.Add(NEW.AssetID, NEW);
                        fs.Close();
                    }
                }
                catch
                {
                    errors.Add(item);
                }
            }
            if(errors.Count!=0)
            {
                string tmp = "";
                foreach (string item in errors)
                {
                    tmp += item + "\r\n";
                }
                MessageBox.Show("以下资源未正常加载:\r\n" + tmp);
            }
        }

        public static WWassetBase WWgetAsset(int id)
        {
            if (ID_Asset.ContainsKey(id)) return ID_Asset[id];
            return null;
        }

        public static bool WWassetRename(string s1, string s2, WWassetBase obj)
        {
            if (s2 == null || Name_Asset.ContainsKey(s2)) return false;
            if (s1 == null)
            {
                Name_Asset.Add(s2, obj);
                return true;
            }
            if (Name_Asset.ContainsKey(s1))
            {
                if (Name_Asset[s1] != obj)
                    return false;
                Name_Asset.Remove(s1);
                Name_Asset.Add(s2, obj);
                return true;
            }
            else
            {
                Name_Asset.Add(s2, obj);
                return true;
            }
        }
    }
}
