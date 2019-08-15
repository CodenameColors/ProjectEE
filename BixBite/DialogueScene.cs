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
		List<Sprite> CharacterSprites;

		//each scene will also contain multiple different GameUI (Dialogue Boxes)
		public List<GameUI> DialogueBoxes = new List<GameUI>();

		//each scene will also have the ability to change the sprites of the character and the text of the dialogue boxes. Multiple "tracks"
		// **this can either be a TimeBlock, OR a ChoiceBlock, DelayBlock **
		List<LinkedList<object>> SceneDataLL = new List<LinkedList<object>>();
	}
}
