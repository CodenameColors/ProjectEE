using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BixBite.Resources;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace BixBite
{
	//how the character will react before and after the event has executed. IE do they move, etc.
	public struct EventData
	{
		public int? newx; //new location of player
		public int? newy; //new location of the player
		public double MoveTime; //the time to get from the start to the new position.
		public string NewFileToLoad; //the new file that we should load.
	}

	public enum EventType
	{
		None,
		LevelTransition,
		DialougeScene,
		Cutscene,
		Trigger,
		Collision,
		BGM,
	}

	public class GameEvent : IProperties
	{
		public delegate void PGridSync_Hook(String Key, object Property, System.Collections.Specialized.NotifyCollectionChangedAction action);
		public PGridSync_Hook PGridSync = null;

		//these varibles determine the cell of the gameeven trigger as well as the grouping.
		//public int xpos, ypos, width, height, group = 0; ///This is the area of activation :: 0 is top left ; 1 is bottom right
		//public String ActivationButton, , DelegateEventName = String.Empty;
		public EventType eventType = new EventType();
		public EventData datatoload = new EventData(); ///this is the data that will load on an activation of this event.
		//private bool isActive = true; //this determines where or not this event should be checked for.

		//Allows easy accesss to the properties collection.
		#region Properties

		#region EventName
		public String EventName
		{
			get
			{
				if (Properties.Any(m => m.Item1 == "EventName"))
				{
					return GetPropertyData("EventName").ToString();
				}
				throw new NullReferenceException(); //doesn't exist
			}
			set
			{
				if (Properties.Any(m => m.Item1 == "EventName"))
				{
					SetProperty("EventName", value);
				}
				else
				{
					AddProperty("EventName", value);
				}
			}
		}
		#endregion

		#region Group
		public int Group
		{
			get
			{
				if (Properties.Any(m => m.Item1 == "group"))
				{
					return (int)GetPropertyData("group");
				}
				throw new NullReferenceException(); //doesn't exist
			}
			set
			{
				if (Properties.Any(m => m.Item1 == "group"))
				{
					SetProperty("group", value);
				}
				else
				{
					AddProperty("group", value);
				}
			}
		}
		#endregion

		#region IsActive
		public bool IsActive
		{
			get
			{
				if (Properties.Any(m => m.Item1 == "isActive"))
				{
					return (bool)GetPropertyData("isActive");
				}
				throw new NullReferenceException(); //doesn't exist
			}
			set
			{
				if (Properties.Any(m => m.Item1 == "isActive"))
				{
					SetProperty("isActive", value);
				}
				else
				{
					AddProperty("isActive", value);
				}
			}
		}
		#endregion

		#region ActivationButton
		public String ActivationButton
		{
			get
			{
				if (Properties.Any(m => m.Item1 == "ActivationButton"))
				{
					return GetPropertyData("ActivationButton").ToString();
				}
				throw new NullReferenceException(); //doesn't exist
			}
			set
			{
				if (Properties.Any(m => m.Item1 == "ActivationButton"))
				{
					SetProperty("ActivationButton",value);
				}
				else
				{
					AddProperty("ActivationButton", value);
				}
			}
		}
		#endregion

		#region ActivationButton
		public String DelegateEventName
		{
			get
			{
				if (Properties.Any(m => m.Item1 == "DelegateEventName"))
				{
					GetPropertyData("DelegateEventName");
				}
				throw new NullReferenceException(); //doesn't exist
			}
			set
			{
				if (Properties.Any(m => m.Item1 == "DelegateEventName"))
				{
					SetProperty("DelegateEventName", value);
				}
				else
				{
					AddProperty("DelegateEventName", value);
				}
			}
		}
		#endregion

		#endregion
		/// <summary>
		/// Holds the properties and data for this object. It needs to be a tuple to allow on change callback
		/// </summary>
		ObservableCollection<Tuple<string, object>> Properties { get; set; }

		public GameEvent(String Name, EventType eventType, int group)
		{
			Properties = new ObservableCollection<Tuple<string, object>>();
			Properties.CollectionChanged += Properties_Changed;
			this.EventName = Name;
			this.eventType = eventType;
			AddProperty("EventName", Name);
			AddProperty("group", group);
			AddProperty("isActive", true);
			AddProperty("ActivationButton", "");
			AddProperty("DelegateEventName", "");

			datatoload.MoveTime = 0;
			datatoload.newx = 0;
			datatoload.newy = 0;
			datatoload.NewFileToLoad = "";
		}

		#region Properties

		#region IPropertiesImplementation
		public void SetNewProperties(ObservableCollection<Tuple<string, object>> NewProperties)
		{
			Properties = NewProperties;
		}

		public void ClearProperties()
		{
			Properties.Clear();
		}

		public void SetProperty(string Key, object Property)
		{
			if (!Properties.Any(m => m.Item1 == Key))
				Properties[GetPropertyIndex(Key)] = new Tuple<string, object>(Key, Property);
			else throw new PropertyNotFoundException(Key);

		}

		public void AddProperty(string Key, object data)
		{
			if (!Properties.Any(m => m.Item1 == Key))
				Properties.Add(new Tuple<String, object>(Key, data));
		}

		public Tuple<String, object> GetProperty(String Key)
		{
			if (Properties.Any(m => m.Item1 == Key))
				return Properties.Single(m => m.Item1 == Key);
			else throw new PropertyNotFoundException();
		}

		public object GetPropertyData(string Key)
		{
			int i = GetPropertyIndex(Key);
			if (-1 == i) throw new PropertyNotFoundException(Key);
			return Properties[i].Item2;
		}

		public ObservableCollection<Tuple<String, object>> GetProperties()
		{
			return Properties;
		}

		#endregion

		#region Helper
		public int GetPropertyIndex(String Key)
		{
			int i = 0;
			foreach(Tuple<String, object> tuple in Properties)
			{
				if (tuple.Item1 == Key)
					return i;
				i++;
			}
			return -1;
		}
		#endregion

		#region PropertiesCallBack
		private void Properties_Changed(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
			{
				if(PGridSync != null)
				{
					foreach(Tuple<String, object> tuple in e.NewItems)
					{
						PGridSync(tuple.Item1, tuple.Item2, System.Collections.Specialized.NotifyCollectionChangedAction.Add);
					}
				}
			}
			else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
			{
				if (PGridSync != null)
				{
					foreach (Tuple<String, object> tuple in e.NewItems)
					{
						PGridSync(tuple.Item1, tuple.Item2, System.Collections.Specialized.NotifyCollectionChangedAction.Remove);
					}
				}
			}
			else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
			{
				if (PGridSync != null)
				{
					foreach (Tuple<String, object> tuple in e.NewItems)
					{
						PGridSync(tuple.Item1, tuple.Item2, System.Collections.Specialized.NotifyCollectionChangedAction.Replace);
					}
				}
			}
			else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
			{
				if (PGridSync != null)
				{
					foreach (Tuple<String, object> tuple in e.NewItems)
					{
						PGridSync(tuple.Item1, tuple.Item2, System.Collections.Specialized.NotifyCollectionChangedAction.Reset);
					}
				}
			}
		}

		#endregion
		#endregion

		public void AddEventData(EventData ed, String newDeleName, String ButtonName)
		{
			switch (eventType)
			{
				case (EventType.Collision):
					//for a collision there is no activation needed it should ALWAYS trigger.
					datatoload.NewFileToLoad = "Collision"; 
					return;
				case (EventType.Cutscene):
					//for a cutscene activation should occur from a "trigger" trigger, and or a button press.
					datatoload.NewFileToLoad = "Cutscene WIP... not working";
					return;
				case (EventType.DialougeScene):
					datatoload.NewFileToLoad = "dialogue scene WIP... not working";
					//for a dialogue activation should occur from a "trigger" trigger, and or a button press.
					return;
				case (EventType.LevelTransition):
					datatoload = ed;
					SetProperty("DelegateEventName", newDeleName);
					SetProperty("ActivationButton", ButtonName);
					//for a Level transisition activation should occur from a "trigger" trigger, and or a button press.
					return;
				case (EventType.Trigger):
					//for a trigger there is no activation needed it should ALWAYS trigger.
					datatoload.NewFileToLoad = "Trigger Area";
					return;
				case (EventType.BGM):
					//for a trigger that will change the music it should activate like a "trigger", but also has the choice of input (button)
					datatoload.NewFileToLoad = "MUSIC WIP... not working";
					return;
				case (EventType.None):
					//this is failsafe AND SHOULD NO BE RAN HERE... if it gets here its bad...
					Console.WriteLine("Added incorrect data to GameeventLayer... ignored add operation");
					return;
			}
		}


	}
}
