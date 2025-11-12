Feature: Time-based cache invalidation construction

  Background:
    Given default sliding expiration interval is 1 minutes
    And cache entry expiry calculator with defined arguments

  Scenario: 01. Can construct time-based cache invalidation with purging interval greater than minimal purging interval
    Given purging interval is 00:06:00
    And maximal cache invalidation duration is 00:05:59
    When I construct time-based cache invalidation with defined arguments
    Then no errors are reported

  Scenario: 02. Can construct time-based cache invalidation with purging interval equals to minimal purging interval
    Given purging interval is 00:01:00
    And maximal cache invalidation duration is 00:00:59
    When I construct time-based cache invalidation with defined arguments
    Then no errors are reported

  Scenario: 03. Should report an error if cache entry expiry calculator not specified
    Given purging interval is 00:01:00
    And maximal cache invalidation duration is 00:00:59
    And cache entry expiry calculator is not specified
    When I construct time-based cache invalidation with defined arguments
    Then argument not specified exception should be reported

  Scenario: 04. Should report an error if time provider not specified
    Given purging interval is 00:01:00
    And maximal cache invalidation duration is 00:00:59
    And time provider is not specified
    When I construct time-based cache invalidation with defined arguments
    Then argument not specified exception should be reported

  Scenario: 05. Should report an error if purging interval is less than minimal purging interval
    Given purging interval is 00:00:59
    And maximal cache invalidation duration is 00:00:58
    When I construct time-based cache invalidation with defined arguments
    Then argument out of range exception should be reported

  Scenario: 06. Should report an error if maximal cache invalidation duration is greater than purging interval
    Given purging interval is 00:01:00
    And maximal cache invalidation duration is 00:01:01
    When I construct time-based cache invalidation with defined arguments
    Then argument out of range exception should be reported

  Scenario: 07. Should report an error if maximal cache invalidation duration is equal to purging interval
    Given purging interval is 00:01:00
    And maximal cache invalidation duration is 00:01:00
    When I construct time-based cache invalidation with defined arguments
    Then argument out of range exception should be reported
