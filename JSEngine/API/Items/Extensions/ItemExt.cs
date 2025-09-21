using Newtonsoft.Json.Linq;
using TerraJS.JSEngine.API.Items.Global;
using Terraria;

namespace TerraJS.JSEngine.API.Items.Extensions
{
    public static class ItemExt
    {
        extension(Item item)
        {
            public JObject GetNbt()
            {
                if (item.TryGetGlobalItem<DataGlobalItem>(out var globalItem))
                    return globalItem.Nbt;

                return null;
            }

            public void SetNbt(JObject nbt)
            {
                if (item.TryGetGlobalItem<DataGlobalItem>(out var globalItem))
                    globalItem.Nbt = nbt;
            }
        }
    }
}
