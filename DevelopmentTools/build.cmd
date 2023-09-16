REM %1 - Version (such as 1.5.1)
REM %2 - API key

CD %~dp0
CD ..\DbxOutlookExpressLibrary

msbuild -property:Configuration=Release;OutputPath=Bin\Nuget -restore DbxOutlookExpressLibrary.csproj

if "%~2"=="" GOTO error1
if "%~3"=="" GOTO error2

CD Bin\Nuget

dotnet nuget push DigitalZenWorks.Email.DbxOutlookExpress.%1.nupkg --api-key %2 --source https://api.nuget.org/v3/index.json


:error1
ECHO No version tag specified
GOTO end

:error2
ECHO No API key specified

:end
cd ..
