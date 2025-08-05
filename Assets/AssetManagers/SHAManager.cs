using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TerraJS.Assets.AssetManagers
{
    public class SHAManager : AssetManager<string>
    {
        public Dictionary<string, string> ScriptsSHA;

        public Dictionary<string, string> TexturesSHA;

        public override void LoadOne(string dir, Dictionary<string, string> dictronary)
        {
            using var sha256 = SHA256.Create();

            var type = dir.Replace("SHA", "");

            string[] files;

            string hashBytes;

            files = Directory.GetFiles(Path.Combine(Pathes.TerraJSPath, type), type == "Scripts" ? "*.js" : "*.png", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                using var stream = File.OpenRead(file);

                hashBytes = BitConverter.ToString(sha256.ComputeHash(stream));

                var key = file.Replace(Path.Combine(Pathes.TerraJSPath, type), "");

                dictronary.Add(key, hashBytes);
            }
        }
    }
}
