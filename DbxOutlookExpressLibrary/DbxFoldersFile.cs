/////////////////////////////////////////////////////////////////////////////
// <copyright file="DbxFoldersFile.cs" company="James John McGuire">
// Copyright © 2021 James John McGuire. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using Common.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace DigitalZenWorks.Email.DbxOutlookExpress
{
	/// <summary>
	/// Dbx folders files class.
	/// </summary>
	public class DbxFoldersFile : DbxFile
	{
		private static readonly ILog Log = LogManager.GetLogger(
			System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private readonly IList<string> folderFiles;

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="DbxFoldersFile"/> class.
		/// </summary>
		/// <param name="filePath">The path of the dbx file.</param>
		public DbxFoldersFile(string filePath)
			: base(filePath)
		{
			folderFiles = new List<string>();

			if (Header.FileType != DbxFileType.FolderFile)
			{
				Log.Error("Folders.dbx not actually folders file");

				// Attempt to process the individual files.
			}
			else
			{
				ReadTree();
			}
		}

		/// <summary>
		/// Gets the list of folder files.
		/// </summary>
		/// <value>The list of folder files.</value>
		public IList<string> FoldersFile { get { return folderFiles; } }

		/// <summary>
		/// Get the next folder in the tree list.
		/// </summary>
		/// <returns>The next folder in the tree list.</returns>
		public DbxFolder GetNextFolder()
		{
			DbxFolder folder = null;

			if (CurrentIndex < Tree.FolderInformationIndexes.Count)
			{
				byte[] fileBytes = GetFileBytes();

				DbxFolderIndexedItem item = new (fileBytes, CurrentIndex);

				folder = item.FolderIndex;

				// Prep for next call.
				CurrentIndex++;
			}

			return folder;
		}

		/// <summary>
		/// List folders method.
		/// </summary>
		public void List()
		{
			if (Tree != null)
			{
				byte[] fileBytes = GetFileBytes();

				foreach (uint index in Tree.FolderInformationIndexes)
				{
					DbxFolderIndexedItem item = new (fileBytes, index);

					DbxFolder folderIndex = item.FolderIndex;

					string message = string.Format(
						CultureInfo.InvariantCulture,
						"item value[{0}] is {1}",
						DbxFolderIndexedItem.Id,
						folderIndex.FolderId);
					Log.Info(message);

					message = string.Format(
						CultureInfo.InvariantCulture,
						"item value[{0}] is {1}",
						DbxFolderIndexedItem.ParentId,
						folderIndex.FolderParentId);
					Log.Info(message);

					message = string.Format(
						CultureInfo.InvariantCulture,
						"item value[{0}] is {1}",
						DbxFolderIndexedItem.Name,
						folderIndex.FolderName);
					Log.Info(message);

					string folderFileName = string.Empty;

					if (!string.IsNullOrWhiteSpace(folderIndex.FolderFileName))
					{
						folderFileName = folderIndex.FolderFileName.Trim();
					}

					message = string.Format(
						CultureInfo.InvariantCulture,
						"item value[{0}] is {1}",
						DbxFolderIndexedItem.FileName,
						folderFileName);
					Log.Info(message);

					// Eventually these will be compared with the existing
					// files by List.Compare, so need to mitigate case
					// sensitivity.
					folderFileName = folderFileName.ToUpperInvariant();
					folderFiles.Add(folderFileName);
				}
			}
		}

		/// <summary>
		/// Migrate folder method.
		/// </summary>
		/// <param name="folderName">The path of the dbx folder file.</param>
		public void MigrateFolder(string folderName)
		{
			if (!string.IsNullOrWhiteSpace(folderName))
			{
				string foldersPath = Path.GetDirectoryName(FolderPath);
				string filePath = Path.Combine(foldersPath, folderName);

				bool exists = File.Exists(filePath);

				if (exists == false)
				{
					Log.Warn(
						folderName + " specified in Folders.dbx not present");
				}
				else
				{
					DbxMessagesFile messagesFile = new (filePath);

					// messagesFile.MigrateMessages();
					messagesFile.List();
				}
			}
		}

		/// <summary>
		/// Migrate folders method.
		/// </summary>
		public void MigrateFolders()
		{
			if (Tree != null)
			{
				byte[] fileBytes = GetFileBytes();

				foreach (uint index in Tree.FolderInformationIndexes)
				{
					DbxFolderIndexedItem item = new (fileBytes, index);

					DbxFolder folderIndex = item.FolderIndex;

					string message = string.Format(
						CultureInfo.InvariantCulture,
						"Migrating folder {0}",
						folderIndex.FolderName);
					Log.Info(message);

					MigrateFolder(folderIndex.FolderFileName);
				}
			}
		}
	}
}
