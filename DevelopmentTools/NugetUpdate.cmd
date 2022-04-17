REM %1 - Version (such as 1.5.1)
REM %2 - API key

CD %~dp0
CD ..\DbxOutlookExpressLibrary

CALL msbuild /p:Configuration=Release DbxOutlookExpressLibrary.csproj
dotnet pack --output Bin
CD Bin

dotnet nuget push DigitalZenWorks.Email.DbxOutlookExpress.%1.nupkg --api-key %2 --source https://api.nuget.org/v3/index.json
