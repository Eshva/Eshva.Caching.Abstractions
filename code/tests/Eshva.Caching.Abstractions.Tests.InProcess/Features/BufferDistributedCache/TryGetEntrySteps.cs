using Eshva.Caching.Abstractions.Tests.InProcess.Common;
using Eshva.Testing.Reqnroll.Contexts;
using FluentAssertions;
using Microsoft.IO;
using Reqnroll;

namespace Eshva.Caching.Abstractions.Tests.InProcess.Features.BufferDistributedCache;

[Binding]
internal class TryGetEntrySteps {
  public TryGetEntrySteps(CachesContext cachesContext, ErrorHandlingContext errorHandlingContext) {
    _cachesContext = cachesContext;
    _errorHandlingContext = errorHandlingContext;
  }

  [When("I try get '(.*)' cache entry asynchronously")]
  public async Task WhenITryGetCacheEntryAsynchronously(string key) {
    try {
      await using var destination = StreamManager.GetStream();
      _isSuccessfullyRead = await _cachesContext.Cache.TryGetAsync(key, destination);
      _cachesContext.GottenCacheEntryValue = destination.ToArray();
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  [When("I try get '(.*)' cache entry synchronously")]
  public void WhenITryGetCacheEntrySynchronously(string key) {
    try {
      using var destination = StreamManager.GetStream();
      _isSuccessfullyRead = _cachesContext.Cache.TryGet(key, destination);
      _cachesContext.GottenCacheEntryValue = destination.ToArray();
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  [Then("cache entry successfully read")]
  public void ThenCacheEntrySuccessfullyRead() => _isSuccessfullyRead.Should().BeTrue();

  [Then("cache entry did not read")]
  public void ThenCacheEntryDidNotRead() => _isSuccessfullyRead.Should().BeFalse();

  private readonly CachesContext _cachesContext;
  private readonly ErrorHandlingContext _errorHandlingContext;
  private bool _isSuccessfullyRead;
  private static readonly RecyclableMemoryStreamManager StreamManager = new();
}
