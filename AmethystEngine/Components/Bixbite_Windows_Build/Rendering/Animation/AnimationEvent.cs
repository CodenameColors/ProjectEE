using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Rendering.Animation
{

	public class AnimationAudioEvent : AnimationEvent
	{
		public String SoundEffectName { get; set; }
		public bool		bIsRepeating { get; set; }
		public bool		BiSPlaying { get; set; }

		private int _frameStart = 0;
		public int FrameStart
		{
			get => _frameStart;
			set => _frameStart = value;
		}


		private int _frameEnd = 0;
		public int FrameEnd
		{
			get => _frameEnd;
			set => _frameEnd = value;
		}

		public AnimationAudioEvent(bool bImmediate, int fstart, int fend, String filename) : base(bImmediate)
		{
			this.FrameEnd = fend;
			this.FrameStart = fstart;
			this.SoundEffectName = filename;
		}

		public void StartPlayingAudioEvent()
		{
			Console.WriteLine("StartPlayingAudioEvent NOT IMPLEMENTED");
		}

		public void StopPlayingAudioEvent()
		{
			Console.WriteLine("StopPlayingAudioEvent NOT IMPLEMENTED");
		}

	}

	/// <summary>
	/// This class is here for when we need to change the AnimationLayer Layer.
	/// An example is X Character is in Idle, and we need to switch from sword to staff.
	/// </summary>
	public class ChangeAnimationLayerEvent : AnimationEvent
	{
		public String LayerName {get;set;}
		public String SheetToReplaceOnLayerName;

		public ChangeAnimationLayerEvent(bool immediate, String layerName, String newSheetName) : base(immediate)
		{
			this.bIsImmediate = immediate;
			this.LayerName = layerName;
			this.SheetToReplaceOnLayerName = newSheetName;
		}

	}

	public class ChangeAnimationStateEvent : AnimationEvent
	{
		private String _fromAnimationName = String.Empty;

		public String FromAnimationName
		{
			get => _fromAnimationName;
			set => _fromAnimationName = value;
		}
		private String _toAnimationName = String.Empty;

		public String ToAnimationName
		{
			get => _toAnimationName;
			set => _toAnimationName = value;
		}

		public ChangeAnimationStateEvent(bool bImmediate, String FromName, String ToName) : base(bImmediate)
		{
			this.FromAnimationName = FromName;
			this.ToAnimationName = ToName;
			this.bIsImmediate = bImmediate;
		}

	}

	public class AnimationEvent
	{
		public bool bIsImmediate = true;

		public AnimationEvent(bool bIsImmediate)
		{
			this.bIsImmediate = bIsImmediate;
		}
	}
}
