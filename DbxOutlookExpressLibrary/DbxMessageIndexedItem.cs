﻿/////////////////////////////////////////////////////////////////////////////
// <copyright file="DbxMessageIndexedItem.cs" company="James John McGuire">
// Copyright © 2021 James John McGuire. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using Common.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace DbxToPstLibrary
{
	/// <summary>
	/// Dbx message indexed item.
	/// </summary>
	public class DbxMessageIndexedItem : DbxIndexedItem
	{
		/// <summary>
		/// The OE mail or news account name.
		/// </summary>
		public const int Account = 0x1a;

		/// <summary>
		/// The answered to message id.
		/// </summary>
		public const int AnswerId = 0x0A;

		/// <summary>
		/// The pointer to the corresponding message.
		/// </summary>
		public const int CorrespoindingMessage = 0x04;

		/// <summary>
		/// The flags index of the folder.
		/// </summary>
		public const int Flags = 0x01;

		/// <summary>
		/// The This index is used for the Hotmail Http email accounts and
		/// stores a message id ("MSG982493141.24"). I don't know if other
		/// Http email accounts are using this index too..
		/// </summary>
		public const int HotmailIndex = 0x23;

		/// <summary>
		/// The id of the message.
		/// </summary>
		public const int Id = 0x07;

		/// <summary>
		/// The index of the message.
		/// </summary>
		public const int Index = 0x00;

		/// <summary>
		/// The number of lines in the body.
		/// </summary>
		public const int LineCount = 0x03;

		/// <summary>
		/// The created or send time of the message.
		/// </summary>
		public const int MessageTime = 0x02;

		/// <summary>
		/// The original subject of the message.
		/// </summary>
		public const int OriginalSubject = 0x05;

		/// <summary>
		/// The priority of the eMail(1 high, 3 normal, 5 low).
		/// </summary>
		public const int Priority = 0x10;

		/// <summary>
		/// The receiver name.
		/// </summary>
		public const int ReceiptentName = 0x13;

		/// <summary>
		/// The receiver mail address.
		/// </summary>
		public const int ReceiptentEmailAddress = 0x14;

		/// <summary>
		/// The time message created/received.
		/// </summary>
		public const int ReceivedTime = 0x12;

		/// <summary>
		/// The Registry key for mail or news account(like "00000008").
		/// </summary>
		public const int RegistryKey = 0x1b;

		/// <summary>
		/// The time message saved in this folder.
		/// </summary>
		public const int SavedInFolderTime = 0x02;

		/// <summary>
		/// The sender mail address and name.
		/// </summary>
		public const int Sender = 0x09;

		/// <summary>
		/// The sender mail address and name.
		/// </summary>
		public const int SenderEmailAddress = 0x0E;

		/// <summary>
		/// The sender name.
		/// </summary>
		public const int SenderName = 0x0D;

		/// <summary>
		/// The subject of the message.
		/// </summary>
		public const int Subject = 0x08;

		private static readonly ILog Log = LogManager.GetLogger(
			System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private readonly DbxMessageIndex messageIndex;

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="DbxMessageIndexedItem"/> class.
		/// </summary>
		/// <param name="fileBytes">The bytes of the file.</param>
		public DbxMessageIndexedItem(byte[] fileBytes)
			: base(fileBytes)
		{
			messageIndex = new DbxMessageIndex();
		}

		/// <summary>
		/// Gets the dbx message index.
		/// </summary>
		/// <value>The dbx message index.</value>
		public DbxMessageIndex MessageIndex { get { return messageIndex; } }

		/// <summary>
		/// Gets the message body.
		/// </summary>
		/// <returns>The message body.</returns>
		public string GetBody()
		{
			string body = null;

			uint address = GetValue(CorrespoindingMessage);

			StringBuilder builder = new StringBuilder();
			byte[] fileBytes = GetFileBytes();

			while (address != 0)
			{
				byte[] headerBytes = new byte[0x10];
				Array.Copy(fileBytes, address, headerBytes, 0, 0x10);

				uint length = Bytes.ToInteger(headerBytes, 8);

				// skip over header
				address += 0x10;

				string section = Encoding.ASCII.GetString(
					fileBytes, (int)address, (int)length);

				builder.Append(section);

				// prep next section
				address = Bytes.ToInteger(headerBytes, 12);
			}

			body = builder.ToString();

			return body;
		}

		/// <summary>
		/// Reads the indexed item and saves the values.
		/// </summary>
		/// <param name="address">The address of the item with in
		/// the file.</param>
		public override void ReadIndex(uint address)
		{
			base.ReadIndex(address);

			messageIndex.SenderName = GetString(SenderName);
			messageIndex.SenderEmailAddress = GetString(SenderEmailAddress);

			long rawTime = (long)GetValueLong(ReceivedTime);
			try
			{
				messageIndex.ReceivedTime = DateTime.FromFileTime(rawTime);
			}
			catch (ArgumentOutOfRangeException exception)
			{
				Log.Error(exception.ToString());
			}

			messageIndex.Subject = GetString(Subject);
			messageIndex.ReceiptentName = GetString(ReceiptentName);
			messageIndex.ReceiptentEmailAddress =
				GetString(ReceiptentEmailAddress);

			messageIndex.Body = GetBody();
		}
	}
}