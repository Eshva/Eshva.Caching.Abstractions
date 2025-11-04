Feature: Cache invalidation with time-based cache invalidation

  Background:
    Given purging interval is 6 minutes
    And default sliding expiration interval is 1 minutes
    And cache entry expiry calculator with defined arguments
    And time-based cache invalidation with defined arguments

  Scenario: 01. Purging should start on time
    Given time passed by 6 minutes
    When I request cache invalidation
    Then purging is successfully done
    And no errors are reported

  Scenario: 02. Purging should not start if time has not yet come
    Given time passed by 5 minutes
    When I request cache invalidation
    Then purging is not started
    And no errors are reported

  Scenario: 03. Only one out of a few concurrent purging should be executed
    Given time passed by 6 minutes
    When a few cache invalidations requested
    Then purging is successfully done
    And no errors are reported
    And only one purging should be done
