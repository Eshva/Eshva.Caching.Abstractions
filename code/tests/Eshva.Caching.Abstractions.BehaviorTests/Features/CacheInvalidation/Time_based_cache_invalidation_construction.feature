Feature: Time-based cache invalidation construction

  Background:
    Given minimal expired entries purging interval is 2 minutes
    And default sliding expiration interval is 1 minutes

  Scenario: 01. Can construct time-based cache invalidation with purging interval greater than minimal purging interval
    Given purging interval is 6 minutes
    When I construct time-based cache invalidation with defined arguments
    Then no errors are reported

  Scenario: 02. Can construct time-based cache invalidation with purging interval equals to minimal purging interval
    Given purging interval is 2 minutes
    When I construct time-based cache invalidation with defined arguments
    Then no errors are reported

  Scenario: 03. Should report an error if purging interval is less than minimal purging interval
    Given purging interval is 1 minutes
    When I construct time-based cache invalidation with defined arguments
    Then argument out of range exception should be reported
