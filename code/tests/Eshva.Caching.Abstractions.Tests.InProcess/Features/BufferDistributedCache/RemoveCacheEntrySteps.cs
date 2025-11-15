using Eshva.Caching.Abstractions.Tests.InProcess.Common;
using Eshva.Testing.Reqnroll.Contexts;
using Reqnroll;

namespace Eshva.Caching.Abstractions.Tests.InProcess.Features.BufferDistributedCache;

[Binding]
internal class RemoveCacheEntrySteps {
  public RemoveCacheEntrySteps(CachesContext cachesContext, ErrorHandlingContext errorHandlingContext) {
    _cachesContext = cachesContext;
    _errorHandlingContext = errorHandlingContext;
  }

  [When("I remove '(.*)' cache entry asynchronously")]
  public async Task WhenIRemoveCacheEntryAsynchronously(string key) {
    try {
      await _cachesContext.Cache.RemoveAsync(key);
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  [When("I remove '(.*)' cache entry synchronously")]
  public void WhenIRemoveCacheEntrySynchronously(string key) {
    try {
      _cachesContext.Cache.Remove(key);
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  private readonly CachesContext _cachesContext;
  private readonly ErrorHandlingContext _errorHandlingContext;
}
