using Microsoft.Extensions.Logging;

namespace Eshva.Caching.Abstractions.Tests.InProcess.Features.CacheInvalidation;

internal sealed class TestCacheInvalidation : TimeBasedCacheInvalidation {
  public TestCacheInvalidation(
    TimeSpan expiredEntriesPurgingInterval,
    CacheEntryExpiryCalculator expiryCalculator,
    TimeProvider timeProvider,
    ILogger<TestCacheInvalidation> logger,
    ManualResetEventSlim purgingSignal)
    : base(
      expiredEntriesPurgingInterval,
      expiryCalculator,
      timeProvider,
      logger) {
    _purgingSignal = purgingSignal;
  }

  public int NumberOfPurgeStarted { get; private set; }

  protected override Task<CacheInvalidationStatistics> DeleteExpiredCacheEntries(CancellationToken token) {
    Logger.LogDebug("Thread ID: {TreadId}", Environment.CurrentManagedThreadId);
    NumberOfPurgeStarted++;
    _purgingSignal.Set();
    return Task.FromResult(new CacheInvalidationStatistics());
  }

  private readonly ManualResetEventSlim _purgingSignal;
}
