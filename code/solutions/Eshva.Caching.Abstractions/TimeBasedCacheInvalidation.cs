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
  /// <param name="settings">Time-based cache invalidation settings.</param>
  /// <param name="minimalExpiredEntriesPurgingInterval">Minimal purging interval allowed.</param>
  /// <param name="timeProvider">Time provider.</param>
  /// <param name="logger">Logger.</param>
  /// <exception cref="ArgumentNullException">
  /// Value of a required parameter isn't specified.
  /// </exception>
  /// <exception cref="ArgumentOutOfRangeException">
  /// <paramref name="settings"/>.ExpiredEntriesPurgingInterval is less than
  /// <paramref name="minimalExpiredEntriesPurgingInterval"/>.
  /// </exception>
  protected TimeBasedCacheInvalidation(
    TimeBasedCacheInvalidationSettings settings,
    TimeSpan minimalExpiredEntriesPurgingInterval,
    TimeProvider timeProvider,
    ILogger? logger = null) {
    if (settings is null) throw new ArgumentNullException(nameof(settings));

    if (settings.ExpiredEntriesPurgingInterval < minimalExpiredEntriesPurgingInterval) {
      throw new ArgumentOutOfRangeException(
        nameof(settings),
        $"Expired entries purging interval {settings.ExpiredEntriesPurgingInterval} is less "
        + $"than minimal allowed value {minimalExpiredEntriesPurgingInterval}.");
    }

    _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    Logger = logger ?? new NullLogger<TimeBasedCacheInvalidation>();
    ExpiryCalculator = new CacheEntryExpiryCalculator(settings.DefaultSlidingExpirationInterval, timeProvider);
    _cacheInvalidatedAt = _timeProvider.GetUtcNow();
  }

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

    if (!ShouldPurgeEntries()) return;

    try {
      _cacheInvalidatedAt = _timeProvider.GetUtcNow();
      _ = Task.Run(() => DeleteExpiredCacheEntries(token), token);
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
  /// Cache entry expiry calculator.
  /// </summary>
  /// <remarks>
  /// Derived types should implement this method and use <see cref="ExpiryCalculator"/> to handle cache entries expiration
  /// calculation.
  /// </remarks>
  protected CacheEntryExpiryCalculator ExpiryCalculator { get; }

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
    if (timePassedSinceTheLastPurging < _settings.ExpiredEntriesPurgingInterval) {
      Logger.LogDebug(
        "Since the last cache invalidation {TimePassed} has passed that is less than {PurgingInterval}. Purging is not required",
        timePassedSinceTheLastPurging,
        _settings.ExpiredEntriesPurgingInterval);
      return false;
    }

    Logger.LogDebug(
      "Since the last cache invalidation {TimePassed} has passed that is greeter than or equals to {PurgingInterval}. Purging is required",
      timePassedSinceTheLastPurging,
      _settings.ExpiredEntriesPurgingInterval);
    return true;
  }

  private readonly TimeBasedCacheInvalidationSettings _settings;
  private readonly TimeProvider _timeProvider;
  private DateTimeOffset _cacheInvalidatedAt;
  private int _isPurgingInProgress;
}
