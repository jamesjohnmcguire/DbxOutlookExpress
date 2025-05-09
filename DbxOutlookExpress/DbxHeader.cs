﻿/////////////////////////////////////////////////////////////////////////////
// <copyright file="DbxHeader.cs" company="James John McGuire">
// Copyright © 2021 - 2025 James John McGuire. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.Email.DbxOutlookExpress
{
	using System;
	using System.Globalization;
	using global::Common.Logging;

	/// <summary>
	/// Dbx header.
	/// </summary>
	public class DbxHeader
	{
		private const int FileInfoLengthIndex = 7;
		private const int FolderCountIndex = 0x31;
		private const int MainTreeRootNodeIndex = 0x3B;
		private const int MessageTreeRootNodeIndex = 0x39;

		private static readonly ILog Log = LogManager.GetLogger(
			System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private readonly uint fileInfoLength;
		private readonly uint folderCount;
		private readonly DbxFileType fileType;
		private readonly uint[] headerArray;
		private readonly uint mainTreeAddress;

		/// <summary>
		/// Initializes a new instance of the <see cref="DbxHeader"/> class.
		/// </summary>
		/// <param name="headerBytes">An array of bytes representing the
		/// file header.</param>
		public DbxHeader(byte[] headerBytes)
		{
			if (headerBytes != null)
			{
				fileType = GetFileType(headerBytes);

				CheckInitialBytes(headerBytes);

				// It will be easier to work with integers as opposed to bytes.
				int size = headerBytes.Length / sizeof(uint);
				headerArray = new uint[size];
				Buffer.BlockCopy(
					headerBytes, 0, headerArray, 0, headerBytes.Length);

				fileInfoLength = headerArray[FileInfoLengthIndex];

				DeletedItems = headerArray[0x12];

				if (fileType == DbxFileType.FolderFile)
				{
					folderCount = headerArray[FolderCountIndex];
					mainTreeAddress = headerArray[MainTreeRootNodeIndex];
				}
				else if (fileType == DbxFileType.MessageFile)
				{
					folderCount = headerArray[FolderCountIndex];
					mainTreeAddress = headerArray[MessageTreeRootNodeIndex];
				}
			}
		}

		/// <summary>
		/// Gets or sets the file type.
		/// </summary>
		/// <value>The file type.</value>
		public uint DeletedItems { get; set; }

		/// <summary>
		/// Gets file type.
		/// </summary>
		/// <value>The file type.</value>
		public DbxFileType FileType
		{
			get { return fileType; }
		}

		/// <summary>
		/// Gets the folder count.
		/// </summary>
		/// <value>The folder count.</value>
		public uint FolderCount
		{
			get { return folderCount; }
		}

		/// <summary>
		/// Gets the main tree address.
		/// </summary>
		/// <value>The main tree address.</value>
		public uint MainTreeAddress
		{
			get { return mainTreeAddress; }
		}

		private static void CheckInitialBytes(byte[] headerBytes)
		{
			byte[] checkBytes =
			[
				0xCF, 0xAD, 0x12, 0xFE, 0xC5, 0xFD, 0x74, 0x6F, 0x66, 0xE3,
				0xD1, 0x11, 0x9A, 0x4E, 0x00, 0xC0, 0x4F, 0xA3, 0x09, 0xD4,
				0x05, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00
			];

			for (int index = 0; index < checkBytes.Length; index++)
			{
				if (index == 4)
				{
					continue;
				}

				ConfirmByte(headerBytes, index, checkBytes[index]);
			}
		}

		private static bool ConfirmByte(
			byte[] bytes, int index, byte checkValue)
		{
			bool confirm = false;

			byte byteToCheck = bytes[index];

			if (byteToCheck == checkValue)
			{
				confirm = true;
			}
			else
			{
				Log.Warn("bytes not matching at" +
					index.ToString(CultureInfo.InvariantCulture));
			}

			return confirm;
		}

		private static DbxFileType GetFileType(byte[] bytes)
		{
			DbxFileType fileType = DbxFileType.Unknown;
			string message;
			byte byteToCheck = bytes[4];

			switch (byteToCheck)
			{
				case 0xC5:
					fileType = DbxFileType.MessageFile;
					break;
				case 0xC6:
					fileType = DbxFileType.FolderFile;
					break;
				case 0xC7:
					fileType = DbxFileType.Pop3uidl;
					break;
				case 0x30:
					if (bytes[5] == 0x9D && bytes[6] == 0xFE &&
						bytes[7] == 0x26)
					{
						fileType = DbxFileType.OffLine;
					}
					else
					{
						message = string.Format(
							CultureInfo.InvariantCulture,
							"File type unknown {0} {1} {2} {3}",
							0x30,
							bytes[5],
							bytes[6],
							bytes[7]);

						Log.Warn(message);
					}

					break;
				default:
					message = string.Format(
						CultureInfo.InvariantCulture,
						"File type unknown {0} {1} {2} {3}",
						bytes[4],
						bytes[5],
						bytes[6],
						bytes[7]);

					Log.Warn(message);
					break;
			}

			return fileType;
		}
	}
}
