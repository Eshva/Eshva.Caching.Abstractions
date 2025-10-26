using Microsoft.Extensions.Time.Testing;
using Xunit.Abstractions;

namespace Eshva.Caching.Abstractions.BehaviorTests.Common;

public class CachesContext {
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

  public TimeSpan ExpiredEntriesPurgingInterval { get; set; }

  public TimeSpan DefaultSlidingExpirationInterval { get; set; }

  public FakeTimeProvider TimeProvider { get; }

  public byte[]? GottenCacheEntryValue { get; set; } = [];
}
