using System.Diagnostics;
using System.Text;
using FluentAssertions;
using Reqnroll;

namespace Eshva.Caching.Abstractions.Tests.InProcess.Common;

[Binding]
internal class CommonCacheSteps {
  public CommonCacheSteps(CachesContext cachesContext) {
    _cachesContext = cachesContext;
  }

  [Given(@"clock set at today (\b\d\d:\d\d:\d\d\b)", ExpressionType = ExpressionType.RegularExpression)]
  public void GivenClockSetAtToday(TimeSpan timeOfDay) =>
    _cachesContext.TimeProvider.AdjustTime(_cachesContext.Today + timeOfDay);

  [Given("purging interval is (.*)")]
  public void GivenPurgingIntervalIs(TimeSpan purgingInterval) =>
    _cachesContext.PurgingInterval = purgingInterval;

  [Given("maximal cache invalidation duration is (.*)")]
  public void GivenMaximalCacheInvalidationDurationIs(TimeSpan maximalCacheInvalidationDuration) =>
    _cachesContext.MaximalPurgingDuration = maximalCacheInvalidationDuration;

  [Given("default sliding expiration interval is (.*) minutes")]
  public void GivenDefaultSlidingExpirationIntervalIsMinutes(double defaultSlidingExpirationInterval) =>
    _cachesContext.DefaultSlidingExpirationInterval = TimeSpan.FromMinutes(defaultSlidingExpirationInterval);

  [Given(@"time passed by (\b\d\d:\d\d:\d\d\b)", ExpressionType = ExpressionType.RegularExpression)]
  public void GivenTimePassedBy(TimeSpan timeAdvance) =>
    _cachesContext.TimeProvider.Advance(timeAdvance);

  [Given("time provider is not specified")]
  public void GivenTimeProviderIsNotSpecified() =>
    _cachesContext.TimeProvider = null!;

  [Given("cache entry expiry calculator with defined arguments")]
  public void GivenCacheEntryExpiryCalculatorWithDefinedArguments() => _cachesContext.ExpiryCalculator =
    new CacheEntryExpiryCalculator(_cachesContext.DefaultSlidingExpirationInterval, _cachesContext.TimeProvider);

  [Given("cache entry expiry calculator is not specified")]
  public void GivenCacheEntryExpiryCalculatorIsNotSpecified() =>
    _cachesContext.ExpiryCalculator = null!;

  [Given("buffer distributed cache")]
  public void GivenBufferDistributedCache() =>
    _cachesContext.CreateAndAssignCacheServices();

  [Given(
    @"entry with key '(.*)' and value '(.*)' which expires in (\b\d\d:\d\d:\d\d\b) put into cache",
    ExpressionType = ExpressionType.RegularExpression)]
  public void GivenEntryWithKeyAndValueWhichExpiresInMinutesPutIntoCache(
    string key,
    string value,
    TimeSpan expiresIn) {
    var expiresAtUtc = _cachesContext.TimeProvider.GetUtcNow().Add(expiresIn);
    _cachesContext.CacheDatastore.Add(
      key,
      Encoding.UTF8.GetBytes(value),
      new CacheEntryExpiry(expiresAtUtc, AbsoluteExpiryAtUtc: null, _cachesContext.DefaultSlidingExpirationInterval));

    _cachesContext.Logger.WriteLine($"Put entry '{key}' that expires at {expiresAtUtc}");
  }

  [Then("I should get value '(.*)' as the requested entry")]
  public void ThenIShouldGetValueAsTheRequestedEntry(string value) =>
    Encoding.UTF8.GetBytes(value).Should().BeEquivalentTo(_cachesContext.GottenCacheEntryValue);

  [Then("I should get a null value as the requested entry")]
  public void ThenIShouldGetANullValueAsTheRequestedEntry() => _cachesContext.GottenCacheEntryValue.Should().BeNull();

  [Given("passed a bit more than purging expired entries interval")]
  public void GivenPassedABitMoreThanPurgingExpiredEntriesInterval() =>
    _cachesContext.TimeProvider.Advance(_cachesContext.PurgingInterval.Add(TimeSpan.FromSeconds(value: 1D)));

  [Then("value of '(.*)' entry is '(.*)'")]
  public void ThenValueOfEntryIs(string key, string value) =>
    Encoding.UTF8.GetString(_cachesContext.CacheDatastore.GetValue(key)).Should().Be(value);

  [Then("'(.*)' entry is not present in cache datastore")]
  public void ThenEntryIsNotPresentInCacheDatastore(string key) {
    try {
      _cachesContext.CacheDatastore.GetExpiry(key);
    }
    catch (KeyNotFoundException) {
      // Expected exception if object already deleted from the bucket.
      return;
    }

    Debug.Fail($"'{key}' entry is still present in the cache");
  }

  [Given("passed a bit less than purging expired entries interval")]
  public void GivenPassedABitLessThanPurgingExpiredEntriesInterval() =>
    _cachesContext.TimeProvider.Advance(_cachesContext.PurgingInterval.Add(TimeSpan.FromSeconds(value: -1D)));

  [Then(@"'(.*)' entry should be expired today at (\b\d\d:\d\d:\d\d\b)", ExpressionType = ExpressionType.RegularExpression)]
  public void ThenEntryShouldBeExpiredTodayAt(string key, TimeSpan timeOfDay) {
    var entryExpiry = _cachesContext.CacheDatastore.GetExpiry(key);
    entryExpiry.ExpiresAtUtc.Should().Be(_cachesContext.Today.Add(timeOfDay));
  }

  [Given("object with key {string} removed from object-store bucket")]
  public void GivenEntryWithKeyStringRemovedFromCache(string key) =>
    _cachesContext.CacheDatastore.Remove(key);

  [Given(
    @"entry with key '(.*)' and random byte array as value which expires in (\b\d\d:\d\d:\d\d\b) put into cache",
    ExpressionType = ExpressionType.RegularExpression)]
  public void GivenEntryWithKeyAndRandomByteArrayAsValueWhichExpiresInPutIntoCache(string key, TimeSpan expiresIn) {
    _originalValue = MakeBigValue();
    var expiresAtUtc = _cachesContext.TimeProvider.GetUtcNow().Add(expiresIn);
    _cachesContext.CacheDatastore.Add(
      key,
      _originalValue,
      new CacheEntryExpiry(expiresAtUtc, AbsoluteExpiryAtUtc: null, _cachesContext.DefaultSlidingExpirationInterval));
    _cachesContext.Logger.WriteLine($"Put entry '{key}' that expires at {expiresAtUtc}");
  }

  [Then("I should get same value as the requested entry")]
  public void ThenIShouldGetSameValueAsTheRequestedEntry() =>
    _cachesContext.GottenCacheEntryValue.Should().BeEquivalentTo(_originalValue);

  [Then("cache invalidation should be triggered")]
  public void ThenCacheInvalidationShouldBeTriggered() =>
    _cachesContext.PurgingSignal.Wait(TimeSpan.FromSeconds(value: 5D)).Should().BeTrue();

  [Then("cache invalidation should not be triggered")]
  public void ThenCacheInvalidationShouldNotBeTriggered() =>
    _cachesContext.PurgingSignal.Wait(TimeSpan.FromSeconds(value: 5D)).Should().BeFalse();

  [Then(@"sliding expiry interval of '(.*)' entry should be (\b\d\d:\d\d:\d\d\b)", ExpressionType = ExpressionType.RegularExpression)]
  public void ThenSlidingExpiryIntervalOfEntryShouldBe(string key, TimeSpan slidingExpiry) =>
    _cachesContext.CacheDatastore.GetExpiry(key).SlidingExpiryInterval.Should().Be(slidingExpiry);

  [Then(@"sliding expiry interval of '(.*)' entry should be null", ExpressionType = ExpressionType.RegularExpression)]
  public void ThenSlidingExpiryIntervalOfEntryShouldBeNull(string key) =>
    _cachesContext.CacheDatastore.GetExpiry(key).SlidingExpiryInterval.Should().BeNull();

  [Then(@"absolute expiry of '(.*)' entry should be today at (\b\d\d:\d\d:\d\d\b)", ExpressionType = ExpressionType.RegularExpression)]
  public void ThenAbsoluteExpiryOfEntryShouldBeTodayAt(string key, TimeSpan absoluteExpiryAtUtc) =>
    _cachesContext.CacheDatastore.GetExpiry(key).AbsoluteExpiryAtUtc.Should().Be(_cachesContext.Today.Add(absoluteExpiryAtUtc));

  [Then(@"absolute expiry of '(.*)' entry should be null", ExpressionType = ExpressionType.RegularExpression)]
  public void ThenAbsoluteExpiryOfEntryShouldBeNull(string key) =>
    _cachesContext.CacheDatastore.GetExpiry(key).AbsoluteExpiryAtUtc.Should().BeNull();

  private byte[] MakeBigValue() {
    const int bufferSize = 1 * 1024 * 1024;
    var bigValue = new byte[bufferSize];
    var random = new Random();
    random.NextBytes(_originalValue);
    return bigValue;
  }

  private readonly CachesContext _cachesContext;
  private byte[] _originalValue = [];
}
