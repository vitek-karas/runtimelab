// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Swift.Runtime;

/// <summary>
/// Internal implementation of ITypeMetadataCache
/// </summary>
internal class TypeMetadataCache : ITypeMetadataCache
{
    ConcurrentDictionary<Type, TypeMetadata> cache = new();

    /// <summary>
    /// Constructs an empty cache.
    /// </summary>
    public TypeMetadataCache()
    {

    }

    /// <summary>
    /// Constructs a cache with the supplied initial values.
    /// </summary>
    /// <param name="initalValues">An enumeration of tuples of Type and TypeMetadata to initialize the cache</param>
    public TypeMetadataCache(IEnumerable<(Type, TypeMetadata)> initalValues)
    {
        var dictCache = (IDictionary<Type, TypeMetadata>)cache;
        foreach (var (key, value) in initalValues)
        {
            dictCache.Add(key, value);
        }
    }


    /// <summary>
    /// Returns true if and only if the cache contains an entry for t.
    /// </summary>
    /// <param name="t">The type to look up in the cache</param>
    /// <returns>true if and only if the cache contains t, false otherwise</returns>
    public bool Contains(Type t)
    {
        return cache.ContainsKey(t);
    }

    /// <summary>
    /// Returns true if the cache contains an entry for t and sets metadata to the resulting value,
    /// otherwise it returns false and metadata will be null.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="metadata"></param>
    /// <returns>true if the lookup was successful, false otherwise</returns>
    public bool TryGet(Type t, [NotNullWhen(true)] out TypeMetadata? metadata)
    {
        if (cache.TryGetValue(t, out var md))
        {
            metadata = md;
            return true;
        }
        else
        {
            metadata = null;
            return false;
        }
    }

    /// <summary>
    /// Gets the TypeMetadata for the given Type t or if it is not present,
    /// adds it to the cache using the given factory to generate the value.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="metadataFactory"></param>
    /// <returns>The TypeMetadata associated with the give Type t</returns>
    public TypeMetadata GetOrAdd(Type t, Func<Type, TypeMetadata> metadataFactory)
    {
        return cache.GetOrAdd(t, metadataFactory);
    }
}
