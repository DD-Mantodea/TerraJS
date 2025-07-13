using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using static TerraJS.Assets.Managers.TextureType;

namespace TerraJS.Assets.Managers
{
    public class TextureManager : AssetManager<Asset<Texture2D>>
    {
        public Dictionary<string, Asset<Texture2D>> Textures;

        public Dictionary<TextureType, Asset<Texture2D>[]> VanillaTextures = [];

        public TerraJS Mod => TerraJS.Instance;

        public override void Load()
        {
            if (Main.netMode is NetmodeID.Server)
                return;

            base.Load();

            if (!TerraJS.IsLoading)
                return;

            foreach (var type in Enum.GetValues<TextureType>())
            {
                if (TryGetVanillaTextures(type, out var textures))
                    VanillaTextures.Add(type, textures);
            }
        }

        public override void LoadOne(string dir, Dictionary<string, Asset<Texture2D>> dictronary)
        {
            var path = Path.Combine(TerraJS.ModPath, dir);

            foreach (var file in Directory.GetFiles(path, "*.png", SearchOption.AllDirectories))
            {
                var key = file.Replace($"{path}\\", "").Replace(".png", "").Replace("\\", "/");

                dictronary.Add(key, TerraJS.Instance.Assets.CreateUntracked<Texture2D>(File.OpenRead(file), file));
            }
        }

        private bool TryGetVanillaTextures(TextureType type, out Asset<Texture2D>[] texture)
        {
            texture = null;

            try
            {
                texture = type switch
                {
                    Items => TextureAssets.Item,
                    NPCs => TextureAssets.Npc,
                    Tiles => TextureAssets.Tile,
                    Buffs => TextureAssets.Buff,
                    Gores => TextureAssets.Gore,
                    Walls => TextureAssets.Wall,
                    Projectiles => TextureAssets.Projectile,
                    Hairs => TextureAssets.PlayerHair,
                    ArmorHeads => TextureAssets.ArmorHead,
                    ArmorArms => TextureAssets.ArmorArm,
                    ArmorBodys => TextureAssets.ArmorBody,
                    ArmorLegs => TextureAssets.ArmorLeg,
                    Wings => TextureAssets.Wings,
                    Extras => TextureAssets.Extra,
                    Cursors => TextureAssets.Cursors,
                    _ => null
                };

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TryGetVanillaTexture(TextureType type, int ID, out Asset<Texture2D> texture)
        {
            texture = null;

            try
            {
                if (VanillaTextures.TryGetValue(type, out var textures))
                {
                    texture = textures[ID];

                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }

    public enum TextureType
    {
        Items,
        NPCs,
        Tiles,
        Buffs,
        Gores,
        Walls,
        Projectiles,
        Hairs,
        ArmorHeads,
        ArmorArms,
        ArmorBodys,
        ArmorLegs,
        Wings,
        Extras,
        Cursors,
        Empty
    }
}
