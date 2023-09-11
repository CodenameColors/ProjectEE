using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Rendering.Animation
{
	public enum EAnimationEventReturnValues
	{
		NONE = -1,
		HANDLED_AND_REMOVED = 0,
		HANDLED_NOT_REMOVED = 1,
		FAILED							= 2
	}

	public class Animation
	{
		private LinkedList<AnimationFrameInfo> _animationFrames = new LinkedList<AnimationFrameInfo>();
		public List<AnimationEvent> AnimationEvents = new List<AnimationEvent>();

		public AnimationLayer ParentAnimationLayer {get;set;}
		public LinkedListNode<AnimationFrameInfo> CurrentAnimationFrame;
		public String AnimationName {get;set;}
		// public Spritesheet ReferenceSpritesheet = new Spritesheet();

		public int ReferenceSpritesheetIndex = -1;

		public Animation(AnimationLayer parent, String animationName)
		{
			this.ParentAnimationLayer = parent;
			this.AnimationName = animationName;
			
		}

		public void AddFrame(AnimationFrameInfo newFrame)
		{
			this._animationFrames.AddLast(newFrame);
			if (_animationFrames.Count == 1)
			{
				CurrentAnimationFrame = _animationFrames.First;
			}
		}

		public void SetFrames(LinkedList<AnimationFrameInfo> animationFrames)
		{
			_animationFrames = animationFrames;
			CurrentAnimationFrame = _animationFrames.First;
		}

		public LinkedListNode<AnimationFrameInfo> GetFirstFrame()
		{
			return _animationFrames.First;
		}

		public LinkedListNode<AnimationFrameInfo> GetLastFrame()
		{
			return _animationFrames.Last;
		}

		public void SetReferenceSpriteSheetIndex(int index)
		{
			this.ReferenceSpritesheetIndex = index;
		}

		public void AddAnimationEvent(AnimationEvent newEvent)
		{
			this.AnimationEvents.Add(newEvent);
		}

	}

	public class AnimationLayer
	{
		#region Delegates

		#endregion

		#region Properties

		public String AnimationLayerName { get; set; }
		public String AnimationName { get; set; }
		public String CurrentLayerInformationName = "";
		public ObservableCollection <SpriteSheet> ReferenceSpriteSheets = new ObservableCollection <SpriteSheet>();

		public SpriteSheet ReferenceSpriteSheet
		{
			get
			{
				if (ReferenceSpriteSheets.Count > 0)
				{
					return ReferenceSpriteSheets[CurrentFrameProperties.ReferenceSpritesheetIndex];
				}
				else return null;
			}
		}

		public Animation CurrentFrameProperties
		{
			get
			{
				if (_possibleAnimationsForThisLayer.ContainsKey(CurrentLayerInformationName))
					return _possibleAnimationsForThisLayer[CurrentLayerInformationName];
				else
					return null;
			}
		}

		public Dictionary<String, Animation> PossibleAnimationsForThisLayer
		{
			get => _possibleAnimationsForThisLayer;
			set => _possibleAnimationsForThisLayer = value;
		}

		public float ScalarX = 1.0f;
		public float ScalarY = 1.0f;

		#endregion

		#region fields
		public Dictionary<String, Animation> _possibleAnimationsForThisLayer = new Dictionary<string, Animation>();

		private AnimationState _parentAnimationState;

		private Rectangle _drawRectangle;
		private double _FPSTimePeriod;
		private double _timeBetweenFrames = 0.0f;
		private bool _bIsDefualt = false;
		private bool _bLoopFinished = false;
		private int _currentFrame;

		#endregion

		#region constructors

		public AnimationLayer(AnimationState parent, String layerName)
		{
			_parentAnimationState = parent;

			_FPSTimePeriod = 1 / _parentAnimationState.FPS;
			AnimationLayerName = layerName;
		}
		#endregion

		#region methods

		public EAnimationEventReturnValues HandleAnimationEvent(AnimationEvent animationEventToHandle)
		{
			EAnimationEventReturnValues returnState = EAnimationEventReturnValues.FAILED;

			if (animationEventToHandle != null)
				switch (animationEventToHandle)
				{
					case AnimationAudioEvent animationAudioEvent:
						if (animationAudioEvent.bIsRepeating)
						{
							returnState = EAnimationEventReturnValues.HANDLED_NOT_REMOVED;
						}
						else
						{
							returnState = EAnimationEventReturnValues.HANDLED_AND_REMOVED;

						}
						break;
					case ChangeAnimationLayerEvent changeAnimationLayerEvent:
							returnState = EAnimationEventReturnValues.HANDLED_AND_REMOVED;

						break;
					case ChangeAnimationStateEvent changeAnimationStateEvent:
							returnState = EAnimationEventReturnValues.HANDLED_AND_REMOVED;

						break;
					default:
						Console.WriteLine("HandleAnimationEvent FAILED");
						break;
				}
			return returnState;
		}

		public EAnimationEventReturnValues HandleAnimationEvent(int currentFrame)
		{
			EAnimationEventReturnValues retAnimationEventReturnValue = EAnimationEventReturnValues.NONE;

			foreach (var animationEvent in CurrentFrameProperties.AnimationEvents)
			{
				switch (animationEvent)
				{
					case AnimationAudioEvent animationAudioEvent:
						if (currentFrame == animationAudioEvent.FrameStart)
						{
							animationAudioEvent.StartPlayingAudioEvent();
						}
						else if (currentFrame == animationAudioEvent.FrameEnd)
						{
							if (animationAudioEvent.BiSPlaying)
							{
								animationAudioEvent.StopPlayingAudioEvent();
							}
						}
						else
						{

						}
						break;
					case ChangeAnimationLayerEvent changeAnimationLayerEvent:
						break;
					case ChangeAnimationStateEvent changeAnimationStateEvent:
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(animationEvent));
				}
			}


			return retAnimationEventReturnValue;
		}

		/// <summary>
		/// Load the Sound Effects needed for this animation into memory.
		/// </summary>
		/// <param name="effectName"></param>
		public void LoadSoundEffect(String effectName)
		{
			//this._currentSoundEffect = ParentSheet.contentManager.Load<SoundEffect>(String.Format("Sound\\Effects\\{0}", effectName));
		}

		/// <summary>
		/// Search the AnimationLayer Events to find if there is a sound effect to play GIVEN the current frameNum
		/// </summary>
		public void FindSoundEffect(int frameNum)
		{
			////Find all the sound effects events for this animation.
			//int index = -1;
			//index = (int)((from x in _animationEvents where x.GetType() == typeof(AudioEvent) select x).ToList()? //CurrentAnimation.GetAnimationEvents().Where(x => x is ChangeAnimationEvent).Select()
			//	.FindIndex(y => (y as AudioEvent).FrameStart == frameNum));

			//index = _animationEvents.FindIndex(x =>
			//	x is AudioEvent && (x as AudioEvent).FrameStart == frameNum);

			//if (index != -1)
			//{
			//	LoadSoundEffect((_animationEvents[index] as AudioEvent).SoundEffectName);
			//	_currentSoundEffect?.Play(1.0f, 0.0f, 0.0f);
			//}
		}

		#endregion

		#region monogame
		public void Update(GameTime gameTime)
		{
			_timeBetweenFrames += gameTime.ElapsedGameTime.Milliseconds;
			if (_timeBetweenFrames > _FPSTimePeriod * 1000 && CurrentFrameProperties.CurrentAnimationFrame != null)
			{
				//Last frameNum
				if (CurrentFrameProperties.CurrentAnimationFrame == CurrentFrameProperties.GetLastFrame())
					_bLoopFinished = true;
				else _bLoopFinished = false;

				CurrentFrameProperties.CurrentAnimationFrame = (CurrentFrameProperties.CurrentAnimationFrame.Next == null ? CurrentFrameProperties.GetFirstFrame() : CurrentFrameProperties.CurrentAnimationFrame.Next);
				//ParentSheet.DrawRectangle.X = (int)CurrentFrameInfo.Value.XPos;
				//ParentSheet.DrawRectangle.Y = (int)CurrentFrameInfo.Value.YPos;
				//ParentSheet.DrawRectangle.Width = (int)CurrentFrameInfo.Value.Width;
				//ParentSheet.DrawRectangle.Height = (int)CurrentFrameInfo.Value.Height;
				_drawRectangle.X = (int)CurrentFrameProperties.CurrentAnimationFrame.Value.GetDrawRectangle().X;
				_drawRectangle.Y = (int)CurrentFrameProperties.CurrentAnimationFrame.Value.GetDrawRectangle().Y;
				_drawRectangle.Width = (int)CurrentFrameProperties.CurrentAnimationFrame.Value.GetDrawRectangle().Width;
				_drawRectangle.Height = (int)CurrentFrameProperties.CurrentAnimationFrame.Value.GetDrawRectangle().Height;

				_timeBetweenFrames = 0;

				if (CurrentFrameProperties.CurrentAnimationFrame == CurrentFrameProperties.GetFirstFrame())
					_currentFrame = 0;
				else _currentFrame++;
				FindSoundEffect(_currentFrame);

			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(ReferenceSpriteSheets[this.CurrentFrameProperties.ReferenceSpritesheetIndex].GetTexture2D(), _drawRectangle.ScaleRectangle(ScalarX, ScalarY), CurrentFrameProperties.CurrentAnimationFrame.Value.GetDrawRectangle(), Color.White);
		}
		#endregion
	}
}
