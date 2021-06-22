using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BixBite.Rendering.UI
{
	public class GameListBoxCustom : GameListBox
	{

		#region Properties



		#endregion

		#region Fields

		private String _UIDescFile = String.Empty;

		#endregion

		#region Constructors
		public GameListBoxCustom()
		{

		}

		public GameListBoxCustom(String positionFile, Vector2 pos, int width, int height, int innerWidth, int innerHeight, int maxDisplayed, Keys Inc, Keys Dec, Keys PageInc, Keys PageDec, Keys Select, Keys back, int holdFrameLimit,
			bool showBorder = true, EPositionType posType = EPositionType.Vertical)
		: base(pos,width, height, innerWidth,innerHeight, maxDisplayed, Inc, Dec, PageInc, PageDec, Select, back, holdFrameLimit, showBorder, EPositionType.Custom)
		{
			_UIDescFile = positionFile;
		}
		#endregion

		public void SetAbsolutePosition_Items()
		{
			int StartPosX = 0;
			int StartPosY = 0;
			


			using (XmlReader reader = XmlReader.Create(_UIDescFile))
			{
				while (reader.Read())
				{

					//Skip to Custom ListBox
					while (reader.Name == "CustomListBox" && reader.NodeType == XmlNodeType.Element)
					{
						StartPosX = int.Parse(reader.GetAttribute("StartX"));
						StartPosY = int.Parse(reader.GetAttribute("StartY"));
						MaxDisplayed = int.Parse(reader.GetAttribute("MaxDisplayed"));
						reader.Read();
					}

					if (reader.Name == "Items" && reader.NodeType == XmlNodeType.Element)
					{
						int counter = 0;
						do
						{
							reader.Read();

							if (reader.Name == "Item")
							{
								Vector2 pos = new Vector2(int.Parse(reader.GetAttribute("PosX")), int.Parse(reader.GetAttribute("PosY")));
								Items[counter].AbsolutePosition = pos;
								//Items[i].AbsolutePosition += this.Position;

								if (counter++ < MaxDisplayed)
									_itemPositions_List.Add(pos);
							}
						} while (reader.Name != "Items");
					}
				}
			}
		}

		public override void SetHighlightedPositions_Items(int xoff, int yoff)
		{
			base.SetHighlightedPositions_Items(xoff, yoff);
		}
	}
}
