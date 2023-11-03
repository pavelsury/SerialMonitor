# Serial Monitor 2 changelog

All notable changes to SerialMonitor2 are documented in this file.

## v1.15.0 (3.11.2023)
### Added
-   Custom send buttons
-   Custom command variables
-   New 'Show Buttons tab' setting
-   New 'Show Commands tab' setting

## v1.14.0 (23.5.2023)
### Added
-   EOL command variables (%EOL_CR%, ...)
-   DATA command variables (%DATA,1,2,3%, ...) with endianness support
-   New 'Write resolved command to console' setting
-   New 'Write sent bytes to console' setting
### Changed
-   Improved USB event handling
-   Drop support for VS2017 (it still might work but is not tested anymore)

## v1.13.0 (6.7.2022)
### Added
-   Winter time command variables (%WINTER_NOW%, %WINTER_NOW_DATE%, ...)

## v1.12.0 (17.4.2022)
### Added
-   Command variables (%NOW%, %UTC_NOW%, ...)
-   New option: Show . for non-printable ascii
-   New option: Clear console before command sent

## v1.11.0 (11.2.2022)
### Added
-   File send

## v1.10.1 (14.12.2021)
### Changed
-   Fix DtrEnable not set

## v1.10.0 (20.11.2021)
### Added
-   VS2022 support
### Changed
-   Standalone application targets .NET 6

## v1.9.0 (14.8.2021)
### Added
-   Available as a standalone application
### Changed
-   Small fixes

## v1.8.0 (9.6.2021)
### Added
-   Automatically switch to available port
### Changed
-   Small fixes

## v1.7.0 (6.6.2021)
### Added
-   Pipe IPC
### Changed
-   Small fixes

## v1.6.0 (1.5.2021)
### Added
-   Hex data view
### Changed
-   Small fixes

## v1.5.0 (14.4.2021)
### Added
-   Autoconnection
### Changed
-   Port reading refactoring

## v1.4.0 (8.4.2021)
### Added
-   Port settings save
### Changed
-   Big refactoring

## v1.3.0 (12.3.2021)
### Added
-   Font style setting
### Changed
-   Support for VS2017 & VS2019
