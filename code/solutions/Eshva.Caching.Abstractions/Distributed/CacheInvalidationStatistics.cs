using JetBrains.Annotations;

namespace Eshva.Caching.Abstractions.Distributed;

/// <summary>
/// Cache invalidation statistics.
/// </summary>
/// <param name="PurgedEntriesCount">Cache entries purged.</param>
/// <param name="TimeTaken">Time taken by the cache invalidation.</param>
[PublicAPI]
public readonly record struct CacheInvalidationStatistics(uint PurgedEntriesCount, TimeSpan TimeTaken);
