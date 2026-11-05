# Development Notes

Run the following commands from the DbxOutlookExpress repository root.

## Local builds

Use Debug for a quick build and the NUnit tests:

```shell
dotnet build DbxOutlookExpress.slnx --configuration Debug
dotnet test DbxOutlookExpress.slnx --configuration Debug
```

The command script also builds the library in Debug configuration:

```shell
.\DevelopmentTools\build.cmd
```

For a Release build across every configured library target:

```shell
dotnet build DbxOutlookExpress.slnx --configuration Release
```

This also generates a NuGet package under `DbxOutlookExpress/bin/Release/`.

## Preparing a release

Update the library project file, then commit and push the changes to `main`. For
example:

```xml
<AssemblyVersion>1.0.0.0</AssemblyVersion>
<FileVersion>1.3.26.0</FileVersion>
<Version>1.3.26</Version>
```

Continue the third-segment build sequence, for example `1.3.25` to `1.3.26`.
Keep `AssemblyVersion` stable for compatible 1.x releases. Update `Version` and
`FileVersion` for each release. Plan an assembly version change for a breaking
major release.

## Version tags

Pushes to `main`, pull requests targeting `main`, and version tags trigger
builds and tests only. Creating or pushing a tag never publishes a package.
Use a matching version tag to mark the release source, then publish explicitly
when that version is ready for production.

For example, after committing the release changes on `main`:

```shell
git push origin main
git tag -a 1.3.26 -m "Release 1.3.26"
git push origin 1.3.26
```

## Publishing from the command script

Install GitHub CLI and sign in once with `gh auth login`. To request a
publication from the current remote `main`:

```shell
.\DevelopmentTools\build.cmd publish
```

The script reads `Version` from the local library project and supplies it as the
expected version when dispatching `.github/workflows/dotnet.yml` through GitHub
CLI. No version argument is required. The workflow tests its checkout of `main`,
checks the expected version, builds the Release package, and publishes it
through trusted publishing. Commit and push first: local uncommitted or unpushed
changes are not included.

The script returns a nonzero exit code if validation or dispatch fails. A
successful exit means the workflow was requested; use GitHub CLI to check
whether testing and publishing succeed:

```shell
gh run list --workflow dotnet.yml
gh run watch
```

This replaces the previous local `nuget push` operation. No NuGet API key is
required on the development PC.

## Manual workflow runs

To explicitly publish the version currently committed on `main`, run the
`.NET` workflow on `main` with **Publish the package from main after tests
pass** checked. Publishing is disabled by default.
Optionally enter the expected package version to guard against publishing a
different version. A manual run without the publishing option only tests.

## Trusted publishing policy

Publishing uses `NuGet/login@v1` as NuGet user `jamesjohnmcguire`. The NuGet
policy must identify repository owner `jamesjohnmcguire`, repository
`DbxOutlookExpress`, and workflow `dotnet.yml`. No GitHub environment is
configured.

The workflow uses Windows runners to build the .NET Framework 4.7.2 target and
.NET 10 for the Debug test suite.

Only the publishing job requests `id-token: write`. It exchanges the GitHub OIDC
token for a temporary NuGet API key immediately before pushing the package. No
permanent NuGet API key secret is needed in GitHub.

NuGet validation and indexing can delay the version appearing in search or the
README badge. Check **Manage packages** on NuGet.org for its status.
