using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using TerraJS.JSEngine.API.Items.Global;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.API.Items.Extensions
{
    public static class ModItemExt
    {
        extension(ModItem modItem)
        {
            public JObject GetNbt()
            {
                if (modItem.Item.TryGetGlobalItem<DataGlobalItem>(out var globalItem))
                    return globalItem.Nbt;

                return null;
            }

            public void SetNbt(JObject nbt)
            {
                if (modItem.Item.TryGetGlobalItem<DataGlobalItem>(out var globalItem))
                    globalItem.Nbt = nbt;
            }
        }
    }
}
