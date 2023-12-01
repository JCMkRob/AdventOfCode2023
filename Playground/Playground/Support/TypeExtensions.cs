using System.Numerics;

namespace AdventOfCode.Support;

public static class DictionaryExtensions
{
    public static void ProductAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value) where TKey : notnull where TValue : INumber<TValue>
    {
        if (dictionary.ContainsKey(key))
        {   
            dictionary[key] *= value;
        }
        else 
        {
            dictionary.Add(key, value);
        }
    }

    public static void SimpleAdd<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary, TKey key, TValue value) where TKey : notnull
    {
        if (dictionary.ContainsKey(key))
        {   
            dictionary[key].Add(value);
        }
        else 
        {
            dictionary.Add(key, new List<TValue> { value });
        }
    }
}