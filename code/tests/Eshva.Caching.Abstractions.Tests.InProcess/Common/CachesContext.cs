using Eshva.Caching.Abstractions.Tests.InProcess.Features.BufferDistributedCache;
using Eshva.Caching.Abstractions.Tests.InProcess.Features.CacheInvalidation;
using Meziantou.Extensions.Logging.Xunit.v3;
using Microsoft.Extensions.Time.Testing;
using Xunit;

namespace Eshva.Caching.Abstractions.Tests.InProcess.Common;

internal class CachesContext {
  public CachesContext(ITestOutputHelper logger) {
    var now = DateTimeOffset.UtcNow;
    Today = new DateTimeOffset(
      now.Year,
      now.Month,
      now.Day,
      hour: 0,
      minute: 0,
      second: 0,
      TimeSpan.Zero);
    TimeProvider = new FakeTimeProvider(Today);
    Logger = logger;
  }

  public ITestOutputHelper Logger { get; }

  public DateTimeOffset Today { get; }

  public TimeSpan PurgingInterval { get; set; }

  public TimeSpan MaximalPurgingDuration { get; set; }

  public TimeSpan DefaultSlidingExpirationInterval { get; set; }

  public FakeTimeProvider TimeProvider { get; set; }

  public CacheEntryExpiryCalculator ExpiryCalculator { get; set; } = null!;

  public TestInMemoryCache Cache { get; private set; } = null!;

  public ManualResetEventSlim PurgingSignal { get; set; } = new(initialState: false);

  public TestInMemoryCacheDatastore CacheDatastore { get; set; } = null!;

  public TestCacheInvalidation CacheInvalidation { get; set; } = null!;

  public byte[]? GottenCacheEntryValue { get; set; } = [];

  public void CreateAndAssignCacheServices() {
    ExpiryCalculator = new CacheEntryExpiryCalculator(DefaultSlidingExpirationInterval, TimeProvider);

    CacheInvalidation = new TestCacheInvalidation(
      PurgingInterval,
      MaximalPurgingDuration,
      ExpiryCalculator,
      TimeProvider,
      XUnitLogger.CreateLogger<TestCacheInvalidation>(Logger),
      PurgingSignal);

    CacheDatastore = new TestInMemoryCacheDatastore();

    Cache = new TestInMemoryCache(
      CacheInvalidation,
      CacheDatastore,
      XUnitLogger.CreateLogger<TestInMemoryCache>(Logger));
  }
}
