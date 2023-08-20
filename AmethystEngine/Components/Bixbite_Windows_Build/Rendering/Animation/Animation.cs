using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Rendering.Animation
{
	public class Animation
	{
		#region Delegates

		#endregion

		#region Properties

		public String AnimationName;
		public Spritesheet ReferenceSpriteSheet = new Spritesheet();
		public LinkedList<AnimationFrameInfo> AnimationFrames = new LinkedList<AnimationFrameInfo>();
		public LinkedListNode<AnimationFrameInfo> CurrentFrameInfo = null;

		public float ScalarX = 1.0f;
		public float ScalarY = 1.0f;

		#endregion

		#region fields
		private AnimationState _parentAnimationState;

		private Rectangle _drawRectangle;
		private double _FPSTimePeriod;
		private double _timeBetweenFrames = 0.0f;
		private bool _bIsDefualt = false;
		private bool _bLoopFinished = false;
		private int _currentFrame;

		#endregion

		#region constructors

		public Animation(AnimationState parent)
		{
			_parentAnimationState = parent;

			_FPSTimePeriod = 1 / _parentAnimationState.FPS;
		}
		#endregion

		#region methods

		/// <summary>
		/// Load the Sound Effects needed for this animation into memory.
		/// </summary>
		/// <param name="effectName"></param>
		public void LoadSoundEffect(String effectName)
		{
			//this._currentSoundEffect = ParentSheet.contentManager.Load<SoundEffect>(String.Format("Sound\\Effects\\{0}", effectName));
		}

		/// <summary>
		/// Search the Animation Events to find if there is a sound effect to play GIVEN the current frameNum
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
			if (_timeBetweenFrames > _FPSTimePeriod * 1000 && CurrentFrameInfo != null)
			{
				//Last frameNum
				if (CurrentFrameInfo == AnimationFrames.Last)
					_bLoopFinished = true;
				else _bLoopFinished = false;

				CurrentFrameInfo = (CurrentFrameInfo.Next == null ? AnimationFrames.First : CurrentFrameInfo.Next);
				//ParentSheet.DrawRectangle.X = (int)CurrentFrameInfo.Value.XPos;
				//ParentSheet.DrawRectangle.Y = (int)CurrentFrameInfo.Value.YPos;
				//ParentSheet.DrawRectangle.Width = (int)CurrentFrameInfo.Value.Width;
				//ParentSheet.DrawRectangle.Height = (int)CurrentFrameInfo.Value.Height;
				_drawRectangle.X = (int)CurrentFrameInfo.Value.GetDrawRectangle().X;
				_drawRectangle.Y = (int)CurrentFrameInfo.Value.GetDrawRectangle().Y;
				_drawRectangle.Width = (int)CurrentFrameInfo.Value.GetDrawRectangle().Width;
				_drawRectangle.Height = (int)CurrentFrameInfo.Value.GetDrawRectangle().Height;

				_timeBetweenFrames = 0;

				if (CurrentFrameInfo == AnimationFrames.First)
					_currentFrame = 0;
				else _currentFrame++;
				FindSoundEffect(_currentFrame);

			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(ReferenceSpriteSheet.GetTexture2D(), _drawRectangle.ScaleRectangle(ScalarX, ScalarY), CurrentFrameInfo.Value.GetDrawRectangle(), Color.White);
		}
		#endregion
	}
}
