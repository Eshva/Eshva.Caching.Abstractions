using Eshva.Caching.Abstractions.Distributed;
using Microsoft.Extensions.Logging;

namespace Eshva.Caching.Abstractions.Tests.InProcess.Common;

internal sealed class TestCacheInvalidation : TimeBasedCacheInvalidation {
  public TestCacheInvalidation(
    TimeSpan expiredEntriesPurgingInterval,
    TimeSpan maximalCacheInvalidationDuration,
    CacheEntryExpiryCalculator expiryCalculator,
    TimeProvider timeProvider,
    ILogger<TestCacheInvalidation> logger,
    ManualResetEventSlim purgingSignal)
    : base(
      expiredEntriesPurgingInterval,
      maximalCacheInvalidationDuration,
      expiryCalculator,
      timeProvider,
      logger) {
    CacheInvalidationCompleted += (_, _) => purgingSignal.Set();
  }

  public int NumberOfPurgeStarted { get; private set; }

  protected override Task<uint> DeleteExpiredCacheEntries(CancellationToken token) {
    NumberOfPurgeStarted++;
    return Task.FromResult(0U);
  }
}
