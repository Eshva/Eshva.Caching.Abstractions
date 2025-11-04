Feature: Time-based cache invalidation construction

  Background:
    Given default sliding expiration interval is 1 minutes
    And cache entry expiry calculator with defined arguments

  Scenario: 01. Can construct time-based cache invalidation with purging interval greater than minimal purging interval
    Given purging interval is 6 minutes
    When I construct time-based cache invalidation with defined arguments
    Then no errors are reported

  Scenario: 02. Can construct time-based cache invalidation with purging interval equals to minimal purging interval
    Given purging interval is 2 minutes
    When I construct time-based cache invalidation with defined arguments
    Then no errors are reported

  Scenario: 03. Should report an error if cache entry expiry calculator not specified
    Given purging interval is 2 minutes
    And cache entry expiry calculator is not specified
    When I construct time-based cache invalidation with defined arguments
    Then argument not specified exception should be reported

  Scenario: 04. Should report an error if time provider not specified
    Given purging interval is 2 minutes
    And time provider is not specified
    When I construct time-based cache invalidation with defined arguments
    Then argument not specified exception should be reported

