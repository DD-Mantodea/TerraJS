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

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            string text = $"damage：{Item.damage}\n" +
                    $"width：{Item.width}\n" +
                    $"height：{Item.height}\n" +
                    $"useTime：{Item.useTime}\n" +
                    $"useAnimation：{Item.useAnimation}\n" +
                    $"useStyle：{Item.useStyle}\n" +
                    $"knockBack：{Item.knockBack}\n" +
                    $"value：{Item.value}\n" +
                    $"rare：{Item.rare}\n" +
                    $"type：{Item.type}\n" +
                    $"LuaLoaderItemtype：{Type}\n" +
                    $"damageType：{Item.DamageType} \n" +
                    $"autoReuse：{Item.autoReuse}\n";

            spriteBatch.DrawString(FontAssets.MouseText.Value, text, Main.LocalPlayer.Center + new Vector2(200, -300) - Main.screenPosition, Color.White);

            return true;
        }
    }
}
