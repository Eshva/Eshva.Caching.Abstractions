# Distributed/stored cache behavior contract

Microsoft in its standard .NET library offers a distributed cache contract [`IDistributedCache`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.distributed.idistributedcache?view=net-9.0-pp) and its further development with an eye on the performance of [`IBufferDistributedCache`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.distributed.ibufferdistributedcache?view=net-9.0-pp). Unfortunately, Microsoft offers only an interface contract, but poorly (or better to say "almost nothing") formalize the behavior contract. I had to reconstruct the behavior contract based on two specific implementations of [`SqlServerCache`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.sqlserver.sqlservercache?view=net-10.0-pp&viewFallbackFrom=net-9.0-pp) and [`RedisCache`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.stackexchangeredis.rediscache?view=net-10.0-pp&viewFallbackFrom=net-9.0-pp). I may have gotten something wrong, but nevertheless, it's better than nothing. Based on my understanding, I created abstractions that implement this behavior contract and leave only the storage interaction issues to the concrete implementations. For an example, you can see [my implementations of a distributed cache based on NATS object and key/value stores](https://github.com/Eshva/Eshva.Caching.Nats).

## Requirements to keys and values of cache entries

- An entry is accessed by its key.
- The key is mandatory and is a non-empty string consisting of non-whitespace characters.
- Whitespace characters at the beginning and the end of the key are not removed.
- Keys in the cache must be unique.
- The value is always an array of bytes.
- The value of an entry cannot be missing, that is, it cannot be set to `null`.
- Specific implementations may impose restrictions on the cache entry key. For example, the NATS key/value store imposes restrictions on special characters that can occur in the key.
- Specific implementations, as well as implementation-specific storage settings, may impose their own restrictions on the maximum size of an entry's value.

## Cache entry expiry

- This is a cache with entry lifetime invalidation.
- Each cache entry must have its expiry parameters associated with it: moment of expiry, absolute expiry moment, sliding expiry interval. They are its metadata.
- Absolute expiry defines a moment in time, in other words date/time.
- Sliding expiry defines the time interval of an entry's lifetime from the last time it was gotten or forced to be refreshed.
- Absolute and/or sliding expiry can be defined at the time a new entry is set or at the time an existing entry is updated.
- If neither absolute nor sliding expiry was specified when set an entry, the entry is assigned sliding expiry with default duration (it is determined when creating the cache, it is better to put it in the application settings).
- If an entry is already expired, it cannot be gotten and must be deleted.
- Absolute expiry has priority over sliding expiry, i.e. if a record is expired from the point of view of absolute expiry, the entry must be deleted.
- An entry's expiry moment can only be updated if sliding expiry is configured for the entry.

## Cache Invalidation

- Cache invalidation involves deleting all expired entries.
- Cache invalidation has a chance to be executed when any cache contract method is called.
- For optimization purposes, cache invalidation is not run more often than a certain interval, such as 1 minute. This interval must be defined by the particular cache implementation.
- Only one invalidation can run at a given moment.
- Cache invalidation can be started on a timer or only at the moment of cache access. I have chosen the latter approach as it is simpler, thriftier and more reliable.
- Cache invalidation is executed in the background so as not to slow down the access operation that started it.

## Errors of working with the cache

Unfortunately, Microsoft doesn't define a contract to report possible errors in the description of distributed cache interfaces methods except for the most basic ones like [`OperationCanceledException`](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception?view=net-10.0-pp) or [`ArgumentException`](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception?view=net-10.0-pp). And they can occur. For example, what should be done if an empty string is passed as an entry's key? Or what exception should be thrown if a cache store becomes inaccessible? Cache methods should not throw implementation-specific exceptions to prevent leaky abstractions (unfortunately, at least in Microsoft's specific implementations, this is not the case). I had to define the error message contract on my own and chose argument-related exceptions and `InvalidOperationException` for everything else.

- Argument-related errors, depending on the context, should be reported using the most appropriate argument-related exceptions: `ArgumentNullException`, `ArgumentException`, `ArgumentOutOfRangeException`.
- Other errors, such as cache store inaccessibility, should be reported using `InvalidOperationException`.
- Deleting a previously deleted, due to expiry or direct deletion, entry must not result in an error.
- Obtaining the value of a nonexistent, previously deleted or expired entry shall not cause errors.
- Updating the expiry of a non-existent, previously deleted or expired entry shall not cause errors.
