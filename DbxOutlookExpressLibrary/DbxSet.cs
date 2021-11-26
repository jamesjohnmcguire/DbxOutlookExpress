/////////////////////////////////////////////////////////////////////////////
// <copyright file="DbxSet.cs" company="James John McGuire">
// Copyright © 2021 James John McGuire. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using Common.Logging;
using System;
using System.Collections.Generic;
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

		private int orphanFileIndex = -1;
		private IList<string> orphanFiles;

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
			}
		}

		/// <summary>
		/// Get the next folder in the tree list.
		/// </summary>
		/// <returns>The next folder in the tree list.</returns>
		public DbxFolder GetNextFolder()
		{
			DbxFolder folder = foldersFile.GetNextFolder();

			if (folder == null)
			{
				if (orphanFileIndex == -1)
				{
					orphanFiles = AppendOrphanedFiles();
					orphanFileIndex = 0;
				}

				if (orphanFiles.Count > orphanFileIndex)
				{
					folder = new DbxFolder(path, orphanFiles[orphanFileIndex]);

					string filePath =
						Path.Combine(path, folder.FolderFileName);
					FileInfo fileInfo = new (filePath);

					folder.FolderName =
						Path.GetFileNameWithoutExtension(fileInfo.Name);

					orphanFileIndex++;
				}
			}

			return folder;
		}

		/// <summary>
		/// List method.
		/// </summary>
		public void List()
		{
			foldersFile.List();

			IList<string> folderFiles = AppendOrphanedFiles();
		}

		/// <summary>
		/// Migrate method.
		/// </summary>
		public void Migrate()
		{
			foldersFile.MigrateFolders();
		}

		private IList<string> AppendOrphanedFiles()
		{
			string[] ignoreFiles =
			{
			"CLEANUP.LOG", "FOLDERS.AVX", "FOLDERS.DBX", "OFFLINE.DBX",
			"POP3UIDL.DBX"
			};

			IList<string> folderFiles = foldersFile.FoldersFile;

			string[] files = Directory.GetFiles(path);

			foreach (string file in files)
			{
				FileInfo fileInfo = new (file);

				string fileName = fileInfo.Name.ToUpperInvariant();

				if (!foldersFile.FoldersFile.Contains(fileName) &&
					!ignoreFiles.Contains(fileName))
				{
					Log.Warn("Orphaned file found: " + fileInfo.Name);

					folderFiles.Add(fileInfo.Name);
				}
			}

			return folderFiles;
		}
	}
}
