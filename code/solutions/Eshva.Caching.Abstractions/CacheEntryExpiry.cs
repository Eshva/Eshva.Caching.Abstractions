namespace Eshva.Caching.Abstractions;

/// <summary>
/// Cache entry expiry metadata.
/// </summary>
/// <param name="ExpiresAtUtc">Entry expiry moment UTC.</param>
/// <param name="AbsoluteExpiryAtUtc">Entry absolute expiry moment UTC.</param>
/// <param name="SlidingExpiryInterval">Entry sliding expiry interval.</param>
public readonly record struct CacheEntryExpiry(
  DateTimeOffset ExpiresAtUtc,
  DateTimeOffset? AbsoluteExpiryAtUtc,
  TimeSpan? SlidingExpiryInterval);
