namespace XboxUpdateTool.Models
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class GetSystemUpdatePackageRequest
    {
        // Example: 3
        [JsonProperty("UpdateMode")]
        public int UpdateMode { get; set; }

        // Example: 4
        [JsonProperty("LicenseProtocol")]
        public int LicenseProtocol { get; set; }

        // Example: ["updater.xvd"]
        [JsonProperty("FileIncludeFilter")]
        public string[] FileIncludeFilter { get; set; }

        // Example: 0
        [JsonProperty("IgnoreRootLicense")]
        public int IgnoreRootLicense { get; set; }

        public static JsonSerializerSettings Settings => new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Settings);
        }
    }
}