﻿using BixBite.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Rendering.UI
{
	public class GameIMG : GameUI, IProperties
	{
		#region Fields
		
		private String TexturePath
		{
			get { return GetPropertyData("Image").ToString(); }
		}
		private Texture2D _texture;

		#endregion

		#region Properties

		public Vector2 Position { get; set; }

		public Rectangle Rectangle
		{
			get
			{
				return new Rectangle(
					(int)Position.X + (int)GetPropertyData("Xoffset"), 
					(int)Position.Y + (int)GetPropertyData("YOffset"), 
					(int)GetPropertyData("Width"), (int)GetPropertyData("Height"));
			}
		}

		public string Text
		{
			get => GetPropertyData("ContentText").ToString();
			set => SetProperty("ContentText", value);
		}
		public GraphicsDevice graphicsDevice;
		#endregion
		public GameIMG(string UIName, int Width, int Height, int Zindex, int xoff, int yoff, String ImagePath = "", String BackgroundPath = "#00000000") : base(UIName, Width, Height, Zindex, BackgroundPath)
		{
			AddProperty("Xoffset", xoff);
			AddProperty("YOffset", yoff);
			AddProperty("Image", ImagePath);
		}

		public override void SetUITexture()
		{
			using (var stream = new System.IO.FileStream(TexturePath, System.IO.FileMode.Open))
			{
				_texture = (Texture2D.FromStream(graphicsDevice, stream));
			}
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if (_texture != null)
				spriteBatch.Draw(_texture, Rectangle, Color.White);
		}

	}
}
