using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LearnTsinghua.Services;

namespace LearnTsinghua.Models
{
    public class AppConfig : IResource
    {
        [JsonProperty(PropertyName = "user.id")]
        public string UserId { get; set; }

        public string DocId()
        {
            return "config";
        }

        public const string RESOURCE_TYPE = "config";

        public string ResourceType()
        {
            return RESOURCE_TYPE;
        }

        public static AppConfig Get()
        {
            return Database.Get<AppConfig>(new AppConfig().DocId());
        }

        public override string ToString()
        {
            return JObject.FromObject(this).ToString();
        }
    }
}
