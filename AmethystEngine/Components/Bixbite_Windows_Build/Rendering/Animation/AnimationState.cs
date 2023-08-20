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
		public String StateName;
		public List<Animation> AnimationLayers = new List<Animation>();
		public int NumOfFrames {get; set;}
		public int FPS = 60;

		// These are checked from the animations tate machine on tick/update.
		public bool bIsAnimationQueued = false;
		public AnimationState NextState;

		#endregion

		#region fields
		private AnimationStateMachine _parentAnimationStateMachine = null;
		private bool _bIsDefaultState = false;

		#endregion

		#region constructors

		public AnimationState(AnimationStateMachine parent)
		{
			this._parentAnimationStateMachine = parent;
		}
		#endregion

		#region methods

		public bool SetDefualtState(bool newState)
		{
			return _bIsDefaultState = newState;
		}


		public bool IsDefualtState()
		{
			return _bIsDefaultState;
		}

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
