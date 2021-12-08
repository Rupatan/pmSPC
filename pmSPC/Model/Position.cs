using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pmSPC.Model
{
    public class Position 
    {
        [JsonProperty("Ссылка")]
        public string Id { get; set; }

        [JsonProperty("Наименование")]
        public string Name { get; set; }

    }
}
