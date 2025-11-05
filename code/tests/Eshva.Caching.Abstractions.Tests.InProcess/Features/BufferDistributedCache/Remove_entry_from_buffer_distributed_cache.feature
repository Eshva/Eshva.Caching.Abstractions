Feature: Remove entry from buffer distributed cache

  Background:
    Given purging interval is 3 minutes
    And default sliding expiration interval is 1 minutes
    And clock set at today 00:00:00
    And buffer distributed cache
    And entry with key 'present' and value 'present value' which expires in 00:03:00 put into cache

  Scenario: 01. Remove a present cache entry asynchronously
    When I remove 'present' cache entry asynchronously
    Then no errors are reported
    And 'present' entry is not present in cache datastore

  Scenario: 02. Remove a present cache entry synchronously
    When I remove 'present' cache entry synchronously
    Then no errors are reported
    And 'present' entry is not present in cache datastore

  Scenario: 03. Remove a missing cache entry by key asynchronously should not report any errors
    When I remove 'missing' cache entry asynchronously
    Then no errors are reported

  Scenario: 04. Remove a cache entry should trigger cache invalidation if its interval has passed
    Given time passed by 00:03:00
    When I remove 'present' cache entry asynchronously
    Then no errors are reported
    And cache invalidation should be triggered

  Scenario: 05. Remove a cache entry should not trigger cache invalidation if its interval has not passed
    Given time passed by 00:02:00
    When I remove 'present' cache entry asynchronously
    Then no errors are reported
    And cache invalidation should not be triggered
