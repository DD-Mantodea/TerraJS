using Newtonsoft.Json;

namespace TerraJS.ModPacks
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ModPackConfig
    {
        [JsonProperty]
        public string ID { get; set; }

        [JsonProperty]
        public string Author { get; set; }
    }
}
