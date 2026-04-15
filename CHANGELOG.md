## [3.9.1]

### Changed

- Upgraded Microsoft.PowerPlatform.Dataverse.Client package to 1.2.10

## [3.9.0]

- Increment version 

## [3.8.0]

### Changed

- Upgraded to net8.0
- Upgraded to DataverseClient 1.2.9
- SendEmailRequest won't generate an email tracking token if tracking token generation in EmailSettings is disabled - https://github.com/DynamicsValue/fake-xrm-easy/issues/196

## [3.7.0]

### Added 

- Adds implementation for SendFaxRequest - https://github.com/DynamicsValue/fake-xrm-easy/issues/188
- Adds implementation for SendTemplateRequest - https://github.com/DynamicsValue/fake-xrm-easy/issues/188
- Adds implementation for SendEmailFromTemplateRequest - https://github.com/DynamicsValue/fake-xrm-easy/issues/186
- Adds implementation for InstantiateTemplateRequest using XSLT transforms - https://github.com/DynamicsValue/fake-xrm-easy/issues/178
 
### Changed 

- Replaced old generated code with generated code by pac modelbuilder
- Populates extra attributes when an email is sent with SendEmailRequest - https://github.com/DynamicsValue/fake-xrm-easy/issues/186

## [3.6.0]

### Added

- Added new messages for File and Image Upload/Download/Delete - https://github.com/DynamicsValue/fake-xrm-easy/issues/157

## [3.5.0]

### Added

- **Minor Breaking Change**:  Generic Message executors have been moved to *FakeXrmEasy.FakeMessageExecutors.GenericExecutors* namespace for consistency with their own interface name
- Add ExportPdfDocumentExecutor - https://github.com/DynamicsValue/fake-xrm-easy/issues/125
- Add RetrieveCurrentOrganizationRequest executor - https://github.com/DynamicsValue/fake-xrm-easy/issues/136 

### Changed

- Introduces validation error when trying to add a user to a default team - https://github.com/DynamicsValue/fake-xrm-easy/issues/108
- Fixes responsible type in CloseIncidentRequestExecutor - https://github.com/DynamicsValue/fake-xrm-easy/issues/116
- Increase code coverage for NavigateToNextEntityOrganizationRequest
- Remove unnecessary checks in NavigateToNextEntityOrganizationRequestExecutor: thanks Temmy

## [3.4.0]

## Added

- **Alpha**: Introduced subscription usage monitoring based on customer feedback

### Changed

- Set default build configuration in solution file to FAKE_XRM_EASY_9
- Remove ReleaseNotes from package description - https://github.com/DynamicsValue/fake-xrm-easy/issues/115
- build.ps1 improvements: do not build project twice (added --no-build) when running dotnet test, do not build again either when packing assemblies either: https://github.com/DynamicsValue/fake-xrm-easy/issues/119
- Update build scripts to use 'all' target frameworks by default - https://github.com/DynamicsValue/fake-xrm-easy/issues/126
- Update github actions to use new Sonar environment variables - https://github.com/DynamicsValue/fake-xrm-easy/issues/120

## [3.3.3]

### Changed

- Updated build scripts so that it actually deletes bin folders as opposed to doing dotnet clean -  https://github.com/DynamicsValue/fake-xrm-easy/issues/76
- Upgraded GitHub Actions to update Java major version to run SonarCloud analysis - https://github.com/DynamicsValue/fake-xrm-easy/issues/110
- Returns organisation and business unit in WhoAmIRequest to support Calendar rules - https://github.com/DynamicsValue/fake-xrm-easy/issues/23
- Update legacy CRM SDK 2011 dependency to use official MS package - https://github.com/DynamicsValue/fake-xrm-easy/issues/105

## [3.3.1]

### Changed

- Bump DataverseClient dependency to upgrade to net6 - https://github.com/DynamicsValue/fake-xrm-easy/issues/90

## [3.3.0]

### Changed

- Updated dependencies

## [3.2.0]

### Changed

- Replace references to PullRequestException by UnsupportedExceptionFactory
- Fix Sonar Quality Gate settings: DynamicsValue/fake-xrm-easy#28

## [3.1.2]

- Bump dataverse dependency to 1.0.1

## [3.1.1]

### Changed 

- Limit FakeItEasy package dependency to v6.x versions - DynamicsValue/fake-xrm-easy#37
- Updated build script to also include the major version in the Title property of the generated .nuspec file - DynamicsValue/fake-xrm-easy#41

## [3.1.0]

### Changed

- Update executor after fix DynamicsValue/fake-xrm-easy#16 was introduced.

## [3.0.2]

### Changed

- Bump Dataverse dependency to 0.6.1 from 0.5.10 to solve DynamicsValue/fake-xrm-easy#20
- Also replaced Microsoft.Dynamics.Sdk.Messages dependency, as it has also been deprecated by MSFT, to Microsoft.PowerPlatform.Dataverse.Client.Dynamics 0.6.1 DynamicsValue/fake-xrm-easy#20


## [3.0.1-rc1] - Initial release

