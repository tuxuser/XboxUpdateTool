namespace XboxUpdateTool.Models
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class IsUpdateAvailableRequest
    {
        [JsonProperty("UpdateMode")]
        public int UpdateMode { get; set; }

        // Example: 73de1908-f71b-4c5c-821e-ed00a426e221
        [JsonProperty("ContentId")]
        public string ContentId { get; set; }

        // Example: 10.0.18362.4050.8577043e-0a4f-42e1-acec-56e7649f21dc
        [JsonProperty("VersionId")]
        public string VersionId { get; set; }

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