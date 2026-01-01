/////////////////////////////////////////////////////////////////////////////
// <copyright file="DbxFolder.cs" company="James John McGuire">
// Copyright © 2021 - 2026 James John McGuire. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.Email.DbxOutlookExpress
{
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using global::Common.Logging;

	/// <summary>
	/// Dbx folder index class.
	/// </summary>
	public class DbxFolder
	{
		private static readonly ILog Log = LogManager.GetLogger(
			System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private readonly uint fileAddress;
		private readonly byte[] fileBytes;
		private readonly string foldersPath;
		private readonly DbxMessagesFile messageFile;
		private readonly Encoding preferredEncoding;
		private readonly List<DbxFolder> childrenFolders = [];

		private bool isOrphan;

		/// <summary>
		/// Initializes a new instance of the <see cref="DbxFolder"/> class.
		/// </summary>
		/// <param name="id">The id of folder.</param>
		/// <param name="parentId">The parent id of the folder.</param>
		/// <param name="name">The name of the folder.</param>
		/// <param name="fileName">The file name of the folder.</param>
		public DbxFolder(uint id, uint parentId, string name, string fileName)
		{
			FolderId = id;
			FolderParentId = parentId;
			FolderName = name;
			FolderFileName = fileName;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbxFolder"/> class.
		/// </summary>
		/// <param name="path">The path of the dbx set.</param>
		/// <param name="folderFileName">The name of the messages
		/// file folder.</param>
		/// <param name="preferredEncoding">The preferred encoding to use as
		/// a fall back when the encoding can not be detected.</param>
		public DbxFolder(
			string path, string folderFileName, Encoding preferredEncoding)
		{
			FolderFileName = folderFileName;
			this.preferredEncoding = preferredEncoding;

			if (string.IsNullOrEmpty(path))
			{
				Log.Error("path is null or empty!");
			}
			else
			{
				string extension = Path.GetExtension(path);

				if (string.IsNullOrEmpty(extension))
				{
					foldersPath = path;

					// Assuming just a directory given.
					// Try getting Folders file.
					path = Path.Combine(path, folderFileName);
				}

				GetMessagesFile(path, ref messageFile);
			}
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="DbxFolder"/> class.
		/// </summary>
		/// <param name="path">The path of the dbx set.</param>
		/// <param name="folderId">The Id of the folder.</param>
		/// <param name="folderFileName">The name of the messages
		/// file folder.</param>
		/// <param name="preferredEncoding">The preferred encoding to use as
		/// a fall back when the encoding can not be detected.</param>
		public DbxFolder(
			string path,
			uint folderId,
			string folderFileName,
			Encoding preferredEncoding)
			: this(path, folderFileName, preferredEncoding)
		{
			FolderId = folderId;
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="DbxFolder"/> class.
		/// </summary>
		/// <param name="fileBytes">The bytes of the file.</param>
		/// <param name="path">The path of the dbx set.</param>
		/// <param name="preferredEncoding">The preferred encoding to use as
		/// a fall back when the encoding can not be detected.</param>
		public DbxFolder(
			byte[] fileBytes,
			string path,
			Encoding preferredEncoding)
		{
			this.fileBytes = fileBytes;
			foldersPath = path;
			this.preferredEncoding = preferredEncoding;
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="DbxFolder"/> class.
		/// </summary>
		/// <param name="fileBytes">The bytes of the file.</param>
		/// <param name="address">The address of the item with in
		/// the file.</param>
		/// <param name="path">The path of the dbx set.</param>
		/// <param name="preferredEncoding">The preferred encoding to use as
		/// a fall back when the encoding can not be detected.</param>
		/// <param name="loadMessagesFile">Indicates whether the messages file
		/// should be loaded.</param>
		public DbxFolder(
			byte[] fileBytes,
			uint address,
			string path,
			Encoding preferredEncoding,
			bool loadMessagesFile)
			: this(fileBytes, path, preferredEncoding)
		{
			fileAddress = address;

			DbxFolderIndexedItem index = new (fileBytes, address);
			index.SetItemValues(this, address);

			if (loadMessagesFile == true)
			{
				SetMessagesFile(ref messageFile);
			}
		}

		/// <summary>
		/// Gets or sets the folder file name.
		/// </summary>
		/// <value>The folder file name.</value>
		public string FolderFileName { get; set; }

		/// <summary>
		/// Gets or sets the folder id.
		/// </summary>
		/// <value>The folder id.</value>
		public uint FolderId { get; set; }

		/// <summary>
		/// Gets or sets the folder name.
		/// </summary>
		/// <value>The folder name.</value>
		public string FolderName { get; set; }

		/// <summary>
		/// Gets or sets the folder parent id.
		/// </summary>
		/// <value>The folder parent id.</value>
		public uint FolderParentId { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether whether the folder
		/// is an orphan.
		/// </summary>
		/// <value>A value indicating whether the folder is an orphan.</value>
		public bool IsOrphan
		{
			get { return isOrphan; }
			set { isOrphan = value; }
		}

		/// <summary>
		/// Get the children list of this folder.
		/// </summary>
		/// <param name="folderIndexes">The list of folder indexes into the
		/// Folder.dbx file.</param>
		/// <returns>A list of child folders.</returns>
		public IList<DbxFolder> GetChildren(IList<uint> folderIndexes)
		{
			if (folderIndexes != null)
			{
				// Go backwards, since we removing items along the way.
				for (int index = folderIndexes.Count - 1; index >= 0; index--)
				{
					try
					{
						if (index >= folderIndexes.Count)
						{
							Log.Warn("Getting Children: Current index " +
								"greater then count - index: " + index);
							index = folderIndexes.Count - 1;
						}

						if (index > -1)
						{
							uint folderIndex = folderIndexes[index];

							DbxFolder folder = new (
								fileBytes,
								folderIndex,
								foldersPath,
								preferredEncoding,
								false);

							if (folder.FolderParentId == FolderId)
							{
								childrenFolders.Add(folder);

								folderIndexes.Remove(folderIndex);

								folder.GetChildren(folderIndexes);
							}
						}
					}
					catch (DbxException exception)
					{
						Log.Error(exception);
					}
				}
			}

			return childrenFolders;
		}

		/// <summary>
		/// Get the children list of this folder.
		/// </summary>
		/// <param name="folders">A list of un-ordered folders.</param>
		/// <returns>A list of child folders.</returns>
		public IList<DbxFolder> GetChildren(IList<DbxFolder> folders)
		{
			if (folders != null)
			{
				// Go backwards, since we removing items along the way.
				for (int index = folders.Count - 1; index >= 0; index--)
				{
					try
					{
						if (index >= folders.Count)
						{
							index = folders.Count - 1;
						}

						DbxFolder folder = folders[index];

						if (folder.FolderParentId == FolderId)
						{
							childrenFolders.Add(folder);

							folders.Remove(folder);

							IList<DbxFolder> children =
								folder.GetChildren(folders);

							index -= children.Count;
						}
					}
					catch (DbxException exception)
					{
						Log.Error(exception);
					}
				}
			}

			return childrenFolders;
		}

		/// <summary>
		/// Gets the next message in enurmation.
		/// </summary>
		/// <returns>The next message.</returns>
		public DbxMessage GetNextMessage()
		{
			DbxMessage message = null;

			if (messageFile != null)
			{
				message = messageFile.GetNextMessage();
			}

			return message;
		}

		/// <summary>
		/// Set the list of ordered indexes.
		/// </summary>
		/// <param name="orderedIndexes">The current list of ordered
		/// indexes.</param>
		/// <returns>A list of ordered indexes including this folder's index
		/// and it's children's indexes.</returns>
		public IList<uint> SetOrderedIndexes(IList<uint> orderedIndexes)
		{
			if (orderedIndexes != null)
			{
				orderedIndexes.Add(fileAddress);

				foreach (DbxFolder childFolder in childrenFolders)
				{
					orderedIndexes =
						childFolder.SetOrderedIndexes(orderedIndexes);
				}
			}

			return orderedIndexes;
		}

		private void GetMessagesFile(
			string path, ref DbxMessagesFile messageFile)
		{
			bool exists = File.Exists(path);

			if (exists == true)
			{
				FileInfo fileInfo = new (path);
				FolderName =
					Path.GetFileNameWithoutExtension(fileInfo.Name);

				try
				{
					messageFile = new DbxMessagesFile(path, preferredEncoding);
				}
				catch (DbxException)
				{
					Log.Warn("Could not create object: " + FolderFileName);
					Log.Warn("Perhaps it is corrupted?");
				}
			}
		}

		private void SetMessagesFile(ref DbxMessagesFile messageFile)
		{
			if (!string.IsNullOrWhiteSpace(FolderFileName))
			{
				string path = Path.GetDirectoryName(foldersPath);

				string filePath = Path.Combine(path, FolderFileName);

				bool exists = File.Exists(filePath);

				if (exists == false)
				{
					Log.Warn(FolderFileName +
						" specified in Folders.dbx not present");
				}
				else
				{
					try
					{
						messageFile =
							new DbxMessagesFile(filePath, preferredEncoding);
					}
					catch (DbxException)
					{
						Log.Warn("Could not create object: " + FolderFileName);
						Log.Warn("Perhaps it is corrupted?");
					}
				}
			}
		}
	}
}
