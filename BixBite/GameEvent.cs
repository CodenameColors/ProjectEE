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
				if (Properties.ContainsKey("EventName"))
				{
					return Properties["EventName"].ToString();
				}
				throw new NullReferenceException(); //doesn't exist
			}
			set
			{
				if (Properties.ContainsKey("EventName"))
				{
					Properties["EventName"] = value;
				}
				else
				{
					Properties.Add("EventName", value);
				}
			}
		}
		#endregion

		#region Group
		public int Group
		{
			get
			{
				if (Properties.ContainsKey("group"))
				{
					return (int)Properties["group"];
				}
				throw new NullReferenceException(); //doesn't exist
			}
			set
			{
				if (Properties.ContainsKey("group"))
				{
					Properties["group"] = value;
				}
				else
				{
					Properties.Add("group", value);
				}
			}
		}
		#endregion

		#region IsActive
		public bool IsActive
		{
			get
			{
				if (Properties.ContainsKey("isActive"))
				{
					return (bool)Properties["isActive"];
				}
				throw new NullReferenceException(); //doesn't exist
			}
			set
			{
				if (Properties.ContainsKey("isActive"))
				{
					Properties["isActive"] = value;
				}
				else
				{
					Properties.Add("isActive", value);
				}
			}
		}
		#endregion

		#region ActivationButton
		public String ActivationButton
		{
			get
			{
				if (Properties.ContainsKey("ActivationButton"))
				{
					return Properties["ActivationButton"].ToString();
				}
				throw new NullReferenceException(); //doesn't exist
			}
			set
			{
				if (Properties.ContainsKey("ActivationButton"))
				{
					Properties["ActivationButton"] = value;
				}
				else
				{
					Properties.Add("ActivationButton", value);
				}
			}
		}
		#endregion

		#region ActivationButton
		public String DelegateEventName
		{
			get
			{
				if (Properties.ContainsKey("DelegateEventName"))
				{
					return Properties["DelegateEventName"].ToString();
				}
				throw new NullReferenceException(); //doesn't exist
			}
			set
			{
				if (Properties.ContainsKey("DelegateEventName"))
				{
					Properties["DelegateEventName"] = value;
				}
				else
				{
					Properties.Add("DelegateEventName", value);
				}
			}
		}
		#endregion

		#endregion
		ObservableCollection<string, object> Properties { get; set; }

		public GameEvent(String Name, EventType eventType, int group)
		{
			Properties = new ObservableCollection<string, object>();
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
		public void SetNewProperties(ObservableCollection<string, object> NewProperties)
		{
			Properties = NewProperties;
		}

		public void ClearProperties()
		{
			Properties.Clear();
		}

		public void SetNewProperty(string Key, object Property)
		{
			if (!Properties.ContainsKey(Key))
				Properties.[Key] = Property;
			else throw new PropertyNotFoundException(Key);

		}

		public void AddProperty(string Key, object data)
		{
			if (!Properties.ContainsKey(Key))
				Properties.Add(Key, data);
		}

		public object GetProperty(String Key)
		{
			if (Properties.ContainsKey(Key))
				return Properties[Key];
			else throw new PropertyNotFoundException();
		}
		public ObservableCollection<String, object> GetProperties()
		{
			return Properties;
		}
		#endregion

		//public void UpdateProperties(Dictionary<String, object> newdict)
		//{
		//	Properties = new ObservableCollection<string, object>(newdict);
		//}

		//public void ClearProperties()
		//{
		//	Properties.Clear();
		//}

		//public void AddProperty(string Pname, object data)
		//{
		//	Properties.Add(Pname, data);
		//}


		//public ObservableCollection<string, object> getProperties()
		//{
		//	return Properties;
		//}

		//public void setProperties(ObservableCollection<string, object> newprops)
		//{
		//	Properties = newprops;
		//}

		//public void SetProperty(string PName, object data)
		//{
		//	Properties[PName] = data;
		//}

		//public object GetProperty(String PName)
		//{
		//	return Properties[PName];
		//}
		#endregion

		#region PropertiesCallBack
		public void PropertyCallback(object sender, System.Windows.RoutedEventArgs e)
		{
			////theres two things that can call this. Textbox, Checkbox
			//if (sender is CheckBox)
			//{
			//	String PName = ((CheckBox)sender).Tag.ToString();
			//	if (GetProperty(PName) is bool)
			//	{

			//		if (PName == "isActive")
			//		{
			//			SetProperty(PName, ((CheckBox)sender).IsChecked);
			//		}
			//		else
			//		{
			//			Console.WriteLine("Others... Saved should be enabled= false...");
			//		}
			//	}
			//}
			//else if(sender is TextBox)
			//{
			//	String PName = ((TextBox)sender).Tag.ToString();
			//	if(GetProperty(PName) is String)
			//	{
			//		SetProperty(PName, ((TextBox)sender).Text);
			//	}
			//}

		}
		#endregion

		public void ChangeGroup(int newgroup)
		{
			SetProperty("group", newgroup);
		}

		public bool GetActiveState()
		{
			return (bool)GetProperty("isActive");
		}

		public void SetActiveState(bool activestate)
		{
			SetProperty("isActive", activestate);
		}

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
