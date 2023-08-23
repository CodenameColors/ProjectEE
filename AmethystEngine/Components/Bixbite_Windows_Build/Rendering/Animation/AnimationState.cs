using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BixBite.Rendering.Animation
{
	public class AnimationState
	{
		#region Delegates

		#endregion

		#region Properties

		public Dictionary<String, AnimationStateConnections> Connections = new Dictionary<string, AnimationStateConnections>();
		public String StateName {get; set;}
		public List<Animation> AnimationLayers = new List<Animation>();
		public int NumOfFrames {get; set;}
		public int FPS
		{
			get => _fps;
			set => _fps = value;
		}

		public bool bIsDefaultState
		{
			get => _bIsDefaultState;
			set => _bIsDefaultState = value;
		}

		// These are checked from the animations tate machine on tick/update.
		public bool bIsAnimationQueued = false;
		public AnimationState NextState;

		#endregion

		#region fields
		private AnimationStateMachine _parentAnimationStateMachine = null;
		private bool _bIsDefaultState = false;
		private int _fps = 24;

		#endregion

		#region constructors

		public AnimationState(AnimationStateMachine parent)
		{
			this._parentAnimationStateMachine = parent;
		}
		#endregion

		#region methods

		public AnimationStateMachine GetParentAnimationStateMachine()
		{
			return _parentAnimationStateMachine;
		}

		#endregion

		#region monogame
		public void Update(GameTime gameTime)
		{

			// Update the Animations.
			foreach (Animation anim in AnimationLayers)
			{
				anim.Update(gameTime);
			}

		}
		#endregion

	}
}
