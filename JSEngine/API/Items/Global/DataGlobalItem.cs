using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TerraJS.Contents.UI;
using TerraJS.JSEngine.API.Items.Extensions;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TerraJS.JSEngine.API.Items.Global
{
    public class DataGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public JObject Nbt;

        public override void LoadData(Item item, TagCompound tag)
        {
            Nbt = [];

            if (tag.TryGet<string>("terrajs:nbt", out var json) && json != "")
                Nbt = JObject.Parse(json);
        }

        public override void SaveData(Item item, TagCompound tag)
        {
            tag.Set("terrajs:nbt", (Nbt ?? new JObject()).ToString(), true);
        }

        public override void UpdateInventory(Item item, Player player)
        {
            Nbt ??= [];
        }

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            Nbt ??= [];
        }

        public override bool CanStack(Item destination, Item source)
        {
            var nbt1 = destination.GetNbt().ToString();

            var nbt2 = source.GetNbt().ToString();

            return destination.GetNbt().ToString() == source.GetNbt().ToString();
        }

        public override bool CanStackInWorld(Item destination, Item source)
        {
            return destination.GetNbt().ToString() == source.GetNbt().ToString();
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var nbt = item.GetNbt();

            var lang = Language.ActiveCulture;

            tooltips.Add(new(TerraJS.Instance, "itemNbt", $"Nbt: {nbt?.Count ?? 0}") { OverrideColor = Color.Gray });

            if (UserInput.Shift)
                tooltips.Add(new(TerraJS.Instance, "nbtContent", nbt.ToString()) { OverrideColor = Color.Gray });
            else
                tooltips.Add(new(TerraJS.Instance, "pressShift", lang.LegacyId == 7 ? "按 Shift 键显示" : "Press Shift to show.") { OverrideColor = Color.Gray });
        }
    }
}
