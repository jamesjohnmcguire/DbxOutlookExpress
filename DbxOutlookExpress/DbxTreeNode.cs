/////////////////////////////////////////////////////////////////////////////
// <copyright file="DbxTreeNode.cs" company="James John McGuire">
// Copyright © 2021 - 2026 James John McGuire. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.Email.DbxOutlookExpress
{
	using System.Collections.Generic;

	/// <summary>
	/// Dbx tree node class.
	/// </summary>
	public class DbxTreeNode
	{
		private readonly IList<DbxNodeItem> nodeItems =
			[];

		/// <summary>
		/// Gets or sets the node file index.
		/// </summary>
		/// <value>The node file index.</value>
		public uint NodeFileIndex { get; set; }

		/// <summary>
		/// Gets or sets the children nodes index.
		/// </summary>
		/// <value>The node children nodes index.</value>
		public uint ChildrenNodesIndex { get; set; }

		/// <summary>
		/// Gets or sets the parent node index.
		/// </summary>
		/// <value>The parent node index.</value>
		public uint ParentNodeIndex { get; set; }

		/// <summary>
		/// Gets or sets the node id.
		/// </summary>
		/// <value>The node id.</value>
		public byte NodeId { get; set; }

		/// <summary>
		/// Gets or sets the node item count.
		/// </summary>
		/// <value>The node item count.</value>
		public byte ItemCount { get; set; }

		/// <summary>
		/// Gets or sets the node chidren count.
		/// </summary>
		/// <value>The node chidren count.</value>
		public uint ChildrenNodesCount { get; set; }

		/// <summary>
		/// Gets the node items.
		/// </summary>
		/// <value>The node items.</value>
		public IList<DbxNodeItem> NodeItems
		{
			get { return nodeItems; }
		}
	}
}
