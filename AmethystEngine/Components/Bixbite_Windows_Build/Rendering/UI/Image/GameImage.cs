using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BixBite.Combat;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Rendering.UI.Image
{
	public class GameImage : BaseImage
	{

		#region Delegates

		#endregion

		#region Fields
		private String TexturePath
		{
			get { return GetPropertyData("BackgroundImage").ToString(); }
		}
		private Texture2D _texture;
		private Vector2 _position = new Vector2();
		private Rectangle _drawRectangle = Rectangle.Empty;
		#endregion

		#region Properties
		public GraphicsDevice graphicsDevice;

		public Vector2 Position
		{
			get
			{
				_position.X = XPos;
				_position.Y = YPos;
				return _position;
			}
		}

		/// <summary>
		/// Where this Button is drawn on the screen.  XPos, YPos, Width, Height
		/// </summary>
		public Rectangle DrawRectangle
		{
			get
			{
				_drawRectangle.X = XPos + XOffset;
				_drawRectangle.Y = YPos + YOffset;
				_drawRectangle.Width = Width;
				_drawRectangle.Height = Height;
				return _drawRectangle;
			}
		}
		#endregion

		#region Contructors
		public GameImage(string UIName, int xPos, int yPos, int width, int height, int zindex, bool border, 
			int xOff, int yOff, string backImage, string backColor, Texture2D image = null) : 
			base(UIName, xPos, yPos, width, height, zindex, border, xOff, yOff, backImage, backColor)
		{
			_texture = image;
		}

		/// <summary>
		/// THIS IS ONLY FOR THE AMETHYST ENGINE IMPORTATION/EXPORT FILES
		/// </summary>
		/// <param name="UIName"></param>
		/// <param name="Width"></param>
		/// <param name="Height"></param>
		/// <param name="Zindex"></param>
		/// <param name="xoff"></param>
		/// <param name="yoff"></param>
		/// <param name="ImagePath"></param>
		/// <param name="graphicsDevice"></param>
		/// <param name="BackgroundPath"></param>
		public GameImage(string UIName, int xPos, int yPos, int Width, int Height, int Zindex, int xoff, int yoff, String ImagePath = "", GraphicsDevice graphicsDevice = null, String BackgroundPath = "#00000000") :
			base(UIName, xPos, yPos, Width, Height, Zindex, xoff, yoff, ImagePath, graphicsDevice, BackgroundPath)
		{
			AddProperty("Xoffset", xoff);
			AddProperty("YOffset", yoff);
			AddProperty("Image", ImagePath);
			this.graphicsDevice = graphicsDevice;
			if(graphicsDevice != null && ImagePath != "")
				SetUITexture();
		}


		#endregion

		#region Methods

		public Texture2D GetUiTexture2D()
		{
			return _texture;
		}

		public void SetUITexture()
		{

			if (graphicsDevice == null) return;

			using (var stream = new System.IO.FileStream(TexturePath, System.IO.FileMode.Open))
			{
				_texture = (Texture2D.FromStream(graphicsDevice, stream));
			}

		}

		public void SetUITexture(bool bUsePipeline = false)
		{

			if (graphicsDevice == null) return;

			if (bUsePipeline)
			{
				using (var stream = new System.IO.FileStream(TexturePath, System.IO.FileMode.Open))
				{
					_texture = (Texture2D.FromStream(graphicsDevice, stream));
				}
			}

		}

		public void SetUITexture(Texture2D text = null)
		{
			if ( text == null) return;
			_texture = text;
		}


		public void Update(GameTime gameTime)
		{
			if (!bIsActive) return;

			base.Update(gameTime);

		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if(!bIsActive) return;

			if (_texture != null)
				spriteBatch.Draw(_texture, DrawRectangle, Color.White * Transparency);
		}
		#endregion


	}
}
