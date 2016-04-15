using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Couchbase.Lite;
using RestSharp.Authenticators;
using LearnTsinghua.Models;

namespace LearnTsinghua.Services
{
    public static class Database
    {
        const string DATABASE_NAME = "learn-tsinghua";

        public static Couchbase.Lite.Database Db { get; set; }

        static Database()
        {
            Db = Manager.SharedInstance.GetDatabase(DATABASE_NAME);
        }

        public static T Get<T>(string id)
        {
            var properties = Db.GetDocument(id).Properties ?? new Dictionary<string, object>();
            return JObject.FromObject(properties).ToObject<T>();
        }

        public static T GetExisting<T>(string id) where T: class
        {
            var doc = Db.GetExistingDocument(id);
            if (doc == null)
                return null;

            var properties = doc.Properties ?? new Dictionary<string, object>();
            return JObject.FromObject(properties).ToObject<T>();
        }

        public static void Set(string id, object obj, string type = null)
        {
            Db.GetDocument(id).Update(newRevision =>
                {
                    var properties = newRevision.Properties;
                    foreach (var pair in JObject.FromObject(obj))
                        properties[pair.Key] = pair.Value;
                    if (type != null)
                        properties["type"] = type;
                    return true;
                });
        }

        public static void Delete(string id)
        {
            Db.DeleteLocalDocument(id);
        }
    }
}

