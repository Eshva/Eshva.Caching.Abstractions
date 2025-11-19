using Eshva.Caching.Abstractions.Tests.InProcess.Common;
using Eshva.Testing.Reqnroll.Contexts;
using Reqnroll;

namespace Eshva.Caching.Abstractions.Tests.InProcess.Features.BufferDistributedCache;

[Binding]
internal class GetEntrySteps {
  public GetEntrySteps(CachesContext cachesContext, ErrorHandlingContext errorHandlingContext) {
    _cachesContext = cachesContext;
    _errorHandlingContext = errorHandlingContext;
  }

  [Given("get an entry {string} to trigger cache invalidation")]
  public async Task GivenGetAnEntryStringToTriggerCacheInvalidation(string key) =>
    await _cachesContext.Cache.GetAsync(key);

  [When("I get '(.*)' cache entry asynchronously")]
  public async Task WhenIGetCacheEntryAsynchronously(string key) {
    try {
      _cachesContext.GottenCacheEntryValue = await _cachesContext.Cache.GetAsync(key);
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  [When("I get '(.*)' cache entry synchronously")]
  public void WhenIGetCacheEntrySynchronously(string key) {
    try {
      _cachesContext.GottenCacheEntryValue = _cachesContext.Cache.Get(key);
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  private readonly CachesContext _cachesContext;
  private readonly ErrorHandlingContext _errorHandlingContext;
}
