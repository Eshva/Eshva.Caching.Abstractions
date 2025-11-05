Feature: Calculate absolute expiration with expiry calculator

  Background:
    Given clock set at today 00:20:00
    And default sliding expiration interval is 1 minutes
    And cache entry expiry calculator with defined arguments

  Scenario: 01. Calculate absolute expiration with only absolute expiration specified
    Given absolute expiration today at 00:21:00
    And no relative expiration time
    When I calculate absolute expiration time
    Then no errors are reported
    And calculated absolute expiration time should be today at 00:21:00

  Scenario: 02. Calculate absolute expiration with only relative expiration specified
    Given no absolute expiration time
    And relative expiration time is 10 minutes
    When I calculate absolute expiration time
    Then no errors are reported
    And calculated absolute expiration time should be today at 00:30:00

  Scenario: 03. Calculate absolute expiration with both absolute and relative expiration specified
    Given absolute expiration today at 00:21:00
    And relative expiration time is 10 minutes
    When I calculate absolute expiration time
    Then no errors are reported
    And calculated absolute expiration time should be today at 00:21:00

  Scenario: 04. Calculate absolute expiration with both absolute and relative expiration unspecified
    Given no absolute expiration time
    And no relative expiration time
    When I calculate absolute expiration time
    Then no errors are reported
    And calculated absolute expiration time should be null
