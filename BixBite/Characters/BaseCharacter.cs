
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using BixBite.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Characters
{
	public class BaseCharacter
	{
		

		//public Texture2D Texture;
		public Vector2 Velocity = new Vector2();

		private float _scaleX = 1.0f;
		public virtual float ScaleX
		{
			get => _scaleX;
			set => _scaleX = value;
		}

		private float _scaleY = 1.0f;
		public virtual float ScaleY
		{
			get => _scaleY;
			set => _scaleY = value;
		}


		public virtual int Width { get; set; }
		public virtual int Height { get; set; }

		public Rectangle DrawPosRectangle = new Rectangle();

		public int ShaderHitActuatTimer = 0;
		public int ShaderHitTimer = 0;
		public int ShaderHitFlash = 0;

		private Vector2 _screenPosition = Vector2.Zero;
		public Vector2 Position
		{
			get
			{
				if(_spriteAnimationSheet != null) return _spriteAnimationSheet.CurrentAnimation.GetScreenPosition();
				else return Vector2.Zero;
			}
			set
			{
				_screenPosition = value;
				_spriteAnimationSheet?.CurrentAnimation.SetScreenPosition((int)value.X, (int)value.Y);
			}
		}
		private Vector2 _spawnPosition = new Vector2();

		public Vector2 SpawnPosition
		{
			get => _spawnPosition;
			set
			{
				_spawnPosition = value;
				if(Position == Vector2.Zero)
					Position = value;
			}
		}

		public bool bHasJumped;
		public int Zindex = 1;

		private SpriteSheet _spriteAnimationSheet;

		public BaseCharacter()
		{
			//_spriteAnimationSheet = new SpriteSheet();
		}

		//public void SetSpawnPosition(int x = 0, int y = 0)
		//{
		//	this.SpawnPosition = new Vector2(x,y);
		//}

		public void SetPosition(int? x, int? y )
		{
			if (x != null)
			{
				this._screenPosition.X = (int)x;
				this._spriteAnimationSheet.CurrentAnimation.SetScreenPosition(x, null);
			}
			if (y != null)
			{
				this._screenPosition.Y = (int)y;
				this._spriteAnimationSheet.CurrentAnimation.SetScreenPosition(null, y);
			}

			

		}

		public SpriteSheet GetSpriteSheet()
		{
			return _spriteAnimationSheet;
		}

		/// <summary>
		/// Loads the SpriteSheet into memory. And sets up the texture of the sprite sheet
		/// </summary>
		/// <param name="sheet"></param>
		/// <param cm="Conent Manager IF the texture is set in the monogame pipleine tool"></param>
		public void LoadSpriteSheet(SpriteSheet sheet, ContentManager cm, String contentName)
		{
			sheet.SetTexture(cm.Load<Texture2D>(contentName));
			this._spriteAnimationSheet = sheet;

			//load the parameters
			this.Width = _spriteAnimationSheet.CurrentAnimation.GetFrameWidth();
			this.Height = _spriteAnimationSheet.CurrentAnimation.GetFrameHeight();

		}


		/// <summary>
		/// Loads the SpriteSheet into memory. And sets up the texture of the sprite sheet
		/// </summary>
		/// <param name="sheet"></param>
		public bool LoadSpriteSheet(SpriteSheet sheet, GraphicsDevice graphicsDevice, String imagepath)
		{
			if (File.Exists(sheet.ImgPathLocation))
				sheet.SetTexture(sheet.ImgPathLocation, graphicsDevice);
			else return false;


			this._spriteAnimationSheet = sheet;

			//load the parameters
			this.Width = _spriteAnimationSheet.CurrentAnimation.GetFrameWidth();
			this.Height = _spriteAnimationSheet.CurrentAnimation.GetFrameHeight();
			return true;
		}
		 
		/// <summary>
		/// This sets the base scale factor, and resizes ALL animations by it.
		/// </summary>
		/// <param name="sx"></param>
		/// <param name="sy"></param>
		public void SetBaseScaling(float sx, float sy)
		{
			foreach (SpriteAnimation sa in _spriteAnimationSheet.SpriteAnimations.Values)
			{
				sa.ScalarX = sx;
				sa.ScalarY = sy;
			}
		}


		//public void SetSize(int w = 0, int h = 0)
		//{
		//	if (_spriteAnimationSheet?.CurrentAnimation != null)
		//	{
		//		this.Width = _spriteAnimationSheet.CurrentAnimation.GetFrameWidth();
		//		this.Height = _spriteAnimationSheet.CurrentAnimation.GetFrameHeight();
		//	}
		//	else
		//	{
		//		this.Width = w;
		//		this.Height = h;
		//	}
		//}

		public void Collision(Rectangle newRectangle, int xOffset, int yOffset, int EventGroup)
		{
			if (EventGroup != -1) return;

			if (DrawPosRectangle.TouchTopOf(newRectangle))
			{
				//Rectangle.Y = newRectangle.Y - Rectangle.Height;
				//Velocity.Y = 0f;
				bHasJumped = false;
				_screenPosition.Y = newRectangle.Y - DrawPosRectangle.Height - 2;
			}
			if (DrawPosRectangle.TouchLeftOf(newRectangle))
			{
				_screenPosition.X = newRectangle.X - DrawPosRectangle.Width - 2;
			}
			if (DrawPosRectangle.TouchRightOf(newRectangle))
			{
				_screenPosition.X = newRectangle.X + DrawPosRectangle.Width + 10;
			}
			if (DrawPosRectangle.TouchBottomOf(newRectangle))
			{
				_screenPosition.Y = newRectangle.Y + DrawPosRectangle.Height + 10;
			}

			//this is for checking the map. Don't allow out of bounds.
			if (Position.X < 0) _screenPosition.X = 0;
			if (Position.X > xOffset - DrawPosRectangle.Width) _screenPosition.X = xOffset - DrawPosRectangle.Width;
			if (Position.Y < 0) Velocity.Y = 1f;
			if (Position.Y > yOffset - DrawPosRectangle.Height) _screenPosition.Y = yOffset - DrawPosRectangle.Height;

		}

		public void SetHitFlashes(int newflashcount)
		{
			this.ShaderHitFlash = newflashcount;
		}

		public void SetHitTimer(int newhittimer)
		{
			this.ShaderHitTimer = newhittimer;
		}

		public virtual void Update(GameTime gameTime)
		{
			_spriteAnimationSheet?.Update(gameTime);
		}

		public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if (_spriteAnimationSheet == null)
			{
				//spriteBatch.Draw(Texture, new Vector2((int)Position.X, (int)Position.Y), new Rectangle(0, 0, Width, Height),
				//	Color.White, 0.0f, Vector2.Zero, new Vector2(ScaleX, ScaleY), SpriteEffects.None, 0);

			}
			else _spriteAnimationSheet?.Draw(spriteBatch);
		}

	}
}
