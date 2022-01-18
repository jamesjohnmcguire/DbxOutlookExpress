/////////////////////////////////////////////////////////////////////////////
// <copyright file="DbxSet.cs" company="James John McGuire">
// Copyright © 2021 - 2022 James John McGuire. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
		private readonly Encoding preferredEncoding;

		private uint maximumFolderId;
		private int orphanFileIndex = -1;
		private IList<string> orphanFiles;

		/// <summary>
		/// Initializes a new instance of the <see cref="DbxSet"/> class.
		/// </summary>
		/// <param name="path">The path of the dbx set.</param>
		/// <param name="preferredEncoding">The preferred encoding to use as
		/// a fall back when the encoding can not be detected.</param>
		public DbxSet(string path, Encoding preferredEncoding)
		{
			this.path = path;
			this.preferredEncoding = preferredEncoding;

			string extension = Path.GetExtension(path);

			if (string.IsNullOrEmpty(extension))
			{
				// Assuming just a directory given.  Try getting Folders file.
				path = Path.Combine(path, "Folders.dbx");
			}

			bool exists = File.Exists(path);

			if (exists == false)
			{
				Log.Warn(path + " not present");
			}
			else
			{
				try
				{
					FileInfo fileInfo = new (path);

					if (fileInfo.Name.Equals(
						"Folders.dbx", StringComparison.Ordinal))
					{
						foldersFile = new (path, preferredEncoding);
					}
					else
					{
						Log.Warn("DbxSet File is not Folders.dbx");
					}
				}
				catch (DbxException)
				{
					Log.Warn("Could not create Folders.dbx object.");
					Log.Warn("Perhaps it is corrupted?");
				}
			}
		}

		/// <summary>
		/// Get the next folder in the tree list.
		/// </summary>
		/// <returns>The next folder in the tree list.</returns>
		public DbxFolder GetNextFolder()
		{
			DbxFolder folder = null;

			if (foldersFile != null)
			{
				folder = foldersFile.GetNextFolder();
			}

			if (folder == null)
			{
				Log.Info("Folders.dbx exhausted");

				if (orphanFileIndex == -1)
				{
					Log.Info("Checking for orphans");
					orphanFiles = AppendOrphanedFiles();
					orphanFileIndex = 0;
				}

				if (orphanFiles.Count > orphanFileIndex)
				{
					Log.Info("Getting next orphan file");
					string fileName = orphanFiles[orphanFileIndex];

					// Best if each folder has it's own unique id, even if it
					// is artifically constructed.
					maximumFolderId++;
					folder = new (
						path, maximumFolderId, fileName, preferredEncoding);

					orphanFileIndex++;
				}
			}

			if (folder != null)
			{
				if (folder.FolderId > maximumFolderId)
				{
					maximumFolderId = folder.FolderId;
				}
			}

			return folder;
		}

		/// <summary>
		/// List method.
		/// </summary>
		public void List()
		{
			Log.Info("Id\tParentId\tName\t\tFile Name");

			if (foldersFile != null)
			{
				foldersFile.List();
			}

			AppendOrphanedFiles();
		}

		/// <summary>
		/// Migrate method.
		/// </summary>
		public void Migrate()
		{
			if (foldersFile != null)
			{
				foldersFile.MigrateFolders();
			}
		}

		/// <summary>
		/// Set tree in an ordered list.
		/// </summary>
		/// <returns>A list of child folders.</returns>
		public IList<DbxFolder> SetTreeOrdered()
		{
			IList<DbxFolder> orderedList = new List<DbxFolder>();

			if (foldersFile != null)
			{
				orderedList = foldersFile.SetTreeOrdered();
			}

			return orderedList;
		}

		private IList<string> AppendOrphanedFiles()
		{
			string[] ignoreFiles =
			{
			"CLEANUP.LOG", "FOLDERS.AVX", "FOLDERS.DBX", "OFFLINE.DBX",
			"POP3UIDL.DBX", "SEARCH FOLDER.DBX"
			};

			IList<string> orphanFolderFiles = new List<string>();

			bool exists = Directory.Exists(path);

			if (exists == true)
			{
				string[] files = Directory.GetFiles(path, "*.dbx");

				foreach (string file in files)
				{
					FileInfo fileInfo = new (file);

					string fileName = fileInfo.Name.ToUpperInvariant();

					if (foldersFile != null)
					{
						if (!foldersFile.FolderFiles.Contains(fileName) &&
							!ignoreFiles.Contains(fileName))
						{
							Log.Warn("Orphaned file found - " +
								"Not in Folders.dbx: " + fileInfo.Name);

							orphanFolderFiles.Add(fileInfo.Name);
						}
					}
					else
					{
						// If no Folders.dbx, then all the files are orhans.
						orphanFolderFiles.Add(fileInfo.Name);
					}
				}
			}

			return orphanFolderFiles;
		}
	}
}
