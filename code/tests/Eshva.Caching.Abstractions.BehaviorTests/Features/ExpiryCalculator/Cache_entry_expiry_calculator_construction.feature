Feature: Cache entry expiry calculator construction

  Scenario: 01. Can construct cache entry expiry calculator construction with correct arguments
    Given default sliding expiration interval is 1 minutes
    When I construct cache entry expiry calculator
    Then no errors are reported

  Scenario: 02. Should report an error if time provider is not specified
    Given time provider is not specified
    And default sliding expiration interval is 1 minutes
    When I construct cache entry expiry calculator
    Then argument not specified exception should be reported

  Scenario: 03. Should report an error if default sliding expiration time is less than a minute
    Given default sliding expiration interval is 0,5 minutes
    When I construct cache entry expiry calculator
    Then argument out of range exception should be reported
