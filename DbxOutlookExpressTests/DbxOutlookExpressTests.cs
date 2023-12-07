/////////////////////////////////////////////////////////////////////////////
// <copyright file="DbxOutlookExpressTests.cs" company="James John McGuire">
// Copyright © 2021 - 2023 James John McGuire. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using DigitalZenWorks.Common.Utilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

[assembly: CLSCompliant(true)]

namespace DigitalZenWorks.Email.DbxOutlookExpress.Tests
{
	/// <summary>
	/// Test class.
	/// </summary>
	public class DbxOutlookExpressTests
	{
		private DirectoryInfo testFolder;

		/// <summary>
		/// One time set up method.
		/// </summary>
		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			testFolder = Directory.CreateDirectory("TestFolder");
		}

		/// <summary>
		/// One time tear down method.
		/// </summary>
		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			bool result = Directory.Exists(testFolder.FullName);

			if (true == result)
			{
				Directory.Delete(testFolder.FullName, true);
			}
		}

		/// <summary>
		/// Test for get bit.
		/// </summary>
		[Test]
		public void TestBytesGetBit()
		{
			// 0 based
			byte sevenOn = 64;
			bool bit = BitBytes.GetBit(sevenOn, 6);
			Assert.That(bit, Is.True);

			byte sevenOff = 63;
			bit = BitBytes.GetBit(sevenOff, 6);
			Assert.That(bit, Is.False);

			sevenOff = 128;
			bit = BitBytes.GetBit(sevenOff, 6);
			Assert.That(bit, Is.False);
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

			uint test = BitBytes.ToInteger(testBytes, 4);
			Assert.That(test, Is.EqualTo(0x4002986));
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

			uint[] integerArray = BitBytes.ToIntegerArray(testBytes);
			int size = integerArray.Length;

			Assert.That(size, Is.EqualTo(0x11));

			Assert.That(integerArray[1], Is.EqualTo(0x38));
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

			uint test = BitBytes.ToIntegerLimit(testBytes, 4, 3);
			Assert.That(test, Is.EqualTo(0x12986));
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

			ulong test = BitBytes.ToLong(testBytes, 2);
			Assert.That(test, Is.EqualTo(0x6964040029860000));
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

			ushort test = BitBytes.ToShort(testBytes, 4);
			Assert.That(test, Is.EqualTo(0x2986));
		}

		/// <summary>
		/// Test for getting children folders.
		/// </summary>
		[Test]
		public void TestGetChildrenFolders()
		{
			Encoding encoding = Encoding.UTF8;

			string path = Path.Combine(testFolder.FullName, "Folders.dbx");
			bool result = FileUtils.CreateFileFromEmbeddedResource(
				"DbxOutlookExpressTests.Folders.dbx", path);

			Assert.That(result, Is.True);

			DbxFoldersFile foldersFile = new (path, encoding);

			IList<DbxFolder> childrenFolders = foldersFile.SetTreeOrdered();
			int count = childrenFolders.Count;

			Assert.That(count, Is.GreaterThan(0));
		}

		/// <summary>
		/// Test for get next folder.
		/// </summary>
		[Test]
		public void TestGetNextFolder()
		{
			Encoding encoding = Encoding.UTF8;

			string path = Path.Combine(testFolder.FullName, "Folders.dbx");
			bool result = FileUtils.CreateFileFromEmbeddedResource(
				"DbxOutlookExpressTests.Folders.dbx", path);
			Assert.That(result, Is.True);

			path = Path.Combine(testFolder.FullName, "Inbox.dbx");
			result = FileUtils.CreateFileFromEmbeddedResource(
				"DbxOutlookExpressTests.Inbox.dbx", path);
			Assert.That(result, Is.True);

			path = Path.Combine(testFolder.FullName, "Offline.dbx");
			result = FileUtils.CreateFileFromEmbeddedResource(
				"DbxOutlookExpressTests.Offline.dbx", path);
			Assert.That(result, Is.True);

			path = Path.Combine(testFolder.FullName, "Outbox.dbx");
			result = FileUtils.CreateFileFromEmbeddedResource(
				"DbxOutlookExpressTests.Outbox.dbx", path);
			Assert.That(result, Is.True);

			DbxSet dbxSet = new (testFolder.FullName, encoding);

			DbxFolder folder = dbxSet.GetNextFolder();
			Assert.That(folder, Is.Not.Null);
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

			DbxSet dbxSet = new (path, encoding);

			DbxFolder folder = dbxSet.GetNextFolder();
			Assert.That(folder, Is.Null);
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
			Assert.That(value, Is.EqualTo(0x11));

			value = item.GetValue(DbxFolderIndexedItem.ParentId);
			Assert.That(value, Is.EqualTo(0));

			string name = item.GetString(DbxFolderIndexedItem.Name);
			string expected = "discussion.fastandfurius.com";
			Assert.That(expected, Is.EqualTo(name));
		}

		/// <summary>
		/// Test for non existant file.
		/// </summary>
		[Test]
		public void TestNonExistantFile()
		{
			string basePath = Path.GetTempPath();

			string nonExistantFilePath = basePath + "nothing.dbx";

			DbxException exception = Assert.Throws<DbxException>(() =>
				new DbxFile(nonExistantFilePath));

			Assert.That(exception, Is.Not.Null);
		}

		/// <summary>
		/// Test for sanity check.
		/// </summary>
		[Test]
		public void TestSanityCheck()
		{
			Assert.Pass();
		}

		/// <summary>
		/// Test for ordered tree.
		/// </summary>
		[Test]
		public void TestTree()
		{
			DbxFolder folder1 = new (1, 0, "A", null);
			DbxFolder folder2 = new (2, 4, "B", null);
			DbxFolder folder3 = new (3, 0, "C", null);
			DbxFolder folder4 = new (4, 5, "D", null);
			DbxFolder folder5 = new (5, 0, "E", null);

			IList<DbxFolder> folders = new List<DbxFolder>
			{
				folder1,
				folder2,
				folder3,
				folder4,
				folder5
			};

			DbxFolder folder = new (0, 0, "root", null);

			IList<DbxFolder> childrenFolders = folder.GetChildren(folders);
			int count = childrenFolders.Count;

			Assert.That(count, Is.GreaterThan(0));

			DbxFolder childFolder = childrenFolders[0];
			Assert.That(childFolder.FolderName, Is.EqualTo("E"));

			childFolder = childrenFolders[1];
			Assert.That(childFolder.FolderName, Is.EqualTo("C"));

			childFolder = childrenFolders[2];
			Assert.That(childFolder.FolderName, Is.EqualTo("A"));
		}
	}
}
