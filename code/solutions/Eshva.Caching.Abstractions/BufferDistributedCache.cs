using System.Buffers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IO;

namespace Eshva.Caching.Abstractions;

/// <summary>
/// Base class for distributed caches. Implements behavioral contract of <see cref="IBufferDistributedCache"/> and
/// <see cref="IDistributedCache"/>.
/// </summary>
public abstract class BufferDistributedCache : IBufferDistributedCache {
  /// <summary>
  /// Initializes a new instance of a distributed cache.
  /// </summary>
  /// <param name="cacheInvalidation">Cache invalidation.</param>
  /// <param name="cacheDatastore"></param>
  /// <param name="logger">Logger.</param>
  /// <exception cref="ArgumentNullException">
  /// Value of a required parameter not specified.
  /// </exception>
  protected BufferDistributedCache(
    TimeBasedCacheInvalidation cacheInvalidation,
    ICacheDatastore cacheDatastore,
    ILogger? logger = null) {
    _cacheInvalidation = cacheInvalidation ?? throw new ArgumentNullException(nameof(cacheInvalidation));
    _cacheDatastore = cacheDatastore ?? throw new ArgumentNullException(nameof(cacheDatastore));
    _logger = logger ?? new NullLogger<BufferDistributedCache>();
  }

  /// <summary>
  /// Get value of a cache key <paramref name="key"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Read the key <paramref name="key"/> value from the object-store bucket and returns it as a byte array if it's
  /// found. If entry has a sliding expiration its expiration time could be refreshed (depends on the purger used).
  /// </para>
  /// <para>
  /// If it's time purge all expired entries in the cache.
  /// </para>
  /// </remarks>
  /// <param name="key">Cache entry key.</param>
  /// <returns>
  /// Depending on different circumstances returns:
  /// <list type="bullet">
  /// <item>byte array - read key value,</item>
  /// <item>null - cache key <paramref name="key"/> isn't found.</item>
  /// </list>
  /// </returns>
  /// <exception cref="ArgumentException">
  /// Cache entry key is invalid.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  /// Failed to read cache entry.
  /// </exception>
  public byte[]? Get(string key) =>
    GetAsync(key).GetAwaiter().GetResult();

  /// <summary>
  /// Get value of a cache key <paramref name="key"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Read the key <paramref name="key"/> value from the object-store bucket and returns it as a byte array if it's
  /// found, otherwise returns <c>null</c>. If entry has a sliding expiration its expiration time could be refreshed.
  /// </para>
  /// <para>
  /// If it's the time purge all expired entries in the cache.
  /// </para>
  /// </remarks>
  /// <param name="key">Cache entry key.</param>
  /// <param name="token">Cancellation token.</param>
  /// <returns>
  /// Depending on different circumstances returns:
  /// <list type="bullet">
  /// <item>byte array - read key value,</item>
  /// <item>null - cache key <paramref name="key"/> isn't found.</item>
  /// </list>
  /// </returns>
  /// <exception cref="ArgumentException">
  /// Cache entry key is invalid.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  /// Failed to read cache entry.
  /// </exception>
  public async Task<byte[]?> GetAsync(string key, CancellationToken token = default) {
    _cacheDatastore.ValidateKey(key);
    _cacheInvalidation.PurgeEntriesIfRequired();

    using var destination = StreamManager.GetStream();
    var (isEntryGotten, cacheEntryExpiry) =
      await _cacheDatastore.TryGetEntry(key, destination, token).ConfigureAwait(continueOnCapturedContext: false);
    if (!isEntryGotten) return null;

    await _cacheDatastore.RefreshEntry(key, UpdateCacheEntryExpiry(cacheEntryExpiry), token)
      .ConfigureAwait(continueOnCapturedContext: false);

    return destination.ToArray();
  }

  /// <summary>
  /// Set a cache entry value.
  /// </summary>
  /// <param name="key">The key of cache entry.</param>
  /// <param name="value">The value of cache entry.</param>
  /// <param name="options">Expiration options.</param>
  /// <exception cref="ArgumentException">
  /// Cache entry key is invalid.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  /// Failed to set cache entry.
  /// </exception>
  public void Set(string key, byte[] value, DistributedCacheEntryOptions options) =>
    SetAsync(key, value, options).GetAwaiter().GetResult();

  /// <summary>
  /// Set a cache entry value.
  /// </summary>
  /// <param name="key">The key of cache entry.</param>
  /// <param name="value">The value of cache entry.</param>
  /// <param name="options">Expiration options.</param>
  /// <param name="token">Cancellation token.</param>
  /// <exception cref="ArgumentException">
  /// Cache entry key is invalid.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  /// Failed to set cache entry.
  /// </exception>
  public async Task SetAsync(
    string key,
    byte[] value,
    DistributedCacheEntryOptions options,
    CancellationToken token = new()) {
    _cacheDatastore.ValidateKey(key);
    _cacheInvalidation.PurgeEntriesIfRequired();
    var entryExpiry = MakeCacheEntryExpiry(options);
    await _cacheDatastore.SetEntry(
        key,
        new ReadOnlySequence<byte>(value),
        entryExpiry,
        token)
      .ConfigureAwait(continueOnCapturedContext: false);
    _logger.LogDebug("Cache entry {EntryKey} set with expiry @{EntryExpiry}", key, entryExpiry);
  }

  /// <summary>
  /// Refresh expiration time of the cache entry with <paramref name="key"/>.
  /// </summary>
  /// <param name="key">The key of refreshing cache entry.</param>
  /// <exception cref="ArgumentException">
  /// Cache entry key is invalid.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  /// Cache entry with <paramref name="key"/> not found.
  /// </exception>
  public void Refresh(string key) =>
    RefreshAsync(key).GetAwaiter().GetResult();

  /// <summary>
  /// Refresh expiration time of the cache entry with <paramref name="key"/>.
  /// </summary>
  /// <param name="key">The key of refreshing cache entry.</param>
  /// <param name="token">Cancellation token.</param>
  /// <exception cref="ArgumentException">
  /// Cache entry key is invalid.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  /// Cache entry with <paramref name="key"/> not found.
  /// </exception>
  public async Task RefreshAsync(string key, CancellationToken token = new()) {
    _cacheDatastore.ValidateKey(key);
    _cacheInvalidation.PurgeEntriesIfRequired();
    var entryExpiry = UpdateCacheEntryExpiry(
      await _cacheDatastore.GetEntryExpiry(key, token).ConfigureAwait(continueOnCapturedContext: false));
    await _cacheDatastore.RefreshEntry(key, entryExpiry, token)
      .ConfigureAwait(continueOnCapturedContext: false);
    _logger.LogDebug("Cache entry {EntryKey} refreshed with expiry @{EntryExpiry}", key, entryExpiry);
  }

  /// <summary>
  /// Remove a cache entry.
  /// </summary>
  /// <remarks>
  /// If cache entry doesn't exist or removed no exception will be thrown.
  /// </remarks>
  /// <param name="key">The key of removing cache entry.</param>
  /// <exception cref="ArgumentException">
  /// Cache entry key is invalid.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  /// Failed to remove cache entry.
  /// </exception>
  public void Remove(string key) =>
    RemoveAsync(key).GetAwaiter().GetResult();

  /// <summary>
  /// Remove a cache entry.
  /// </summary>
  /// <remarks>
  /// If cache entry doesn't exist or removed no exception will be thrown.
  /// </remarks>
  /// <param name="key">The key of removing cache entry.</param>
  /// <param name="token">Cancellation token.</param>
  /// <exception cref="ArgumentException">
  /// Cache entry key is invalid.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  /// Failed to remove cache entry.
  /// </exception>
  public async Task RemoveAsync(string key, CancellationToken token = new()) {
    _cacheDatastore.ValidateKey(key);
    _cacheInvalidation.PurgeEntriesIfRequired();
    await _cacheDatastore.RemoveEntry(key, token).ConfigureAwait(continueOnCapturedContext: false);
    _logger.LogDebug("Cache entry {EntryKey} removed from cache", key);
  }

  /// <summary>
  /// Try to get a cache entry.
  /// </summary>
  /// <param name="key">Cache entry key.</param>
  /// <param name="destination">Buffer writer to write cache entry value into.</param>
  /// <exception cref="ArgumentNullException">
  /// The key is not specified.
  /// </exception>
  /// <returns>
  /// <c>true</c> - value successfully read, <c>false</c> - entry not found in the cache.
  /// </returns>
  /// <exception cref="ArgumentException">
  /// Cache entry key is invalid.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  /// Failed to read cache entry.
  /// </exception>
  public bool TryGet(string key, IBufferWriter<byte> destination) =>
    TryGetAsync(key, destination).AsTask().GetAwaiter().GetResult();

  /// <summary>
  /// Try to get a cache entry.
  /// </summary>
  /// <param name="key">Cache entry key.</param>
  /// <param name="destination">Buffer writer to write cache entry value into.</param>
  /// <param name="token">Cancellation token.</param>
  /// <returns>
  /// <c>true</c> - value successfully read, <c>false</c> - entry not found in the cache.
  /// </returns>
  /// <exception cref="ArgumentException">
  /// Cache entry key is invalid.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  /// Failed to read cache entry.
  /// </exception>
  public async ValueTask<bool> TryGetAsync(string key, IBufferWriter<byte> destination, CancellationToken token = new()) {
    _cacheDatastore.ValidateKey(key);
    _cacheInvalidation.PurgeEntriesIfRequired();
    var (isEntryGotten, cacheEntryExpiry) = await _cacheDatastore.TryGetEntry(
        key,
        destination,
        token)
      .ConfigureAwait(continueOnCapturedContext: false);
    if (!isEntryGotten) return false;

    var entryExpiry = UpdateCacheEntryExpiry(cacheEntryExpiry);
    await _cacheDatastore.RefreshEntry(key, entryExpiry, token)
      .ConfigureAwait(continueOnCapturedContext: false);
    _logger.LogDebug("Cache entry {EntryKey} gotten and its expiry updated to @{EntryExpiry}", key, entryExpiry);
    return true;
  }

  /// <summary>
  /// Set cache entry with <paramref name="key"/> with <paramref name="value"/>.
  /// </summary>
  /// <param name="key">Cache entry key.</param>
  /// <param name="value">Cache entry value.</param>
  /// <param name="options">Expiration options.</param>
  /// <exception cref="ArgumentException">
  /// Cache entry key is invalid.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  /// Failed to set cache entry.
  /// </exception>
  public void Set(string key, ReadOnlySequence<byte> value, DistributedCacheEntryOptions options) =>
    SetAsync(key, value, options).AsTask().GetAwaiter().GetResult();

  /// <summary>
  /// Set cache entry with <paramref name="key"/> with <paramref name="value"/>.
  /// </summary>
  /// <param name="key">Cache entry key.</param>
  /// <param name="value">Cache entry value.</param>
  /// <param name="options">Expiration options.</param>
  /// <param name="token">Cancellation token.</param>
  /// <exception cref="ArgumentException">
  /// Cache entry key is invalid.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  /// Failed to set cache entry.
  /// </exception>
  public async ValueTask SetAsync(
    string key,
    ReadOnlySequence<byte> value,
    DistributedCacheEntryOptions options,
    CancellationToken token = default) {
    _cacheDatastore.ValidateKey(key);
    _cacheInvalidation.PurgeEntriesIfRequired();
    var entryExpiry = MakeCacheEntryExpiry(options);
    await _cacheDatastore.SetEntry(
        key,
        value,
        entryExpiry,
        token)
      .ConfigureAwait(continueOnCapturedContext: false);
    _logger.LogDebug("Cache entry {EntryKey} set with expiry @{EntryExpiry}", key, entryExpiry);
  }

  private CacheEntryExpiry UpdateCacheEntryExpiry(CacheEntryExpiry cacheEntryExpiry) =>
    cacheEntryExpiry with {
      ExpiresAtUtc = _cacheInvalidation.ExpiryCalculator.CalculateExpiration(
        cacheEntryExpiry.AbsoluteExpiryAtUtc,
        cacheEntryExpiry.SlidingExpiryInterval)
    };

  private CacheEntryExpiry MakeCacheEntryExpiry(DistributedCacheEntryOptions options) {
    var absoluteExpirationUtc = _cacheInvalidation.ExpiryCalculator.CalculateAbsoluteExpiration(
      options.AbsoluteExpiration,
      options.AbsoluteExpirationRelativeToNow);
    var expiresAtUtc = _cacheInvalidation.ExpiryCalculator.CalculateExpiration(absoluteExpirationUtc, options.SlidingExpiration);
    var cacheEntryExpiry = new CacheEntryExpiry(expiresAtUtc, absoluteExpirationUtc, options.SlidingExpiration);
    return cacheEntryExpiry;
  }

  private readonly ICacheDatastore _cacheDatastore;
  private readonly TimeBasedCacheInvalidation _cacheInvalidation;
  private readonly ILogger _logger;
  private static readonly RecyclableMemoryStreamManager StreamManager = new();
}
