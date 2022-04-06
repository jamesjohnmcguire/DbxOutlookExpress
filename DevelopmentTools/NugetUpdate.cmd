REM %1 - API key
REM %2 - Version (such as 1.0.0.5)

CD %~dp0
CD ..\DbxOutlookExpressLibrary

CALL msbuild /p:Configuration=Release DbxOutlookExpressLibrary.csproj
CALL nuget pack DbxOutlookExpress.nuspec

CALL dotnet nuget push DigitalZenWorks.Email.DbxOutlookExpress.%2.nupkg --api-key %1 --source https://api.nuget.org/v3/index.json
