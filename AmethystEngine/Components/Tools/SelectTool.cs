using BixBite.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace AmethystEngine.Components.Tools
{
	/// <summary>
	/// This class is here for the select tool.
	/// The select tool is used in the editor to select objects.
	/// The selected objects vary on what type of editor is currently in use. 
	/// </summary>
	public class SelectTool
	{

		SpriteLayer Currentlayer;
		public List<Rectangle> SelectedTiles = new List<Rectangle>();

		/// <summary>
		/// This method will return ALL the tiles on the level editor tile canvas that are located 
		/// on a certain layer.
		/// </summary>
		public static List<Rectangle> FindTilesOnLayer(Canvas lcanvas, int zindex)
		{
			//get all the children

			//seperate all the children based on layers.

			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="layertiles">all the tiles on a given layer</param>
		/// <param name="x">the x position of the mouse in relative origin to the canvas</param>
		/// <param name="y">the y position of the mouse in relative origin to the canvas</param>
		/// <returns>Returns the rectangle that the mouse has clicked on. NULL if not found.</returns>
		public static Rectangle FindTile(Canvas backcanvas, List<Rectangle> layertiles, int zindex, int x, int y)
		{
			foreach (Rectangle r in layertiles)
			{
				if (inTile(backcanvas, r, x, y))
				{
					if (Canvas.GetZIndex(r as System.Windows.UIElement) == zindex)
						return r;
				}
			}
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="levelEditorCanvas">the main level editor canvas</param>
		/// <param name="zindex">The Z index of the layer current selected/desired layer</param>
		/// <param name="x">the x position of the mouse in relative origin to the canvas</param>
		/// <param name="y">the y position of the mouse in relative origin to the canvas</param>
		/// <returns></returns>
		public static Rectangle FindTile(Canvas levelEditorCanvas, int zindex, int x, int y)
		{



			return null;
		}
		
		/// <summary>
		/// Takes in a tile, and a mouse position. Determines if the mouse is within the tiles bounds.
		/// </summary>
		/// <returns>true if the mouse location is within a given range</returns>
		public static bool inTile(Canvas backcanvas, Rectangle tile, int x, int y)
		{
			int left = (int)(Canvas.GetLeft(tile) * backcanvas.RenderTransform.Value.M11);
			int right = left + (int)(tile.ActualWidth * backcanvas.RenderTransform.Value.M11);
			int top = (int)(Canvas.GetTop(tile) * backcanvas.RenderTransform.Value.M11);
			int bottom = top + (int)(tile.ActualHeight * backcanvas.RenderTransform.Value.M11);
			if (x >= left && x <= right)
			{
				if (y >= top && y <= bottom )
					return true;
			}
			return false;
		}

	}
}
