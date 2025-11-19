using Eshva.Caching.Abstractions.Distributed;
using Microsoft.Extensions.Logging;

namespace Eshva.Caching.Abstractions.Tests.InProcess.Features.CacheInvalidation;

internal sealed class InvalidationDurationTestingCacheInvalidation : TimeBasedCacheInvalidation {
  public InvalidationDurationTestingCacheInvalidation(
    TimeSpan expiredEntriesPurgingInterval,
    TimeSpan maximalCacheInvalidationDuration,
    CacheEntryExpiryCalculator expiryCalculator,
    TimeProvider timeProvider,
    ILogger<InvalidationDurationTestingCacheInvalidation> logger,
    ManualResetEventSlim purgingSignal)
    : base(
      expiredEntriesPurgingInterval,
      maximalCacheInvalidationDuration,
      expiryCalculator,
      timeProvider,
      logger) {
    _expiredEntriesPurgingInterval = expiredEntriesPurgingInterval;
    CacheInvalidationCompleted += (_, _) => purgingSignal.Set();
  }

  protected override async Task<uint> DeleteExpiredCacheEntries(CancellationToken token) {
    await Task.Delay(_expiredEntriesPurgingInterval, token);
    return 0;
  }

  private readonly TimeSpan _expiredEntriesPurgingInterval;
}
