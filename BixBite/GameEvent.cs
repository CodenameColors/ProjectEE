using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
		LevelTransistion,
		DialougeScene,
		Cutscene,
		Trigger,
		Collision,
		BGM,
	}

	public class GameEvent
	{
		//these varibles determine the cell of the gameeven trigger as well as the grouping.
		public int xpos, ypos, width, height, group = 0; ///This is the area of activation :: 0 is top left ; 1 is bottom right
		public String ActivationButton, EventName, DelegateEventName = String.Empty;
		
		public EventType eventType = new EventType();
		private EventData datatoload = new EventData(); ///this is the data that will load on an activation of this event.
		private bool isActive = true; //this determines where or not this event should be checked for.

		public GameEvent(String Name, EventType eventType, int xpos, int ypos, int w, int h, int group)
		{
			this.EventName = Name;
			this.eventType = eventType;
			this.xpos = xpos;
			this.ypos = ypos;
			this.width = w;
			this.height = h;
			this.group = group;
		}

		public void ChangeGroup(int newgroup)
		{
			this.group = newgroup;
		}

		public bool GetActiveState()
		{
			return this.isActive;
		}

		public void SetActiveState(bool activestate)
		{
			this.isActive = activestate;
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
				case (EventType.LevelTransistion):
					datatoload = ed;
					this.DelegateEventName = newDeleName;
					this.ActivationButton = ButtonName;

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
