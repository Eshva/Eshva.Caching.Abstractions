using System.Buffers;
using System.Text;
using Eshva.Caching.Abstractions.Tests.InProcess.Common;
using Microsoft.Extensions.Caching.Distributed;
using Reqnroll;

namespace Eshva.Caching.Abstractions.Tests.InProcess.Features.BufferDistributedCache;

[Binding]
internal class SetEntryUsingSequenceReaderSteps {
  public SetEntryUsingSequenceReaderSteps(CachesContext cachesContext, ErrorHandlingContext errorHandlingContext) {
    _cachesContext = cachesContext;
    _errorHandlingContext = errorHandlingContext;
  }

  [When(
    "I set using sequence reader asynchronously '(.*)' cache entry with value '(.*)' and sliding expiration in (.*) minutes",
    ExpressionType = ExpressionType.RegularExpression)]
  public async Task WhenISetUsingSequenceReaderAsynchronouslyCacheEntryWithValueAndSlidingExpirationInMinutes(
    string key,
    string value,
    int minutes) {
    try {
      await _cachesContext.Cache.SetAsync(
        key,
        new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(value)),
        new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(minutes) });
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  [When(
    "I set using sequence reader synchronously '(.*)' cache entry with value '(.*)' and sliding expiration in (.*) minutes",
    ExpressionType = ExpressionType.RegularExpression)]
  public void WhenISetUsingSequenceReaderSynchronouslyCacheEntryWithValueAndSlidingExpirationInMinutes(
    string key,
    string value,
    int minutes) {
    try {
      _cachesContext.Cache.Set(
        key,
        new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(value)),
        new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(minutes) });
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  [When(
    @"I set using sequence reader asynchronously '(.*)' cache entry with value '(.*)' and absolute expiration at today at (\b\d\d:\d\d:\d\d\b)",
    ExpressionType = ExpressionType.RegularExpression)]
  public async Task WhenISetUsingSequenceReaderAsynchronouslyCacheEntryWithValueAndAbsoluteExpirationAtTodayAtDd(
    string key,
    string value,
    TimeSpan timeOfDay) {
    try {
      await _cachesContext.Cache.SetAsync(
        key,
        new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(value)),
        new DistributedCacheEntryOptions { AbsoluteExpiration = _cachesContext.Today.Add(timeOfDay) });
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  [When(
    @"I set using sequence reader asynchronously '(.*)' cache entry with value '(.*)' and absolute expiration at today at (\b\d\d:\d\d:\d\d\b) and sliding expiration in (.*) minutes",
    ExpressionType = ExpressionType.RegularExpression)]
  public async Task
    WhenISetUsingSequenceReaderAsynchronouslyCacheEntryWithValueAndAbsoluteExpirationAtTodayAtDdAndSlidingExpirationInMinutes(
      string key,
      string value,
      TimeSpan timeOfDay,
      int minutes) {
    try {
      await _cachesContext.Cache.SetAsync(
        key,
        new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(value)),
        new DistributedCacheEntryOptions {
          AbsoluteExpiration = _cachesContext.Today.Add(timeOfDay), SlidingExpiration = TimeSpan.FromMinutes(minutes)
        });
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  [When(
    @"I set using sequence reader synchronously '(.*)' cache entry with value '(.*)' and absolute expiration at today at (\b\d\d:\d\d:\d\d\b) and sliding expiration in (.*) minutes",
    ExpressionType = ExpressionType.RegularExpression)]
  public async Task
    WhenISetUsingSequenceReaderSynchronouslyCacheEntryWithValueAndAbsoluteExpirationAtTodayAtDdAndSlidingExpirationInMinutes(
      string key,
      string value,
      TimeSpan timeOfDay,
      int minutes) {
    try {
      await _cachesContext.Cache.SetAsync(
        key,
        new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(value)),
        new DistributedCacheEntryOptions {
          AbsoluteExpiration = _cachesContext.Today.Add(timeOfDay), SlidingExpiration = TimeSpan.FromMinutes(minutes)
        });
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  [When(
    @"I set using sequence reader synchronously '(.*)' cache entry with value '(.*)' and absolute expiration at today at (\b\d\d:\d\d:\d\d\b)",
    ExpressionType = ExpressionType.RegularExpression)]
  public void WhenISetUsingSequenceReaderSynchronouslyCacheEntryWithValueAndAbsoluteExpirationAtTodayAtDd(
    string key,
    string value,
    TimeSpan timeOfDay) {
    try {
      _cachesContext.Cache.Set(
        key,
        new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(value)),
        new DistributedCacheEntryOptions { AbsoluteExpiration = _cachesContext.Today.Add(timeOfDay) });
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  [When(
    @"I set using sequence reader asynchronously '(.*)' cache entry with value '(.*)' and absolute expiration (\b\d\d:\d\d:\d\d\b) relative to now",
    ExpressionType = ExpressionType.RegularExpression)]
  public async Task
    WhenISetUsingSequenceReaderAsynchronouslyCacheEntryWithValueAndAbsoluteExpirationRelativeToNow(
      string key,
      string value,
      TimeSpan timeOfDay) {
    try {
      await _cachesContext.Cache.SetAsync(
        key,
        new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(value)),
        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = timeOfDay });
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  [When(
    @"I set using sequence reader synchronously '(.*)' cache entry with value '(.*)' and absolute expiration (\b\d\d:\d\d:\d\d\b) relative to now",
    ExpressionType = ExpressionType.RegularExpression)]
  public void WhenISetUsingSequenceReaderSynchronouslyCacheEntryWithValueAndAbsoluteExpirationRelativeToNow(
    string key,
    string value,
    TimeSpan timeOfDay) {
    try {
      _cachesContext.Cache.Set(
        key,
        new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(value)),
        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = timeOfDay });
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  private readonly CachesContext _cachesContext;
  private readonly ErrorHandlingContext _errorHandlingContext;
}
