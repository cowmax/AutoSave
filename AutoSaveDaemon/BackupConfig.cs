using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSaveDaemon
{
    class BackupConfig
    {
        Dictionary<string, string> tag2DirMap = new Dictionary<string, string>();
        List<string> backupFileTypes = new List<string>();

        private string sourceDir = null;
        private string defaultBakDir = null;

        public BackupConfig(string backupDir)
        {
            this.defaultBakDir = backupDir;
        }

        public string SourceDir
        {
            get
            {
                return sourceDir;
            }

            set
            {
                sourceDir = value;
            }
        }

        public string DefaultBakDir
        {
            get
            {
                return defaultBakDir;
            }

            set
            {
                defaultBakDir = value;
            }
        }

        public bool addBackupFileTypes(string[] filetypes)
        {
            int n = 0;

            foreach(string filetype in filetypes)
            {
                string extype = findBackupFileType(filetype);
                if (extype == null)
                {
                    backupFileTypes.Add(filetype);
                    n++;
                }
            }

            return (n == filetypes.Length);
        }

        public bool addBackupFileType(string filetype)
        {
            bool succ = false;

            string extype = findBackupFileType(filetype);
            if (extype == null)
            {
                backupFileTypes.Add(filetype);
                succ = true;
            }
            return succ;
        }

        public string findBackupFileType(string filetype)
        {
            string extype = null;
            foreach(string t in this.backupFileTypes)
            {
                if (isMatchFileType(t, filetype))
                {
                    extype = filetype;
                    break;
                }
            }
            return extype;
        }

        public bool isBackupFileType(string filetype)
        {
            bool found = false;

            foreach(string t in this.backupFileTypes)
            {
                if (isMatchFileType(t, filetype))
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

        public static bool isMatchFileType(string type1, string type2)
        {
            bool rsl = false;
            char[] cc = new char[] { '.', ' ' };
            string ty1 = type1.ToLower();
            string ty2 = type2.ToLower();
             
            if (ty1 == ty2)
            {
                rsl = true;
            }
            else if (ty1.Trim(cc) == ty2.Trim(cc))
            {
                rsl = true;
            }
            else if (ty1.IndexOf(ty2)>0|| ty2.IndexOf(ty1)>0)
            {
                rsl = true;
            }
            return rsl;
        }


        public string makeBackupFilePath(string srcfilepath)
        {
            string bakfilepath = null;
            string dstDir = null;
            string srcfilename = Path.GetFileName(srcfilepath);
            string tag = getTagInFilename(srcfilename);

            if (tag == null)
            {
                dstDir = this.defaultBakDir;
            }
            else
            {
                dstDir = this.tag2DirMap[tag];
            }
            
            if (!Directory.Exists(dstDir))
            {
                Directory.CreateDirectory(dstDir);
            }

            bakfilepath = dstDir + "/" + srcfilename;

            return bakfilepath;
        }

        public string getTagInFilename(string filename)
        {
            string tag = null;
            string fname = filename.ToLower();
            
            foreach(string t in tag2DirMap.Keys)
            {
                if (fname.IndexOf(t) >= 0)
                {
                    tag = t;
                    break;
                }
            }

            return tag;
        }

        private string findBackDirByTag(string tag)
        {
            string dir = null;

            if (tag2DirMap.ContainsKey(tag))
            {
                dir = tag2DirMap[tag];
            }
            return dir;
        }

        public bool addBackupDir(string tag, string dir)
        {
            bool succ = false;

            string exdir = findBackDirByTag(tag);
            if (exdir == null)
            {
                tag2DirMap.Add(tag, dir);
                succ = true;
            }

            return succ;
        }
    }
}
