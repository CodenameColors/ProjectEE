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
using System.Xml;
using BixBite.Resources;
using NodeEditor.Components;

namespace BixBite
{

	/// <summary>
	/// This class is here to hold the data, and for the creation of dialogue scene. BOTH for engine and game.
	/// </summary>
	public class DialogueScene : IProperties
	{

		/// <summary>
		/// Name of the dialogue Scene
		/// </summary>
		public String SceneName
		{
			get => GetProperty("SceneName").ToString();
			set => SetProperty("SceneName", value);
		}

		//Each scene can have multiple different characters
		/// <summary>
		/// List of characters in this scene
		/// </summary>
		public ObservableCollection<Character> Characters = new ObservableCollection<Character>();

		//The displaying of the current character sprite
		//public List<Sprite> CharacterSprites = new List<Sprite>();

		//each scene will also contain multiple different GameUI (Dialogue Boxes)
		public List<GameUI> DialogueBoxes = new List<GameUI>();

		/// <summary>
		/// Dialogue scenes have internal parameters that are used for this scene ALONE.
		/// this list keeps track of them.
		/// </summary>
		public List<Tuple<String, object>> DialogueSceneParams = new List<Tuple<string, object>>();

		/// <summary>
		/// Every dialogue scene can have branching paths and or multiple dialogue blocks.
		/// this List keep tracks of all the node editors blocks data.
		/// </summary>
		public List<BaseNodeBlock> DialogueBlockNodes = new List<BaseNodeBlock>();

		//each scene will also have the ability to change the sprites of the character and the text of the dialogue boxes. Multiple "tracks"
		// **this can either be a TimeBlock, OR a ChoiceBlock, DelayBlock **
		//public Dictionary<String, LinkedList<TimeBlock>> TrackData = new Dictionary<String, LinkedList<TimeBlock>>();

		ObservableCollection<Tuple<string, object>> Properties
		{
			get; set;
		}


		public DialogueScene(String Name)
		{
			this.SceneName = Name;
			AddProperty("SceneName", Name);
		}

		//PROPERTIES 

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
			if (Properties.Any(m => m.Item1 == Key))
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
			foreach (Tuple<String, object> tuple in Properties)
			{
				if (tuple.Item1 == Key)
					return i;
				i++;
			}
			return -1;
		}
		#endregion

		#region PropertiesCallBack
		#endregion
		#endregion

		#region PropertyCallbacks

		#endregion


		public void AddCharacterToScene(Character deschar)
		{
			Characters.Add(deschar);
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

		public void ImportScene(String FilePath)
		{

		}

		/// <summary>
		/// This method is here to save all the dialogue scene data to a file (xml)
		/// </summary>
		/// <param name="filePath">Path to create file and save to</param>
		/// <param name="characters">List of the characters in this current scene</param>
		/// <param name="GameUIs">List of UI files in use</param>
		/// <param name="sceneParams">List of params for this scene</param>
		/// <param name="SceneBlockNodes">List of all the block nodes for this scene</param>
		public void ExportScene(String filePath, List<Character> characters, List<String> GameUIs,List<Tuple<String, object>> sceneParams, List<BaseNodeBlock> SceneBlockNodes)
		{
			XmlWriterSettings settings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "  ",
				NewLineChars = "\r\n",
				NewLineHandling = NewLineHandling.Replace

			};
			//settings.Async = true;

			using (XmlWriter writer = XmlWriter.Create(filePath, settings))
			{
				writer.WriteStartElement(null, "DialogueScene", null);
				writer.WriteAttributeString(null, "Name", null, GetPropertyData("SceneName").ToString());

				writer.WriteStartElement(null, "Characters", null); //create "Characters" Tag
				foreach (Character character in characters) //create a character tag
				{
					writer.WriteStartElement(null, "Character", null);
					writer.WriteAttributeString(null, "Name", null, character.Name);

					writer.WriteStartElement(null, "Images", null);
					foreach (Sprite sprite in character.DialogueSprites)
					{
						writer.WriteStartElement(null, "Image", null);
						writer.WriteAttributeString(null, "Path", null, sprite.ImgPathLocation);
						writer.WriteEndElement();//end of Image tag
					}
					writer.WriteEndElement();//end of Images tag
					writer.WriteEndElement();//end of character tag
				}
				writer.WriteEndElement();//end of characters tag


				writer.WriteStartElement(null, "NodeEditor", null);
				writer.WriteStartElement(null, "Params", null);
				foreach (Tuple<String, object> varTuple in sceneParams)
				{
					writer.WriteStartElement(null, "Var", null);
					writer.WriteAttributeString(null, "Name", null, varTuple.Item1);
					writer.WriteAttributeString(null, "Type", null, varTuple.Item2.GetType().ToString());
					writer.WriteAttributeString(null, "DefaultVal", null,varTuple.Item2.ToString());
					writer.WriteEndElement();//end of Var tag
				}
				writer.WriteEndElement();//end of Params tag

				writer.WriteStartElement(null, "BlockNodes", null);
				//TODO: create a methods for each block node to xml nodes.
				foreach (BaseNodeBlock baseNode in SceneBlockNodes)
				{
					CreateBlockNodeXML(writer, baseNode);
				}

				writer.WriteEndElement();//end of BlockNodes tag

				writer.WriteEndElement();//end of NodeEditor tag
				writer.WriteEndElement(); //end of the DialogueScene Tag
			} //end of dialogue scene creation
		}

		private void CreateBlockNodeXML(XmlWriter writer, BaseNodeBlock blocknode)
		{
			writer.WriteStartElement(null, blocknode.GetType().ToString(), null);
			writer.WriteAttributeString(null, "Key", null, blocknode.Name);
			writer.WriteAttributeString(null, "LocX", null, blocknode.LocX);
			writer.WriteAttributeString(null, "LocY", null, blocknode.LocY);
			if(blocknode is DialogueNodeBlock dialogue) writer.WriteAttributeString(null, "Character", null, dialogue.Header);

			writer.WriteStartElement(null, "Nodes", null);
			//entry node
			foreach (ConnectionNode cn in blocknode.EntryNode.ConnectedNodes)
			{
				writer.WriteStartElement(null, "EntryNode", null);
				writer.WriteAttributeString(null, "Type", null, "Entry");

				writer.WriteStartElement(null, "Connection", null);
				writer.WriteAttributeString(null, "FromBlock", null, cn.ParentBlock.Name);
				writer.WriteAttributeString(null, "Node", null, "Exit");
				writer.WriteAttributeString(null, "Ind", null, "0");
				writer.WriteEndElement(); //end of the Connection Tag
				writer.WriteEndElement(); //end of the EntryNode Tag
			}
			//exit node
			foreach (ConnectionNode cn in blocknode.ExitNode.ConnectedNodes)
			{
				writer.WriteStartElement(null, "ExitNode", null);
				writer.WriteAttributeString(null, "Type", null, "Exit");

				writer.WriteStartElement(null, "Connection", null);
				writer.WriteAttributeString(null, "ToBlock", null, cn.ParentBlock.Name);
				writer.WriteAttributeString(null, "Node", null, "Entry");
				writer.WriteAttributeString(null, "Ind", null, "0");
				writer.WriteEndElement(); //end of the Connection Tag
				writer.WriteEndElement(); //end of the EntryNode Tag
			}
		
			//input nodes
			foreach (ConnectionNode input in blocknode.InputNodes)
			{
				writer.WriteStartElement(null, "Inputs", null);
				foreach (ConnectionNode cn in input.ConnectedNodes)
				{
					writer.WriteStartElement(null, "InputNode", null);
					writer.WriteAttributeString(null, "Type", null, cn.NodeType.ToString());

					writer.WriteStartElement(null, "Connection", null);
					writer.WriteAttributeString(null, "FromNode", null, cn.ParentBlock.Name);
					writer.WriteAttributeString(null, "Node", null, cn.ParentBlock.DType.ToString());
					writer.WriteAttributeString(null, "Ind", null, );
				}
				writer.WriteEndElement(); //end of the Inputs Tag
			}
			//output nodes
			foreach (var VARIABLE in Characters)
			{
				
			}
			writer.WriteEndElement(); //end of the Nodes Tag

			writer.WriteEndElement(); //end of the BlockType Tag
		}

	}
}