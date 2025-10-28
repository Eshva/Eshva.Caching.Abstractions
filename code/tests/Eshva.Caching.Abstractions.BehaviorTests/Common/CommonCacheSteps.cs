using Reqnroll;

namespace Eshva.Caching.Abstractions.BehaviorTests.Common;

[Binding]
public class CommonCacheSteps {
  public CommonCacheSteps(CachesContext cachesContext) {
    _cachesContext = cachesContext;
  }

  [Given("clock set at today (.*)")]
  public void GivenClockSetAtToday(TimeSpan timeOfDay) =>
    _cachesContext.TimeProvider.AdjustTime(_cachesContext.Today + timeOfDay);

  [Given("minimal expired entries purging interval is {double} minutes")]
  public void GivenMinimalExpiredEntriesPurgingIntervalIsDoubleMinutes(double minutes) =>
    _cachesContext.MinimalPurgingInterval = TimeSpan.FromMinutes(minutes);

  [Given("purging interval is {double} minutes")]
  public void GivenPurgingIntervalIsDoubleMinutes(double purgingInterval) =>
    _cachesContext.PurgingInterval = TimeSpan.FromMinutes(purgingInterval);

  [Given("default sliding expiration interval is {double} minutes")]
  public void GivenDefaultSlidingExpirationIntervalIsDoubleMinutes(double defaultSlidingExpirationInterval) =>
    _cachesContext.DefaultSlidingExpirationInterval = TimeSpan.FromMinutes(defaultSlidingExpirationInterval);

  [Given("time passed by {double} minutes")]
  public void GivenTimePassedByDoubleMinutes(double minutes) =>
    _cachesContext.TimeProvider.Advance(TimeSpan.FromMinutes(minutes));

  private readonly CachesContext _cachesContext;
}
