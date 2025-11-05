using System.Buffers;

namespace Eshva.Caching.Abstractions.Tests.InProcess.Features.BufferDistributedCache;

internal sealed class TestInMemoryCacheDatastore : ICacheDatastore {
  public void Add(string key, byte[] value, CacheEntryExpiry expiry) =>
    _entries.Add(key, new Entry(value, expiry));

  public void Remove(string key) =>
    _entries.Remove(key);

  public CacheEntryExpiry GetExpiry(string key) =>
    _entries[key].Expiry;

  public byte[] GetValue(string key) =>
    !_entries.TryGetValue(key, out var entry)
      ? throw new InvalidOperationException("The entry does not exist.")
      : entry.Value;

  Task ICacheDatastore.RefreshEntry(string key, CacheEntryExpiry cacheEntryExpiry, CancellationToken cancellation) {
    if (!_entries.TryGetValue(key, out var entry)) throw new InvalidOperationException("The entry does not exist.");
    entry.Expiry = cacheEntryExpiry;
    return Task.CompletedTask;
  }

  Task<CacheEntryExpiry> ICacheDatastore.GetEntryExpiry(string key, CancellationToken cancellation) =>
    !_entries.TryGetValue(key, out var entry)
      ? throw new InvalidOperationException("The entry does not exist.")
      : Task.FromResult(entry.Expiry);

  Task ICacheDatastore.RemoveEntry(string key, CancellationToken cancellation) {
    _entries.Remove(key);
    return Task.CompletedTask;
  }

  Task<(bool isEntryGotten, CacheEntryExpiry cacheEntryExpiry)> ICacheDatastore.TryGetEntry(
    string key,
    IBufferWriter<byte> destination,
    CancellationToken cancellation) {
    if (!_entries.TryGetValue(key, out var entry)) {
      return Task.FromResult<(bool isEntryGotten, CacheEntryExpiry cacheEntryExpiry)>(
        (false, new CacheEntryExpiry(DateTimeOffset.MinValue, AbsoluteExpiryAtUtc: null, SlidingExpiryInterval: null)));
    }

    destination.Write(entry.Value);
    return Task.FromResult<(bool isEntryGotten, CacheEntryExpiry cacheEntryExpiry)>((true, _entries[key].Expiry));
  }

  Task ICacheDatastore.SetEntry(
    string key,
    ReadOnlySequence<byte> value,
    CacheEntryExpiry cacheEntryExpiry,
    CancellationToken cancellation) {
    _entries[key] = new Entry(value.ToArray(), cacheEntryExpiry);
    return Task.CompletedTask;
  }

  void ICacheDatastore.ValidateKey(string key) {
    if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key is invalid.");
  }

  private readonly Dictionary<string, Entry> _entries = new();

  private sealed record Entry {
    public Entry(byte[] value, CacheEntryExpiry expiry) {
      Value = value;
      Expiry = expiry;
    }

    public byte[] Value { get; }

    public CacheEntryExpiry Expiry { get; set; }
  }
}
