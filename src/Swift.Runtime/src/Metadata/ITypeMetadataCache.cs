// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;

namespace Swift.Runtime;

/// <summary>
/// Represents an interface that defines how a cache for TypeMetadata should operate.
/// The implementation of this interface needs to be thread safe.
/// </summary>
public interface ITypeMetadataCache
{
    /// <summary>
    /// Returns true if and only if the cache contains an entry for t
    /// </summary>
    /// <param name="t">The type to look up in the cache</param>
    /// <returns>true if and only if the cache contains t, false otherwise</returns>
    bool Contains(Type t);

    /// <summary>
    /// Returns true if the cache contains an entry for t and sets metadata to the resulting value,
    /// otherwise it returns false and metadata will be null.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="metadata"></param>
    /// <returns>true if the lookup was successful, false otherwise</returns>
    bool TryGet(Type t, [NotNullWhen(true)]out TypeMetadata? metadata);

    /// <summary>
    /// Gets the TypeMetadata for the given Type t or if it is not present,
    /// adds it to the cache using the given factory to generate the value.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="metadataFactory"></param>
    /// <returns>The TypeMetadata associated with the give Type t</returns>
    TypeMetadata GetOrAdd(Type t, Func<Type, TypeMetadata> metadataFactory);
}