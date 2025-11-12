Feature: Limiting cache invalidation duration

  Scenario: 01. Should limit maximal cache duration
    Given purging interval is 00:01:00
    And maximal cache invalidation duration is 00:00:01
    And default sliding expiration interval is 1 minutes
    And cache entry expiry calculator with defined arguments
    And invalidation duration testing cache invalidation with defined arguments
    And time passed by 00:01:00
    When I request cache invalidation
    Then purging is successfully done in about 00:00:01
    And no errors are reported

