using System;
using Newtonsoft.Json.Linq;

namespace Our.Umbraco.DocTypeGridEditor.Extensions
{
    internal static class JsonExtensions
    {
        public static void Rename(this JToken token, string newName)
        {
            var parent = token.Parent;

            if (parent == null)
                throw new InvalidOperationException("The parent is missing.");

            var newToken = new JProperty(newName, token);
            parent.Replace(newToken);
        }
    }
}