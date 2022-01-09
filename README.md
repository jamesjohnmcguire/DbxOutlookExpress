# DbxOutlookExpress

This is a C# library for interacting with Outlook Express dbx files.  The dbx files are where Outlook Express stores the folders and email messages data.

## Getting Started

### Prerequisites

This project includes the [UTF-unknown project](https://github.com/CharsetDetector/UTF-unknown) as a submodule.  So, be sure to include submodules when retreiving the repository contents. 

### Installation
#### Git
git clone --recurse-submodules https://github.com/jamesjohnmcguire/DbxOutlookExpress

#### Nuget
PM> Install-Package DigitalZenWorks.Email.DbxOutlookExpress

### Usage

A good starting point...  

DbxSet dbxSet = new (@"\path\to\your\dbx\files", Encoding.UTF8);  
DbxFolder dbxFolder = dbxSet.GetNextFolder();  
DbxMessage dbxMessage = dbxFolder.GetNextMessage();  

For a more in-depth example of a project using this library, please refer to the [DigitalZenWorks.Email.ToolKit Project](https://github.com/jamesjohnmcguire/.Email.ToolKit)  
  
By default, when using GetNextFolder(), folders come out in an un-ordered list, mirroring the way they are stored in Folders.dbx file.  This means that, occasionally, some child folders may come up before their parents.  If you need the folders to  be in an ordered list, use  
dbxFoldersFile.SetTreeOrdered();  
  
The list will then come up in an 'pre-order' ordered list.  

## Contributing

Any contributions you make are **greatly appreciated**.  If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".

Don't forget to give the project a star! Thanks again!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## Acknowledgments

This project is heavily indebted to previous efforts of others to decipher the DBX file format.  Most notably, the original file format work by Arne Schloh and updated by Zvonko Tesic. [Details Here](https://www.infobyte.hr/oedbx/)

## License

Distributed under the MIT License. See `LICENSE` for more information.

## Contact

James John McGuire - [@jamesmc](https://twitter.com/jamesmc) - jamesjohnmcguire@gmail.com

Project Link: [https://github.com/jamesjohnmcguire/DbxOutlookExpress](https://github.com/jamesjohnmcguire/DbxOutlookExpress)
