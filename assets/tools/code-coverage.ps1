dotnet-coverage collect "dotnet test --solution Eshva.Caching.Abstractions.slnx --retry-failed-tests 5" --output-format cobertura --output cobertura.xml
reportgenerator -reports:cobertura.xml -targetdir:coverage -reporttypes:Html_Dark -assemblyfilters:+Eshva.Caching.Abstractions -classfilters:"-*Settings;-*CacheInvalidationStatistics;-*CacheEntryExpiry"
./coverage/index.html
