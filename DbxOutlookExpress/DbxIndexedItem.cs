﻿/////////////////////////////////////////////////////////////////////////////
// <copyright file="DbxIndexedItem.cs" company="James John McGuire">
// Copyright © 2021 - 2025 James John McGuire. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.Email.DbxOutlookExpress
{
	using System;
	using System.Text;
	using DigitalZenWorks.Common.Utilities;
	using global::Common.Logging;
	using UtfUnknown;

	/// <summary>
	/// Dbx indexed item class.
	/// </summary>
	public class DbxIndexedItem
	{
		// Somewhat arbitrary, as other references have this as 0x20, but other
		// notes indicate this may not enough.
		private const int MaximumIndexes = 0x40;

		private static readonly ILog Log = LogManager.GetLogger(
			System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private readonly byte[] fileBytes;
		private readonly uint[] indexes;
		private readonly int[] indexSizes;

		private byte[] bodyBytes;

		/// <summary>
		/// Initializes a new instance of the <see cref="DbxIndexedItem"/>
		/// class.
		/// </summary>
		/// <param name="fileBytes">The bytes of the file.</param>
		/// <param name="address">The address of the item with in
		/// the file.</param>
		public DbxIndexedItem(byte[] fileBytes, uint address)
		{
			this.fileBytes = fileBytes;

			indexes = new uint[MaximumIndexes];
			indexSizes = new int[MaximumIndexes];

			SetIndexes(address);
		}

		/// <summary>
		/// Gets or sets the last encoding used.
		/// </summary>
		/// <value>The last encoding used.</value>
		public Encoding LastEncoding { get; set; }

		/// <summary>
		/// Gets or sets the preferred encoding.
		/// </summary>
		/// <remarks>In cases where the encoding can not be detected,
		/// use this encoding.</remarks>
		/// <value>The preferred encoding.</value>
		public Encoding PreferredEncoding { get; set; }

		/// <summary>
		/// Get a string value directly from the file buffer.
		/// </summary>
		/// <param name="buffer">The byte buffer to check within.</param>
		/// <param name="address">The address of the item to retrieve.</param>
		/// <returns>The value of the itemed item.</returns>
		public string GetStringDirect(byte[] buffer, uint address)
		{
			string item = null;

			if (buffer != null && address > 0)
			{
				uint end = address;
				byte check;

				do
				{
					check = buffer[end];

					if (check == 0)
					{
						break;
					}

					end++;
				}
				while (check > 0);

				int length = (int)(end - address);

				item = GetStringDirect(buffer, address, length);
			}

			return item;
		}

		/// <summary>
		/// Get a string value directly from the file buffer.
		/// </summary>
		/// <param name="buffer">The byte buffer to check within.</param>
		/// <param name="address">The address of the item to retrieve.</param>
		/// <param name="length">The length of the string to get.</param>
		/// <returns>The value of the itemed item.</returns>
		public string GetStringDirect(byte[] buffer, uint address, int length)
		{
			string item = null;

			if (buffer != null && address > 0)
			{
				byte[] stringBytes = new byte[length];

				Array.Copy(buffer, address, stringBytes, 0, length);

				DetectionResult results =
					CharsetDetector.DetectFromBytes(stringBytes);

				Encoding encoding = Encoding.UTF8;

				DetectionDetail resultDetected = results.Detected;

				if (resultDetected != null && resultDetected.Encoding != null)
				{
					encoding = resultDetected.Encoding;
				}
				else
				{
					Log.Warn("Failed detecting encoding");

					if (PreferredEncoding != null)
					{
						Log.Info("Trying preferred encoding");
						encoding = PreferredEncoding;
					}
				}

				// save encoding for future use
				LastEncoding = encoding;

				item = encoding.GetString(buffer, (int)address, length);
			}

			return item;
		}

		/// <summary>
		/// Gets the file body bytes.
		/// </summary>
		/// <returns>The file body bytes.</returns>
		public byte[] GetBodyBytes()
		{
			return bodyBytes;
		}

		/// <summary>
		/// Gets the file bytes.
		/// </summary>
		/// <returns>The file bytes.</returns>
		public byte[] GetFileBytes()
		{
			return fileBytes;
		}

		/// <summary>
		/// Reads the indexed item and saves the values.
		/// </summary>
		/// <param name="address">The address of the item with in
		/// the file.</param>
		public void SetIndexes(uint address)
		{
			byte[] initialBytes = new byte[12];

			Array.Copy(fileBytes, address, initialBytes, 0, 12);

			// It will be easier to work with integers as opposed to bytes.
			uint[] initialArray = BitBytes.ToIntegerArray(initialBytes);

			if (initialArray[0] != address)
			{
				throw new DbxException("Wrong object marker!");
			}

			uint bodyLength = initialArray[1];
			byte itemsCount = initialBytes[10];

			uint offset = address + 12;

			bodyBytes = new byte[bodyLength];
			Array.Copy(fileBytes, offset, bodyBytes, 0, bodyLength);

			uint itemsCountBytes = (uint)itemsCount * 4;

			bool isIndirect = false;
			uint lastIndirect = 0;

			for (uint index = 0; index < itemsCountBytes; index += 4)
			{
				byte rawValue = bodyBytes[index];
				bool isDirect = BitBytes.GetBit(rawValue, 7);
				byte indexOffset = (byte)(rawValue & 0x7F);

				if (isDirect == true)
				{
					uint value = index;
					value++;
					SetIndex(indexOffset, value);
				}
				else
				{
					uint value = BitBytes.ToIntegerLimit(bodyBytes, index + 1, 2);
					offset = itemsCountBytes;
					value = offset + value;
					SetIndex(indexOffset, value);

					if (isIndirect == true)
					{
						SetIndexSize(lastIndirect, value);
					}

					isIndirect = true;
					lastIndirect = indexOffset;
				}
			}
		}

		/// <summary>
		/// Get a string value from the indexed item.
		/// </summary>
		/// <param name="index">The index item to retrieve.</param>
		/// <returns>The value of the itemed item.</returns>
		public string GetString(uint index)
		{
			uint subIndex = indexes[index];

			string item = GetStringDirect(bodyBytes, subIndex);

			return item;
		}

		/// <summary>
		/// Gets the size of the index.
		/// </summary>
		/// <param name="index">The index to check.</param>
		/// <returns>The size of the index.</returns>
		public int GetSize(uint index)
		{
			return indexSizes[index];
		}

		/// <summary>
		/// Get the values from the indexed item.
		/// </summary>
		/// <param name="index">The index item to retrieve.</param>
		/// <returns>The value of the itemed item.</returns>
		public uint GetValue(uint index)
		{
			uint item = GetValue(index, 3);

			return item;
		}

		/// <summary>
		/// Get the values from the indexed item.
		/// </summary>
		/// <param name="index">The index item to retrieve.</param>
		/// <param name="amount">The amount of bytes to retrieve.</param>
		/// <returns>The value of the itemed item.</returns>
		public uint GetValue(uint index, int amount)
		{
			uint item = 0;
			uint subIndex = indexes[index];

			if (subIndex > 0)
			{
				item = BitBytes.ToIntegerLimit(bodyBytes, subIndex, amount);
			}

			return item;
		}

		/// <summary>
		/// Get the values from the indexed item.
		/// </summary>
		/// <param name="index">The index item to retrieve.</param>
		/// <returns>The value of the itemed item.</returns>
		public ulong GetValueLong(uint index)
		{
			ulong item = 0;
			uint subIndex = indexes[index];

			if (subIndex > 0)
			{
				item = BitBytes.ToLong(bodyBytes, subIndex);
			}

			return item;
		}

		private void SetIndex(uint index, uint value)
		{
			indexes[index] = value;
			indexSizes[index] = 3;
		}

		private void SetIndexSize(uint index, uint offset)
		{
			if (index < indexSizes.Length)
			{
				int size = (int)(offset - indexes[index]);
				indexSizes[index] = size;
			}
		}
	}
}
