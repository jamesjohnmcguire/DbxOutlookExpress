using NUnit.Framework;
using DigitalZenWorks.Email.DbxOutlookExpress;
using System;
using System.Text;
using System.IO;

[assembly: CLSCompliant(true)]

namespace DigitalZenWorks.Email.DbxOutlookExpress.Tests
{
	/// <summary>
	/// Test class.
	/// </summary>
	public class DbxOutlookExpressTests
	{
		/// <summary>
		/// Set up method.
		/// </summary>
		[SetUp]
		public void Setup()
		{
		}

		/// <summary>
		/// Test for get bit.
		/// </summary>
		[Test]
		public void TestBytesGetBit()
		{
			// 0 based
			byte sevenOn = 64;
			bool bit = Bytes.GetBit(sevenOn, 6);
			Assert.True(bit);

			byte sevenOff = 63;
			bit = Bytes.GetBit(sevenOff, 6);
			Assert.False(bit);

			sevenOff = 128;
			bit = Bytes.GetBit(sevenOff, 6);
			Assert.False(bit);
		}

		/// <summary>
		/// Test bytes to integer.
		/// </summary>
		[Test]
		public void TestBytesToInteger()
		{
			byte[] testBytes =
			{
				0x05, 0x1d, 0x00, 0x00, 0x86, 0x29, 0x00, 0x04, 0x64, 0x69
			};

			uint test = Bytes.ToInteger(testBytes, 4);
			Assert.AreEqual(test, 0x4002986);
		}

		/// <summary>
		/// Test bytes to array.
		/// </summary>
		[Test]
		public void TestBytesToIntegerArray()
		{
			byte[] testBytes =
			{
				0x00, 0x00, 0x00, 0x00, 0x38, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x04, 0x01, 0x80, 0x11, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00,
				0x05, 0x1d, 0x00, 0x00, 0x86, 0x29, 0x00, 0x04, 0x64, 0x69,
				0x73, 0x63, 0x75, 0x73, 0x73, 0x69, 0x6f, 0x6e, 0x2e, 0x66,
				0x61, 0x73, 0x74, 0x61, 0x6E, 0x64, 0x66, 0x75, 0x72, 0x69,
				0x75, 0x73, 0x2e, 0x63, 0x6f, 0x6d, 0x00, 0x30, 0x30, 0x30,
				0x30, 0x30, 0x30, 0x31, 0x37, 0x00, 0x00, 0x00
			};

			uint[] integerArray = Bytes.ToIntegerArray(testBytes);
			int size = integerArray.Length;

			Assert.AreEqual(size, 0x11);

			Assert.AreEqual(integerArray[1], 0x38);
		}

		/// <summary>
		/// Test bytes to integer.
		/// </summary>
		[Test]
		public void TestBytesToIntegerLimit()
		{
			byte[] testBytes =
			{
				0x05, 0x1d, 0x00, 0x00, 0x86, 0x29, 0x01, 0x04, 0x64, 0x69
			};

			uint test = Bytes.ToIntegerLimit(testBytes, 4, 3);
			Assert.AreEqual(test, 0x12986);
		}

		/// <summary>
		/// Test bytes to long.
		/// </summary>
		[Test]
		public void TestBytesToLong()
		{
			byte[] testBytes =
			{
				0x05, 0x1d, 0x00, 0x00, 0x86, 0x29, 0x00, 0x04, 0x64, 0x69
			};

			ulong test = Bytes.ToLong(testBytes, 2);
			Assert.AreEqual(test, 0x6964040029860000);
		}

		/// <summary>
		/// Test bytes to short.
		/// </summary>
		[Test]
		public void TestBytesToShort()
		{
			byte[] testBytes =
			{
				0x05, 0x1d, 0x00, 0x00, 0x86, 0x29, 0x00, 0x04, 0x64, 0x69
			};

			ushort test = Bytes.ToShort(testBytes, 4);
			Assert.AreEqual(test, 0x2986);
		}

		/// <summary>
		/// Test for get next folder.
		/// </summary>
		[Test]
		public void TestGetNextFolderFail()
		{
			string basePath = Path.GetTempPath();
			string path = basePath + "Nothing";

			Encoding encoding = Encoding.UTF8;

			DbxSet dbxSet = new(path, encoding);

			DbxFolder folder = dbxSet.GetNextFolder();
			Assert.Null(folder);
		}

		/// <summary>
		/// Test indexed info.
		/// </summary>
		[Test]
		public void TestIndexedInfo()
		{
			byte[] testBytes =
			{
				0x00, 0x00, 0x00, 0x00, 0x38, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x04, 0x01, 0x80, 0x11, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00,
				0x05, 0x1d, 0x00, 0x00, 0x86, 0x29, 0x00, 0x04, 0x64, 0x69,
				0x73, 0x63, 0x75, 0x73, 0x73, 0x69, 0x6f, 0x6e, 0x2e, 0x66,
				0x61, 0x73, 0x74, 0x61, 0x6E, 0x64, 0x66, 0x75, 0x72, 0x69,
				0x75, 0x73, 0x2e, 0x63, 0x6f, 0x6d, 0x00, 0x30, 0x30, 0x30,
				0x30, 0x30, 0x30, 0x31, 0x37, 0x00, 0x00, 0x00
			};

			DbxIndexedItem item = new (testBytes, 0);

			uint value = item.GetValue(DbxFolderIndexedItem.Id);
			Assert.AreEqual(value, 0x11);

			value = item.GetValue(DbxFolderIndexedItem.ParentId);
			Assert.AreEqual(value, 0);

			string name = item.GetString(DbxFolderIndexedItem.Name);
			string expected = "discussion.fastandfurius.com";
			Assert.AreEqual(name, expected);
		}

		/// <summary>
		/// Test for non existant file.
		/// </summary>
		[Test]
		public void TestNonExistantFile()
		{
			string basePath = Path.GetTempPath();

			string nonExistantFilePath = basePath + "nothing.dbx";

			DbxFile dbxFile = new(nonExistantFilePath);

			Assert.Null(dbxFile.Header);
		}

		/// <summary>
		/// Test for sanity check.
		/// </summary>
		[Test]
		public void TestSanityCheck()
		{
			Assert.Pass();
		}
	}
}
