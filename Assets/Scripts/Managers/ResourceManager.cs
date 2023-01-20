using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class ResourceManager
{
    static Dictionary<string, object[]> loadedDirectiories = new Dictionary<string, object[]>();
    static Dictionary<string, object> loadedObjects = new Dictionary<string, object>();

    public static IEnumerable<T> LoadAll<T>(string path) where T : Object
    {
        if (loadedDirectiories.ContainsKey(path))
        {
            return loadedDirectiories[path].Cast<T>();
        }

        T[] loaded = Resources.LoadAll<T>(path);
        loadedDirectiories.Add(path, loaded);
        return loaded;
    }

    public static T Load<T>(string path) where T : Object
    {
        if (loadedDirectiories.ContainsKey(path))
        {
            return loadedObjects[path] as T;
        }

        T loaded = Resources.Load<T>(path);
        loadedObjects.Add(path, loaded);
        return loaded;
    }
}
