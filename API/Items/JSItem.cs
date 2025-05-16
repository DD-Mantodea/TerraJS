using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using System.IO;
using Jint.Native;
using Jint;

namespace TerraJS.API.Items
{
    [Autoload(false)]
    public class JSItem : ModItem
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

        public override void SetDefaults() => InvokeDelegate("SetDefaults", Item);

        public override void UpdateAccessory(Player player, bool hideVisual) => InvokeDelegate("UpdateAccessory", player, hideVisual);

        public override bool? UseItem(Player player) => InvokeDelegate("UseItem", player);
    }
}
