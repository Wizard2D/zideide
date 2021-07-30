using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Management.Automation;
using System.IO;
using Newtonsoft.Json;
using LibGit2Sharp;

namespace ZIDE
{
    public enum WorkloadType
    {
        NET,
        C,
        CPP,
        LUA,
        RECT
    }
    public static class WorkloadMan
    {
        public static void AddWorkloadToFile(WorkloadType type)
        {
            using(var file = new StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Zide", "InstalledWorkloads.json")))
            {
                var result = JsonConvert.SerializeObject(type);
                if (result == null)
                    System.Windows.Forms.MessageBox.Show("ZIDE Error: Unsuccessful Attempt to write to InstalledWorkloads.json file. 001");
                file.WriteLine(result);
            }
        }
        public static void InstallWorkload(WorkloadType type)
        {
            switch (type) {
                case WorkloadType.NET:
                    AddWorkloadToFile(type);
                    using(var client = new System.Net.WebClient())
                    {
                        client.DownloadFile(@"https://download.visualstudio.microsoft.com/download/pr/9b564b8a-a26d-4bb3-8f30-1101ae71a55a/2cee4f9e4c0e77e3d0f866fdf690864e/windowsdesktop-runtime-5.0.8-win-x64.exe", "dotnetfx.exe");
                        Process proc = Process.Start("dotnetfx.exe");
                        proc.Exited += (o, i) =>
                        {
                            File.Delete("dotnetfx.exe");
                        };
                    }
                    break;
            }
        }
    }
}
