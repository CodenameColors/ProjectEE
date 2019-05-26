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

	public enum EventType
	{
		None,
		LevelTransistion,
		DialougeScene,
		Cutscene,
		Trigger,
		Collision,
	}

	public class GameEvent
	{
		public Point[] RectArea = new Point[2]; ///This is the area of activation :: 0 is top left ; 1 is bottom right
		public Buttons ActivationButton = new Buttons();
		public EventType eventType = new EventType();
		public int group { get; set; } ///this is the group vaule. Allows the user to set multiple tiles to one delegate.
		object datatoload; ///this is the data that will load on an activation of this event.
		public String EventName = String.Empty;	/// Name Event object.
		public String DelegateEventName = String.Empty; ///Name of the method that will be invoked on event activation.


	}
}
