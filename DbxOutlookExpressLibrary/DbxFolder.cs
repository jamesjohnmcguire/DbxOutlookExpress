﻿/////////////////////////////////////////////////////////////////////////////
// <copyright file="DbxFolder.cs" company="James John McGuire">
// Copyright © 2021 - 2022 James John McGuire. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DigitalZenWorks.Email.DbxOutlookExpress
{
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
		private IList<DbxFolder> childrenFolders = new List<DbxFolder>();
		private IList<uint> orderedIndexes = new List<uint>();

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="DbxFolder"/> class.
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

			string extension = Path.GetExtension(path);

			if (string.IsNullOrEmpty(extension))
			{
				foldersPath = path;

				// Assuming just a directory given.  Try getting Folders file.
				path = Path.Combine(path, folderFileName);
			}

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

			if (!string.IsNullOrWhiteSpace(FolderFileName))
			{
				path = Path.GetDirectoryName(path);

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
		public DbxFolder(
			byte[] fileBytes,
			uint address,
			string path,
			Encoding preferredEncoding)
			: this(fileBytes, path, preferredEncoding)
		{
			fileAddress = address;

			DbxFolderIndexedItem index = new (fileBytes, address);
			index.SetItemValues(this, address);
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
		/// Get the children list of this folder.
		/// </summary>
		/// <param name="folderIndexes">The list of folder indexes into the
		/// Folder.dbx file.</param>
		public void GetChildren(IList<uint> folderIndexes)
		{
			if (folderIndexes != null)
			{
				// Go backwards, since we removing items along the way.
				for (int index = folderIndexes.Count - 1; index >= 0; index--)
				{
					try
					{
						while (index >= folderIndexes.Count)
						{
							Log.Warn("Getting Children: Current index " +
								"greater then count - index: " + index);
							index--;
						}

						uint folderIndex = folderIndexes[index];

						DbxFolder folder = new (
							fileBytes,
							folderIndex,
							foldersPath,
							preferredEncoding);

						if (folder.FolderParentId == FolderId)
						{
							childrenFolders.Add(folder);

							folderIndexes.Remove(folderIndex);

							folder.GetChildren(folderIndexes);
						}
					}
					catch (DbxException exception)
					{
						Log.Error(exception);
					}
				}
			}
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
	}
}
