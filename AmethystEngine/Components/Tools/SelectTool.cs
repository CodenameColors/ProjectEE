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
		public static Rectangle FindTile(Canvas backcanvas, List<Rectangle> layertiles, int zindex, int x, int y, int xoff, int yoff)
		{
			foreach (Rectangle r in layertiles)
			{
				if (inTile(backcanvas, r, x, y, xoff, yoff))
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
		/// <param name="LSprites">all the sprites on a given layer</param>
		/// <param name="x">the x position of the mouse in relative origin to the canvas</param>
		/// <param name="y">the y position of the mouse in relative origin to the canvas</param>
		/// <returns>Returns the rectangle that the mouse has clicked on. NULL if not found.</returns>
		public static ContentControl FindSpriteControl(Canvas backcanvas, List<ContentControl> LSprites, int zindex, int x, int y)
		{
			foreach (ContentControl r in LSprites)
			{
				if (inContentControl(backcanvas, r, x, y))
				{
					if (Canvas.GetZIndex(r as System.Windows.UIElement) == zindex)
						return r;
				}
			}
			return null;
		}

		public static Sprite FindSprite(List<Sprite> sprites, ContentControl cc)
		{
			foreach(Sprite spr in sprites)
			{
				//since the top, left are doubles check both Math.Floor, and Math.Ceil
				if ((int)spr.GetProperty("x") == Math.Floor(Canvas.GetLeft(cc)) &&
					(int)spr.GetProperty("y") == Math.Floor(Canvas.GetTop(cc)))
					return spr;
				else if ((int)spr.GetProperty("x") == Math.Ceiling(Canvas.GetLeft(cc)) &&
					(int)spr.GetProperty("y") == Math.Ceiling(Canvas.GetTop(cc)))
					return spr;
			}
			return null;
		}

		public static Border FindBorder(Canvas backcavas, List<Border> GEBorders, int zindex, int x, int y)
		{
			foreach (Border item in GEBorders)
			{
				if(inBorder(backcavas, item, x, y))
				{ 
					if(Canvas.GetZIndex(item) == zindex)
						return item;
				}
			}
			return null;
		}

		public static BixBite.GameEvent FindGameEvent(SpriteLayer SLayer, Border bor, int group)
		{
			//get the location of the border in CELL DIM
			int celly = (int)Canvas.GetLeft(bor)/40;
			int cellx = (int)Canvas.GetTop(bor)/40;
			
			//get the GEs
			List<BixBite.GameEvent> SLGEvents = ((Tuple<int[,], List<BixBite.GameEvent>>)SLayer.LayerObjects).Item2;
			int[,] GETileData = ((Tuple<int[,], List<BixBite.GameEvent>>)SLayer.LayerObjects).Item1;

			int DesGroup = GETileData[cellx, celly];
			if(group == DesGroup)
			{
				foreach(BixBite.GameEvent ge in SLGEvents)
				{
					if((int)ge.GetProperty("group") == group)
					{
						return ge;
					}
				}
			}
			return null;
		}

		
		/// <summary>
		/// Takes in a tile, and a mouse position. Determines if the mouse is within the tiles bounds.
		/// </summary>
		/// <returns>true if the mouse location is within a given range</returns>
		public static bool inTile(Canvas backcanvas, Rectangle tile, int x, int y, int xoff, int yoff)
		{
			int left = (int)(Canvas.GetLeft(tile) * backcanvas.RenderTransform.Value.M11);
			int right = left + (int)(tile.ActualWidth * backcanvas.RenderTransform.Value.M11);
			int top = (int)(Canvas.GetTop(tile) * backcanvas.RenderTransform.Value.M11);
			int bottom = top + (int)(tile.ActualHeight * backcanvas.RenderTransform.Value.M11);

			//the get left works off the viewport, not the canvas as a whole. So wee need to do the offset math IF veiwport > 0
			left -= xoff; right -= xoff;
			top -= yoff; bottom -= yoff;
			x -= xoff; y -= yoff;

			if (x >= left && x < right)
			{
				if (y >= top && y < bottom )
					return true;
			}
			return false;
		}

		/// <summary>
		///	Determines if we are clicking in an already created content control/ Sprite object on the canvas
		/// </summary>
		/// <param name="backcanvas"></param>
		/// <param name="bor"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static bool inContentControl(Canvas backcanvas, ContentControl bor, int x, int y)
		{
			int left = (int)(Canvas.GetLeft(bor) * backcanvas.RenderTransform.Value.M11);
			int right = left + (int)(bor.ActualWidth * backcanvas.RenderTransform.Value.M11);
			int top = (int)(Canvas.GetTop(bor) * backcanvas.RenderTransform.Value.M11);
			int bottom = top + (int)(bor.ActualHeight * backcanvas.RenderTransform.Value.M11);

			//the get left works off the viewport, not the canvas as a whole. So wee need to do the offset math IF veiwport > 0
			left -= xoff; right -= xoff;
			top -= yoff; bottom -= yoff;
			x -= xoff; y -= yoff;


			if (x >= left && x < right)
			{
				if (y >= top && y < bottom)
					return true;
			}
			return false;
		}

		public static bool inBorder(Canvas backcanvas, Border bor, int x, int y)
		{
			int left = (int)(Canvas.GetLeft(bor) * backcanvas.RenderTransform.Value.M11);
			int right = left + (int)(bor.ActualWidth * backcanvas.RenderTransform.Value.M11);
			int top = (int)(Canvas.GetTop(bor) * backcanvas.RenderTransform.Value.M11);
			int bottom = top + (int)(bor.ActualHeight * backcanvas.RenderTransform.Value.M11);

			//the get left works off the viewport, not the canvas as a whole. So wee need to do the offset math IF veiwport > 0
			left -= xoff; right -= xoff;
			top -= yoff; bottom -= yoff;
			x -= xoff; y -= yoff;


			if (x >= left && x < right)
			{
				if (y >= top && y < bottom)
					return true;
			}
			return false;
		}

	}
}
