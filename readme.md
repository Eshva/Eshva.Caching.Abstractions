# Abstractions for different cache implementations

Here I collect abstractions required for my cache implementations.

Why this package is required:
- Currently only [NATS object-store based cache](https://github.com/Eshva/Eshva.Caching.Nats) is implemented. In the future I plan to add a NATS key-value store cache and may be more.
- Here I place contracts and abstract classes required to implement behavioral contract I test with BDD-tests. Behavior of the cache and support classes should be implemented once and be inherited by realizations to adjust only storage-related aspects.
- Stored/distributed cache has more responsibilities than just store and retrieve cache entries. According to SOLID they should be separated into different classes.

