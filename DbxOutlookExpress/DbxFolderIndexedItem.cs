/////////////////////////////////////////////////////////////////////////////
// <copyright file="DbxFolderIndexedItem.cs" company="James John McGuire">
// Copyright © 2021 - 2025 James John McGuire. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.Email.DbxOutlookExpress
{
	/// <summary>
	/// Represents a dbx folder indexed item.
	/// </summary>
	/// <remarks>
	/// Initializes a new instance of the
	/// <see cref="DbxFolderIndexedItem"/> class.
	/// </remarks>
	/// <param name="fileBytes">The bytes of the file.</param>
	/// <param name="address">The address of the item with in
	/// the file.</param>
	public class DbxFolderIndexedItem(byte[] fileBytes, uint address)
		: DbxIndexedItem(fileBytes, address)
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

		/// <summary>
		/// Sets the indexed items and saves the values.
		/// </summary>
		/// <param name="folder">The dbx folder to set.</param>
		/// <param name="address">The address of the item with in
		/// the file.</param>
		public void SetItemValues(DbxFolder folder, uint address)
		{
			if (folder != null)
			{
				folder.FolderId = this.GetValue(Id);
				folder.FolderParentId = this.GetValue(ParentId);
				folder.FolderName = this.GetString(Name);
				folder.FolderFileName = this.GetString(FileName);
			}
		}
	}
}
