Feature: Is cache entry expired with expiry calculator

  Background:
    Given clock set at today 20:00
    And default sliding expiration interval is 1 minutes
    And purging interval is 6 minutes
    And cache entry expiry calculator with defined arguments

  Scenario: 01. Cache entry which expires later than current time should not be reported as not expired
    Given cache entry that expires today at 20:00:01
    When I check is cache entry expired
    Then it should be not expired

  Scenario: 02. Cache entry which expires at current time should be reported as expired
    Given cache entry that expires today at 20:00:00
    When I check is cache entry expired
    Then it should be expired

  Scenario: 03. Cache entry which expires earlier than current time should be reported as expired
    Given cache entry that expires today at 19:59:59
    When I check is cache entry expired
    Then it should be expired
