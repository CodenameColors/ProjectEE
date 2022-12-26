using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BixBite.Rendering.UI.ListBox
{
	public class GameCustomListBox : GameListBox
	{
		#region Delegates

		#endregion

		#region Fields
		private String _UIDescFile = String.Empty;
		#endregion

		#region Properties

		#endregion

		#region Contructors
		public GameCustomListBox(string UIName, int xPos, int yPos, int width, int height, int zindex, bool border, int borderW, Color borderColor,
			int xOff, int yOff, int innerW, int InnerH, int innerSpacing, int maxDisplayedItems, int holdFrameLimit, 
			Keys Inc, Keys Dec, Keys PageInc, Keys PageDec, Keys Select, Keys back, String uiFile, GraphicsDevice graphicsDevice, Texture2D borderTexture = null, 
			Texture2D highlightedTexture = null, EPositionType positionType = EPositionType.Vertical) 
			: base(UIName, xPos, yPos, width, height, zindex, border, borderW, borderColor, xOff, yOff, innerW, InnerH, innerSpacing,
				maxDisplayedItems, holdFrameLimit, Inc, Dec, PageInc, PageDec, Select, back,graphicsDevice , borderTexture, highlightedTexture, positionType)
		{
			_UIDescFile = uiFile;

		}
		#endregion

		#region Methods
		/// <summary>
		/// Used for CUSTOM UI FILES!
		/// </summary>
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
						MaxDisplayedItems = int.Parse(reader.GetAttribute("MaxDisplayed"));
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
								Items[counter].XPos = (int)pos.X + XPos;
								Items[counter].YPos = (int)pos.Y + YPos;
								//Items[i].AbsolutePosition += this.BarPosition;

								if (counter++ < MaxDisplayedItems)
									_itemPositions_List.Add(pos);
							}
						} while (reader.Name != "Items");
					}
				}
			}
		}
		#endregion


	}
}
