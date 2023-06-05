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
	public class FrameInfo
	{
		public int XPos { get; set; }
		public int YPos { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int RenderPointX { get; set; }
		public int RenderPointY { get; set; }

		public FrameInfo(int x, int y, int width, int height, int renderx, int rendery)
		{
			this.XPos = x;
			this.YPos = y;
			this.Width = width;
			this.Height = height;
			this.RenderPointX = renderx;
			this.RenderPointY = rendery;
		}
	}


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

		//private LinkedList<Vector2> _frameDrawRects = new LinkedList<Vector2>();
		//private LinkedList<Rect> _frameDrawRects = new LinkedList<Rect>();
		private LinkedList<FrameInfo> _frameDrawRects = new LinkedList<FrameInfo>();
		private List<AnimationEvent> _animationEvents = new List<AnimationEvent>();

		private SoundEffect _currentSoundEffect = null;
		#endregion

		#region Properties

		public String Name
		{
			get => _name;
			set => _name = value;
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

		//public LinkedList<Vector2> FrameDrawRects
		//{
		//	get => _frameDrawRects;
		//	set => _frameDrawRects = value;
		//}

		//public LinkedList<Rect> FrameDrawRects
		//{
		//	get => _frameDrawRects;
		//	set => _frameDrawRects = value;
		//}

		public LinkedList<FrameInfo> FrameDrawRects
		{
			get => _frameDrawRects;
			set => _frameDrawRects = value;
		}


		public float ScalarX = 1.0f;
		public float ScalarY = 1.0f;

		public double FPSTimePeriod = 1 / 12.0f;
		public double TimeBetweenFrames = 0.0f;
		public bool bIsDefualt = false;

		public Vector2 RelativeOrigin { get; set; }
		//public LinkedListNode<Vector2> CurrentFrameRect { get; set; }
		//public LinkedListNode<Rect> CurrentFrameRect { get; set; }
		public LinkedListNode<FrameInfo> CurrentFrameRect { get; set; }

		public int CurrentFrameIndex
		{
			get
			{
				return _frameDrawRects.ToList().IndexOf(CurrentFrameRect.Value);
			}
		}

		public bool bLoopFinished = false;

		#endregion

		public SpriteAnimation(SpriteSheet parent, String animName, int frameCount, int fps)
		{
			this.ParentSheet = parent;
			this.Name = animName;

			this._frameCount = frameCount;
			this.FPSTimePeriod = 1 / (float) fps;
			this._fps = fps;

			CurrentFrameRect = FrameDrawRects.First;
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

		public void AddFramePosition(FrameInfo frameInfo)
		{
			this._frameDrawRects.AddLast(frameInfo);
			if (_frameDrawRects.Count == 1)
				CurrentFrameRect = FrameDrawRects.First;
		}

		public void ResetAnimation()
		{
			CurrentFrameRect = this._frameDrawRects.First;
			ParentSheet.DrawRectangle.X = (int)CurrentFrameRect.Value.XPos;
			ParentSheet.DrawRectangle.Y = (int)CurrentFrameRect.Value.YPos;
			ParentSheet.DrawRectangle.Width = (int)CurrentFrameRect.Value.Width;
			ParentSheet.DrawRectangle.Height = (int)CurrentFrameRect.Value.Height;
			_drawRectangle.X = (int)CurrentFrameRect.Value.XPos;
			_drawRectangle.Y = (int)CurrentFrameRect.Value.YPos;
			_drawRectangle.Width = (int)CurrentFrameRect.Value.Width;
			_drawRectangle.Height = (int)CurrentFrameRect.Value.Height;
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
			if (TimeBetweenFrames > FPSTimePeriod * 1000 && CurrentFrameRect != null)
			{
				//Last frameNum
				if (CurrentFrameRect == _frameDrawRects.Last)
					bLoopFinished = true;
				else bLoopFinished = false;

				CurrentFrameRect = (CurrentFrameRect.Next == null ? _frameDrawRects.First : CurrentFrameRect.Next);
				ParentSheet.DrawRectangle.X = (int)CurrentFrameRect.Value.XPos;
				ParentSheet.DrawRectangle.Y = (int)CurrentFrameRect.Value.YPos;
				ParentSheet.DrawRectangle.Width = (int)CurrentFrameRect.Value.Width;
				ParentSheet.DrawRectangle.Height = (int)CurrentFrameRect.Value.Height;
				_drawRectangle.X = (int)CurrentFrameRect.Value.XPos;
				_drawRectangle.Y = (int)CurrentFrameRect.Value.YPos;
				_drawRectangle.Width = (int)CurrentFrameRect.Value.Width;
				_drawRectangle.Height = (int)CurrentFrameRect.Value.Height;

				TimeBetweenFrames = 0;

				if (CurrentFrameRect == _frameDrawRects.First)
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

