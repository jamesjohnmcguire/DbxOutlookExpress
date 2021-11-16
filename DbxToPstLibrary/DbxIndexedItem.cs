﻿/////////////////////////////////////////////////////////////////////////////
// <copyright file="DbxIndexedItem.cs" company="James John McGuire">
// Copyright © 2021 James John McGuire. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Text;

namespace DbxToPstLibrary
{
	/// <summary>
	/// Dbx indexed item class.
	/// </summary>
	public class DbxIndexedItem
	{
		// Somewhat arbitrary, as other references have this as 0x20, but other
		// notes indicate this may not enough.
		private const int MaximumIndexes = 0x40;

		private readonly uint[] indexes;

		private byte[] bodyBytes;

		/// <summary>
		/// Initializes a new instance of the <see cref="DbxIndexedItem"/>
		/// class.
		/// </summary>
		public DbxIndexedItem()
		{
			indexes = new uint[MaximumIndexes];
		}

		/// <summary>
		/// Reads the indexed item and saves the values.
		/// </summary>
		/// <param name="fileBytes">The bytes of the file.</param>
		/// <param name="address">The address of the item with in
		/// the file.</param>
		public virtual void ReadIndex(byte[] fileBytes, uint address)
		{
			byte[] initialBytes = new byte[12];

			Array.Copy(fileBytes, address, initialBytes, 0, 12);

			// It will be easier to work with integers as opposed to bytes.
			uint[] initialArray = Bytes.ToIntegerArray(initialBytes);

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

			for (uint index = 0; index < itemsCountBytes; index += 4)
			{
				byte rawValue = bodyBytes[index];
				bool isDirect = Bytes.GetBit(rawValue, 7);
				byte indexOffset = (byte)(rawValue & 0x7F);

				if (isDirect == true)
				{
					uint value = index;
					value++;
					SetIndex(indexOffset, value);
				}
				else
				{
					uint value = Bytes.ToIntegerLimit(bodyBytes, index + 1, 2);
					offset = itemsCountBytes;
					value = offset + value;
					SetIndex(indexOffset, value);
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
			string item = null;
			uint subIndex = indexes[index];

			if (subIndex > 0)
			{
				uint end = subIndex;
				byte check;

				do
				{
					check = bodyBytes[end];

					if (check == 0)
					{
						break;
					}

					end++;
				}
				while (check > 0);

				int length = (int)(end - subIndex);

				item =
					Encoding.ASCII.GetString(bodyBytes, (int)subIndex, length);
			}

			return item;
		}

		/// <summary>
		/// Get the values from the indexed item.
		/// </summary>
		/// <param name="index">The index item to retrieve.</param>
		/// <returns>The value of the itemed item.</returns>
		public uint GetValue(uint index)
		{
			uint item = 0;
			uint subIndex = indexes[index];

			if (subIndex > 0)
			{
				item = Bytes.ToIntegerLimit(bodyBytes, subIndex, 3);
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
				item = Bytes.ToLong(bodyBytes, subIndex);
			}

			return item;
		}

		private void SetIndex(uint index, uint value)
		{
			indexes[index] = value;
		}
	}
}
