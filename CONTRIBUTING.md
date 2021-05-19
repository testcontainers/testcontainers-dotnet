# Contributing

You are thinking about contributing to .NET Testcontainers? Awesome, it’s absolutely appreciated. When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change. Or pick an already existing issue.

## Guidelines

### Pull Requests

To build the project just run the provided Cake script, `dotnet cake --target=Build`.

1. Fork the .NET Testcontainers repository.
2. Create a branch to work with and use `feature/` or `bugfix/` as a prefix.
3. Do not forget the unit tests and keep the SonarCloud statistics in mind.
4. If you are finished rebase and create a pull request.
5. Cheers, :beers:.

### Commit Messages

.NET Testcontainers uses a consitent and structured vocabulary for commit messages with the following pattern:

```
[ISSUE] #LABEL 'specification'
{Comment}
```

#### Labels

- \#INIT: Initializes a repository or a new release
    - `assemblyName`: assembly name
    - `version`: version
- \#IMPLEMENT: Implement a new function
    - `assemblyName`: assembly name
    - `function`: class
- \#CHANGE: Change an existing function
    - `assemblyName`: assembly name
    - `function`: class
- \#EXTEND: Extend an existing function
    - `assemblyName`: assembly name
    - `function`: class
- \#BUGFIX: Bugfix
    - `assemblyName`: assembly name
- \#REVIEW: Quality control
    - `assemblyName`: assembly name
    - `refactor`: function
    - `analyze`: quality
    - `migrate`: function
    - `format`: source

#### Examples

```
[1] #INIT 'assemblyName: DotNet.Testcontainers; version: 1.0.0'

[2] #IMPLEMENT 'assemblyName: DotNet.Testcontainers; function: TestcontainersClient'
{Add Dockerfile support.}

[3] #CHANGE 'assemblyName: DotNet.Testcontainers; function: TestcontainersConfiguration'
{Change default wait strategy to WaitUntilContainerIsRunning.}

[4] #EXTEND 'assemblyName: DotNet.Testcontainers; function: TestcontainersConfiguration'
{Add new configuration property WaitStrategy.}
```
