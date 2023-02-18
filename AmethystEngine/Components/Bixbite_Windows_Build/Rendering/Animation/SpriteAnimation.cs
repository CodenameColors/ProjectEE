using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BixBite.Rendering.Animation;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Color = Microsoft.Xna.Framework.Color;


namespace BixBite.Rendering
{
	public class SpriteAnimation
	{

		#region Fields

		private String _name = "";

		public SpriteSheet ParentSheet;
		private Vector2 _startPosition = Vector2.Zero;
		
		private int _frameWidth;
		private int _frameHeight;
		private int _numberOfRows;
		private int _frameCount = 0;
		private int _currentFrame = 0;
		private int _fps = 0;
		
		private Rectangle _drawRectangle;

		private LinkedList<Vector2> _framePositions = new LinkedList<Vector2>();
		private List<AnimationEvent> _animationEvents = new List<AnimationEvent>();

		private SoundEffect _currentSoundEffect = null;
		#endregion

		#region Properties

		public String Name
		{
			get => _name;
			set => _name = value;
		}
		public int StartXPos
		{
			get => (int)_startPosition.X;
			set => _startPosition.X = value;
		}
		public int StartYPos
		{
			get => (int)_startPosition.Y;
			set => _startPosition.Y = value;
		}

		public int FrameWidth
		{
			get => _frameWidth;
			set => _frameWidth = value;
		}
		public int FrameHeight
		{
			get => _frameHeight;
			set => _frameHeight = value;
		}

		public int FrameCount
		{
			get => _frameCount;
			set => _frameCount = value;
		}

		public int FPS
		{
			get => _fps;
			set => _fps = value;
		}
		public bool bIsDefaultState
		{
			get => bIsDefualt;
			set => bIsDefualt = value;
		}

		public LinkedList<Vector2> FramePositions
		{
			get => _framePositions;
			set => _framePositions = value;
		}


		public float ScalarX = 1.0f;
		public float ScalarY = 1.0f;

		public double FPSTimePeriod = 1 / 12.0f;
		public double TimeBetweenFrames = 0.0f;
		public bool bIsDefualt = false;

		public Vector2 RelativeOrigin { get; set; }
		public LinkedListNode<Vector2> CurrentFramePosition { get; set; }

		public int CurrentFrameIndex
		{
			get
			{
				return _framePositions.ToList().IndexOf(CurrentFramePosition.Value);
			}
		}

		public bool bLoopFinished = false;

		#endregion

		public SpriteAnimation(SpriteSheet parent, String animName, Vector2 startPosition, int frameWidth, int frameHeight, int frameCount, int fps)
		{
			this.ParentSheet = parent;
			this.Name = animName;
			this._startPosition = startPosition;
			this._frameWidth = frameWidth;
			this._frameHeight = frameHeight;
			this._frameCount = frameCount;
			this.FPSTimePeriod = 1 / (float) fps;
			this._fps = fps;

			_drawRectangle.Width = frameWidth;
			_drawRectangle.Height = frameHeight;

		}

		public Rectangle GetScreenRectangle()
		{
			return _drawRectangle;
		}

		public void SetScreenPosition(int? x, int? y)
		{
			if(x != null)
				this._drawRectangle.X = (int)x;
			if (y != null) 
			this._drawRectangle.Y = (int)y;
		}

		public void SetDrawScreenPosRectangle(Rectangle r)
		{
			this._drawRectangle = r;
		}

		public List<AnimationEvent> GetAnimationEvents()
		{
			return this._animationEvents;
		}

		public void AddAnimationEvents(AnimationEvent _event)
		{
			this._animationEvents.Add(_event);
		}

		public Vector2 GetScreenPosition()
		{
			return _drawRectangle.Location.ToVector2();
		}

		public Vector2 GetPosition()
		{
			return _startPosition;
		}

		public int GetFrameWidth()
		{
			return _frameWidth;
		}

		public int GetFrameHeight()
		{
			return _frameHeight;
		}

		public void AddFramePosition(Vector2 v)
		{
			this._framePositions.AddLast(v);
		}

		public void ResetAnimation()
		{
			CurrentFramePosition = this._framePositions.First;
			ParentSheet.DrawRectangle.X = (int)CurrentFramePosition.Value.X;
			ParentSheet.DrawRectangle.Y = (int)CurrentFramePosition.Value.Y;
		}

		/// <summary>
		/// Search the Animation Events to find if there is a sound effect to play GIVEN the current frameNum
		/// </summary>
		public void FindSoundEffect(int frameNum)
		{
			//Find all the sound effects events for this animation.
			int index = -1;
			index = (int)((from x in _animationEvents where x.GetType() == typeof(AudioEvent) select x).ToList()? //CurrentAnimation.GetAnimationEvents().Where(x => x is ChangeAnimationEvent).Select()
				.FindIndex(y => (y as AudioEvent).FrameStart == frameNum));

			index = _animationEvents.FindIndex(x =>
				x is AudioEvent && (x as AudioEvent).FrameStart == frameNum);

			if (index != -1)
			{
				LoadSoundEffect((_animationEvents[index] as AudioEvent).SoundEffectName);
				_currentSoundEffect?.Play(1.0f, 0.0f, 0.0f);
			}
		}

		public void LoadSoundEffect(String effectName)
		{
			this._currentSoundEffect = ParentSheet.contentManager.Load<SoundEffect>(String.Format("Sound\\Effects\\{0}", effectName));
		}

		public void Update(GameTime gameTime)
		{
			TimeBetweenFrames += gameTime.ElapsedGameTime.Milliseconds;
			if (TimeBetweenFrames > FPSTimePeriod * 1000 && CurrentFramePosition != null)
			{
				//Last frameNum
				if (CurrentFramePosition == _framePositions.Last)
					bLoopFinished = true;
				else bLoopFinished = false;

				CurrentFramePosition = (CurrentFramePosition.Next == null ? _framePositions.First : CurrentFramePosition.Next);
				ParentSheet.DrawRectangle.X = (int)CurrentFramePosition.Value.X;
				ParentSheet.DrawRectangle.Y = (int)CurrentFramePosition.Value.Y;

				TimeBetweenFrames = 0;

				if (CurrentFramePosition == _framePositions.First)
					_currentFrame = 0;
				else _currentFrame++;
				FindSoundEffect(_currentFrame);

			}
		}

		public void Draw( SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(ParentSheet.getTexture(), _drawRectangle.ScaleRectangle(ScalarX, ScalarY), ParentSheet.DrawRectangle, Color.White);
		}
	}
}

