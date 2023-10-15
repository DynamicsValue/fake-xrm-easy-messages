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

