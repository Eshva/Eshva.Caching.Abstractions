using JetBrains.Annotations;

namespace Eshva.Caching.Abstractions.Distributed;

/// <summary>
/// Contract of cache invalidation notifier.
/// </summary>
[PublicAPI]
public interface ICacheInvalidationNotifier {
  /// <summary>
  /// Notifies cache invalidation started.
  /// </summary>
  event EventHandler CacheInvalidationStarted;

  /// <summary>
  /// Notifies cache invalidation completed.
  /// </summary>
  event EventHandler<CacheInvalidationStatistics> CacheInvalidationCompleted;
}
