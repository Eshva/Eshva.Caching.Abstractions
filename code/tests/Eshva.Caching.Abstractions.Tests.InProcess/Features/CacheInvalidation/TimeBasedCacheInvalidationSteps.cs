using Eshva.Caching.Abstractions.Tests.InProcess.Common;
using FluentAssertions;
using Meziantou.Extensions.Logging.Xunit;
using Microsoft.Extensions.Logging;
using Reqnroll;

namespace Eshva.Caching.Abstractions.Tests.InProcess.Features.CacheInvalidation;

[Binding]
public class TimeBasedCacheInvalidationSteps {
  public TimeBasedCacheInvalidationSteps(CachesContext cachesContext, ErrorHandlingContext errorHandlingContext) {
    _cachesContext = cachesContext;
    _errorHandlingContext = errorHandlingContext;
    _purgingSignal = new ManualResetEventSlim(initialState: false);
  }

  [Given("time-based cache invalidation with defined arguments")]
  public void GivenTimeBasedCacheInvalidationWithDefinedArguments() =>
    WhenIConstructTimeBasedCacheInvalidationWithDefinedArguments();

  [When("I construct time-based cache invalidation with defined arguments")]
  public void WhenIConstructTimeBasedCacheInvalidationWithDefinedArguments() =>
    CreateTimeBasedCacheInvalidation(
      new TimeBasedCacheInvalidationSettings {
        DefaultSlidingExpirationInterval = _cachesContext.DefaultSlidingExpirationInterval,
        ExpiredEntriesPurgingInterval = _cachesContext.PurgingInterval
      });

  [When("I construct time-based cache invalidation with settings not specified")]
  public void WhenIConstructTimeBasedCacheInvalidationWithSettingsNotSpecified() =>
    CreateTimeBasedCacheInvalidation(settings: null);

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
      Parallel.For(fromInclusive: 0, toExclusive: 5, _ => _sut.PurgeEntriesIfRequired());
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  [Then("purging is successfully done")]
  public void ThenPurgingIsSuccessfullyDone() =>
    _purgingSignal.Wait(TimeSpan.FromSeconds(value: 1D)).Should().BeTrue();

  [Then("purging is not started")]
  public void ThenPurgingIsNotStarted() =>
    _purgingSignal.Wait(TimeSpan.FromSeconds(value: 1D)).Should().BeFalse();

  [Then("only one purging should be done")]
  public void ThenOnlyOnePurgingShouldBeDone() =>
    _sut.NumberOfPurgeStarted.Should().Be(expected: 1);

  private void CreateTimeBasedCacheInvalidation(TimeBasedCacheInvalidationSettings? settings) {
    try {
      _sut = new TestCacheInvalidation(
        _cachesContext.MinimalPurgingInterval,
        settings!,
        _cachesContext.TimeProvider,
        XUnitLogger.CreateLogger<TestCacheInvalidation>(_cachesContext.XUnitLogger),
        _purgingSignal);
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  private readonly CachesContext _cachesContext;
  private readonly ErrorHandlingContext _errorHandlingContext;
  private readonly ManualResetEventSlim _purgingSignal;
  private TestCacheInvalidation _sut = null!;

  private sealed class TestCacheInvalidation : TimeBasedCacheInvalidation {
    public TestCacheInvalidation(
      TimeSpan minimalExpiredEntriesPurgingInterval,
      TimeBasedCacheInvalidationSettings settings,
      TimeProvider timeProvider,
      ILogger logger,
      ManualResetEventSlim purgingSignal)
      : base(
        settings,
        minimalExpiredEntriesPurgingInterval,
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
}
