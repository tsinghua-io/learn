using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Couchbase.Lite;

namespace LearnTsinghua.Extensions
{
    public static class Couchbase_Object
    {
        public static T getObject<T>(this Database db, string id) where T: class
        {
            var properties = db.GetDocument(id).Properties ?? new Dictionary<string, object>();
            return JObject.FromObject(properties).ToObject<T>();
        }

        public static T getExistingObject<T>(this Database db, string id) where T: class
        {
            var properties = db.GetExistingLocalDocument(id);
            return properties == null ? null : JObject.FromObject(properties).ToObject<T>();
        }

        public static void putObject<T>(this Database db, string id, T obj)
        {
            db.GetDocument(id).Update(newRevision =>
                {
                    var properties = JObject.FromObject(newRevision.Properties ?? new Dictionary<string, object>());
                    properties.Merge(JObject.FromObject(obj));
                    newRevision.Properties = properties.ToObject<Dictionary<string, object>>();
                    return true;
                });
        }
    }
}

