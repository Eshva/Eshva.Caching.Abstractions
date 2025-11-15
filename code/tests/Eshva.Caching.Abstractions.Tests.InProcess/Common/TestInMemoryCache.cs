using Eshva.Caching.Abstractions.Distributed;
using Microsoft.Extensions.Logging;

namespace Eshva.Caching.Abstractions.Tests.InProcess.Common;

internal sealed class TestInMemoryCache : BufferDistributedCache {
  public TestInMemoryCache(TestCacheInvalidation cacheInvalidation, TestInMemoryCacheDatastore cacheDatastore, ILogger? logger = null)
    : base(cacheInvalidation, cacheDatastore, logger) { }
}
