using System.Collections.Generic;
using LearnTsinghua.Services;

namespace LearnTsinghua.Models
{
    public interface IResource
    {
        string DocId();

        string ResourceType();
    }

    public static class ResourceExtension
    {
        public static void Save(this IResource resource)
        {
            Database.Set(resource.DocId(), resource, resource.ResourceType());
        }

        public static void Set(this IResource resource, string key, object obj)
        {
            var pair = new Dictionary<string, object>(){ { key, obj } };
            Database.Set(resource.DocId(), pair);
        }

        public static void Delete(this IResource resource)
        {
            Database.Delete(resource.DocId());
        }
    }
}
