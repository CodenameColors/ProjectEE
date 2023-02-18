using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BixBite.Rendering.UI.Button
{
	public class GameButton : BaseButtonUI
	{
		#region Delegates
		public delegate void Button_OnClick(List<object> paramsList);
		public Button_OnClick Button_OnClick_Dele = null;
		#endregion

		#region Fields
		private List<object> _paramList = new List<object>();

		private MouseState _previousMouse;
		private MouseState _currentMouse;

		private SpriteFont _font;
		
		private Texture2D _texture;

		private Vector2 _position = new Vector2();
		private Rectangle _drawRectangle = Rectangle.Empty;
		private Rectangle _mouseRectangle = Rectangle.Empty;
		#endregion

		#region Properties
		public Color TextColor { get; set; }

		//public System.Windows.Media.Color FontColor
		//{
		//	get => (GetPropertyData("FontColor").ToString());
		//	set => SetProperty("FontColor", value);
		//}

		public Vector2 Position
		{
			get
			{
				_position.X = XPos;
				_position.Y = YPos;
				return _position;
			}
		}


		public float Rotation
		{
			get => float.Parse(GetPropertyData("Rotation").ToString());
			set => SetProperty("Rotation", value);
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
		public GameButton(string UIName, int xPos, int yPos, int width, int height, int zindex, bool bBorder,
			int xOff, int yOff, String buttonText, string backColor, string backImage,
			Texture2D texture, SpriteFont font, Color color )
			: base(UIName, xPos, yPos, width, height, zindex, bBorder, xOff, yOff, buttonText, backColor, backImage)
		{
			AddProperty("Rotation", 0.0f);


			_texture = texture;
			_font = font;
			TextColor = color;
			ButtonText = buttonText;


			_paramList.Add(this);
		}
		#endregion

		#region Methods

		/// <summary>
		/// New list of params to pass to the On Click Delegate. NOTE it AUTO ADDS the UI as the first element of the list
		/// </summary>
		/// <param name="paramList">Used to send to the delegate. Example Data being set to a new scene, or different UI to load</param>
		public void LoadParamList(List<object> paramList)
		{
			paramList.Insert(0, this);
			this._paramList = paramList;
		}

		public void Update(GameTime gameTime)
		{
			if(!bIsActive) return;

			_previousMouse = _currentMouse;
			_currentMouse = Mouse.GetState();

			_mouseRectangle.X = _currentMouse.X;
			_mouseRectangle.Y = _currentMouse.Y;
			_mouseRectangle.Width = 1;
			_mouseRectangle.Height = 1;


			_isHovering = false;

			if (_mouseRectangle.Intersects(DrawRectangle))
			{
				_isHovering = true;

				if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
				{
					Button_OnClick_Dele?.Invoke(_paramList);
				}
			}
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if (!bIsActive) return;
			var colour = Color.White;

			if (_isHovering)
				colour = Color.Gray;

			if (Rotation == 0.0f)
				spriteBatch.Draw(_texture, DrawRectangle, colour);
			else
				spriteBatch.Draw(_texture, Position, DrawRectangle, colour, Rotation,
					Vector2.Zero, Vector2.One , SpriteEffects.None, 1);
			

			if (!string.IsNullOrEmpty(ButtonText))
			{
				var x = (XPos + (Width / 2)) - (_font.MeasureString(ButtonText).X / 2);
				var y = (YPos + (Height / 2)) - (_font.MeasureString(ButtonText).Y / 2);

				if(Rotation == 0.0f)
					spriteBatch.DrawString(_font, ButtonText, new Vector2(x, y), TextColor);
				else
					spriteBatch.DrawString(_font, ButtonText, new Vector2(x, y), colour, Rotation,
						Vector2.Zero, Vector2.One, SpriteEffects.None, 1);
			}
		}

		#endregion

	}
}
