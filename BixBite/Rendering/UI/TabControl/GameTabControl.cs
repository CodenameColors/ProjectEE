using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BixBite.Rendering.UI.Checkbox;
using BixBite.Rendering.UI.ListBox;
using BixBite.Rendering.UI.ProgressBar;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BixBite.Rendering.UI.TabControl
{
	public class GameTabControl : BaseTabControl
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
		private bool _isHovering;
		private Texture2D _texture;

		private Vector2 _position = new Vector2();
		private Rectangle _drawRectangle = Rectangle.Empty;
		private Rectangle _mouseRectangle = Rectangle.Empty;

		private List<Texture2D> _tabImages = new List<Texture2D>();
		private List<Rectangle> _tabDrawRectangles = new List<Rectangle>();
		private List<String> _tabDrawString = new List<String>();
		private  List<bool> _bIsTabHovering = new List<bool>();

		private int _selectedTabIndex = -1;
		private Dictionary<BaseUI, int> _uiComponentDictionary = new Dictionary<BaseUI, int>();
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

		public int TabHeight
		{
			get => int.Parse(GetPropertyData("TabHeight").ToString());
			set => SetProperty("TabHeight", value);
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

		public GameTabControl(string UIName, int xPos, int yPos, int xOff, int yOff, int width, int height, int tabHeight, int zindex, bool bBorder, String backColor, String backImage, Texture2D borderTexture, SpriteFont sf) :
			base(UIName, xPos, yPos, xOff, yOff, width, height, zindex, bBorder, backColor, backImage)
		{
			AddProperty("Rotation", 0.0f);
			AddProperty("TabHeight", tabHeight);

			_font = sf;
			_texture = borderTexture;
		}

		public void AddNewTab(String tabText, Texture2D tabTexture2D)
		{
			// how long is the text?
			int width = (int) (_font.MeasureString(tabText).X);
			if (width < TabHeight || width > TabHeight) width = TabHeight;
			if(_tabDrawRectangles.Count == 0)
			{
				_tabDrawRectangles.Add(new Rectangle(DrawRectangle.X, DrawRectangle.Y, width, TabHeight));
				_tabImages.Add(tabTexture2D);
				_tabDrawString.Add(tabText);
				_bIsTabHovering.Add(false);
				_selectedTabIndex = 0;
			}
			else
			{
				_tabDrawRectangles.Add(new Rectangle(
					_tabDrawRectangles.Last().X + _tabDrawRectangles.Last().Width + 2, DrawRectangle.Y, width, TabHeight)); //2 for padding
				_tabImages.Add(tabTexture2D);
				_tabDrawString.Add(tabText);
				_bIsTabHovering.Add(false);
			}
		}

		public void AddUIElement(int tabNum, BaseUI ui)
		{
			ui.XPos += DrawRectangle.X;
			ui.YPos += DrawRectangle.Y;
			_uiComponentDictionary.Add(ui, tabNum);
		}

		public void ChangeSelectedTab(int selectedIndex)
		{
			_selectedTabIndex = selectedIndex;
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

			_isHovering = false;

			if (_mouseRectangle.Intersects(DrawRectangle))
			{
				_isHovering = true;

				if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
				{
					Button_OnClick_Dele?.Invoke(_paramList);
				}
			}

			for (int i = 0; i < _tabDrawRectangles.Count; i++)
			{
				if (_mouseRectangle.Intersects(_tabDrawRectangles[i]))
				{
					_bIsTabHovering[i] = true;

					if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
					{
						Button_OnClick_Dele?.Invoke(_paramList);
						_selectedTabIndex = i;
					}
				}
				else _bIsTabHovering[i] = false;
			}

			// DRAW the current tabs ui
			foreach (var ui in _uiComponentDictionary.Where(kvp => kvp.Value == _selectedTabIndex))
			{
				switch (ui.Key)
				{
					case Button.GameButton gameButton:
						gameButton.Update(gameTime);
						break;
					case GameCheckBox gameCheckBox:
						gameCheckBox.Update(gameTime);
						break;
					case Image.GameImage gameImage:
						gameImage.Update(gameTime);
						break;
					case GameCustomListBox gameCustomListBox:
						gameCustomListBox.Update(gameTime);
						break;
					case ListBox.GameListBox gameListBox:
						gameListBox.Update(gameTime);
						break;
					case ListBox.ListBoxItems.GameListBoxItem gameListBoxItem:
						gameListBoxItem.Update(gameTime);
						break;
					case GameCustomProgressBar gameCustomProgressBar:
						gameCustomProgressBar.Update(gameTime);
						break;
					case ProgressBar.GameProgressBar gameProgressBar:
						gameProgressBar.Update(gameTime);
						break;
					case GameTabControl gameTabControl:
						gameTabControl.Update(gameTime);
						break;
					case TextBlock.GameTextBlock gameTextBlock:
						gameTextBlock.Update(gameTime);
						break;
					default:
						throw new ArgumentOutOfRangeException();
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
					Vector2.Zero, Vector2.One, SpriteEffects.None, 1);

			for (int i = 0; i < _tabImages.Count; i++)
			{
				Color innerColor = Color.White;
				if (_bIsTabHovering[i])
					innerColor = Color.Gray;

				if (Rotation == 0.0f)
				{
					spriteBatch.DrawString(_font, _tabDrawString[i], new Vector2(_tabDrawRectangles[i].X, _tabDrawRectangles[i].Y), TextColor);
					spriteBatch.Draw(_tabImages[i], _tabDrawRectangles[i], innerColor);
				}
				else
				{
					spriteBatch.DrawString(_font, _tabDrawString[i], new Vector2(_tabDrawRectangles[i].X, _tabDrawRectangles[i].Y), TextColor, Rotation,
						Vector2.Zero, Vector2.One, SpriteEffects.None, 1);
				}
			}

			// DRAW the current tabs ui
			foreach (var ui in _uiComponentDictionary.Where(kvp => kvp.Value == _selectedTabIndex))
			{
				switch (ui.Key)
				{
					case Button.GameButton gameButton:
						gameButton.Draw(gameTime, spriteBatch);
						break;
					case GameCheckBox gameCheckBox:
						gameCheckBox.Draw(gameTime, spriteBatch);
						break;
					case Image.GameImage gameImage:
						gameImage.Draw(gameTime, spriteBatch);
						break;
					case GameCustomListBox gameCustomListBox:
						gameCustomListBox.Draw(gameTime, spriteBatch);
						break;
					case ListBox.GameListBox gameListBox:
						gameListBox.Draw(gameTime, spriteBatch);
						break;
					case ListBox.ListBoxItems.GameListBoxItem gameListBoxItem:
						gameListBoxItem.Draw(gameTime, spriteBatch);
						break;
					case GameCustomProgressBar gameCustomProgressBar:
						gameCustomProgressBar.Draw(gameTime, spriteBatch);
						break;
					case ProgressBar.GameProgressBar gameProgressBar:
						gameProgressBar.Draw(gameTime, spriteBatch);
						break;
					case GameTabControl gameTabControl:
						gameTabControl.Draw(gameTime, spriteBatch);
						break;
					case TextBlock.GameTextBlock gameTextBlock:
						gameTextBlock.Draw(gameTime, spriteBatch);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

		}

	}
}
