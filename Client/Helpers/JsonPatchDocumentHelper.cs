
using System.Linq;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json.Linq;

namespace CyberSaloon.Client.Helpers
{
    public static class JsonPatchDocumentHelper
    {
        public static JsonPatchDocument CreatePatch<T>(T sourceObject, T destinationObject) where T : class
        {
            var source = JObject.FromObject(sourceObject);
            var destination = JObject.FromObject(destinationObject);

            var patch = new JsonPatchDocument();
            FillPatchForObject(source, destination, patch, "/");

            return patch;
        }

        static void FillPatchForObject(JObject source, JObject destination, JsonPatchDocument patch, string path)
        {
            var sourceKeys = source.Properties().Select(it => it.Name).ToArray();
            var destinationKeys = destination.Properties().Select(it => it.Name).ToArray();

            // Names removed in modified
            foreach (var key in sourceKeys.Except(destinationKeys))
            {
                var property = source.Property(key);
                patch.Remove(path + property.Name);
            }

            // Names added in modified
            foreach (var key in destinationKeys.Except(sourceKeys))
            {
                var property = destination.Property(key);
                patch.Add(path + property.Name, property.Value);
            }

            // Present in both
            foreach (var keys in sourceKeys.Intersect(destinationKeys))
            {
                var sourceProperty = source.Property(keys);
                var destinationProperty = destination.Property(keys);

                if (sourceProperty.Value.Type != destinationProperty.Value.Type)
                {
                    patch.Replace(path + destinationProperty.Name, destinationProperty.Value);
                }
                else 
                    if (
                        !string.Equals(
                                sourceProperty
                                    .Value
                                    .ToString(Newtonsoft.Json.Formatting.None),
                                destinationProperty
                                    .Value
                                    .ToString(Newtonsoft.Json.Formatting.None)
                        )
                    )
                {
                    if (sourceProperty.Value.Type == JTokenType.Object)
                    {
                        // Recurse into objects
                        FillPatchForObject(sourceProperty.Value as JObject, destinationProperty.Value as JObject, patch, path + destinationProperty.Name + "/");
                    }
                    else
                    {
                        // Replace values directly
                        patch.Replace(path + destinationProperty.Name, destinationProperty.Value);
                    }
                }
            }
        }
    }
}