using Eshva.Caching.Abstractions.Tests.InProcess.Common;
using Eshva.Testing.Reqnroll.Contexts;
using Meziantou.Extensions.Logging.Xunit.v3;
using Microsoft.Extensions.Logging;
using Reqnroll;

namespace Eshva.Caching.Abstractions.Tests.InProcess.Features.BufferDistributedCache;

[Binding]
internal sealed class BufferDistributedCacheSteps {
  public BufferDistributedCacheSteps(CachesContext cachesContext, ErrorHandlingContext errorHandlingContext) {
    _cachesContext = cachesContext ?? throw new ArgumentNullException(nameof(cachesContext));
    _errorHandlingContext = errorHandlingContext ?? throw new ArgumentNullException(nameof(errorHandlingContext));
  }

  [Given("cache invalidation")]
  public void GivenCacheInvalidation() =>
    _invalidation = new TestCacheInvalidation(
      _cachesContext.PurgingInterval,
      _cachesContext.MaximalPurgingDuration,
      new CacheEntryExpiryCalculator(_cachesContext.DefaultSlidingExpirationInterval, _cachesContext.TimeProvider),
      _cachesContext.TimeProvider,
      XUnitLogger.CreateLogger<TestCacheInvalidation>(_cachesContext.Logger),
      _cachesContext.PurgingSignal);

  [Given("cache datastore")]
  public void GivenCacheDatastore() => _datastore = new TestInMemoryCacheDatastore();

  [Given("logger")]
  public void GivenLogger() => _logger = XUnitLogger.CreateLogger<TestInMemoryCache>(_cachesContext.Logger);

  [Given("cache invalidation not specified")]
  public void GivenCacheInvalidationNotSpecified() => _invalidation = null;

  [Given("cache datastore not specified")]
  public void GivenCacheDatastoreNotSpecified() => _datastore = null;

  [Given("logger not specified")]
  public void GivenLoggerNotSpecified() => _logger = null;

  [When("I construct buffer distributed cache")]
  public void WhenIConstructBufferDistributedCache() {
    try {
      // ReSharper disable once ObjectCreationAsStatement
      new TestInMemoryCache(_invalidation!, _datastore!, _logger);
    }
    catch (Exception exception) {
      _errorHandlingContext.LastException = exception;
    }
  }

  private readonly CachesContext _cachesContext;
  private readonly ErrorHandlingContext _errorHandlingContext;
  private TestCacheInvalidation? _invalidation;
  private TestInMemoryCacheDatastore? _datastore;
  private ILogger<TestInMemoryCache>? _logger;
}
