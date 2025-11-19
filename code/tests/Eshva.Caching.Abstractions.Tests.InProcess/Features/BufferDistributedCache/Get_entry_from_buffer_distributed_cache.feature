Feature: Get entry from buffer distributed cache

  Background:
    Given purging interval is 00:03:00
    And maximal cache invalidation duration is 00:02:59
    And default sliding expiration interval is 1 minutes
    And clock set at today 00:00:00
    And buffer distributed cache
    And entry with key 'present' and value 'present value' which expires in 00:03:00 put into cache
    And entry with key 'expired' and value 'expired value' which expires in 00:04:00 put into cache

  Scenario: 01. Get a present cache entry async should return the entry value and advance its expiry by sliding interval
    When I get 'present' cache entry asynchronously
    Then no errors are reported
    And I should get value 'present value' as the requested entry
    And 'present' entry should be expired today at 00:01:00

  Scenario: 02. Get a present cache entry sync should return the entry value and advance its expiry by sliding interval
    When I get 'present' cache entry synchronously
    Then no errors are reported
    And I should get value 'present value' as the requested entry
    And 'present' entry should be expired today at 00:01:00

  Scenario: 03. Get a missing cache entry should not report errors and return null as entry value
    When I get 'missing' cache entry asynchronously
    Then no errors are reported
    And I should get a null value as the requested entry

  Scenario: 04. Get a cache entry should trigger cache invalidation if its interval has passed
    Given time passed by 00:03:00
    When I get 'present' cache entry asynchronously
    Then no errors are reported
    And cache invalidation should be triggered

  Scenario: 05. Get a cache entry should not trigger cache invalidation if its interval has not passed
    Given time passed by 00:02:00
    When I get 'present' cache entry asynchronously
    Then no errors are reported
    And cache invalidation should not be triggered

  Scenario: 06. Get an expired cache entry should not return it
    Given time passed by 00:03:00
    And get an entry 'expired' to trigger cache invalidation
    And time passed by 00:02:00
    When I get 'expired' cache entry asynchronously
    Then cache entry did not read
    And no errors are reported
