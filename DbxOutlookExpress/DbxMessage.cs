﻿/////////////////////////////////////////////////////////////////////////////
// <copyright file="DbxMessage.cs" company="James John McGuire">
// Copyright © 2021 - 2025 James John McGuire. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.Email.DbxOutlookExpress
{
	using System;
	using System.IO;
	using System.Text;
	using global::Common.Logging;

	/// <summary>
	/// Dbx message indx class.
	/// </summary>
	public class DbxMessage
	{
		private static readonly ILog Log = LogManager.GetLogger(
			System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="DbxMessage"/> class.
		/// </summary>
		public DbxMessage()
		{
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="DbxMessage"/> class.
		/// </summary>
		/// <param name="fileBytes">The bytes of the file.</param>
		/// <param name="address">The address of the item with in
		/// the file.</param>
		/// <param name="preferredEncoding">The preferred encoding to use as
		/// a fall back when the encoding can not be detected.</param>
		public DbxMessage(
			byte[] fileBytes, uint address, Encoding preferredEncoding)
		{
			DbxMessageIndexedItem index = new (fileBytes, address);
			index.PreferredEncoding = preferredEncoding;
			index.SetItemValues(this);
		}

		/// <summary>
		/// Gets or sets the account associated with the message.
		/// </summary>
		/// <value>The account associated with the message.</value>
		public int Account { get; set; }

		/// <summary>
		/// Gets or sets the answered to message id.
		/// </summary>
		/// <value>The answered to message id.</value>
		public int AnswerId { get; set; }

		/// <summary>
		/// Gets or sets the pointer to the corresponding message.
		/// </summary>
		/// <value>The pointer to the corresponding message.</value>
		public string CorrespoindingMessage { get; set; }

		/// <summary>
		/// Gets or sets the encoding of the message.
		/// </summary>
		/// <value>The encoding of the message.</value>
		public Encoding Encoding { get; set; }

		/// <summary>
		/// Gets or sets the flags of the message.
		/// </summary>
		/// <value>The flags of the message.</value>
		public int Flags { get; set; }

		/// <summary>
		/// Gets or sets the index is used for the Hotmail Http email accounts.
		/// </summary>
		/// <value>The index is used for the Hotmail Http
		/// email accounts.</value>
		/// <remarks>
		/// The This index is used for the Hotmail Http email accounts and
		/// stores a message id ("MSG982493141.24"). I don't know if other
		/// Http email accounts are using this index too..
		/// </remarks>
		public int HotmailIndex { get; set; }

		/// <summary>
		/// Gets or sets the message id.
		/// </summary>
		/// <value>The message id.</value>
		public uint Id { get; set; }

		/// <summary>
		/// Gets or sets the index of the message.
		/// </summary>
		/// <value>The index of the message.</value>
		public int Index { get; set; }

		/// <summary>
		/// Gets or sets the number of lines in the body.
		/// </summary>
		/// <value>The number of lines in the body.</value>
		public int LineCount { get; set; }

		/// <summary>
		/// Gets or sets the entire message.
		/// </summary>
		/// <value>The entire message.</value>
#pragma warning disable CA1819 // Properties should not return arrays
		public byte[] Message { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

		/// <summary>
		/// Gets the message as a stream.
		/// </summary>
		/// <value>The message as a stream.</value>
		public Stream MessageStream
		{
			get
			{
				MemoryStream stream = new (Message);
				return stream;
			}
		}

		/// <summary>
		/// Gets or sets the created or send time of the message.
		/// </summary>
		/// <value>The created or send time of the message.</value>
		public DateTime MessageTime { get; set; }

		/// <summary>
		/// Gets or sets the original subject of the message.
		/// </summary>
		/// <value>The original subject of the message.</value>
		public string OriginalSubject { get; set; }

		/// <summary>
		/// Gets or sets the priority of the eMail.
		/// </summary>
		/// <value>The priority of the eMail(1 high, 3 normal, 5 low).</value>
		public int Priority { get; set; }

		/// <summary>
		/// Gets or sets the recipient name.
		/// </summary>
		/// <value>The recipient name.</value>
		public string ReceiptentName { get; set; }

		/// <summary>
		/// Gets or sets the recipient email address.
		/// </summary>
		/// <value>The recipient email address.</value>
		public string ReceiptentEmailAddress { get; set; }

		/// <summary>
		/// Gets or sets the time message created/received.
		/// </summary>
		/// <value>The time message created/received.</value>
		public DateTime ReceivedTime { get; set; }

		/// <summary>
		/// Gets or sets the registry key for mail or news account.
		/// </summary>
		/// <value>The registry key for mail or news account
		/// (like "00000008").</value>
		public int RegistryKey { get; set; }

		/// <summary>
		/// Gets or sets the time message saved in this folder.
		/// </summary>
		/// <value>The time message saved in this folder.</value>
		public DateTime SavedInFolderTime { get; set; }

		/// <summary>
		/// Gets or sets the sender mail address and name.
		/// </summary>
		/// <value>The sender mail address and name.</value>
		public string Sender { get; set; }

		/// <summary>
		/// Gets or sets the sender mail address and name.
		/// </summary>
		/// <value>The sender mail address and name.</value>
		public string SenderEmailAddress { get; set; }

		/// <summary>
		/// Gets or sets the sender name.
		/// </summary>
		/// <value>The sender name.</value>
		public string SenderName { get; set; }

		/// <summary>
		/// Gets or sets the subject of the message.
		/// </summary>
		/// <value>The subject of the message.</value>
		public string Subject { get; set; }

		/// <summary>
		/// Gets as file.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns>A value indicating success or not.</returns>
		public bool GetAsFile(string filePath)
		{
			bool result = false;

			try
			{
				FileStream stream = File.OpenWrite(filePath);
				using BinaryWriter writer = new (stream);
				writer.Write(Message);

				result = true;
			}
			catch (Exception exception) when
				(exception is ArgumentException ||
				exception is ArgumentNullException ||
				exception is DirectoryNotFoundException ||
				exception is IOException ||
				exception is NotSupportedException ||
				exception is ObjectDisposedException ||
				exception is PathTooLongException ||
				exception is UnauthorizedAccessException)
			{
				Log.Error(exception.ToString());
			}

			return result;
		}
	}
}
