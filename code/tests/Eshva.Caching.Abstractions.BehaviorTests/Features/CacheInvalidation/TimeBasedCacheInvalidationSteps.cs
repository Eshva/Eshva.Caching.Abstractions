using Eshva.Caching.Abstractions.BehaviorTests.Common;
using FluentAssertions;
using Meziantou.Extensions.Logging.Xunit;
using Microsoft.Extensions.Logging;
using Reqnroll;

namespace Eshva.Caching.Abstractions.BehaviorTests.Features.CacheInvalidation;

[Binding]
public class TimeBasedCacheInvalidationSteps {
  public TimeBasedCacheInvalidationSteps(CachesContext cachesContext, ErrorHandlingContext errorHandlingContext) {
    _cachesContext = cachesContext;
    _errorHandlingContext = errorHandlingContext;
    _purgingSignal = new ManualResetEventSlim(initialState: false);
  }

  [Given("minimal expired entries purging interval is {double} minutes")]
  public void GivenMinimalExpiredEntriesPurgingIntervalIsDoubleMinutes(double minutes) =>
    _minimalPurgingInterval = TimeSpan.FromMinutes(minutes);

  [Given("purging interval is {double} minutes")]
  public void GivenPurgingIntervalIsDoubleMinutes(double purgingInterval) =>
    _purgingInterval = TimeSpan.FromMinutes(purgingInterval);

  [Given("default sliding expiration interval is {double} minutes")]
  public void GivenDefaultSlidingExpirationIntervalIsDoubleMinutes(double defaultSlidingExpirationInterval) =>
    _defaultSlidingExpirationInterval = TimeSpan.FromMinutes(defaultSlidingExpirationInterval);

  [Given("time-based cache invalidation with defined arguments")]
  public void GivenTimeBasedCacheInvalidationWithDefinedArguments() {
    try {
      _sut = new TestCacheInvalidation(
        _purgingInterval,
        _defaultSlidingExpirationInterval,
        _minimalPurgingInterval,
        _cachesContext.TimeProvider,
        XUnitLogger.CreateLogger<TestCacheInvalidation>(_cachesContext.XUnitLogger),
        _purgingSignal);
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  [Given("absolute expiration today at (.*)")]
  public void GivenAbsoluteExpirationTodayAt(TimeSpan absoluteExpirationTime) =>
    _absoluteExpiration = _cachesContext.Today.Add(absoluteExpirationTime);

  [Given("sliding expiration in {int} minutes")]
  public void GivenSlidingExpirationInIntMinutes(int minutes) =>
    _slidingExpiration = TimeSpan.FromMinutes(minutes);

  [Given("time passed by {double} minutes")]
  public void GivenTimePassedByDoubleMinutes(double minutes) =>
    _cachesContext.TimeProvider.Advance(TimeSpan.FromMinutes(minutes));

  [Given("no absolute expiration")]
  public void GivenNoAbsoluteExpiration() =>
    _absoluteExpiration = null;

  [Given("no sliding expiration")]
  public void GivenNoSlidingExpiration() =>
    _slidingExpiration = null;

  [Given("cache entry that expires today at (.*)")]
  public void GivenCacheEntryThatExpiresTodayAt(TimeSpan expiresAt) =>
    _expiresAt = _cachesContext.Today.Add(expiresAt);

  [When("I construct time-based cache invalidation with defined arguments")]
  public void WhenIConstructTimeBasedCacheInvalidationWithDefinedArguments() =>
    GivenTimeBasedCacheInvalidationWithDefinedArguments();

  [When("cache invalidation requested")]
  public void WhenCacheInvalidationRequested() {
    try {
      _sut.PurgeEntriesIfRequired();
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  [When($"a few cache invalidations requested")]
  public void WhenAFewCacheInvalidationsRequested() {
    try {
      _sut.PurgeEntriesIfRequired();
      _sut.PurgeEntriesIfRequired();
      _sut.PurgeEntriesIfRequired();
      _sut.PurgeEntriesIfRequired();
      _sut.PurgeEntriesIfRequired();
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  [Then("purging should be done")]
  public void ThenPurgingShouldBeDone() =>
    _sut.NumberOfPurgeStarted.Should().BeGreaterThan(expected: 0);

  [Then("purging should not start")]
  public void ThenPurgingShouldNotStart() =>
    _sut.NumberOfPurgeStarted.Should().Be(expected: 0);

  [Then("only one purging should be done")]
  public void ThenOnlyOnePurgingShouldBeDone() =>
    _sut.NumberOfPurgeStarted.Should().Be(expected: 1);

  [When("I check is cache entry expired")]
  public void WhenICheckIsCacheEntryExpired() =>
    _isExpired = _sut.IsCacheEntryExpired(_expiresAt);

  [Then("it should be not expired")]
  public void ThenItShouldBeNotExpired() =>
    _isExpired.Should().BeFalse();

  [Then("it should be expired")]
  public void ThenItShouldBeExpired() =>
    _isExpired.Should().BeTrue();

  [When("I calculate expiration time")]
  public void WhenICalculateExpirationTime() =>
    _calculatedExpiration = _sut.CalculateExpiration(_absoluteExpiration, _slidingExpiration);

  [Then("it should be today at (.*)")]
  public void ThenItShouldBeTodayAt(TimeSpan expirationTime) =>
    _calculatedExpiration.Should().Be(_cachesContext.Today.Add(expirationTime));

  [Then("awaited purging is finished")]
  public void ThenAwaitedPurgingIsFinished() =>
    _purgingSignal.Wait(TimeSpan.FromSeconds(seconds: 1));

  private readonly CachesContext _cachesContext;
  private readonly ErrorHandlingContext _errorHandlingContext;
  private readonly ManualResetEventSlim _purgingSignal;
  private DateTimeOffset? _absoluteExpiration;
  private DateTimeOffset _calculatedExpiration;
  private TimeSpan _defaultSlidingExpirationInterval;
  private DateTimeOffset _expiresAt;
  private bool _isExpired;
  private TimeSpan _minimalPurgingInterval;
  private TimeSpan _purgingInterval;
  private TimeSpan? _slidingExpiration;
  private TestCacheInvalidation _sut = null!;

  private sealed class TestCacheInvalidation : TimeBasedCacheInvalidation {
    public TestCacheInvalidation(
      TimeSpan expiredEntriesPurgingInterval,
      TimeSpan defaultSlidingExpirationInterval,
      TimeSpan minimalExpiredEntriesPurgingInterval,
      TimeProvider timeProvider,
      ILogger logger,
      ManualResetEventSlim purgingSignal)
      : base(
        new TimeBasedCacheInvalidationSettings {
          ExpiredEntriesPurgingInterval = expiredEntriesPurgingInterval, DefaultSlidingExpirationInterval = defaultSlidingExpirationInterval
        },
        minimalExpiredEntriesPurgingInterval,
        timeProvider,
        logger) {
      _purgingSignal = purgingSignal;
    }

    public int NumberOfPurgeStarted { get; private set; }

    protected override Task<CacheInvalidationStatistics> DeleteExpiredCacheEntries(CancellationToken token) {
      NumberOfPurgeStarted++;
      _purgingSignal.Set();
      return Task.FromResult(new CacheInvalidationStatistics());
    }

    private readonly ManualResetEventSlim _purgingSignal;
  }
}
