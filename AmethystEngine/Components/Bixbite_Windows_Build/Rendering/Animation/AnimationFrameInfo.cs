using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BixBite.Rendering.Animation
{
	public class AnimationFrameInfo
	{
		#region Delegates

		#endregion

		#region Properties

		public int OriginPointOffsetX { get; set; }
		public int OriginPointOffsetY { get; set; }
		public int RenderPointOffsetX { get; set; }
		public int RenderPointOffsetY { get; set; }

		#endregion

		#region fields
		private Rectangle _drawRectangle = new Rectangle();

		private int SheetPosX { get; set; }
		private int SheetPosY { get; set; }
		private int Width { get; set; }
		private int Height { get; set; }
		#endregion

		#region constructors

		#endregion

		#region methods

		public Rectangle GetDrawRectangle()
		{
			return _drawRectangle;
		}

		public void SetSheetPosition(int x, int y)
		{
			Rectangle newRectangle =  new Rectangle(x,y , Width, Height);
			_drawRectangle = newRectangle;
		}

		public void SetSheetSize(int width, int height)
		{
			Rectangle newRectangle = new Rectangle(SheetPosX, SheetPosY, width, height);
			_drawRectangle = newRectangle;
		}

		public void SetRectangle(Rectangle newRectangle)
		{
			_drawRectangle = newRectangle;
		}


		#endregion

		#region monogame

		#endregion
	}
}
