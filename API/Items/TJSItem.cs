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
using Terraria.DataStructures;

namespace TerraJS.API.Items
{
    [Autoload(false)]
    public class TJSItem : ModItem
    {
        public object? InvokeDelegate(string delegateName, object defaultRet, params object[] args)
        {
            if (!ItemAPI.ItemDelegates.TryGetValue(GetType().FullName, out var dict) || !dict.TryGetValue(delegateName, out var @delegate)) return defaultRet;

            var jsArgs = args.Select((obj, i) => JsValue.FromObject(TerraJS.Engine, obj)).ToArray();

            return @delegate.DynamicInvoke(JsValue.Undefined, jsArgs);
        }

        internal static string _texture = "";

        public override string Texture
        {
            get => File.Exists(_texture) ? _texture : "TerraJS/Textures/NULL";
        }

        public override void SetDefaults() => InvokeDelegate("SetDefaults", null, Item);

        public override void UpdateAccessory(Player player, bool hideVisual) => InvokeDelegate("UpdateAccessory", null, player, hideVisual);

        public override bool? UseItem(Player player) => InvokeDelegate("UseItem", true, player) as bool?;

        public override bool CanUseItem(Player player) => (InvokeDelegate("CanUseItem", true, player) as bool?).Value;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
            => (InvokeDelegate("Shoot", player, source, position, velocity, type, damage, knockback) as bool?).Value;
    }
}
