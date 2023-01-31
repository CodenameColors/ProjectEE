using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using BixBite.Rendering.Helpers;
using BixBite.Rendering.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BixBite.Rendering.UI.Checkbox
{
	public class GameCheckBox : BaseCheckBoxUI
	{
		#region Delegates
		public delegate void CheckBox_OnCheck(List<object> paramsList);
		public CheckBox_OnCheck CheckBox_OnCheck_Dele = null;

		public delegate void CheckBox_OnEnter(List<object> paramsList);
		public CheckBox_OnEnter CheckBox_OnEnter_Dele = null;

		public delegate void CheckBox_OnExit(List<object> paramsList);
		public CheckBox_OnExit CheckBox_OnExit_Dele = null;
		#endregion

		#region Fields
		private List<object> _paramList = new List<object>();

		private MouseState _previousMouse;
		private MouseState _currentMouse;

		private SpriteFont _font;
		private Texture2D _texture;
		private Texture2D _checkTexture;

		private Vector2 _position = new Vector2();
		private Rectangle _drawRectangle = Rectangle.Empty;
		private Rectangle _mouseRectangle = Rectangle.Empty;
		private  Color _innerCheckBoxColor = Color.Gray;

		private bool _bHasEntered = false;
		private bool _isChecked = false;
		private UInt16 _innerCheckBoxWidth = 20;
		private UInt16 _innerCheckBoxHeight = 20;
		#endregion

		#region Properties
		public Color TextColor { get; set; }

		//public System.Windows.Media.Color FontColor
		//{
		//	get => (GetPropertyData("FontColor").ToString());
		//	set => SetProperty("FontColor", value);
		//}

		public bool bIsChecked
		{
			get => _isChecked;
			set => _isChecked = value;
		}

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
		/// Where this CheckBox is drawn on the screen.  XPos, YPos, Width, Height
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

		/// <summary>
		/// Where this CheckBox inner button on the screen?  XPos, YPos, Width, Height
		/// </summary>
		public Rectangle Checkbox_DrawRectangle
		{
			get
			{
				_drawRectangle.X = XPos + XOffset;
				_drawRectangle.Y = YPos + (Height / 2) - (_innerCheckBoxHeight/2);
				_drawRectangle.Width = _innerCheckBoxWidth;
				_drawRectangle.Height = _innerCheckBoxHeight;
				return _drawRectangle;
			}
		}
		#endregion

		#region Contructors
		public GameCheckBox(string UIName, int xPos, int yPos, int width, int height, int zindex, bool bBorder,
			GraphicsDevice graphicsDevice, int xOff, int yOff, String checkBoxContentText, string backColor, string backImage,
			SpriteFont font, Color color)
			: base(UIName, xPos, yPos, xOff, yOff, width, height, zindex, bBorder, checkBoxContentText, backColor, backImage)
		{
			AddProperty("Rotation", 0.0f);

			_texture = new Texture2D(graphicsDevice, Checkbox_DrawRectangle.Width, Checkbox_DrawRectangle.Height);
			_texture.CreateBorder(2, Color.Black);
			_texture.CreateFilledRectangle(2, Color.White);

			_font = font;
			TextColor = color;
			CheckBoxContentText = checkBoxContentText;

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

		public void SetCheckTexture(Texture2D checkTexture2D)
		{
			_checkTexture = checkTexture2D;
		}

		public void Update(GameTime gameTime)
		{
			if (!bIsActive) return;

			_previousMouse = _currentMouse;
			_currentMouse = Mouse.GetState();

			_mouseRectangle.X = _currentMouse.X;
			_mouseRectangle.Y = _currentMouse.Y;
			_mouseRectangle.Width = 1;
			_mouseRectangle.Height = 1;


			_isHoveringBox = false;

			// On Hover Event
			if (_mouseRectangle.Intersects(DrawRectangle))
			{
					_isHoveringBox = true;
					//CheckBox_OnCheck_Dele?.Invoke(_paramList);
			}

			// On Enter Event
			if (!_bHasEntered && _mouseRectangle.Intersects(DrawRectangle))
			{
				CheckBox_OnEnter_Dele?.Invoke(_paramList);
				_innerCheckBoxColor = Color.LightSlateGray;
				_bHasEntered = true;

			}

			// On Exit Event
			if (_bHasEntered && !_mouseRectangle.Intersects(DrawRectangle))
			{
				CheckBox_OnExit_Dele?.Invoke(_paramList);
				_innerCheckBoxColor = Color.Gray;
				_bHasEntered = false;

			}

			if (_isHoveringBox)
			{
				if (_currentMouse.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released)
				{
					if (!_isChecked)
					{
						CheckBox_OnCheck_Dele?.Invoke(_paramList);
						_isChecked = true;
					}
					else
					{
						_isChecked = false;
					}
				}
				
			}

			_previousMouse = _currentMouse;
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if (!bIsActive) return;


			// Draw the small box where we display the check
			spriteBatch.Draw(_texture, Checkbox_DrawRectangle, _innerCheckBoxColor);
			if(_checkTexture != null && _isChecked)
				spriteBatch.Draw(_checkTexture, Checkbox_DrawRectangle, Color.White);

			if (!string.IsNullOrEmpty(CheckBoxContentText))
			{
				var x = (XPos + (Width / 2)) - (_font.MeasureString(CheckBoxContentText).X / 2)  + Checkbox_DrawRectangle.Width; // Add Padding
				var y = (YPos + (Height / 2)) - (_font.MeasureString(CheckBoxContentText).Y / 2);

				if (Rotation == 0.0f)
					spriteBatch.DrawString(_font, CheckBoxContentText, new Vector2(x, y), TextColor);
				else
					spriteBatch.DrawString(_font, CheckBoxContentText, new Vector2(x, y), TextColor, Rotation,
						Vector2.Zero, Vector2.One, SpriteEffects.None, 1);
			}
		}

		#endregion

	}
}
