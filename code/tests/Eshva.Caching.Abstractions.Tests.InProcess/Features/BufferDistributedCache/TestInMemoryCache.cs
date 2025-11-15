using Eshva.Caching.Abstractions.Tests.InProcess.Common;
using Microsoft.Extensions.Logging;

namespace Eshva.Caching.Abstractions.Tests.InProcess.Features.BufferDistributedCache;

internal sealed class TestInMemoryCache : Abstractions.BufferDistributedCache {
  public TestInMemoryCache(TestCacheInvalidation cacheInvalidation, TestInMemoryCacheDatastore cacheDatastore, ILogger? logger = null)
    : base(cacheInvalidation, cacheDatastore, logger) { }
}
