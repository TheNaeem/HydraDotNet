using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace HydraDotNet.Core.Extensions;

public static class DictionaryExtensions
{
    /// <summary>
    /// Main purpose is to make getting types from Hydra decoded binary responses cleaner and easier.
    /// </summary>
    /// <typeparam name="T">Desired type.</typeparam>
    public static bool TryGetValueAs<K, V, T>(this IDictionary<K, V> dict, K key, [NotNullWhen(returnValue: true)] out T? ret) 
    {
        if (!dict.TryGetValue(key, out var value) || value is not T val)
        {
            ret = default;
            return false;
        }

        ret = val;
        return true;
    }

    /// <summary>
    /// Main purpose is to make getting user defined type properties from Hydra decoded binary responses cleaner and easier.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetNested<K, V>(this IDictionary<K, V> dict, K key, [NotNullWhen(returnValue: true)] out Dictionary<object, object?>? ret)
        => dict.TryGetValueAs(key, out ret);
}
