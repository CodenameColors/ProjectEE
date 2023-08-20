
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using BixBite.Combat;
using BixBite.Rendering;
using BixBite.Rendering.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Characters
{
	public class BaseEntity 
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
				if(_spriteAnimationStatemachine != null)
					return new Vector2(_spriteAnimationStatemachine.DrawRectangle.X, _spriteAnimationStatemachine.DrawRectangle.Y);
				else return Vector2.Zero;
			}
			set
			{
				_screenPosition = value;
				_spriteAnimationStatemachine?.SetScreenPosition((int)value.X, (int)value.Y);
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

		#region Modifiers
		public List<ModifierData> StatusEffectModifer_List = new List<ModifierData>();

		#endregion
		public List<Tweening.Tweening> interpolationMovement = new List<Tweening.Tweening>();

		private AnimationStateMachine _spriteAnimationStatemachine;

		public BaseEntity()
		{
			//_spriteAnimationStatemachine = new SpriteSheet();
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
				this._spriteAnimationStatemachine.SetScreenPosition(x, null);
			}
			if (y != null)
			{
				this._screenPosition.Y = (int)y;
				this._spriteAnimationStatemachine.SetScreenPosition(null, y);
			}

			

		}

		public AnimationStateMachine GetAnimationStateMachine()
		{
			return _spriteAnimationStatemachine;
		}

		/// <summary>
		/// Loads the SpriteSheet into memory. And sets up the texture of the sprite sheet
		/// </summary>
		/// <param name="sheet"></param>
		/// <param cm="Conent Manager IF the texture is set in the monogame pipleine tool"></param>
		public void LoadSpriteSheet(Spritesheet sheet, ContentManager cm, String contentName)
		{
		//	sheet.SetTexture(cm.Load<Texture2D>(contentName));
		//	this._spriteAnimationStatemachine = sheet;

		//	//load the parameters
		//	this.Width = _spriteAnimationStatemachine.CurrentAnimation.GetFrameWidth();
		//	this.Height = _spriteAnimationStatemachine.CurrentAnimation.GetFrameHeight();

		}

		/// <summary>
		/// Loads the SpriteSheet into memory. And sets up the texture of the sprite sheet
		/// </summary>
		/// <param name="sheet"></param>
		public bool LoadSpriteSheet(Spritesheet sheet, GraphicsDevice graphicsDevice, String imagepath)
		{
			//if (File.Exists(sheet.SpriteSheetPath))
			//	sheet.SetTexture(sheet.SpriteSheetPath, graphicsDevice);
			//else return false;


			//this._spriteAnimationStatemachine = sheet;

			////load the parameters
			//this.Width = _spriteAnimationStatemachine.CurrentAnimation.GetFrameWidth();
			//this.Height = _spriteAnimationStatemachine.CurrentAnimation.GetFrameHeight();
			//return true;

			return false;
		}
		 
		/// <summary>
		/// This sets the base scale factor, and resizes ALL animations by it.
		/// </summary>
		/// <param name="sx"></param>
		/// <param name="sy"></param>
		public void SetBaseScaling(float sx, float sy)
		{
			_spriteAnimationStatemachine.ScaleX = sx;
			_spriteAnimationStatemachine.ScaleY = sy;
		}


		//public void SetSize(int w = 0, int h = 0)
		//{
		//	if (_spriteAnimationStatemachine?.CurrentAnimation != null)
		//	{
		//		this.Width = _spriteAnimationStatemachine.CurrentAnimation.GetFrameWidth();
		//		this.Height = _spriteAnimationStatemachine.CurrentAnimation.GetFrameHeight();
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

		public void AddInterpolationMovement(Tweening.Tweening tween)
		{
			interpolationMovement.Add(tween);
		}


		public virtual void Update(GameTime gameTime)
		{
			_spriteAnimationStatemachine?.Update(gameTime);

			for (int i = interpolationMovement.Count - 1; i >= 0; i--)
			{
				if (interpolationMovement[i].bIsDone)
					interpolationMovement.Remove(interpolationMovement[i]);
				else
					interpolationMovement[i].Update(gameTime);
			}
		}

		public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if (_spriteAnimationStatemachine == null)
			{
				//spriteBatch.Draw(Texture, new Vector2((int)Position.X, (int)Position.Y), new Rectangle(0, 0, Width, Height),
				//	Color.White, 0.0f, Vector2.Zero, new Vector2(ScaleX, ScaleY), SpriteEffects.None, 0);

			}
			else
			{
				foreach (Animation animationLayer in _spriteAnimationStatemachine.CurrentState.AnimationLayers)
				{
					animationLayer.Draw(spriteBatch);
				}
			}
		}

	}

	/// <summary>
	/// Interface used to define all Combat required methods that every type of battle entity needs to be able to do
	/// </summary>
	public interface ICombat
	{
		/// <summary>
		/// This function is here to handle stats that need to change on the loading of a battle entity 
		/// in the combat system. Simple things, like adding and subtracting stats, status effects. etc
		/// </summary>
		void Initialize();

		/// <summary>
		/// Method used to find out whether a battle entity can attack the target, or even at all
		/// </summary>
		/// <returns></returns>
		bool CanAttack();

		/// <summary>
		/// This function is here to handle all things that need to be done when a battle entity attacks
		/// Examples include stealing stats, stealing money, subtracting ammo/charge count, other stat changes etc.
		/// THIS means the attacking entity NOT the target entity
		/// </summary>
		void Attack();

		/// <summary>
		/// Handles all the stat changes that are needed when an entity chooses to defend
		/// </summary>
		void Defend();

		/// <summary>
		/// Handles all the required changes that need to be done when an entity gets hit.
		/// Stat changes, take damage, maybe deal damage (thorns), etc
		/// </summary>
		void GotHit(BattleEntity attackingBattleEntity);


	}
}
