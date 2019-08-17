using BixBite.Rendering;
using BixBite.Rendering.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		public ObservableCollection<Sprite> DialogueSprites { get; set; }
		public ObservableCollection<String> DialogueSpritePaths { get; set; }
		//every character has list of animations.

		public Character()
		{
			DialogueSpritePaths = new ObservableCollection<string>();
			DialogueSprites = new ObservableCollection<Sprite>();
		}

	}
}
