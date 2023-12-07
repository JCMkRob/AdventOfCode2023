using System.Numerics;

namespace AdventOfCode.Support;


// Note, once the file gets large, split into separate files with a folder called: TypeExtensions 
public static class DictionaryExtensions
{
    public static void ProductAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value) 
        where TKey : notnull 
        where TValue : INumber<TValue>
    {
        if (dictionary.TryGetValue(key, out TValue? _))
        {
            dictionary[key] *= value;
        }
        else 
        {
            dictionary.Add(key, value);
        }
    }

    public static void ListAdd<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary, TKey key, TValue value) where TKey : notnull
    {
        if (dictionary.TryGetValue(key, out List<TValue>? v))
        {
            v.Add(value);
        }
        else 
        {
            dictionary.Add(key, [value]);
        }
    }

    public static void SumAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value) 
        where TKey : notnull 
        where TValue : INumber<TValue>
    {
        if (dictionary.TryGetValue(key, out TValue? _))
        {
            dictionary[key] += value;
        }
        else 
        {
            dictionary.Add(key, value);
        }
    }
}