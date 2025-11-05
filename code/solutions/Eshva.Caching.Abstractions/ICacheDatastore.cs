using System.Buffers;

namespace Eshva.Caching.Abstractions;

/// <summary>
/// Cache datastore.
/// </summary>
public interface ICacheDatastore {
  /// <summary>
  /// Get cache entry expiry information.
  /// </summary>
  /// <param name="key">Cache entry key.</param>
  /// <param name="cancellation">Cancellation token.</param>
  /// <returns>Cache entry expiry information.</returns>
  /// <exception cref="InvalidOperationException">
  /// The entry with <paramref name="key"/> not found.
  /// </exception>
  Task<CacheEntryExpiry> GetEntryExpiry(string key, CancellationToken cancellation);

  /// <summary>
  /// Refresh a cache entry expiry information.
  /// </summary>
  /// <param name="key">Cache entry key.</param>
  /// <param name="cacheEntryExpiry">Cache entry expiry information.</param>
  /// <param name="cancellation">Cancellation token.</param>
  /// <returns>A task that represents the entry refreshing.</returns>
  /// <exception cref="InvalidOperationException">
  /// The entry with <paramref name="key"/> not found.
  /// </exception>
  Task RefreshEntry(string key, CacheEntryExpiry cacheEntryExpiry, CancellationToken cancellation);

  /// <summary>
  /// Remove a entry from cache.
  /// </summary>
  /// <param name="key">Cache entry key.</param>
  /// <param name="cancellation">Cancellation token.</param>
  /// <returns>A task that represents the entry removal.</returns>
  /// <exception cref="InvalidOperationException">
  /// Failed to remove cache entry.
  /// </exception>
  Task RemoveEntry(string key, CancellationToken cancellation);

  /// <summary>
  /// Try to get a cache entry.
  /// </summary>
  /// <param name="key">Cache entry key.</param>
  /// <param name="destination">Entry value destination buffer writer.</param>
  /// <param name="cancellation">Cancellation token.</param>
  /// <returns>Tuple: is entry gotten, cache entry expiry information.</returns>
  /// <exception cref="InvalidOperationException">
  /// Failed to get cache entry.
  /// </exception>
  Task<(bool isEntryGotten, CacheEntryExpiry cacheEntryExpiry)> TryGetEntry(
    string key,
    IBufferWriter<byte> destination,
    CancellationToken cancellation);

  /// <summary>
  /// Set a cache entry.
  /// </summary>
  /// <param name="key">Cache entry key.</param>
  /// <param name="value">Cache entry value.</param>
  /// <param name="cacheEntryExpiry">Cache entry expiry information.</param>
  /// <param name="cancellation">Cancellation token.</param>
  /// <returns>A task that represents the entry setting.</returns>
  /// <exception cref="InvalidOperationException">
  /// Failed to set cache entry.
  /// </exception>
  Task SetEntry(
    string key,
    ReadOnlySequence<byte> value,
    CacheEntryExpiry cacheEntryExpiry,
    CancellationToken cancellation);

  /// <summary>
  /// Validate cache entry key.
  /// </summary>
  /// <param name="key">Cache entry key.</param>
  /// <exception cref="ArgumentException">
  /// Cache entry key is invalid.
  /// </exception>
  void ValidateKey(string key);
}
