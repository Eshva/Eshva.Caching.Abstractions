Feature: Try get entry from buffer distributed cache

  Background:
    Given purging interval is 3 minutes
    And default sliding expiration interval is 1 minutes
    And clock set at today 00:00:00
    And buffer distributed cache
    And entry with key 'present' and value 'present value' which expires in 00:03:00 put into cache
    And entry with key 'big one' and random byte array as value which expires in 00:03:00 put into cache

  Scenario: 01. Try get a present cache entry async should return the entry value and advance its expiry by sliding interval
    When I try get 'present' cache entry asynchronously
    Then cache entry successfully read
    And no errors are reported
    And I should get value 'present value' as the requested entry
    And 'present' entry should be expired today at 00:01:00

  Scenario: 02. Get present cache entry by key synchronously
    When I try get 'present' cache entry synchronously
    Then cache entry successfully read
    And no errors are reported
    And I should get value 'present value' as the requested entry
    And 'present' entry should be expired today at 00:01:00

  Scenario: 03. Try get a missing cache entry should not report errors and return empty entry value
    When I try get 'missing' cache entry asynchronously
    Then cache entry did not read
    Then no errors are reported
    Then I should get value '' as the requested entry

  Scenario: 04. Try get a cache entry should trigger cache invalidation if its interval has passed
    Given time passed by 00:03:00
    When I try get 'present' cache entry asynchronously
    Then cache entry successfully read
    And no errors are reported
    And cache invalidation should be triggered

  Scenario: 05. Try get a cache entry should not trigger cache invalidation if its interval has not passed
    Given time passed by 00:02:00
    When I try get 'present' cache entry asynchronously
    Then no errors are reported
    And cache invalidation should not be triggered

  Scenario: 06. Try get a big cache entry async should return original value
    When I try get 'big one' cache entry asynchronously
    Then no errors are reported
    And cache entry successfully read
    And I should get same value as the requested entry
