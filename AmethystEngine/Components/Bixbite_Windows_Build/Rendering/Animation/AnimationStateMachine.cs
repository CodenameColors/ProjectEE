using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Xna.Framework;

namespace BixBite.Rendering.Animation
{
	public class AnimationStateMachine
	{

		#region Delegates

		#endregion

		#region Properties

		public Dictionary<String, AnimationState> States = new Dictionary<string, AnimationState>();
		public AnimationState CurrentState = null;

		public List<AnimationState> ObservableAnimationStates
		{
			get => States.Values.ToList();
		}

		/// <summary>
		/// The Screen Draw position and size of the animation state machine
		/// </summary>
		public Rectangle DrawRectangle
		{
			get
			{
				_drawRectangle.X = _xpos + _xoffset;
				_drawRectangle.Y = _ypos + _yoffset;
				_drawRectangle.Width = (int)(_width * _scaleX);
				_drawRectangle.Height = (int)(_height * _scaleY);
				return _drawRectangle;
			}
			set => _drawRectangle = value;
		}

		public int XPos
		{
			get => _xpos;
			set => _xpos = value;
		}
		public int YPos
		{
			get => _ypos;
			set => _ypos = value;
		}

		public int XOffset
		{
			get => _xoffset;
			set => _xoffset = value;
		}
		public int YOffset
		{
			get => _yoffset;
			set => _yoffset = value;
		}

		public virtual float ScaleX
		{
			get => _scaleX;
			set => _scaleX = value;
		}

		public virtual float ScaleY
		{
			get => _scaleY;
			set => _scaleY = value;
		}
		#endregion

		#region fields
		private Rectangle _drawRectangle = new Rectangle();

		private int _xpos = -1;
		private int _ypos = -1;

		private int _xoffset = 0;
		private int _yoffset = 0;

		private float _scaleX = -1f;
		private float _scaleY = -1f;

		private int _width = -1;
		private int _height = -1;
		#endregion

		#region constructors


		#endregion

		#region methods

		public bool AttemptToAddAnimationEvent(AnimationEvent animationEvent)
		{
			return (this.CurrentState != null && this.CurrentState.AttemptToAddAnimationEvent(animationEvent));
		}

		public void SetScreenPosition(int? x, int? y)
		{
			if (x != null)
				this._xpos = (int)x;
			if (y != null)
				this._ypos = (int) y;
		}
		
		public bool ChangeAnimation(String nextDesiredAnimationState)
		{
			bool returnStatus = false;
			if (CurrentState.Connections.Count > 0)
			{
				// we have the connection but do we need to wait to change it?
				AnimationStateConnections desiredConnection =
					CurrentState.Connections.Find(x => x.DestinationAnimationState.StateName == nextDesiredAnimationState);
				if(desiredConnection != null)
				{
					if (desiredConnection.bIsForceFinish)
					{
						// We will be queuing up the animation state.
						CurrentState.bIsAnimationQueued = true;
						CurrentState.NextState = desiredConnection.DestinationAnimationState;
						returnStatus = true;
					}
					else
					{
						this.CurrentState = desiredConnection.DestinationAnimationState;
						returnStatus = true;
					}
				}
			}
			return returnStatus;
		}

		public static AnimationStateMachine ImportAnimationStateMachine(String AnimationStateMachineFilePath)
		{
			AnimationStateMachine returnAnimationStateMachine = new AnimationStateMachine();

			return returnAnimationStateMachine;
		}
		#endregion

		#region monogame

		public void Update(GameTime gameTime)
		{
			// Update the current AnimationState
			if (CurrentState != null)
			{
				CurrentState.Update(gameTime);
			}
		}

		#endregion

	}
}
