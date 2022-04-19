using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Rendering.Animation
{

	public class AudioEvent : AnimationEvent
	{
		public String SoundEffectName { get; set; }

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

		public AudioEvent(int fstart, int fend, String filename)
		{
			this.FrameEnd = fend;
			this.FrameStart = fstart;
			this.SoundEffectName = filename;
		}
	}

	public class ChangeLayeredAnimationEvent : AnimationEvent
	{

		public bool bImmediate = true;

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

		public Dictionary<String, String> SubLayerAnimationChangeEvents = new Dictionary<string, String>();

		public ChangeLayeredAnimationEvent(string FromName, string ToName)
		{
			this.FromAnimationName = FromName;
			this.ToAnimationName = ToName;

		}
	}

	public class ChangeAnimationEvent : AnimationEvent
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


		public bool bImmediate = true;
		//public List<String> ConnectedAnimationNames_List = new List<string>();

		public ChangeAnimationEvent(String FromName, String ToName, bool bImmediate)
		{
			this.FromAnimationName = FromName;
			this.ToAnimationName = ToName;
			this.bImmediate = bImmediate;
		}

	}

	public class AnimationEvent
	{
	}
}
