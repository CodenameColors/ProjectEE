using System;
using System.Collections.Generic;

namespace BixBite.NodeEditor
{
	public enum ECOnnectionType
	{
		NONE,
		Enter,
		Exit,
		Int,
		Bool
	}


	public partial class ConnectionNode // : Button
	{
		public new String Name { get; set; }

		public List<ConnectionNode> ConnectedNodes = new List<ConnectionNode>();
		public int PositionX;
		public int PositionY;
		//public List<Path> Curves = new List<Path>();
		public ECOnnectionType NodeType;
		public BaseNodeBlock ParentBlock = null;


		public ConnectionNode(BaseNodeBlock pblock, String Name, int xPos, int yPos, ECOnnectionType nodetype)
		{
			this.ParentBlock = pblock;
			this.Name = Name;
			this.PositionX = xPos;
			this.PositionY = yPos;
			this.NodeType = nodetype;
		}

		/// <summary>
		/// Constructing our connection node. this is here when we don't want to set the position.
		/// </summary>
		/// <param name="pblock"></param>
		/// <param name="Name"></param>
		/// <param name="nodetype"></param>
		public ConnectionNode(BaseNodeBlock pblock, String Name, ECOnnectionType nodetype)
		{
			this.ParentBlock = pblock;
			this.Name = Name;
			this.NodeType = nodetype;
		}

	}
}
