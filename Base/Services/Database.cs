using System;
using Newtonsoft.Json;
using Couchbase.Lite;

namespace LearnTsinghua.Services
{
    public interface IDatabase
    {
        T GetDocument<T>(string documentId) where T: class;
    }

    public class Database
    {
        const string DATABASE_PREFIX = "learn-tsinghua-";
        Couchbase.Lite.Database db;

        public Database()
        {
        }

        void SetupViews()
        {
            
        }

        T GetObject<T>(string docId) where T : class
        {
            var doc = db.GetExistingDocument(docId);
            if (doc == null)
                return null;

            doc.Properties.ToString
        }

    }
}

