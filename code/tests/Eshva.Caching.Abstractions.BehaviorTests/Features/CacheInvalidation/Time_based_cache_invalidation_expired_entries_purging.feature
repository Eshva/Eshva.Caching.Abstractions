Feature: Time-based cache invalidation expired entries purging

  Background:
    Given minimal expired entries purging interval is 2 minutes
    And purging interval is 6 minutes
    And default sliding expiration interval is 1 minutes
    And time-based cache invalidation with defined arguments

  Scenario: 01. Purging should start on time
    Given time passed by 6 minutes
    When cache invalidation requested
    Then awaited purging is finished
    And no errors are reported
    And purging should be done

  Scenario: 02. Purging should not start if time has not yet come
    Given time passed by 5 minutes
    When cache invalidation requested
    Then awaited purging is finished
    And no errors are reported
    And purging should not start

  Scenario: 03. Only one out of a few concurrent purging should be executed
    Given time passed by 6 minutes
    When a few cache invalidations requested
    Then awaited purging is finished
    And no errors are reported
    And only one purging should be done
