using Eshva.Caching.Abstractions.Tests.InProcess.Features.BufferDistributedCache;
using Eshva.Caching.Abstractions.Tests.InProcess.Features.CacheInvalidation;
using Microsoft.Extensions.Time.Testing;
using Xunit.Abstractions;

namespace Eshva.Caching.Abstractions.Tests.InProcess.Common;

internal class CachesContext {
  public CachesContext(ITestOutputHelper xUnitLogger) {
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
    XUnitLogger = xUnitLogger;
  }

  public ITestOutputHelper XUnitLogger { get; }

  public DateTimeOffset Today { get; }

  public TimeSpan PurgingInterval { get; set; }

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
      ExpiryCalculator,
      TimeProvider,
      Meziantou.Extensions.Logging.Xunit.XUnitLogger.CreateLogger<TestCacheInvalidation>(XUnitLogger),
      PurgingSignal);

    CacheDatastore = new TestInMemoryCacheDatastore();

    Cache = new TestInMemoryCache(
      CacheInvalidation,
      CacheDatastore,
      Meziantou.Extensions.Logging.Xunit.XUnitLogger.CreateLogger<TestInMemoryCache>(XUnitLogger));
  }

  public readonly int MaxBufferSize = 1 * 1024 * 1024;
}
