Feature: Refresh entry in buffer distributed cache

  Background:
    Given purging interval is 00:03:00
    And maximal cache invalidation duration is 00:02:59
    And default sliding expiration interval is 1 minutes
    And clock set at today 00:00:00
    And buffer distributed cache
    And entry with key 'will be refreshed' and value 'will be refreshed value' which expires in 00:03:00 put into cache
    And entry with key 'will be removed' and value 'will be removed value' which expires in 00:03:00 put into cache

  Scenario: 01. Refresh an existing entry asynchronously
    Given time passed by 00:02:00
    When I refresh 'will be refreshed' cache entry asynchronously
    Then 'will be refreshed' entry should be expired today at 00:03:00

  Scenario: 02. Refresh an existing entry synchronously
    Given time passed by 00:02:00
    When I refresh 'will be refreshed' cache entry synchronously
    Then 'will be refreshed' entry should be expired today at 00:03:00

  Scenario: 03. Refresh missed entry should report an error
    When I refresh 'missing' cache entry asynchronously
    Then invalid operation error should be reported

  Scenario: 04. Refresh a cache entry should trigger cache invalidation if its interval has passed
    Given time passed by 00:03:00
    When I refresh 'will be refreshed' cache entry synchronously
    Then cache invalidation should be triggered

  Scenario: 05. Get a cache entry should not trigger cache invalidation if its interval has not passed
    Given time passed by 00:02:00
    When I refresh 'will be refreshed' cache entry synchronously
    Then cache invalidation should not be triggered
