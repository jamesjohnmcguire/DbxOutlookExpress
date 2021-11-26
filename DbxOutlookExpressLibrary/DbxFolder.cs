/////////////////////////////////////////////////////////////////////////////
// <copyright file="DbxFolder.cs" company="James John McGuire">
// Copyright © 2021 James John McGuire. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using System;

namespace DigitalZenWorks.Email.DbxOutlookExpress
{
	/// <summary>
	/// Dbx folder index class.
	/// </summary>
	public class DbxFolder
	{
		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="DbxFolder"/> class.
		/// </summary>
		public DbxFolder()
		{
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="DbxFolder"/> class.
		/// </summary>
		/// <param name="fileBytes">The bytes of the file.</param>
		/// <param name="address">The address of the item with in
		/// the file.</param>
		public DbxFolder(byte[] fileBytes, uint address)
		{
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
	}
}
