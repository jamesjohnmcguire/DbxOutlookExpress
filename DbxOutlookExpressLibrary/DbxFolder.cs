/////////////////////////////////////////////////////////////////////////////
// <copyright file="DbxFolder.cs" company="James John McGuire">
// Copyright © 2021 James John McGuire. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;

namespace DigitalZenWorks.Email.DbxOutlookExpress
{
	/// <summary>
	/// Dbx folder index class.
	/// </summary>
	public class DbxFolder
	{
		private readonly DbxMessagesFile messageFile;

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="DbxFolder"/> class.
		/// </summary>
		/// <param name="path">The path of the dbx set.</param>
		/// <param name="folderFileName">The name of the messages
		/// file folder.</param>
		public DbxFolder(string path, string folderFileName)
		{
			string filePath = Path.Combine(path, folderFileName);

			messageFile = new DbxMessagesFile(filePath);
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="DbxFolder"/> class.
		/// </summary>
		/// <param name="fileBytes">The bytes of the file.</param>
		/// <param name="address">The address of the item with in
		/// the file.</param>
		/// <param name="path">The path of the dbx set.</param>
		public DbxFolder(byte[] fileBytes, uint address, string path)
		{
			DbxFolderIndexedItem index = new (fileBytes, address);
			index.SetItemValues(this, address);

			string filePath = Path.Combine(path, FolderFileName);

			messageFile = new DbxMessagesFile(filePath);
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
		/// Gets the next message in enurmation.
		/// </summary>
		/// <returns>The next message.</returns>
		public DbxMessage GetNextMessage()
		{
			DbxMessage message = messageFile.GetNextMessage();

			return message;
		}
	}
}
