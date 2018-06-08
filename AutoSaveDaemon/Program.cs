using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSaveDaemon
{
    class Program
    {
        static List<string> promptMessages = new List<string>();
        static BackupConfig bakcfg = new BackupConfig("C:/Users/Jenseng/OneDrive/有推/BAK/");

        static void xprintline(string fmtstr, params object[] prms)
        {
            promptMessages.Add(string.Format(fmtstr, prms));
        }

        static void xprint(string fmtstr, params object[] prms)
        {
            int i = promptMessages.Count;
            if (i == 0)
            {
                promptMessages.Add(string.Format(fmtstr, prms));
            }
            else
            {
                promptMessages[i-1] += string.Format(fmtstr, prms);
            }
        }

        // Print info onto console
        static void printAll()
        {
            foreach (string msg in promptMessages)
            {
                Console.WriteLine(msg + "\n");
            }

            promptMessages.Clear();
        }

        static void Main(string[] args)
        {
            
            bakcfg.addBackupFileType(".doc");
            bakcfg.addBackupFileType(".docx");
            bakcfg.addBackupFileType(".xls");
            bakcfg.addBackupFileType(".xlsx");
            bakcfg.addBackupFileType(".ppt");
            bakcfg.addBackupFileType(".pptx");
            bakcfg.addBackupFileType(".vsd");
            bakcfg.addBackupFileType(".vsdx");
            bakcfg.addBackupFileType(".mmap");

            bakcfg.SourceDir = "C:/Users/Jenseng/Desktop";
            bakcfg.addBackupDir("xin", "C:/Users/Jenseng/OneDrive/有推/BAK/XIN");
            bakcfg.addBackupDir("yot", "C:/Users/Jenseng/OneDrive/有推/BAK/YOT");

            string[] allFiles = scanSourceDir(bakcfg.SourceDir);
            xprint("{0} files was found.", allFiles.Length);

            List<string> srcFiles = getNeedBackupFiles(allFiles);
            xprint(" {0} document.", srcFiles.Count);

            int n = backupFiles(srcFiles);

            printAll();
            Console.ReadKey();

        }

        private static int backupFiles(List<string> srcFiles)
        {
            int n = 0;
            foreach(string srcfile in srcFiles)
            {
                string dstfile = bakcfg.makeBackupFilePath(srcfile);
                FileInfo srcfileinfo = new FileInfo(srcfile);
                FileInfo dstfileinfo = new FileInfo(dstfile);

                if (isNeedToBackup(srcfileinfo, dstfileinfo))
                {
                    try
                    {
                        srcfileinfo.CopyTo(dstfile, true);
                        xprintline("BACKUP [{0}] TO [{1}] COMPLETED.", srcfileinfo.Name, bakcfg.getTagInFilename(srcfileinfo.Name));
                    }
                    catch(IOException ex)
                    {
                        xprintline("FAILD to backup {0}", srcfileinfo.Name);
                    }
                }
            }

            return n;
        }

        private static bool isNeedToBackup(FileInfo srcfileinfo, FileInfo dstfileinfo)
        {
            int sz = 0;
            int tm = -1;
            if (dstfileinfo.Exists)
            {
                xprintline("[{0}] has been backuped before.", srcfileinfo.Name);
                TimeSpan tsp = srcfileinfo.LastWriteTime - dstfileinfo.LastWriteTime;
                tm = tsp.Seconds;
                if (tm == 0)
                {
                    xprintline("[{0}] has NO CHANGE.", srcfileinfo.Name);
                }
                else if (tm > 0)
                {
                    xprintline("[{0}] has BEEN MODIFIED {1} sec. after last backup.", srcfileinfo.Name, tm);
                }
                else
                {
                    xprintline("[{0}]'s backup file IS NEWER {1} sec.", srcfileinfo.Name, tm);
                }

                sz = (int)(srcfileinfo.Length - dstfileinfo.Length);

                if (sz == 0)
                {
                    xprintline("[{0}] has NO SIZE CHANGE", srcfileinfo.Name, sz);
                }
                else if(sz > 0)
                {
                    xprintline("[{0}] has BEEN INCREASED {1} bytes.", srcfileinfo.Name, sz);
                }
                else
                {
                    xprintline("[{0}] has BEEN DECREASED {1} bytes.", srcfileinfo.Name, sz);
                }
                
            }
            else
            {
                xprintline("[{0}] has never been backuped.", srcfileinfo.Name);
            }

            return (!dstfileinfo.Exists) || (sz != 0 && tm > 0);
        }

        private static List<string> getNeedBackupFiles(string[] allFiles)
        {
            List<string> bakfiles = new List<string>();
            foreach(string f in allFiles)
            {
                string filetype = getFileType(f);
                if (bakcfg.isBackupFileType(filetype))
                {
                    bakfiles.Add(f);
                }
            }
            return bakfiles;
        }

        private static string getFileType(string filepath)
        {
            return Path.GetExtension(filepath);
        }

        static string[] scanSourceDir(string srcDir)
        {
            string[] allfiles = null;

            if (!Directory.Exists(srcDir))
            {
                xprintline("Directory ({0}) does not exist!", srcDir);
            }
            else
            {
                 allfiles = Directory.GetFiles(srcDir, "*", SearchOption.TopDirectoryOnly);
            }

            return allfiles;
        }
    }
}
