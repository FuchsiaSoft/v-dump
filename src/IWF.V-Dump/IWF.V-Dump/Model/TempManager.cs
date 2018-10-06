using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWF.V_Dump.Model
{
    //We need this here just to keep tabs on the temp folder
    //that will hold all of our working files, this way we
    //can access it on application shutdown
    public static class TempManager
    {
        static TempManager()
        {
            SetNewWorkingDir();
        }

        public static List<DirectoryInfo> PreviousDirs { get; set; } = new List<DirectoryInfo>();

        public static DirectoryInfo WorkingDir { get; set; }

        public static void FinalCleanUp()
        {
            //TODO: this doesn't do anything if it can't delete temp files
            //but we probably should handle that somehow

            if (WorkingDir != null) PreviousDirs.Add(WorkingDir);

            foreach (DirectoryInfo dir in PreviousDirs)
            {
                try
                {
                    dir.Delete(true);
                }
                catch (Exception) { }
            }
        }

        public static void SetNewWorkingDir()
        {
            if (WorkingDir != null) PreviousDirs.Add(WorkingDir);

            string tempPath = Path.GetTempPath();
            tempPath += "\\" + Guid.NewGuid().ToString();
            WorkingDir = new DirectoryInfo(tempPath);
            WorkingDir.Create();
        }
    }
}
