## [2.3.3]

### Changed

- Updated build scripts so that it actually deletes bin folders as opposed to doing dotnet clean -  https://github.com/DynamicsValue/fake-xrm-easy/issues/76
- Upgraded GitHub Actions to update Java major version to run SonarCloud analysis - https://github.com/DynamicsValue/fake-xrm-easy/issues/110
- Returns organisation and business unit in WhoAmIRequest to support Calendar rules - https://github.com/DynamicsValue/fake-xrm-easy/issues/23
- Update legacy CRM SDK 2011 dependency to use official MS package - https://github.com/DynamicsValue/fake-xrm-easy/issues/105

## [2.3.0]

### Changed

- Updated dependencies

## [2.2.0]

### Changed

- Replace references to PullRequestException by UnsupportedExceptionFactory
- Fix Sonar Quality Gate settings: DynamicsValue/fake-xrm-easy#28

## [2.1.1]

### Changed 

- Limit FakeItEasy package dependency to v6.x versions - DynamicsValue/fake-xrm-easy#37
- Made CRM SDK v8.2 dependencies less specific - DynamicsValue/fake-xrm-easy#21
- Updated build script to also include the major version in the Title property of the generated .nuspec file - DynamicsValue/fake-xrm-easy#41

## [2.1.0]

### Changed

 - Update executor after fix DynamicsValue/fake-xrm-easy#16 was introduced.
 - Update package reference ranges to use 2.x versions only.
 - Bump Microsoft.CrmSdk.CoreAssemblies to version 9.0.2.27 to support plugin telemetry - DynamicsValue/fake-xrm-easy#24



## [2.0.1-rc1] - Initial release