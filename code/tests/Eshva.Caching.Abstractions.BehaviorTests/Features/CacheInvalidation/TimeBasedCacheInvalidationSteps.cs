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

  [Given("time-based cache invalidation with defined arguments")]
  public void GivenTimeBasedCacheInvalidationWithDefinedArguments() {
    try {
      _sut = new TestCacheInvalidation(
        _cachesContext.PurgingInterval,
        _cachesContext.DefaultSlidingExpirationInterval,
        _cachesContext.MinimalPurgingInterval,
        _cachesContext.TimeProvider,
        XUnitLogger.CreateLogger<TestCacheInvalidation>(_cachesContext.XUnitLogger),
        _purgingSignal);
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  [When("I construct time-based cache invalidation with defined arguments")]
  public void WhenIConstructTimeBasedCacheInvalidationWithDefinedArguments() =>
    GivenTimeBasedCacheInvalidationWithDefinedArguments();

  [When("I request cache invalidation")]
  public void WhenIRequestCacheInvalidation() {
    try {
      _sut.PurgeEntriesIfRequired();
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  [When("a few cache invalidations requested")]
  public void WhenAFewCacheInvalidationsRequested() {
    try {
      // TODO: Execute requests in parallel.
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

  [Then("awaited purging is finished")]
  public void ThenAwaitedPurgingIsFinished() =>
    _purgingSignal.Wait(TimeSpan.FromSeconds(seconds: 1));

  private readonly CachesContext _cachesContext;
  private readonly ErrorHandlingContext _errorHandlingContext;
  private readonly ManualResetEventSlim _purgingSignal;
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
