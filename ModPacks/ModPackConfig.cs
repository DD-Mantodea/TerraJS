using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
