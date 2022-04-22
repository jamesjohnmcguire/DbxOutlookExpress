REM %1 - Version (such as 1.5.1)
REM %2 - API key

CD %~dp0
CD ..\DbxOutlookExpressLibrary

msbuild -property:Configuration=Release -restore DbxOutlookExpressLibrary.csproj
msbuild -property:Configuration=Release;OutputPath=Bin\Nuget DbxOutlookExpressLibrary.csproj
CD Bin\Nuget

dotnet nuget push DigitalZenWorks.Email.DbxOutlookExpress.%1.nupkg --api-key %2 --source https://api.nuget.org/v3/index.json
