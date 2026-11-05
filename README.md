# DbxOutlookExpress

[![NuGet version][version-badge]][nuget]
[![NuGet downloads][downloads-badge]][nuget]
[![Build and tests][build-badge]][workflow]
[![License: MIT][license-badge]][license]
[![GitHub issues][issues-badge]][issues]

A C# library for reading Outlook Express DBX files: folder information,
message metadata, and message content. It reads files directly and does not
require Outlook or Outlook Express to be running.

## Installation

Add the package to your application:

```shell
dotnet add package DigitalZenWorks.Email.DbxOutlookExpress
```

Or use Visual Studio's Package Manager Console:

```powershell
Install-Package DigitalZenWorks.Email.DbxOutlookExpress
```

## Quick start

A good starting point...  

DbxSet dbxSet = new (@"\path\to\your\dbx\files", Encoding.UTF8);  
DbxFolder dbxFolder = dbxSet.GetNextFolder();  
DbxMessage dbxMessage = dbxFolder.GetNextMessage();  

For a more in-depth example of a project using this library, please refer to the [DigitalZenWorks.Email.ToolKit Project](https://github.com/jamesjohnmcguire/DigitalZenWorks.Email.ToolKit)  
  
By default, when using GetNextFolder(), folders come out in an un-ordered list, mirroring the way they are stored in Folders.dbx file.  This means that, occasionally, some child folders may come up before their parents.  If you need the folders to  be in an ordered list, use  
dbxFoldersFile.SetTreeOrdered();  
  
The list will then come up in an 'pre-order' ordered list.  

Another Way:

Pass the directory containing the mail store's DBX files. `Folders.dbx`
provides the folder hierarchy; the other DBX files contain folder messages.
The encoding argument is a fallback when encoding detection is unsuccessful.

In a console application:

```csharp
using System;
using System.Text;
using DigitalZenWorks.Email.DbxOutlookExpress;

if (args.Length != 1)
{
	Console.Error.WriteLine("Supply the directory containing the DBX files.");
	return;
}

DbxSet dbxSet = new(args[0], Encoding.UTF8);
dbxSet.SetTreeOrdered();
DbxFolder folder = dbxSet.GetNextFolder();

while (folder != null)
{
	string folderName = folder.FolderName ?? folder.FolderFileName;
	Console.WriteLine(folderName);
	DbxMessage message = folder.GetNextMessage();

	while (message != null)
	{
		Console.WriteLine(message.Subject);
		message = folder.GetNextMessage();
	}

	folder = dbxSet.GetNextFolder();
}
```

For example:

```shell
dotnet run -- "C:\MailBackup\OutlookExpress"
```

`GetNextFolder()` and `GetNextMessage()` return null when iteration is
complete. A `DbxMessage` exposes metadata such as `Subject`, `Sender`, and
`ReceivedTime`, plus message bytes through `Message` and a stream through
`MessageStream`. Use `GetAsFile(path)` to write a message to a file.

## Folder ordering

The example calls `SetTreeOrdered()` before iteration to traverse the
`Folders.dbx` hierarchy in pre-order, with parents before their children.
Omit that call to use the order recorded in `Folders.dbx`, which can place
children before parents.

`DbxSet` also looks for DBX files missing from the folder index and appends
those orphaned folders after indexed folders. `DbxFolder.IsOrphan` identifies
these entries.

See the XML documentation in the [library source][source] for individual
API contracts. [Email.ToolKit][email-toolkit] is an example consumer of this
library for migration workflows.

## Target frameworks

The current Release configuration targets .NET Standard 2.0, .NET Framework
4.7.2, and .NET 5, 6, 7, 8, 9, and 10. Debug builds target .NET 10 only.
See the [package page][nuget] for a published version's frameworks and
its dependencies.

## Build and test

Install the .NET 10 SDK, then clone and build the repository:

```shell
git clone https://github.com/jamesjohnmcguire/DbxOutlookExpress.git
cd DbxOutlookExpress
dotnet build DbxOutlookExpress.slnx --configuration Debug
dotnet test DbxOutlookExpress.slnx --configuration Debug
```

See the [development notes][development-notes] for Release builds,
versioning, trusted publishing, and the `build.cmd` helper.

## Contributing

If you have found a bug or have a suggestion that would make this better, please fork this repository and create a pull request. You can also simply open an issue with the tag "bug" or "enhancement".

### Process:

1. Fork the Project
2. Create your Bug / Feature Branch (`git checkout -b feature/amazing-feature`)
3. Commit your Changes (`git commit -m 'Add some amazing feature'`)
4. Push to the Branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Coding style
Please match the current coding style.  Most notably:
1. One operation per line
2. Use complete English words in variable and method names
3. Attempt to declare variable and method names in a self-documenting manner
4. Add unit tests


## Acknowledgments

This project builds on earlier work deciphering the DBX format, notably Arne Schloh's original file-format research and Zvonko Tesic's updates.
See the [OE DBX format documentation][dbx-format].

## License

Distributed under the [MIT License][license].

## Contact

James John McGuire — jamesjohnmcguire@gmail.com

[Project repository][repository].

[version-badge]:
  https://img.shields.io/nuget/v/DigitalZenWorks.Email.DbxOutlookExpress.svg
[downloads-badge]:
  https://img.shields.io/nuget/dt/DigitalZenWorks.Email.DbxOutlookExpress.svg
[license-badge]:
  https://img.shields.io/badge/license-MIT-blue.svg
[build-badge]:
  https://img.shields.io/github/actions/workflow/status/jamesjohnmcguire/DbxOutlookExpress/dotnet.yml?branch=main
[workflow]:
  https://github.com/jamesjohnmcguire/DbxOutlookExpress/actions/workflows/dotnet.yml
[issues-badge]:
  https://img.shields.io/github/issues/jamesjohnmcguire/DbxOutlookExpress
[nuget]: https://www.nuget.org/packages/DigitalZenWorks.Email.DbxOutlookExpress
[license]:
  https://github.com/jamesjohnmcguire/DbxOutlookExpress/blob/main/LICENSE
[issues]: https://github.com/jamesjohnmcguire/DbxOutlookExpress/issues
[source]:
  https://github.com/jamesjohnmcguire/DbxOutlookExpress/tree/main/DbxOutlookExpress
[repository]: https://github.com/jamesjohnmcguire/DbxOutlookExpress
[development-notes]:
  https://github.com/jamesjohnmcguire/DbxOutlookExpress/blob/main/Support/DevelopmentNotes.md
[email-toolkit]:
  https://github.com/jamesjohnmcguire/DigitalZenWorks.Email.ToolKit
[dbx-format]: https://www.infobyte.hr/oedbx/
