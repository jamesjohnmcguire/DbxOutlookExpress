/////////////////////////////////////////////////////////////////////////////
// <copyright file="DbxFolderIndexedItem.cs" company="James John McGuire">
// Copyright © 2021 James John McGuire. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using System;

namespace DigitalZenWorks.Email.DbxOutlookExpress
{
	/// <summary>
	/// Represents a dbx folder indexed item.
	/// </summary>
	public class DbxFolderIndexedItem : DbxIndexedItem
	{
		/// <summary>
		/// The file name index of the folder.
		/// </summary>
		public const int FileName = 0x03;

		/// <summary>
		/// The flags index of the folder.
		/// </summary>
		public const int Flags = 0x06;

		/// <summary>
		/// The id index of the folder.
		/// </summary>
		public const int Id = 0x00;

		/// <summary>
		/// The subfolder index of the folder.
		/// </summary>
		public const int Index = 0x09;

		/// <summary>
		/// The name index of the folder.
		/// </summary>
		public const int Name = 0x02;

		/// <summary>
		/// The parent id index of the folder.
		/// </summary>
		public const int ParentId = 0x01;

		private readonly DbxFolder folder;

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="DbxFolderIndexedItem"/> class.
		/// </summary>
		/// <param name="fileBytes">The bytes of the file.</param>
		public DbxFolderIndexedItem(byte[] fileBytes)
			: base(fileBytes)
		{
			folder = new DbxFolder();
		}

		/// <summary>
		/// Gets the dbx folder index.
		/// </summary>
		/// <value>The dbx folder index.</value>
		public DbxFolder FolderIndex { get { return folder; } }

		/// <summary>
		/// Reads the indexed item and saves the values.
		/// </summary>
		/// <param name="address">The address of the item with in
		/// the file.</param>
		public override void ReadIndex(uint address)
		{
			base.ReadIndex(address);

			folder.FolderId = this.GetValue(Id);
			folder.FolderParentId = this.GetValue(ParentId);
			folder.FolderName = this.GetString(Name);
			folder.FolderFileName = this.GetString(FileName);
		}
	}
}
