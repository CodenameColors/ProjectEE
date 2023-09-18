using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Rendering.Animation
{
	public class AnimationStateConnections
	{
		#region Delegates

		#endregion

		#region Properties

		public AnimationState OriginAnimationState => _parentState.GetParentAnimationStateMachine().States[OriginStateName];
		public AnimationState DestinationAnimationState => _parentState.GetParentAnimationStateMachine().States[DestinationStateName];
		public bool bIsForceFinish {get;set;}
		public float StateChangeThreshold {get; set;}

		public String OriginStateName { get; set; }
		public String DestinationStateName { get; set; }


		#endregion

		#region fields

		private readonly AnimationState _parentState = null;
		#endregion

		#region constructors

		public AnimationStateConnections(AnimationState parent)
		{
			this._parentState = parent;
		}
		#endregion

		#region methods

		#endregion

		#region monogame

		#endregion
	}
}
