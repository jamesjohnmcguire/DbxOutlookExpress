REM %1 - Version (such as 1.5.1)
REM %2 - API key

CD %~dp0
CD ..\DbxOutlookExpressLibrary

msbuild -property:Configuration=Release -restore -target:rebuild;pack DigitalZenWorks.Email.DbxOutlookExpress.csproj

if "%~1"=="" GOTO error1
if "%~2"=="" GOTO error2

CD bin\Release

nuget push DigitalZenWorks.Email.DbxOutlookExpress.%1.nupkg %2 -Source https://api.nuget.org/v3/index.json

:error1
ECHO No version tag specified
GOTO end

:error2
ECHO No API key specified

:end
cd ..
