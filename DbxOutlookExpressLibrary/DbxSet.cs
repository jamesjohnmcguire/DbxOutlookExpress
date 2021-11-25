/////////////////////////////////////////////////////////////////////////////
// <copyright file="DbxSet.cs" company="James John McGuire">
// Copyright © 2021 James John McGuire. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using Common.Logging;
using System;
using System.IO;
using System.Linq;

[assembly: CLSCompliant(false)]

namespace DigitalZenWorks.Email.DbxOutlookExpress
{
	/// <summary>
	/// Dbx set class.
	/// </summary>
	public class DbxSet
	{
		private static readonly ILog Log = LogManager.GetLogger(
			System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private readonly DbxFoldersFile foldersFile;
		private readonly string path;

		/// <summary>
		/// Initializes a new instance of the <see cref="DbxSet"/> class.
		/// </summary>
		/// <param name="path">The path of the dbx set.</param>
		public DbxSet(string path)
		{
			this.path = path;

			string extension = Path.GetExtension(path);

			if (string.IsNullOrEmpty(extension))
			{
				path = Path.Combine(path, "Folders.dbx");
			}

			bool exists = File.Exists(path);

			if (exists == false)
			{
				Log.Error(path + " not present");

				// Attempt to process the individual files.
			}
			else
			{
				foldersFile = new (path);

				if (foldersFile.Header.FileType != DbxFileType.FolderFile)
				{
					Log.Error("Folders.dbx not actually folders file");

					// Attempt to process the individual files.
				}
				else
				{
					foldersFile.ReadTree();
				}
			}
		}

		/// <summary>
		/// List method.
		/// </summary>
		public void List()
		{
			string[] ignoreFiles =
			{
				"CLEANUP.LOG", "FOLDERS.AVX", "FOLDERS.DBX", "OFFLINE.DBX",
				"POP3UIDL.DBX"
			};

			foldersFile.List();

			string[] files = Directory.GetFiles(path);

			foreach (string file in files)
			{
				FileInfo fileInfo = new (file);

				string fileName = fileInfo.Name.ToUpperInvariant();

				if (!foldersFile.FoldersFile.Contains(fileName) &&
					!ignoreFiles.Contains(fileName))
				{
					Log.Warn("Orphaned file found: " + fileInfo.Name);
				}
			}
		}

		/// <summary>
		/// Migrate method.
		/// </summary>
		public void Migrate()
		{
			foldersFile.MigrateFolders();
		}
	}
}
