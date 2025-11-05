Feature: Set entry in buffer distributed cache. Sequence reader

  Background:
    Given purging interval is 3 minutes
    And default sliding expiration interval is 1 minutes
    And clock set at today 00:00:00
    And buffer distributed cache
    And entry with key 'present' and value 'present value' which expires in 00:03:00 put into cache

  Scenario: 01. Set a new cache entry asynchronosly with sliding expiration
    When I set using sequence reader asynchronously 'new' cache entry with value 'some value' and sliding expiration in 5 minutes
    Then value of 'new' entry is 'some value'
    And 'new' entry should be expired today at 00:05:00
    And sliding expiry interval of 'new' entry should be 00:05:00
    And absolute expiry of 'new' entry should be null

  Scenario: 02. Set a new cache entry synchronosly with sliding expiration
    When I set using sequence reader synchronously 'new' cache entry with value 'some value' and sliding expiration in 5 minutes
    Then value of 'new' entry is 'some value'
    And 'new' entry should be expired today at 00:05:00
    And sliding expiry interval of 'new' entry should be 00:05:00
    And absolute expiry of 'new' entry should be null

  Scenario: 03. Set a new cache entry asynchronosly with absolute expiration
    When I set using sequence reader asynchronously 'new' cache entry with value 'some value' and absolute expiration at today at 00:30:00
    Then value of 'new' entry is 'some value'
    And 'new' entry should be expired today at 00:30:00
    And sliding expiry interval of 'new' entry should be null
    And absolute expiry of 'new' entry should be today at 00:30:00

  Scenario: 04. Set a new cache entry synchronosly with absolute expiration
    When I set using sequence reader synchronously 'new' cache entry with value 'some value' and absolute expiration at today at 00:30:00
    Then value of 'new' entry is 'some value'
    And 'new' entry should be expired today at 00:30:00
    And sliding expiry interval of 'new' entry should be null
    And absolute expiry of 'new' entry should be today at 00:30:00

  Scenario: 05. Set a new cache entry asynchronosly with absolute expiration relative to now
    Given clock set at today 00:05:00
    When I set using sequence reader asynchronously 'new' cache entry with value 'some value' and absolute expiration 00:20:00 relative to now
    Then value of 'new' entry is 'some value'
    And 'new' entry should be expired today at 00:25:00
    And sliding expiry interval of 'new' entry should be null
    And absolute expiry of 'new' entry should be today at 00:25:00

  Scenario: 06. Set a new cache entry synchronosly with absolute expiration relative to now
    Given clock set at today 00:05:00
    When I set using sequence reader synchronously 'new' cache entry with value 'some value' and absolute expiration 00:20:00 relative to now
    Then value of 'new' entry is 'some value'
    And 'new' entry should be expired today at 00:25:00
    And sliding expiry interval of 'new' entry should be null
    And absolute expiry of 'new' entry should be today at 00:25:00

  Scenario: 07. Set a new cache entry asynchronosly with absolute and sliding expiration
    When I set using sequence reader asynchronously 'new' cache entry with value 'some value' and absolute expiration at today at 00:30:00 and sliding expiration in 5 minutes
    Then value of 'new' entry is 'some value'
    And 'new' entry should be expired today at 00:05:00
    And sliding expiry interval of 'new' entry should be 00:05:00
    And absolute expiry of 'new' entry should be today at 00:30:00

  Scenario: 08. Set a new cache entry synchronosly with absolute and sliding expiration
    When I set using sequence reader synchronously 'new' cache entry with value 'some value' and absolute expiration at today at 00:30:00 and sliding expiration in 5 minutes
    Then value of 'new' entry is 'some value'
    And 'new' entry should be expired today at 00:05:00
    And sliding expiry interval of 'new' entry should be 00:05:00
    And absolute expiry of 'new' entry should be today at 00:30:00

  Scenario: 09. Set an present cache entry asynchronosly with sliding expiration
    Given clock set at today 00:02:00
    When I set using sequence reader asynchronously 'present' cache entry with value 'some value' and sliding expiration in 5 minutes
    Then value of 'present' entry is 'some value'
    And 'present' entry should be expired today at 00:07:00
    And sliding expiry interval of 'present' entry should be 00:05:00
    And absolute expiry of 'present' entry should be null

  Scenario: 10. Set a new cache entry synchronosly with sliding expiration
    Given clock set at today 00:02:00
    When I set using sequence reader synchronously 'present' cache entry with value 'some value' and sliding expiration in 5 minutes
    Then value of 'present' entry is 'some value'
    And 'present' entry should be expired today at 00:07:00
    And sliding expiry interval of 'present' entry should be 00:05:00
    And absolute expiry of 'present' entry should be null

  Scenario: 11. Set an present cache entry asynchronosly with absolute expiration
    When I set using sequence reader asynchronously 'present' cache entry with value 'some value' and absolute expiration at today at 00:30:00
    Then value of 'present' entry is 'some value'
    And 'present' entry should be expired today at 00:30:00
    And sliding expiry interval of 'present' entry should be null
    And absolute expiry of 'present' entry should be today at 00:30:00

  Scenario: 12. Set a new cache entry synchronosly with absolute expiration
    When I set using sequence reader synchronously 'present' cache entry with value 'some value' and absolute expiration at today at 00:30:00
    Then value of 'present' entry is 'some value'
    And 'present' entry should be expired today at 00:30:00
    And sliding expiry interval of 'present' entry should be null
    And absolute expiry of 'present' entry should be today at 00:30:00

  Scenario: 13. Set an present cache entry asynchronosly with absolute and sliding expiration
    Given clock set at today 00:02:00
    When I set using sequence reader asynchronously 'present' cache entry with value 'some value' and absolute expiration at today at 00:30:00 and sliding expiration in 5 minutes
    Then value of 'present' entry is 'some value'
    And 'present' entry should be expired today at 00:07:00
    And sliding expiry interval of 'present' entry should be 00:05:00
    And absolute expiry of 'present' entry should be today at 00:30:00

  Scenario: 14. Set an present cache entry synchronosly with absolute and sliding expiration
    Given clock set at today 00:02:00
    When I set using sequence reader synchronously 'present' cache entry with value 'some value' and absolute expiration at today at 00:30:00 and sliding expiration in 5 minutes
    Then value of 'present' entry is 'some value'
    And 'present' entry should be expired today at 00:07:00
    And sliding expiry interval of 'present' entry should be 00:05:00
    And absolute expiry of 'present' entry should be today at 00:30:00

  Scenario: 15. Set an present cache entry asynchronosly with absolute expiration relative to now
    Given clock set at today 00:05:00
    When I set using sequence reader asynchronously 'present' cache entry with value 'some value' and absolute expiration 00:20:00 relative to now
    Then value of 'present' entry is 'some value'
    And 'present' entry should be expired today at 00:25:00
    And sliding expiry interval of 'present' entry should be null
    And absolute expiry of 'present' entry should be today at 00:25:00

  Scenario: 16. Set an present cache entry synchronosly with absolute expiration relative to now
    Given clock set at today 00:05:00
    When I set using sequence reader synchronously 'present' cache entry with value 'some value' and absolute expiration 00:20:00 relative to now
    Then value of 'present' entry is 'some value'
    And 'present' entry should be expired today at 00:25:00
    And sliding expiry interval of 'present' entry should be null
    And absolute expiry of 'present' entry should be today at 00:25:00

  Scenario: 17. Set a cache entry should trigger cache invalidation if its interval has passed
    Given time passed by 00:03:00
    When I set using sequence reader asynchronously 'new' cache entry with value 'some value' and sliding expiration in 5 minutes
    Then no errors are reported
    And cache invalidation should be triggered

  Scenario: 18. Set a cache entry should not trigger cache invalidation if its interval has not passed
    Given time passed by 00:02:59
    When I set using sequence reader asynchronously 'new' cache entry with value 'some value' and sliding expiration in 5 minutes
    Then no errors are reported
    And cache invalidation should not be triggered
