using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using BixBite.Resources;

namespace BixBite.QuickTimeEvents
{
	/// <summary>
	/// Choose how to show this Quick Time Event is "counting down"
	/// </summary>
	public enum EQuickTimeEventEffect
	{
		None = 0,
		Dispapear = 1,
		Fade = 2,
		Crop = 3,
		GreyOut = 4,
		Shader = 5
	}

	/// <summary>
	/// Choose how to show the quick time event to the player
	/// </summary>
	public enum EQuickTimeEventDisplayType
	{
		None = 0,
		ProgressBar = 1, 
		Image = 2,
	}


	/// <summary>
	/// This Class is here for creating and displaying Quick Time Events. As well as handling
	/// the control/processing flow of the quick time events.
	/// </summary>
	public abstract class BaseQuickTimeEvents : IProperties
	{

		#region Delegates
		public delegate void QuickTimeEvent_OnFinished(List<object> paramsList);
		public QuickTimeEvent_OnFinished OnQuickTimeEventFinished = null;

		public delegate void QuickTimeEvent_OnWrongKey(List<object> paramsList);
		public QuickTimeEvent_OnWrongKey OnQuickTimeEventWrongKey = null;

		public delegate void QuickTimeEvent_OnCorrectKey(List<object> paramsList);
		public QuickTimeEvent_OnCorrectKey OnQuickTimeEventCorrectKey = null;
		#endregion

		#region Properties
		protected ObservableCollection<Tuple<string, object>> Properties { get; set; }


		public String QuickTimeEventName { get; set; }

		public bool bIsActive { get; set; }

		public int ZIndex { get; set; }
		public int XPos { get; set; }
		public int YPos { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }


		#endregion

		#region Fields
		protected int _totalRunTimeInMs = 0;
		protected int _currentRunTimeInMs = 0;
		protected int _currentKey = 0;
		protected int _requiredKey = 0;

		protected bool _bHitRightKey = false;

		protected EQuickTimeEventDisplayType DisplayQuickTimeEventType = EQuickTimeEventDisplayType.None;
		protected EQuickTimeEventEffect QuickTimeEventEffect = EQuickTimeEventEffect.None;


		#endregion

		#region Methods

		#region Constructors

		#endregion

		#region Helpers
		public abstract void SetNewProperties(ObservableCollection<Tuple<string, object>> NewProperties);
		public abstract void ClearProperties();
		public abstract void SetProperty(string Key, object Property);
		public abstract void AddProperty(string Key, object data);
		public abstract object GetPropertyData(string Key);
		public abstract Tuple<string, object> GetProperty(string Key);
		public abstract ObservableCollection<Tuple<string, object>> GetProperties();
		public abstract int GetPropertyIndex(string Key);
		#endregion

		#region Monogame

		#endregion

		#endregion


	}
}
