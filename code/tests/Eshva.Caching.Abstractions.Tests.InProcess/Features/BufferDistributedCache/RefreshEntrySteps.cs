using Eshva.Caching.Abstractions.Tests.InProcess.Common;
using Reqnroll;

namespace Eshva.Caching.Abstractions.Tests.InProcess.Features.BufferDistributedCache;

[Binding]
internal class RefreshEntrySteps {
  public RefreshEntrySteps(CachesContext cachesContext, ErrorHandlingContext errorHandlingContext) {
    _cachesContext = cachesContext;
    _errorHandlingContext = errorHandlingContext;
  }

  [When("I refresh '(.*)' cache entry asynchronously")]
  public async Task WhenIRefreshCacheEntryAsynchronously(string key) {
    try {
      await _cachesContext.Cache.RefreshAsync(key);
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  [When("I refresh '(.*)' cache entry synchronously")]
  public async Task WhenIRefreshCacheEntrySynchronously(string key) {
    try {
      // ReSharper disable once MethodHasAsyncOverload
      _cachesContext.Cache.Refresh(key);
      await Task.Delay(millisecondsDelay: 1000);
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  private readonly CachesContext _cachesContext;
  private readonly ErrorHandlingContext _errorHandlingContext;
}
