using BixBite.Characters;
using BixBite.Rendering;
using BixBite.Rendering.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using BixBite.Resources;
//using TimelinePlayer.Components;

namespace BixBite
{

	/// <summary>
	/// This class is here to hold the data, and for the creation of dialogue scene. BOTH for engine and game.
	/// </summary>
	public class DialogueScene
	{

		public String SceneName { get; set; }

		//Each scene can have multiple different characters
		public ObservableCollection<Character> Characters = new ObservableCollection<Character>();

		//The displaying of the current character sprite
		public List<Sprite> CharacterSprites;

		//each scene will also contain multiple different GameUI (Dialogue Boxes)
		public List<GameUI> DialogueBoxes = new List<GameUI>();

		//each scene will also have the ability to change the sprites of the character and the text of the dialogue boxes. Multiple "tracks"
		// **this can either be a TimeBlock, OR a ChoiceBlock, DelayBlock **
		public Dictionary<String, LinkedList<TimeBlock>> TrackData = new Dictionary<String, LinkedList<TimeBlock>>();



		public DialogueScene(String Name)
		{
			this.SceneName = Name;
		}

		public void AddCharacterToScene(Character deschar)
		{
			Characters.Add(deschar);
		}

		public void AddTrackData(String CharName, TimeBlock newTimeBlock)
		{
			if (TrackData.ContainsKey(CharName))
			{
				TrackData[CharName].AddLast(newTimeBlock);
			}
		}

		/// <summary>
		/// This is here to do a QUICK reset on the data. Rather than do math, since the timeline control already has a correct list.
		/// </summary>
		/// <param name="CharName"></param>
		/// <param name="TimeBlockLL"></param>
		public void SetTrack(String CharName, LinkedList<TimeBlock> TimeBlockLL)
		{
			if (TrackData.ContainsKey(CharName))
			{
				TrackData[CharName] = TimeBlockLL;
			}
		}

		/// <summary>
		/// Add a new sprite to the list, if one the character desired exists. AND TWO it's not a repeat.
		/// </summary>
		/// <param name="CharName"></param>
		/// <param name="sprite"></param>
		public void AddSprite(String CharName, Sprite sprite)
		{
			foreach (Character c in Characters)
			{
				if (c.Name.Contains(CharName))
				{
					foreach (Sprite spr in c.DialogueSprites)
						if (spr.ImgPathLocation == sprite.ImgPathLocation) return; //don't add a repeat
					c.DialogueSprites.Add(sprite);
				}
			}

		}


	}
}