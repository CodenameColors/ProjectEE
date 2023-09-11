using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Rendering.Animation
{
	public class SpriteSheet
	{
		#region Delegates

		#endregion

		#region Properties
		public String SpriteSheetPath {get; set;}
		#endregion

		#region fields
		private Texture2D SpriteSheetImageTexture2D;

		#endregion

		#region constructors

		#endregion

		#region methods

		public Texture2D GetTexture2D()
		{
			if (SpriteSheetImageTexture2D == null)
			{
				return null;
			}
			else
			{
				return SpriteSheetImageTexture2D;
			}
		}
		#endregion

		#region monogame

		#endregion
	}
}
