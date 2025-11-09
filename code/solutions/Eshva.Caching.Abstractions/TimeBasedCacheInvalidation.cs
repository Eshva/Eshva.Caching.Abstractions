using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Eshva.Caching.Abstractions;

/// <summary>
/// Time-based cache invalidation.
/// </summary>
/// <remarks>
/// Cache invalidation is not executed in constant intervals with a timer. It executed if the time passed from the last
/// execution is  greater than configured purging interval.
/// </remarks>
[PublicAPI]
public abstract class TimeBasedCacheInvalidation : ICacheInvalidation, ICacheInvalidationNotifier {
  /// <summary>
  /// Initializes a new instance of a time-based cache invalidation.
  /// </summary>
  /// <param name="expiredEntriesPurgingInterval">Expired entries purging interval.</param>
  /// <param name="expiryCalculator">Cache entry expiry calculator.</param>
  /// <param name="timeProvider">Time provider.</param>
  /// <param name="logger">Logger.</param>
  /// <exception cref="ArgumentNullException">
  /// Value of a required parameter isn't specified.
  /// </exception>
  protected TimeBasedCacheInvalidation(
    TimeSpan expiredEntriesPurgingInterval,
    CacheEntryExpiryCalculator expiryCalculator,
    TimeProvider timeProvider,
    ILogger? logger = null) {
    _expiredEntriesPurgingInterval = expiredEntriesPurgingInterval;
    ExpiryCalculator = expiryCalculator ?? throw new ArgumentNullException(nameof(expiryCalculator));
    _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    Logger = logger ?? new NullLogger<TimeBasedCacheInvalidation>();
    _cacheInvalidatedAt = _timeProvider.GetUtcNow();
  }

  /// <summary>
  /// Cache entry expiry calculator.
  /// </summary>
  /// <remarks>
  /// Derived types should implement this method and use <see cref="ExpiryCalculator"/> to handle cache entries expiration
  /// calculation.
  /// </remarks>
  public CacheEntryExpiryCalculator ExpiryCalculator { get; }

  /// <inheritdoc/>
  public event EventHandler? CacheInvalidationStarted;

  /// <inheritdoc/>
  public event EventHandler<CacheInvalidationStatistics>? CacheInvalidationCompleted;

  /// <inheritdoc/>
  public void PurgeEntriesIfRequired(CancellationToken token = default) {
    const byte purgingInProgress = 1;
    const byte notYetPurging = 0;

    if (Interlocked.CompareExchange(ref _isPurgingInProgress, purgingInProgress, notYetPurging) == purgingInProgress) {
      Logger.LogDebug("Purging already in progress");
      return;
    }

    try {
      if (!ShouldPurgeEntries()) return;

      _cacheInvalidatedAt = _timeProvider.GetUtcNow();
      _ = Task.Run(() => InvalidateCache(token), token);
    }
    finally {
      Interlocked.Exchange(ref _isPurgingInProgress, notYetPurging);
    }
  }

  /// <summary>
  /// Logger.
  /// </summary>
  protected ILogger Logger { get; }

  /// <summary>
  /// Purger logic.
  /// </summary>
  /// <remarks>
  /// Derived types should implement this method and use <see cref="ExpiryCalculator"/> to handle cache entries expiration
  /// calculation.
  /// </remarks>
  /// <param name="cancellation">Cancellation token.</param>
  /// <returns>Cache invalidation statistics.</returns>
  protected abstract Task<CacheInvalidationStatistics> DeleteExpiredCacheEntries(CancellationToken cancellation);

  /// <summary>
  /// Notify cache invalidation started.
  /// </summary>
  private void NotifyPurgeStarted() =>
    CacheInvalidationStarted?.Invoke(this, EventArgs.Empty);

  /// <summary>
  /// Notify cache invalidation completed.
  /// </summary>
  /// <param name="statistics">Cache invalidation statistics.</param>
  private void NotifyPurgeCompleted(CacheInvalidationStatistics statistics) =>
    CacheInvalidationCompleted?.Invoke(this, statistics);

  private async Task InvalidateCache(CancellationToken token = default) {
    CacheInvalidationStatistics statistics = default;
    try {
      NotifyPurgeStarted();
      statistics = await DeleteExpiredCacheEntries(token).ConfigureAwait(continueOnCapturedContext: false);
    }
    finally {
      NotifyPurgeCompleted(statistics);
    }
  }

  private bool ShouldPurgeEntries() {
    var utcNow = _timeProvider.GetUtcNow();
    var timePassedSinceTheLastPurging = utcNow - _cacheInvalidatedAt;
    if (timePassedSinceTheLastPurging < _expiredEntriesPurgingInterval) {
      Logger.LogDebug(
        "Since the last cache invalidation {TimePassed} has passed that is less than {PurgingInterval}. Purging is not required",
        timePassedSinceTheLastPurging,
        _expiredEntriesPurgingInterval);
      return false;
    }

    Logger.LogDebug(
      "Since the last cache invalidation {TimePassed} has passed that is greeter than or equals to {PurgingInterval}. Purging is required",
      timePassedSinceTheLastPurging,
      _expiredEntriesPurgingInterval);
    return true;
  }

  private readonly TimeSpan _expiredEntriesPurgingInterval;
  private readonly TimeProvider _timeProvider;
  private DateTimeOffset _cacheInvalidatedAt;
  private int _isPurgingInProgress;
}
