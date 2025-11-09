Feature: Cache invalidation with time-based cache invalidation

  Background:
    Given purging interval is 6 minutes
    And default sliding expiration interval is 1 minutes
    And cache entry expiry calculator with defined arguments
    And time-based cache invalidation with defined arguments

  Scenario: 01. Purging should start on time
    Given time passed by 00:06:00
    When I request cache invalidation
    Then purging is successfully done
    And no errors are reported
    And purging started event risen once
    And purging completed event risen once

  Scenario: 02. Purging should not start if time has not yet come
    Given time passed by 00:05:59
    When I request cache invalidation
    Then purging is not started
    And no errors are reported

  Scenario: 03. Only one out of a few concurrent purging should be executed
    Given time passed by 00:06:00
    When a few cache invalidations requested
    Then purging is successfully done
    And no errors are reported
    And only one purging should be done

  Scenario: 04. Early purging should not block purging on time
    Given time passed by 00:05:59
    And cache invalidation requested
    And purging is not started
    Given time passed by 00:00:01
    When I request cache invalidation
    Then purging is successfully done
    And no errors are reported
