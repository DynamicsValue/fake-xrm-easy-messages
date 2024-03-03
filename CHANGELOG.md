## [2.5.0]

### Added

- **Minor Breaking Change**:  Generic Message executors have been moved to *FakeXrmEasy.FakeMessageExecutors.GenericExecutors* namespace for consistency with their own interface name
- Add ExportPdfDocumentExecutor - https://github.com/DynamicsValue/fake-xrm-easy/issues/125
- Add RetrieveCurrentOrganizationRequest executor - https://github.com/DynamicsValue/fake-xrm-easy/issues/136 

### Changed

- Introduces validation error when trying to add a user to a default team - https://github.com/DynamicsValue/fake-xrm-easy/issues/108
- Fixes responsible type in CloseIncidentRequestExecutor - https://github.com/DynamicsValue/fake-xrm-easy/issues/116
- Increase code coverage for NavigateToNextEntityOrganizationRequest
- Remove unnecessary checks in NavigateToNextEntityOrganizationRequestExecutor: thanks Temmy

## [2.4.0]

## Added

- **Alpha**: Introduced subscription usage monitoring based on customer feedback

### Changed

- Set default build configuration in solution file to FAKE_XRM_EASY_9
- Remove ReleaseNotes from package description - https://github.com/DynamicsValue/fake-xrm-easy/issues/115
- build.ps1 improvements: do not build project twice (added --no-build) when running dotnet test, do not build again either when packing assemblies either: https://github.com/DynamicsValue/fake-xrm-easy/issues/119
- Update build scripts to use 'all' target frameworks by default - https://github.com/DynamicsValue/fake-xrm-easy/issues/126
- Update github actions to use new Sonar environment variables - https://github.com/DynamicsValue/fake-xrm-easy/issues/120

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