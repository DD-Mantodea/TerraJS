using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReLogic.OS;
using Terraria.UI;

namespace TerraJS.Contents.Utils
{
    public class FileUtils
    {

        public static void CreateDirectoryIfNotExist(string path)
        {
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception e)
                {
                    FancyErrorPrinter.ShowDirectoryCreationFailError(e, path);
                }
            }
        }

        public static void CopyDirectory(string sourceDir, string targetDir, bool recursive = true)
        {
            var dir = new DirectoryInfo(sourceDir);

            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            var dirs = dir.GetDirectories();

            Directory.CreateDirectory(targetDir);

            foreach (var file in dir.GetFiles())
            {
                var targetFilePath = Path.Combine(targetDir, file.Name);

                file.CopyTo(targetFilePath);
            }

            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string subTargetDir = Path.Combine(targetDir, subDir.Name);

                    CopyDirectory(subDir.FullName, subTargetDir, true);
                }
            }
        }

        public static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
                new DirectoryInfo(path).Delete(true);
        }

        public static void OpenDirectory(string path)
        {
            CreateDirectoryIfNotExist(path);

            if (Platform.IsLinux)
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = "xdg-open",
                    Arguments = path,
                    UseShellExecute = true,
                    CreateNoWindow = true
                };

                processStartInfo.EnvironmentVariables["LD_LIBRARY_PATH"] = "/usr/lib:/lib";
                Process.Start(processStartInfo);
            }
            else
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }

        public static void MoveDirectory(string from, string to)
        {
            if (Directory.Exists(from))
                Directory.Move(from, to);
        }

        public static void CopyModFile(string path, string to)
        {
            if (!File.Exists(to))
            {
                try
                {
                    var target = TerraJS.Instance.GetFileBytes(path);

                    var file = File.Create(to);

                    file.Write(target, 0, target.Length);

                    file.Close();
                }
                catch
                {
                    return;
                }
            }
        }
    }
}
