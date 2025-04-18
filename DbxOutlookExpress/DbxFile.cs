﻿/////////////////////////////////////////////////////////////////////////////
// <copyright file="DbxFile.cs" company="James John McGuire">
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
	/// Dbx file class.
	/// </summary>
	public class DbxFile
	{
		private static readonly ILog Log = LogManager.GetLogger(
			System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private readonly byte[] fileBytes;
		private readonly string folderPath;
		private DbxTree tree;

		/// <summary>
		/// Initializes a new instance of the <see cref="DbxFile"/> class.
		/// </summary>
		/// <param name="filePath">The path of the dbx file.</param>
		public DbxFile(string filePath)
		{
			folderPath = filePath;

			if (File.Exists(filePath))
			{
				string extension = Path.GetExtension(filePath);

				if (extension.Equals(".dbx", StringComparison.Ordinal))
				{
					fileBytes = File.ReadAllBytes(filePath);

					byte[] headerBytes = new byte[0x24bc];
					Array.Copy(fileBytes, headerBytes, 0x24bc);

					Header = new (headerBytes);
				}
				else
				{
					Log.Error("File does not have dbx extension: " + filePath);

					throw new DbxException(
						"File does not have dbx extension: " + filePath);
				}
			}
			else
			{
				FileInfo fileInfo = new (filePath);
				string name = fileInfo.Name;
				Log.Error("File does not exist!: " + name);

				throw new DbxException("File does not exist!: " + name);
			}
		}

		/// <summary>
		/// Gets or sets the current index of items being enumerated.
		/// </summary>
		/// <value>The dbx current index of items being enumerated.</value>
		public int CurrentIndex { get; set; }

		/// <summary>
		/// Gets or sets the dbx file header.
		/// </summary>
		/// <value>The dbx file header.</value>
		public DbxHeader Header { get; set; }

		/// <summary>
		/// Gets the dbx folder file path.
		/// </summary>
		/// <value>The dbx folder file path.</value>
		public string FolderPath
		{
			get { return folderPath; }
		}

		/// <summary>
		/// Gets or sets the preferred encoding.
		/// </summary>
		/// <value>The preferred encoding.</value>
		public Encoding PreferredEncoding { get; set; }

		/// <summary>
		/// Gets the dbx tree.
		/// </summary>
		/// <value>The dbx tree.</value>
		public DbxTree Tree
		{
			get { return tree; }
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
		/// Read the tree method.
		/// </summary>
		public virtual void ReadTree()
		{
			byte[] fileBytes = GetFileBytes();

			tree = new (fileBytes, Header.MainTreeAddress);
		}
	}
}
