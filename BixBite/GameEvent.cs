using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BixBite.Resources;

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
		public String EventName;
		public EventType eventType = new EventType();
		public  EventData datatoload = new EventData(); ///this is the data that will load on an activation of this event.
		//private bool isActive = true; //this determines where or not this event should be checked for.

		ObservableDictionary<string, object> Properties { get; set; }

		public GameEvent(String Name, EventType eventType, int group)
		{
			Properties = new ObservableDictionary<string, object>();
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
		public void UpdateProperties(Dictionary<String, object> newdict)
		{
			Properties = new ObservableDictionary<string, object>(newdict);
		}

		public void ClearProperties()
		{
			Properties.Clear();
		}

		public void AddProperty(string Pname, object data)
		{
			Properties.Add(Pname, data);
		}


		public ObservableDictionary<string, object> getProperties()
		{
			return Properties;
		}

		public void setProperties(ObservableDictionary<string, object> newprops)
		{
			Properties = newprops;
		}

		public void SetProperty(string PName, object data)
		{
			Properties[PName] = data;
		}

		public object GetProperty(String PName)
		{
			return Properties[PName];
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
