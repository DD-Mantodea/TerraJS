using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint.Native;
using TerraJS.API.Items;
using Terraria;
using Terraria.ModLoader;

namespace TerraJS.API.Projectiles
{
    [Autoload(false)]
    public class JSProjectile : ModProjectile
    {
        public void InvokeDelegate(string delegateName, params object[] args)
        {
            if (!ItemAPI.ItemDelegates.ContainsKey(Name)) return;

            if (ItemAPI.ItemDelegates[Name].TryGetValue(delegateName, out var @delegate))
            {
                var jsArgs = args.Select((obj, i) => JsValue.FromObject(TerraJS.Engine, obj)).ToArray();

                @delegate.DynamicInvoke(JsValue.Undefined, jsArgs);
            }
        }

        internal static string _texture = "";

        public override string Texture
        {
            get => File.Exists(_texture) ? _texture : "TerraJS/Textures/NULL";
        }

        public override void SetDefaults() => InvokeDelegate("SetDefaults", Projectile);
    }
}
