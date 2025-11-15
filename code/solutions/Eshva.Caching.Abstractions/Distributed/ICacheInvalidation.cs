using JetBrains.Annotations;

namespace Eshva.Caching.Abstractions.Distributed;

/// <summary>
/// Contract of a cache invalidation.
/// </summary>
[PublicAPI]
public interface ICacheInvalidation {
  /// <summary>
  /// Execute scan for expired cache entries if required.
  /// </summary>
  void PurgeEntriesIfRequired();
}
