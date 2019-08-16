using BixBite.Rendering;
using BixBite.Rendering.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using TimelinePlayer.Components;

namespace BixBite.Characters
{
	public class Character
	{
		//Every Character has a name
		public String Name { get; set; }

		//every character has Dialogue Sprites
		public List<Sprite> DialogueSprites = new List<Sprite>();

		//every character has list of animations.

	}
}
