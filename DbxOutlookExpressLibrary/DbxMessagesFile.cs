/////////////////////////////////////////////////////////////////////////////
// <copyright file="DbxMessagesFile.cs" company="James John McGuire">
// Copyright © 2021 James John McGuire. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using Common.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace DigitalZenWorks.Email.DbxOutlookExpress
{
	/// <summary>
	/// Dbx emails file.
	/// </summary>
	public class DbxMessagesFile : DbxFile
	{
		private static readonly ILog Log = LogManager.GetLogger(
			System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="DbxMessagesFile"/> class.
		/// </summary>
		/// <param name="filePath">The path of the dbx file.</param>
		/// <param name="preferredEncoding">The preferred encoding to use as
		/// a fall back when the encoding can not be detected.</param>
		public DbxMessagesFile(string filePath, Encoding preferredEncoding)
			: base(filePath)
		{
			PreferredEncoding = preferredEncoding;

			DbxFileType check = Header.FileType;

			if (check != DbxFileType.MessageFile)
			{
				Log.Error(filePath + " not actually a messagess file");
			}
			else
			{
				FileInfo fileInfo = new (filePath);

				string folderName =
					Path.GetFileNameWithoutExtension(fileInfo.Name);
				Log.Info("Checking folder: " + folderName);

				ReadTree();
			}
		}

		/// <summary>
		/// Get the next message.
		/// </summary>
		/// <returns>The next message.</returns>
		public DbxMessage GetNextMessage()
		{
			DbxMessage message = null;

			if (CurrentIndex < Tree.FolderInformationIndexes.Count)
			{
				try
				{
					message = GetNextMessageInner();
				}
				catch (DbxException exception)
				{
					do
					{
						CurrentIndex++;

						message = GetNextMessageInner();
					}
					while (message == null &&
						CurrentIndex < Tree.FolderInformationIndexes.Count);
				}

				// Prep for next call.
				CurrentIndex++;
			}

			return message;
		}

		/// <summary>
		/// List messages method.
		/// </summary>
		public void List()
		{
			if (Tree != null)
			{
				byte[] fileBytes = GetFileBytes();

				uint itemIndex = 0;

				foreach (uint index in Tree.FolderInformationIndexes)
				{
					DbxMessage message =
						new (fileBytes, index, PreferredEncoding);

					string logMessage = string.Format(
						CultureInfo.InvariantCulture,
						"{0} From: {1}: {2}",
						itemIndex,
						message.SenderName,
						message.SenderEmailAddress);
					Log.Info(logMessage);

					logMessage = string.Format(
						CultureInfo.InvariantCulture,
						"To: {0}: {1}",
						message.ReceiptentName,
						message.ReceiptentEmailAddress);
					Log.Info(logMessage);

					logMessage = string.Format(
						CultureInfo.InvariantCulture,
						"Received at: {0} Subject: {1}",
						message.ReceivedTime,
						message.Subject);
					Log.Info(logMessage);

					bool showBody = false;
					if (showBody == true)
					{
						logMessage = string.Format(
							CultureInfo.InvariantCulture,
							"item value[{0}] is {1}",
							DbxMessageIndexedItem.CorrespoindingMessage,
							message.Body);
						Log.Info(logMessage);
					}

					itemIndex++;
				}
			}
		}

		/// <summary>
		/// List deleted segments.
		/// </summary>
		public void ListDeletedSegments()
		{
			byte[] fileBytes = GetFileBytes();

			uint address = Header.DeletedItems;

			while (address != 0)
			{
				byte[] headerBytes = new byte[0x10];
				Array.Copy(fileBytes, address, headerBytes, 0, 0x10);

				uint length = Bytes.ToInteger(headerBytes, 8);

				// skip over header
				address += 0x10;

				string section = Encoding.ASCII.GetString(
					fileBytes, (int)address, (int)length);

				string logMessage = string.Format(
					CultureInfo.InvariantCulture,
					"deleted item value is {0}",
					section);
				Log.Info(logMessage);

				// prep next section
				address = Bytes.ToInteger(headerBytes, 12);
			}
		}

		private DbxMessage GetNextMessageInner()
		{
			byte[] fileBytes = GetFileBytes();

			uint address = Tree.FolderInformationIndexes[CurrentIndex];

			DbxMessage message = new (fileBytes, address, PreferredEncoding);

			string logMessage = string.Format(
				CultureInfo.InvariantCulture,
				"message {0} From: {1} Subject: {2}",
				CurrentIndex,
				message.SenderEmailAddress,
				message.Subject);
			Log.Info(logMessage);

			return message;
		}
	}
}
