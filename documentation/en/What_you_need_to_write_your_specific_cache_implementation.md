# What you need to write your specific cache implementation

To write your specific cache implementation, for example, using some database, you need to implement only 2 classes:

1. CacheDatastore, which implements the `ICacheDatastore` interface. As the name implies, it realizes the interaction of cache methods with a specific storage.
2. Cache invalidation, which inherits the `TimeBasedCacheInvalidation` abstract class. Also interacts with a specific storage to retrieve all obsolete entries and purge them.

## Cache Storage

The behavior contract of the methods is described in their documentation directly in the source code, there is nothing to add.

TODO: Write abstract tests to verify that the behavior contract is met.

## Cache invalidation

The inheritor class should implement the `DeleteExpiredCacheEntries` method, the `TimeBasedCacheInvalidation` base class will do the rest. The implementation of this method must use the item expiry calculator available in the `ExpiryCalculator` property.

The minimum interval between cache invalidation runs is set to 1 minute. If this is somehow an issue for your implementation, let me know.

As a bonus, not contracted behavior of Microsoft distributed cache implementations, this cache invalidation allows you to track when invalidation starts and ends using the `CacheInvalidationStarted` and `CacheInvalidationCompleted` events respectively. The only limitation is that these events do not take into account the synchronization context, i.e. they cannot update the properties of controls of UI frameworks such as WPF or WinForms. I don't consider this a serious limitation, as displaying such statistics on the screen is something strange to me.
