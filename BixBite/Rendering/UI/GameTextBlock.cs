using BixBite.Resources;
using System;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Rendering.UI
{
	public class GameTextBlock : GameUI, IProperties
	{


		#region Fields

		public SpriteFont _font;

		private String TexturePath
		{
			get { return GetPropertyData("Image").ToString(); }
		}

		private Texture2D _texture { get; set; }

		#endregion

		#region Properties


		public bool bMiddleHorizontal = false;
		public bool bMiddleVertical = false;

		public event EventHandler Click;

		public Color PenColour { get; set; }

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


		public String TBName { get; set; } 

		public GameTextBlock(string UIName, int Width, int Height, int xoff, int yoff, int Zindex ,string BackgroundPath = "#00000000", String Text= "") 
			: base(UIName, Width, Height, Zindex ,BackgroundPath)
		{
			TBName = UIName;
			AddProperty("Xoffset", xoff);
			AddProperty("YOffset", yoff);
			AddProperty("ContentText", Text);
			AddProperty("Font", "Ariel");
			AddProperty("FontSize", 24);
			AddProperty("FontColor", "Black");
			AddProperty("FontStyle", "Normal");
			AddProperty("TextSpeed", 1.0);
			AddProperty("TextTime", 1.0);//number of seconds to "type" the text to the screen
			AddProperty("Image", "");

		}

		public override void PropertyCallback(object sender, RoutedEventArgs e)
		{

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
			if(PenColour == null)
				PenColour = Color.White;
			//Text = "test";
			//base.Draw(gameTime, spriteBatch);
			

			if (_texture != null)
				spriteBatch.Draw(_texture, Rectangle, Color.White);

			float x, y;

			if (!string.IsNullOrEmpty(Text))
			{
				if (bMiddleHorizontal)
					x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
				else
					x = Rectangle.X;
				if (bMiddleVertical)
					y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2);
				else
					y = Rectangle.Y;

				spriteBatch.DrawString(_font, Text, new Vector2(x, y), PenColour);
				
			}

		}
	}
}
