/////////////////////////////////////////////////////////////////////////////
// <copyright file="DbxFoldersFile.cs" company="James John McGuire">
// Copyright © 2021 - 2024 James John McGuire. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using Common.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

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
		private IList<uint> orderedIndexes = new List<uint>();

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="DbxFoldersFile"/> class.
		/// </summary>
		/// <param name="filePath">The path of the dbx file.</param>
		/// <param name="preferredEncoding">The preferred encoding to use as
		/// a fall back when the encoding can not be detected.</param>
		public DbxFoldersFile(string filePath, Encoding preferredEncoding)
			: base(filePath)
		{
			folderFiles = new List<string>();

			PreferredEncoding = preferredEncoding;

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
		public IList<string> FolderFiles { get { return folderFiles; } }

		/// <summary>
		/// Get the next folder in the tree list.
		/// </summary>
		/// <returns>The next folder in the tree list.</returns>
		public DbxFolder GetNextFolder()
		{
			DbxFolder folder = null;

			IList<uint> folderIndexes = GetActiveIndexes();

			if (CurrentIndex < folderIndexes.Count)
			{
				byte[] fileBytes = GetFileBytes();

				uint address = folderIndexes[CurrentIndex];

				// Special case for root folder
				if (address == 0 && CurrentIndex == 0)
				{
					CurrentIndex++;
					address = folderIndexes[CurrentIndex];
				}

				folder = new (
					fileBytes,
					address,
					FolderPath,
					PreferredEncoding,
					true);

				if (!string.IsNullOrWhiteSpace(folder.FolderFileName))
				{
					string folderFileName = folder.FolderFileName.Trim();

					// Eventually these will be compared with the existing
					// files by List.Compare, so need to mitigate case
					// sensitivity.
					folderFileName = folderFileName.ToUpperInvariant();
					folderFiles.Add(folderFileName);
				}

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

				IList<uint> folderIndexes = GetActiveIndexes();

				foreach (uint index in folderIndexes)
				{
					if (index == 0)
					{
						Log.Warn("List: index is 0");
						continue;
					}

					DbxFolder folder = new (
						fileBytes,
						index,
						FolderPath,
						PreferredEncoding,
						false);

					string folderFileName = string.Empty;

					if (!string.IsNullOrWhiteSpace(folder.FolderFileName))
					{
						folderFileName = folder.FolderFileName.Trim();
					}

					string message = string.Format(
						CultureInfo.InvariantCulture,
						"{0}\t{1}\t\t{2} '{3}'",
						folder.FolderId,
						folder.FolderParentId,
						folder.FolderName,
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
					try
					{
						DbxMessagesFile messagesFile =
							new (filePath, PreferredEncoding);

						// messagesFile.MigrateMessages();
						messagesFile.List();
					}
					catch (DbxException)
					{
						Log.Warn("Could not create object: " + folderName);
						Log.Warn("Perhaps it is corrupted?");
					}
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
					DbxFolder folder = new (
						fileBytes,
						index,
						FolderPath,
						PreferredEncoding,
						true);

					string message = string.Format(
						CultureInfo.InvariantCulture,
						"Migrating folder {0}",
						folder.FolderName);
					Log.Info(message);

					MigrateFolder(folder.FolderFileName);
				}
			}
		}

		/// <summary>
		/// Set tree in an ordered list.
		/// </summary>
		/// <returns>A list of child folders.</returns>
		public IList<DbxFolder> SetTreeOrdered()
		{
			byte[] fileBytes = GetFileBytes();

			// Set root folder
			DbxFolder folder =
				new (fileBytes, FolderPath, PreferredEncoding);

			IList<DbxFolder> childrenFolders =
				folder.GetChildren(Tree.FolderInformationIndexes);

			orderedIndexes.Clear();
			orderedIndexes = folder.SetOrderedIndexes(orderedIndexes);

			return childrenFolders;
		}

		private IList<uint> GetActiveIndexes()
		{
			IList<uint> folderIndexes;

			if (orderedIndexes.Count > 0)
			{
				folderIndexes = orderedIndexes;
			}
			else
			{
				folderIndexes = Tree.FolderInformationIndexes;
			}

			return folderIndexes;
		}
	}
}
