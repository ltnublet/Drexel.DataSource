using System;
using System.Collections.Generic;

namespace Drexel.DataSources.FolderData
{
    internal static class ExtensionMethods
    {
        public static object GetOrDefault<T, U>(this Dictionary<T, U> dictionary, T key, Func<U> defaultValue)
        {
            if (!dictionary.TryGetValue(key, out U result))
            {
                result = defaultValue.Invoke();
            }

            return result;
        }
    }
}
