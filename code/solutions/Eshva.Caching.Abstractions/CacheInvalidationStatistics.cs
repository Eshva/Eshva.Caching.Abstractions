using JetBrains.Annotations;

namespace Eshva.Caching.Abstractions;

/// <summary>
/// Cache invalidation statistics.
/// </summary>
/// <param name="PurgedEntriesCount">Cache entries purged.</param>
[PublicAPI]
public readonly record struct CacheInvalidationStatistics(uint PurgedEntriesCount);
