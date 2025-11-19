# Eshva.Caching.Abstractions
This package is the basis for creating caches. Currently, it only has abstractions to implement a distributed cache
based on the `IDistributedCache` contract and its development `IBufferDistributedCache` from the .NET standard library.
In the future, I plan to add primitives to implement what I call the proactive cache.

You can read more about the distributed cache behavior contract and implementation of specific caches in the following
articles:
* [Distributed/stored cache behavior contract](documentation/en/Distributed_stored_cache_behavior_contract.md)
* [What you need to write your specific cache implementation](documentation/en/What_you_need_to_write_your_specific_cache_implementation.md)

---
Данный пакет является основой для создания компонентов кэширования. В настоящее время он имеет абстракции только для
реализации распределённого кэша на основе контракта `IDistributedCache` и его развития `IBufferDistributedCache` из
стандартной библиотеки .NET. В будущем я планирую добавить примитивы для реализации, как я его называю, проактивного
кэша.

Более подробно о контракте поведения распределённого кэша и реализации конкретных кэшей можно прочесть в следующих
статьях:
* [Контракт поведения распределённого/хранимого кэша](documentation/ru/%D0%9A%D0%BE%D0%BD%D1%82%D1%80%D0%B0%D0%BA%D1%82_%D0%BF%D0%BE%D0%B2%D0%B5%D0%B4%D0%B5%D0%BD%D0%B8%D1%8F_%D1%80%D0%B0%D1%81%D0%BF%D1%80%D0%B5%D0%B4%D0%B5%D0%BB%D1%91%D0%BD%D0%BD%D0%BE%D0%B3%D0%BE_%D1%85%D1%80%D0%B0%D0%BD%D0%B8%D0%BC%D0%BE%D0%B3%D0%BE_%D0%BA%D1%8D%D1%88%D0%B0.md)
* [Что нужно, чтобы написать свою конкретную реализацию кэша](documentation/ru/%D0%A7%D1%82%D0%BE_%D0%BD%D1%83%D0%B6%D0%BD%D0%BE_%D1%87%D1%82%D0%BE%D0%B1%D1%8B_%D0%BD%D0%B0%D0%BF%D0%B8%D1%81%D0%B0%D1%82%D1%8C_%D1%81%D0%B2%D0%BE%D1%8E_%D0%BA%D0%BE%D0%BD%D0%BA%D1%80%D0%B5%D1%82%D0%BD%D1%83%D1%8E_%D1%80%D0%B5%D0%B0%D0%BB%D0%B8%D0%B7%D0%B0%D1%86%D0%B8%D1%8E_%D0%BA%D1%8D%D1%88%D0%B0.md)
