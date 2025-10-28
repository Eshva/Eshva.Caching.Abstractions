using Eshva.Caching.Abstractions.BehaviorTests.Common;
using FluentAssertions;
using Reqnroll;

namespace Eshva.Caching.Abstractions.BehaviorTests.Features.ExpiryCalculator;

[Binding]
public class CacheEntryExpiryCalculatorSteps {
  public CacheEntryExpiryCalculatorSteps(CachesContext cachesContext, ErrorHandlingContext errorHandlingContext) {
    _cachesContext = cachesContext;
    _errorHandlingContext = errorHandlingContext;
  }

  [Given("cache entry expiry calculator with defined arguments")]
  public void GivenCacheEntryExpiryCalculatorWithDefinedArguments() {
    try {
      _sut = new CacheEntryExpiryCalculator(_cachesContext.DefaultSlidingExpirationInterval, _cachesContext.TimeProvider);
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  [Given("no absolute expiration time")]
  public void GivenNoAbsoluteExpirationTime() =>
    _absoluteExpiration = null;

  [Given("no sliding expiration time")]
  public void GivenNoSlidingExpirationTime() =>
    _slidingExpiration = null;

  [Given("cache entry that expires today at (.*)")]
  public void GivenCacheEntryThatExpiresTodayAt(TimeSpan expiresAt) =>
    _expiresAt = _cachesContext.Today.Add(expiresAt);

  [Given("sliding expiration in {int} minutes")]
  public void GivenSlidingExpirationInIntMinutes(int minutes) =>
    _slidingExpiration = TimeSpan.FromMinutes(minutes);

  [Given("absolute expiration today at (.*)")]
  public void GivenAbsoluteExpirationTodayAt(TimeSpan absoluteExpirationTime) =>
    _absoluteExpiration = _cachesContext.Today.Add(absoluteExpirationTime);

  [Given("relative expiration time is {double} minutes")]
  public void GivenRelativeExpirationTimeIsDoubleMinutes(double relativeExpiration) =>
    _relativeExpiration = TimeSpan.FromMinutes(relativeExpiration);

  [Given("no relative expiration time")]
  public void GivenNoRelativeExpirationTime() =>
    _relativeExpiration = null;

  [When("I construct cache entry expiry calculator")]
  public void WhenIConstructCacheEntryExpiryCalculator() =>
    GivenCacheEntryExpiryCalculatorWithDefinedArguments();

  [When("I check is cache entry expired")]
  public void WhenICheckIsCacheEntryExpired() =>
    _isExpired = _sut.IsCacheEntryExpired(_expiresAt);

  [When("I calculate expiration time")]
  public void WhenICalculateExpirationTime() =>
    _calculatedExpiration = _sut.CalculateExpiration(_absoluteExpiration, _slidingExpiration);

  [When("I calculate absolute expiration time")]
  public void WhenICalculateAbsoluteExpirationTime() {
    try {
      _calculatedAbsoluteExpiration = _sut.CalculateAbsoluteExpiration(_absoluteExpiration, _relativeExpiration);
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  [Then("calculated expiration time should be today at (.*)")]
  public void ThenCalculatedExpirationTimeShouldBeTodayAt(TimeSpan expirationTime) =>
    _calculatedExpiration.Should().Be(_cachesContext.Today.Add(expirationTime));

  [Then("it should be not expired")]
  public void ThenItShouldBeNotExpired() =>
    _isExpired.Should().BeFalse();

  [Then("it should be expired")]
  public void ThenItShouldBeExpired() =>
    _isExpired.Should().BeTrue();

  [Then("calculated absolute expiration time should be today at (.*)")]
  public void ThenCalculatedAbsoluteExpirationTimeShouldBe(TimeSpan absoluteExpiration) =>
    _calculatedAbsoluteExpiration.Should().Be(_cachesContext.Today.Add(absoluteExpiration));

  [Then("calculated absolute expiration time should be null")]
  public void ThenCalculatedAbsoluteExpirationTimeShouldBeNull() =>
    _calculatedAbsoluteExpiration.Should().BeNull();

  private readonly CachesContext _cachesContext;
  private readonly ErrorHandlingContext _errorHandlingContext;
  private DateTimeOffset? _absoluteExpiration;
  private TimeSpan? _slidingExpiration;
  private TimeSpan? _relativeExpiration;
  private DateTimeOffset _calculatedExpiration;
  private DateTimeOffset? _calculatedAbsoluteExpiration;
  private DateTimeOffset _expiresAt;
  private bool _isExpired;
  private CacheEntryExpiryCalculator _sut = null!;
}
