Feature: Calculate expiration with expiry calculator

  Background:
    Given clock set at today 20:00:00
    And default sliding expiration interval is 1 minutes
    And cache entry expiry calculator with defined arguments

  Scenario: 01. Given only absolute expiration on expiration calculation it should return absolute expriation
    Given absolute expiration today at 21:00:00
    And no sliding expiration time
    When I calculate expiration time
    Then calculated expiration time should be today at 21:00:00

  Scenario: 02. Given only sliding expiration on expiration calculation it should advance current time with sliding expriation
    Given no absolute expiration time
    And sliding expiration in 10 minutes
    And time passed by 00:05:00
    When I calculate expiration time
    Then calculated expiration time should be today at 20:15:00

  Scenario: 03. Given no expirations on expiration calculation it should advance current time with default sliding expriation
    Given no absolute expiration time
    And no sliding expiration time
    And time passed by 00:05:00
    When I calculate expiration time
    Then calculated expiration time should be today at 20:06:00

  Scenario: 04. Given both expirations and absolute expiration in future of sliding expiration on expiration calculation it should advance current time with sliding expriation
    Given absolute expiration today at 21:00:00
    And sliding expiration in 40 minutes
    And time passed by 00:19:00
    When I calculate expiration time
    Then calculated expiration time should be today at 20:59:00

  Scenario: 05. Given both expirations and absolute expiration in past of sliding expiration on expiration calculation it should return absolute expriation
    Given absolute expiration today at 21:00:00
    And sliding expiration in 40 minutes
    And time passed by 00:21:00
    When I calculate expiration time
    Then calculated expiration time should be today at 21:00:00
