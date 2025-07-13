using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NetSimplified;
using NetSimplified.Syncing;
using TerraJS.Contents.Attributes;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerraJS.Networks
{
    [HideToJS]
    [AutoSync]
    public class CustomModCheckPacket : NetModule
    {
        private int _type;

        private string SHA;

        public static CustomModCheckPacket Get(ModCheckType type)
        {
            var module = NetModuleLoader.Get<CustomModCheckPacket>();

            module._type = (int)type;

            module.SHA = type == ModCheckType.Script ?
                string.Join("||", TerraJS.SHAManager.ScriptsSHA.Select(p => $"{p.Key}:{p.Value}")) :
                string.Join("||", TerraJS.SHAManager.TexturesSHA.Select(p => $"{p.Key}:{p.Value}"));

            return module;
        }

        public override void Receive()
        {
            if (!CanConnect())
            {
                NetMessage.BootPlayer(Sender, NetworkText.FromLiteral(Lang.mp[4].Value + $" {Enum.GetName((ModCheckType)_type)} SHA256 code not match."));
            }
        }

        public bool CanConnect()
        {
            var shaList = SHA.Split("||").ToList();

            var type = (ModCheckType)_type;

            switch (type)
            {
                case ModCheckType.Script:

                    if (TerraJS.SHAManager.ScriptsSHA.Count != shaList.Count)
                        return false;

                    foreach (var sha in TerraJS.SHAManager.ScriptsSHA)
                    {
                        if (!shaList.Contains($"{sha.Key}:{sha.Value}"))
                            return false;
                    }

                    break;

                case ModCheckType.Texture:

                    if (TerraJS.SHAManager.TexturesSHA.Count != shaList.Count)
                        return false;

                    foreach (var sha in TerraJS.SHAManager.TexturesSHA)
                    {
                        if (!shaList.Contains($"{sha.Key}:{sha.Value}"))
                            return false;
                    }

                    break;
            }

            return true;
        }
    }

    public enum ModCheckType
    {
        Script,
        Texture
    }
}
