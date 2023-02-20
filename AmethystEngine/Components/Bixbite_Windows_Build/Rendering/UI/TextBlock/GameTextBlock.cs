using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Rendering.UI.TextBlock
{
	public class GameTextBlock : BaseTextBlock
	{

		#region Delegates


		#endregion

		#region Fields

		private SpriteFont _font;
		private Vector2 _position = new Vector2();
		private Rectangle _drawRectangle = Rectangle.Empty;

		private Texture2D _texture { get; set; }

		private String TexturePath
		{
			get { return GetPropertyData("BackgroundImage").ToString(); }
		}

		private float _scale => float.Parse(GetPropertyData("FontSize").ToString()) / 12.0f;

		#endregion

		#region Properties
		public GraphicsDevice graphicsDevice;
		public Color TextColor { get; set; }

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

		public float Rotation
		{
			get => float.Parse(GetPropertyData("Rotation").ToString());
			set => SetProperty("Rotation", value);
		}

		public string Text
		{
			get => GetPropertyData("Text").ToString();
			set => SetProperty("Text", value);
		}

		#endregion

		#region Contructors

		public GameTextBlock(string UIName, int xPos, int yPos, int width, int height, int zindex, bool border,
			int xOff, int yOff, string text, float textTime, string backColor, string backImage, SpriteFont font,
			Texture2D texture, Color textColor)
			: base(UIName, xPos, yPos, width, height, zindex, border, xOff, yOff, text, textTime, backColor, backImage)
		{
			AddProperty("Rotation", 0.0f);
			AddProperty("Text", Text);
			AddProperty("Font", "Ariel");
			AddProperty("FontSize", 24);
			AddProperty("FontColor", "Black");
			AddProperty("FontStyle", "Normal");
			AddProperty("TextSpeed", 1.0);
			AddProperty("TextTime", 1.0);//number of seconds to "type" the text to the screen

			_texture = texture;
			_font = font;
			TextColor = textColor;
			Text = text;
		}



		#endregion

		#region Methods

		public void SetUITexture()
		{
			using (var stream = new System.IO.FileStream(TexturePath, System.IO.FileMode.Open))
			{
				_texture = (Texture2D.FromStream(graphicsDevice, stream));
			}
		}


		public void Update(GameTime gameTime)
		{
			if (!bIsActive) return;
			//base.Update(gameTime);
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if (!bIsActive) return;
			if (TextColor == null)
				TextColor = Color.White;
			//Text = "test";
			//base.Draw(gameTime, spriteBatch);


			if (_texture != null)
				spriteBatch.Draw(_texture, DrawRectangle, Color.White);

			float x, y;

			if (!string.IsNullOrEmpty(Text))
			{
				if (bMiddleHorizontal)
					x = (DrawRectangle.X + (DrawRectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
				else
					x = DrawRectangle.X;
				if (bMiddleVertical)
					y = (DrawRectangle.Y + (DrawRectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2);
				else
					y = DrawRectangle.Y;

				if (_scale == 1.0f && Rotation == 0.0f)
					spriteBatch.DrawString(_font, Text, new Vector2(x, y), TextColor);
				else
					spriteBatch.DrawString(_font, Text, new Vector2(x, y), TextColor, Rotation, Vector2.Zero, 
						new Vector2(_scale, _scale ), SpriteEffects.None,0);
				
			}

			#endregion

		}

	}
}
