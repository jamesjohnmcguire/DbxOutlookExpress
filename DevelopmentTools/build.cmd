@ECHO OFF
SETLOCAL
SET "dbxBuildExitCode=0"

:: publish dispatches trusted publishing from committed main.
PUSHD "%~dp0..\DbxOutlookExpress"
IF ERRORLEVEL 1 EXIT /B 1

IF /I "%~1"=="publish" GOTO publish

dotnet build --configuration Debug -p:PlatformTarget=AnyCPU -p:Prefer32Bit=false
SET "dbxBuildExitCode=%ERRORLEVEL%"
GOTO end

:publish
WHERE gh >NUL 2>&1
SET "dbxBuildExitCode=%ERRORLEVEL%"
IF NOT "%dbxBuildExitCode%"=="0" SET "message=GitHub CLI is required. Install gh and run gh auth login."
IF NOT "%dbxBuildExitCode%"=="0" GOTO error

SET "dbxProjectVersion="
FOR /F "delims=" %%V IN ('dotnet msbuild -nologo -getProperty:Version') DO SET "dbxProjectVersion=%%V"

IF NOT DEFINED dbxProjectVersion SET "message=Unable to read the project version."
IF NOT DEFINED dbxProjectVersion GOTO error

ECHO Requesting publication of version %dbxProjectVersion% from main.
gh workflow run dotnet.yml --ref main -F publish=true -f version="%dbxProjectVersion%" --repo jamesjohnmcguire/DbxOutlookExpress

SET "dbxBuildExitCode=%ERRORLEVEL%"
IF NOT "%dbxBuildExitCode%"=="0" SET "message=Problem Running: gh workflow run dotnet.yml"
IF NOT "%dbxBuildExitCode%"=="0" GOTO error

ECHO Workflow requested. GitHub will build, test, and publish the package.
ECHO Use gh run watch from the DbxOutlookExpress repository to follow its progress.
GOTO end

:error
ECHO %message%
SET "dbxBuildExitCode=1"

:end
POPD
EXIT /B %dbxBuildExitCode%
