Feature: Buffer distributed cache construction

  Background:
    Given purging interval is 00:03:00
    And maximal cache invalidation duration is 00:02:59
    And default sliding expiration interval is 1 minutes
    And cache invalidation
    And cache datastore
    And logger

  Scenario: 01. Can construct buffer distributed cache with valid argurments
    When I construct buffer distributed cache
    Then no errors are reported

  Scenario: 02. Constructing without cache invalidation should report an error
    Given cache invalidation not specified
    When I construct buffer distributed cache
    Then argument not specified error should be reported

  Scenario: 03. Can construct buffer distributed cache with valid argurments
    Given cache datastore not specified
    When I construct buffer distributed cache
    Then argument not specified error should be reported

  Scenario: 04. Can construct buffer distributed cache with valid argurments
    Given logger not specified
    When I construct buffer distributed cache
    Then no errors are reported
