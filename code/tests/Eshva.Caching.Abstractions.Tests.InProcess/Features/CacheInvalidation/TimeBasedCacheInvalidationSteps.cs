using Eshva.Caching.Abstractions.Tests.InProcess.Common;
using Eshva.Testing.Reqnroll.Contexts;
using FluentAssertions;
using Meziantou.Extensions.Logging.Xunit.v3;
using Microsoft.Extensions.Logging;
using Reqnroll;

namespace Eshva.Caching.Abstractions.Tests.InProcess.Features.CacheInvalidation;

[Binding]
internal class TimeBasedCacheInvalidationSteps {
  public TimeBasedCacheInvalidationSteps(CachesContext cachesContext, ErrorHandlingContext errorHandlingContext) {
    _cachesContext = cachesContext;
    _errorHandlingContext = errorHandlingContext;
    _purgingSignal = new ManualResetEventSlim(initialState: false);
  }

  [Given("time-based cache invalidation with defined arguments")]
  public void GivenTimeBasedCacheInvalidationWithDefinedArguments() =>
    CreateTimeBasedCacheInvalidation();

  [Given("invalidation duration testing cache invalidation with defined arguments")]
  public void GivenInvalidationDurationTestingCacheInvalidationWithDefinedArguments() =>
    _sut = new InvalidationDurationTestingCacheInvalidation(
      _cachesContext.PurgingInterval,
      _cachesContext.MaximalPurgingDuration,
      _cachesContext.ExpiryCalculator,
      _cachesContext.TimeProvider,
      XUnitLogger.CreateLogger<InvalidationDurationTestingCacheInvalidation>(_cachesContext.Logger),
      _purgingSignal);

  [Given("cache invalidation requested")]
  public void GivenCacheInvalidationRequested() =>
    RequestCacheInvalidation();

  [Given("purging is not started")]
  public void GivenPurgingIsNotStarted() =>
    CheckCacheInvalidationNotStarted();

  [Given("cache invalidation logger")]
  public void GivenCacheInvalidationLogger() =>
    _invalidationLogger = XUnitLogger.CreateLogger<TestCacheInvalidation>(_cachesContext.Logger);

  [Given("cache invalidation logger not specified")]
  public void GivenCacheInvalidationLoggerNotSpecified() => _invalidationLogger = null;

  [When("I construct time-based cache invalidation with defined arguments")]
  public void WhenIConstructTimeBasedCacheInvalidationWithDefinedArguments() =>
    CreateTimeBasedCacheInvalidation();

  [When("I request cache invalidation")]
  public void WhenIRequestCacheInvalidation() => RequestCacheInvalidation();

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
    CheckCacheInvalidationNotStarted();

  [Then("only one purging should be done")]
  public void ThenOnlyOnePurgingShouldBeDone() =>
    (_sut as TestCacheInvalidation)?.NumberOfPurgeStarted.Should().Be(expected: 1);

  [Then("purging started event risen once")]
  public void ThenPurgingStartedEventRisenOnce() =>
    _purgingStartedCount.Should().Be(expected: 1);

  [Then("purging completed event risen once")]
  public void ThenPurgingCompletedEventRisenOnce() =>
    _purgingCompletedCount.Should().Be(expected: 1);

  [Then("purging is successfully done in about (.*)")]
  public void ThenPurgingIsSuccessfullyDoneInAbout(TimeSpan invalidationDuration) {
    var invalidationCompletedAt = DateTimeOffset.UtcNow;
    _invalidationStartedAt.Add(invalidationDuration).Should().BeCloseTo(invalidationCompletedAt, TimeSpan.FromSeconds(value: 2D));
  }

  private void CheckCacheInvalidationNotStarted() =>
    _purgingSignal.Wait(TimeSpan.FromSeconds(value: 1D)).Should().BeFalse();

  private void RequestCacheInvalidation() {
    try {
      _invalidationStartedAt = DateTimeOffset.UtcNow;
      _sut.PurgeEntriesIfRequired();
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  private void CreateTimeBasedCacheInvalidation() {
    try {
      _sut = new TestCacheInvalidation(
        _cachesContext.PurgingInterval,
        _cachesContext.MaximalPurgingDuration,
        _cachesContext.ExpiryCalculator,
        _cachesContext.TimeProvider,
        _invalidationLogger!,
        _purgingSignal);
      _sut.CacheInvalidationStarted += (_, _) => _purgingStartedCount++;
      _sut.CacheInvalidationCompleted += (_, _) => _purgingCompletedCount++;
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  private readonly CachesContext _cachesContext;
  private readonly ErrorHandlingContext _errorHandlingContext;
  private readonly ManualResetEventSlim _purgingSignal;
  private int _purgingCompletedCount;
  private int _purgingStartedCount;
  private TimeBasedCacheInvalidation _sut = null!;
  private DateTimeOffset _invalidationStartedAt;
  private ILogger<TestCacheInvalidation>? _invalidationLogger;
}
