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
	public class SceneCharacter : BaseCharacter
	{
		//Every Character has a name
		public String Name { get; set; }

		//every character has Dialogue Sprites
		public ObservableCollection<Sprite> DialogueSprites { get; set; }
		//public ObservableCollection<String> DialogueSpritePaths { get; set; }
		//every character has list of animations.

		public String LinkedImageBox = "";

		public String HorizontalAnchor = "";
		public String VerticalAnchor = "";

		public SceneCharacter(String horizontal, String vertical)
		{
			DialogueSprites = new ObservableCollection<Sprite>();
			HorizontalAnchor = horizontal;
			VerticalAnchor = vertical;
	}

	}
}
