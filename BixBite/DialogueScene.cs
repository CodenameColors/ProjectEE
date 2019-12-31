#define DEV_DEBUG

using BixBite.Characters;
using BixBite.Rendering;
using BixBite.Rendering.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using BixBite.Resources;
using NodeEditor;
using NodeEditor.Components;
using NodeEditor.Components.Logic;
using TimelinePlayer.Components;

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
		public ObservableCollection<SceneCharacter> Characters = new ObservableCollection<SceneCharacter>();

		public ObservableCollection<Timeline> Timelines = new ObservableCollection<Timeline>();

		//The displaying of the current character sprite
		//public List<Sprite> CharacterSprites = new List<Sprite>();

		//each scene will also contain multiple different GameUI (Dialogue Boxes)
		public List<GameUI> DialogueBoxes = new List<GameUI>();
		public List<String> DialogueBoxesFilePaths = new List<string>();
		/// <summary>
		/// Dialogue scenes have internal parameters that are used for this scene ALONE.
		/// this list keeps track of them.
		/// </summary>
		public List<BlockNodeEditor.RuntimeVars> DialogueSceneParams = new List<BlockNodeEditor.RuntimeVars>();

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
			Properties = new ObservableCollection<Tuple<string, object>>();
			AddProperty("SceneName", Name);

			this.SceneName = Name;
			
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


		public void AddCharacterToScene(SceneCharacter deschar)
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
			foreach (SceneCharacter c in Characters)
			{
				if (c.Name.Contains(CharName))
				{
					foreach (Sprite spr in c.DialogueSprites)
						if (spr.ImgPathLocation == sprite.ImgPathLocation) return; //don't add a repeat
					c.DialogueSprites.Add(sprite);
				}
			}
		}

		public static DialogueScene ImportScene(String FilePath, ref List<Tuple<String, String, int, String, String, int>> connectionList)
		{
			//Create our return GameUI
			DialogueScene retDialogueScene = null;

			XmlReaderSettings settings = new XmlReaderSettings
			{
				//Async = true
			};

			using (XmlReader reader = XmlReader.Create(FilePath, settings))
			{
				while (reader.Read() )
				{
					//skip to a DialogueScene node
					while (reader.Name != "DialogueScene")
						reader.Read();

					//by this time we have found the dialogue scene tag
					retDialogueScene = new DialogueScene(reader.GetAttribute("Name"));

					//skip to a Characters node
					while (reader.Name != "Characters")
						reader.Read();

					//at this point we have found the characters section of the file. So loop until the end of the tag is found.
					do
					{
						reader.Read();
						if (reader.Name == "Character" && reader.NodeType == XmlNodeType.Element)
						{
							//add the character to the scene
							retDialogueScene.Characters.Add(new SceneCharacter(reader.GetAttribute("Horizontal"), reader.GetAttribute("Vertical"))
							{
								Name = reader.GetAttribute("Name"),
								LinkedImageBox = reader.GetAttribute("LinkedImgBox"),
								HorizontalAnchor = reader.GetAttribute("Horizontal"),
								VerticalAnchor = reader.GetAttribute("Vertical"),
							});
							//add the timeline for the added character to the scene. SET THE Windows settigns in the engine. NOT HERE.
							retDialogueScene.Timelines.Add(new Timeline(60, 200) {TrackName = retDialogueScene.Characters.Last().Name});
							retDialogueScene.DialogueBoxesFilePaths.Add(reader.GetAttribute("UI"));
							do
							{
								reader.Read();
								if (reader.Name == "Image" && reader.NodeType == XmlNodeType.Element)
								{
									String path = reader.GetAttribute("Path");
									if (System.IO.File.Exists(reader.GetAttribute("Path")))
									{
										System.Drawing.Image img =
											System.Drawing.Image.FromFile(
												reader.GetAttribute("Path") ?? throw new InvalidOperationException());
										retDialogueScene.Characters.Last().DialogueSprites.Add(
											new Sprite(
												path.Substring(path.LastIndexOfAny(new char[]{'/','\\'}),  path.LastIndexOf('.') - path.LastIndexOfAny(new char[] { '/', '\\' })),
												path,
												0,0,
												img.Width,
												img.Height
											)
										);
									}

								}
							} while (reader.Name.Trim() != "Character" && XmlNodeType.EndElement != reader.NodeType && !reader.EOF);
						}
					} while (reader.Name.Trim() != "Characters");

					//skip to a Characters node
					while (reader.Name != "Params")
						reader.Read();

					//at this point we have found the params section of the file. So loop until the end of the tag is found.
					do
					{
						reader.Read();
						if (reader.Name == "Var" && reader.NodeType == XmlNodeType.Element)
						{
							retDialogueScene.DialogueSceneParams.Add(
								new BlockNodeEditor.RuntimeVars() {
									VarName = reader.GetAttribute("Name"),
									Type = Type.GetType(reader.GetAttribute("Type")) ,
									OrginalVarData = Convert.ChangeType(reader.GetAttribute("DefaultVal"), Type.GetType(reader.GetAttribute("Type"))),
									VarData = Convert.ChangeType(reader.GetAttribute("DefaultVal"), Type.GetType(reader.GetAttribute("Type"))),
								}
								//new Tuple<string, object>(reader.GetAttribute("Name"), Convert.ChangeType(reader.GetAttribute("DefaultVal"), Type.GetType(reader.GetAttribute("Type"))))  //())
								);
						}

					} while (reader.Name.Trim() != "Params");


					//skip to a BlockNodes node
					while (reader.Name != "BlockNodes")
						reader.Read();


					//The last type of data to load into memory is all the block nodes for this scene. There are quite a few, so we use a helper.
					do
					{
						reader.Read();
						if (reader.Name.Contains("NodeEditor") && reader.NodeType == XmlNodeType.Element)
						{
							CreateBlockNodeFromXML(reader, ref retDialogueScene, ref connectionList);
						}
					}
					while (reader.Name.Trim() != "BlockNodes") ;

					//The last type of data to load into memory is all the block nodes for this scene. There are quite a few, so we use a helper.
					do
					{
						reader.Read();
					}
					while (reader.Name.Trim() != "DialogueScene" && reader.NodeType != XmlNodeType.EndElement);

					return retDialogueScene;
				}
			}
			return retDialogueScene;
		}

		/// <summary>
		/// This method will take any base block node and create and fill in that block nodes data from the XML given.
		/// </summary>
		/// <param name="reader">XMLReader current place</param>
		/// <param name="blocknodes_list">This is used to add to [ref]</param>
		/// <param name="ConnectionList">this is used to add to [ref]. {from, ConType, to, ConType, index}</param>
		private static void CreateBlockNodeFromXML(XmlReader reader, ref DialogueScene curDialogueScene, ref List<Tuple<String, String, int, String, String, int>> ConnectionList)
		{
			//first what type of block node is it?
			Type t = Type.GetType(String.Format("{0}, {1}", reader.Name, "NodeEditor"));
			BaseNodeBlock baseNode = null;
			if (t.Name.Contains("Dialogue"))
				baseNode = (DialogueNodeBlock)Activator.CreateInstance(t, new object[]{reader.GetAttribute("Character"), false});
			else if (t.Name.Contains("GetConstant"))
				baseNode = (GetConstantNodeBlock)Activator.CreateInstance(t, new object[]
				{
					 ECOnnectionType.Int,
					 curDialogueScene.DialogueSceneParams.Find(x=>x.VarName == reader.GetAttribute("Key").Split('_')[1]),
					 false
				});
			else if(t.Name.Contains("SetConstant"))
				baseNode = (BaseNodeBlock)Activator.CreateInstance(t, new object[] { ECOnnectionType.NONE, false });
			else if (t.Name.Contains("Conditional"))
			{
				if (reader.GetAttribute("Key").Contains("LT_"))
				{
					baseNode = (ConditionalNodeBlock)Activator.CreateInstance(t, new object[] { ECOnnectionType.Int, EConditionalTypes.Less, false });
				}
				else if (reader.GetAttribute("Key").Contains("LE_"))
				{
					baseNode = (ConditionalNodeBlock)Activator.CreateInstance(t, new object[] { ECOnnectionType.Int, EConditionalTypes.LessEquals, false });
				}
				else if (reader.GetAttribute("Key").Contains("GT_"))
				{
					baseNode = (ConditionalNodeBlock)Activator.CreateInstance(t, new object[] { ECOnnectionType.Int, EConditionalTypes.Greater, false });
				}
				else if (reader.GetAttribute("Key").Contains("GE_"))
				{
					baseNode = (ConditionalNodeBlock)Activator.CreateInstance(t, new object[] { ECOnnectionType.Int, EConditionalTypes.GreaterEquals, false });
				}
				else if (reader.GetAttribute("Key").Contains("Eq_"))
				{
					baseNode = (ConditionalNodeBlock)Activator.CreateInstance(t, new object[] { ECOnnectionType.Int, EConditionalTypes.Equals, false });
				}
				else if (reader.GetAttribute("Key").Contains("NEq_"))
				{
					baseNode = (ConditionalNodeBlock)Activator.CreateInstance(t, new object[] { ECOnnectionType.Int, EConditionalTypes.NotEquals, false });
				}
				baseNode.DType = ECOnnectionType.Int;
			}
			else if(t.Namespace.Contains("Logic") || t.Namespace.Contains("Arithmetic"))
				baseNode = (BaseNodeBlock)Activator.CreateInstance(t, new object[]{false});
			else
			{
				baseNode = (BaseNodeBlock) Activator.CreateInstance(t);
#if DEV_DEBUG
				Console.WriteLine(t.Name + "||" + t.Namespace);
#endif
			}

			baseNode.Name = reader.GetAttribute("Key");
			baseNode.LocX = double.Parse(reader.GetAttribute("LocX"));
			baseNode.LocY = double.Parse(reader.GetAttribute("LocY"));
			if (baseNode is SetConstantNodeBlock setvar)
			{
				setvar.DType = (ECOnnectionType)Enum.Parse(typeof(ECOnnectionType), reader.GetAttribute("DataType"));
			}
			if (reader.GetAttribute("NewVal") != null)
			{
				baseNode.NewValConnected = false;
				baseNode.NewValue_Constant = reader.GetAttribute("NewVal");
			}
			
			//skip to nodes
			while (reader.Name != "Nodes")
				reader.Read();

			//there are 4 different nodes types
			do
			{
				reader.Read();

				//entry
				if (reader.Name == "EntryNode")
				{
					baseNode.EntryNode.NodeType = (ECOnnectionType) Enum.Parse(typeof(ECOnnectionType), reader.GetAttribute("Type")); //(ECOnnectionType) reader.GetAttribute("Type");
					do
					{
						reader.Read();
						if (reader.Name == "Connection")
						{
							ConnectionList.Add(new Tuple<string, string, int, string, string, int>(
								reader.GetAttribute("FromBlock"),
								ECOnnectionType.Exit.ToString(),
								int.Parse(reader.GetAttribute("Ind")),
								baseNode.Name,
								ECOnnectionType.Enter.ToString(),
								0
							));
						}
					} while (reader.Name.Trim() != "EntryNode");
				}

				//exit
				if (reader.Name == "ExitNode")
				{
					baseNode.ExitNode.NodeType = (ECOnnectionType)Enum.Parse(typeof(ECOnnectionType), reader.GetAttribute("Type"));
					do
					{
						reader.Read();
						//we are not adding a connection to the connection list because this is input to output NOT output to input
					} while (reader.Name.Trim() != "ExitNode");
				}

				//inputs multiples are possible
				if (reader.Name == "Inputs")
				{
					do
					{
						reader.Read();
						reader.Read();
						if (reader.Name == "InputNode" && reader.NodeType != XmlNodeType.EndElement)
						{
							baseNode.InputNodes.Add(new ConnectionNode(
								baseNode, "InputNode"+baseNode.InputNodes.Count, 
								(ECOnnectionType)Enum.Parse(typeof(ECOnnectionType), reader.GetAttribute("Type")))
							);
							if(!(baseNode is DialogueNodeBlock)) baseNode.DType = baseNode.InputNodes[0].NodeType;
							do
							{
								reader.Read();
								if (reader.Name == "Connection")
								{
									if (reader.GetAttribute("Node") == "") break; // this means the node exists BUT there is no connection set for this node
									ConnectionList.Add(new Tuple<string, string, int, string, string, int>(
										reader.GetAttribute("FromBlock"), 
										reader.GetAttribute("Node"),
										int.Parse(reader.GetAttribute("Ind")),
										baseNode.Name,
										(reader.GetAttribute("Node") == "Exit" ? ECOnnectionType.Enter.ToString() : reader.GetAttribute("Node")),
										baseNode.InputNodes.Count-1
									));
								}
							} while (reader.Name.Trim() != "InputNode");
						}
					} while (reader.Name.Trim() != "Inputs");
				}


				//outputs multiples are possible
				if (reader.Name == "Outputs" && !reader.IsEmptyElement)
				{
					do
					{
						reader.Read();
						reader.Read();
						if (reader.Name == "OutputNode")
						{
							baseNode.OutputNodes.Add(new ConnectionNode(
								baseNode, "OutputNode" + baseNode.OutputNodes.Count ,
								(ECOnnectionType)Enum.Parse(typeof(ECOnnectionType), reader.GetAttribute("Type")))
							);
							if (!(baseNode is DialogueNodeBlock || baseNode is ConditionalNodeBlock)) baseNode.DType = baseNode.OutputNodes[0].NodeType;
							do
							{
								reader.Read();
								if (reader.Name == "Connection")
								{
									//ConnectionList.Add(new Tuple<string, string, int>(reader.GetAttribute("FromBlock"), baseNode.Name, int.Parse(reader.GetAttribute("Ind"))));
								}
							} while (reader.Name.Trim() != "OutputNode");
						}
					} while (reader.Name.Trim() != "Outputs");
				}
			}
			while (reader.Name.Trim() != "Nodes");

			//if it's a dialogue node there are a few more things of data we need to import
			if (baseNode is DialogueNodeBlock dialogue)
			{
				//First up load the Timeblock data

				//skip to TimeBlock
				while (reader.Name != "Timeblock")
					reader.Read();

				//TimeLines have not been made yet. So we need to set the parent timeline to null for now. But set this in the engine.
				if (dialogue.OutputNodes.Count > 1)
				{
					Timeline tltemp = null;
					if (Array.Find(curDialogueScene.Timelines.ToArray(), x => x.TrackName == dialogue.Header) != null)
						tltemp = Array.Find(curDialogueScene.Timelines.ToArray(), x => x.TrackName == dialogue.Header);
					else
						tltemp = curDialogueScene.Timelines[0];
					dialogue.LinkedTimeBlock = new ChoiceTimeBlock(tltemp, Double.Parse(reader.GetAttribute("Start")))
					{
						EndTime = Double.Parse(reader.GetAttribute("End")), Trackname = dialogue.Header,
						LinkedDialogueBlock = dialogue,
						LinkedTextBoxName = reader.GetAttribute("LinkedTextBox")
					};
				}
				else
				{
					dialogue.LinkedTimeBlock = new TimeBlock(
						Array.Find(curDialogueScene.Timelines.ToArray(), x => x.TrackName == dialogue.Header),
						Double.Parse(reader.GetAttribute("Start")))
					{
						EndTime = Double.Parse(reader.GetAttribute("End")), Trackname = dialogue.Header,
						LinkedDialogueBlock = dialogue,
						LinkedTextBoxName = reader.GetAttribute("LinkedTextBox"),
					};
				}

				//skip to Data
				while (reader.Name != "Data")
					reader.Read();

				do
				{
					reader.Read();
					if (reader.Name == "DiaChoice")
					{
						do
						{
							reader.Read();
							if (reader.Name == "Sprite")
							{
								(dialogue.LinkedTimeBlock as TimeBlock).TrackSpritePath = reader.GetAttribute("Location");
								dialogue.DialogueSprites.Add(
									new Sprite(
									reader.GetAttribute("Name"),
									reader.GetAttribute("Location"),
									int.Parse(reader.GetAttribute("x")),
									int.Parse(reader.GetAttribute("y")),
									int.Parse(reader.GetAttribute("Width")),
									int.Parse(reader.GetAttribute("Height"))
									));
							}
							else if (reader.Name == "DialogueText")
							{
								dialogue.DialogueTextOptions.Add(reader.GetAttribute("Text"));
								(dialogue.LinkedTimeBlock as TimeBlock).CurrentDialogue = dialogue.DialogueTextOptions[0];
							}
						} while (reader.Name != "DiaChoice");
					}
				} while (reader.Name != "Data");

				//Last load the DialogueData
			}
			curDialogueScene.DialogueBlockNodes.Add(baseNode);
		}

		/// <summary>
		/// This method is here to save all the dialogue scene data to a file (xml)
		/// </summary>
		/// <param name="filePath">Path to create file and save to</param>
		/// <param name="characters">List of the characters in this current scene</param>
		/// <param name="GameUIs">List of UI files in use</param>
		/// <param name="sceneParams">List of params for this scene</param>
		/// <param name="SceneBlockNodes">List of all the block nodes for this scene</param>
		public void ExportScene(String filePath, List<SceneCharacter> characters, List<String> GameUIs,List<Tuple<String, object>> sceneParams, List<BaseNodeBlock> SceneBlockNodes)
		{
			XmlWriterSettings settings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "  ",
				NewLineChars = "\r\n",
				NewLineHandling = NewLineHandling.Replace

			};
			//settings.Async = true;

			using (XmlWriter writer = XmlWriter.Create(filePath+".dials", settings))
			{
				writer.WriteStartElement(null, "DialogueScene", null);
				writer.WriteAttributeString(null, "Name", null, GetPropertyData("SceneName").ToString());

				writer.WriteStartElement(null, "Characters", null); //create "Characters" Tag
				int i = 0;
				foreach (SceneCharacter character in characters) //create a character tag
				{
					writer.WriteStartElement(null, "Character", null);
					writer.WriteAttributeString(null, "Name", null, character.Name);
					writer.WriteAttributeString(null, "UI", null, GameUIs[i++]);
					writer.WriteAttributeString(null, "LinkedImgBox", null, character.LinkedImageBox);
					writer.WriteAttributeString(null, "Horizontal", null, character.HorizontalAnchor);
					writer.WriteAttributeString(null, "Vertical", null, character.VerticalAnchor);


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

		private void CreateBlockNodeXML(XmlWriter writer, BaseNodeBlock blockNode)
		{
			writer.WriteStartElement(null, blockNode.GetType().ToString(), null);
			writer.WriteAttributeString(null, "Key", null, blockNode.Name);
			writer.WriteAttributeString(null, "LocX", null, blockNode.LocX.ToString());
			writer.WriteAttributeString(null, "LocY", null, blockNode.LocY.ToString());
			if (blockNode.DType != ECOnnectionType.NONE)
				writer.WriteAttributeString(null, "DataType", null, blockNode.DType.ToString());

			if (!blockNode.NewValConnected && blockNode.NewValue_Constant != null)
				writer.WriteAttributeString(null, "NewVal", null, blockNode.NewValue_Constant);

			if (blockNode is DialogueNodeBlock dialogue)
				writer.WriteAttributeString(null, "Character", null, dialogue.Header);

			writer.WriteStartElement(null, "Nodes", null);
			//entry node
			if (blockNode.EntryNode != null)
			{
				foreach (ConnectionNode cn in blockNode.EntryNode.ConnectedNodes)
				{
					writer.WriteStartElement(null, "EntryNode", null);
					writer.WriteAttributeString(null, "Type", null, "Enter");

					writer.WriteStartElement(null, "Connection", null);
					writer.WriteAttributeString(null, "FromBlock", null, cn.ParentBlock.Name);
					writer.WriteAttributeString(null, "Node", null, cn.NodeType.ToString());
					int ind = -1;
					if (blockNode.EntryNode.ConnectedNodes[0].ParentBlock.ExitNode != null)
					{
						ind = Array.FindIndex(blockNode.EntryNode.ConnectedNodes[0].ParentBlock.ExitNode.ConnectedNodes.ToArray(),
							x => x == blockNode.EntryNode);
						if (ind >= 0)
							writer.WriteAttributeString(null, "Ind", null, ind.ToString());
					}
					if (blockNode.EntryNode.ConnectedNodes[0].ParentBlock.OutputNodes != null)
					{
						ind = Array.FindIndex(blockNode.EntryNode.ConnectedNodes[0].ParentBlock.OutputNodes.ToArray(),
							x => x == blockNode.EntryNode.ConnectedNodes[0]);
						if (ind >= 0)
							writer.WriteAttributeString(null, "Ind", null, ind.ToString());
					}
					if(ind == -1)
						Console.WriteLine("Incorrect!! No node found during saving.");
					writer.WriteEndElement(); //end of the Connection Tag
					writer.WriteEndElement(); //end of the EntryNode Tag
				}
			}

			//exit node
			if (blockNode.ExitNode != null)
			{
				foreach (ConnectionNode cn in blockNode.ExitNode.ConnectedNodes)
				{
					writer.WriteStartElement(null, "ExitNode", null);
					writer.WriteAttributeString(null, "Type", null, "Exit");

					writer.WriteStartElement(null, "Connection", null);
					writer.WriteAttributeString(null, "ToBlock", null, cn.ParentBlock.Name);
					writer.WriteAttributeString(null, "Node", null, cn.NodeType.ToString());
					writer.WriteAttributeString(null, "Ind", null, Array.FindIndex(blockNode.ExitNode.ConnectedNodes.ToArray(), x => x == cn).ToString());
					writer.WriteEndElement(); //end of the Connection Tag
					writer.WriteEndElement(); //end of the EntryNode Tag
				}
			}

			//input nodes
			if (blockNode.InputNodes != null && blockNode.InputNodes.Count > 0 )
			{
				writer.WriteStartElement(null, "Inputs", null);
				foreach (ConnectionNode input in blockNode.InputNodes)
				{
					if (input.ConnectedNodes.Count > 0)
					{
						foreach (ConnectionNode cn in input.ConnectedNodes)
						{
							writer.WriteStartElement(null, "InputNode", null);
							writer.WriteAttributeString(null, "Type", null, input.NodeType.ToString());

							writer.WriteStartElement(null, "Connection", null);
							writer.WriteAttributeString(null, "FromBlock", null, cn.ParentBlock.Name);
							writer.WriteAttributeString(null, "Node", null, cn.ParentBlock.DType.ToString());
							int ind = 0;
							if (!(cn.ParentBlock is StartBlockNode start))
								ind = Array.FindIndex(cn.ParentBlock.OutputNodes.ToArray(), x => x == cn);
							if (ind != -1) writer.WriteAttributeString(null, "Ind", null, ind.ToString()); //get the index
							else writer.WriteAttributeString(null, "Ind", null, Array.FindIndex(blockNode.InputNodes.ToArray(), x => x == input).ToString()); //get the index
							writer.WriteEndElement(); //end of the Connection Tag
							writer.WriteEndElement(); //end of the Input Tag
						}
					}
					else
					{
						writer.WriteStartElement(null, "InputNode", null);
						writer.WriteAttributeString(null, "Type", null, input.NodeType.ToString());

						writer.WriteStartElement(null, "Connection", null);
						writer.WriteAttributeString(null, "FromBlock", null, "");
						writer.WriteAttributeString(null, "Node", null, "");
						writer.WriteAttributeString(null, "Ind", null, ""); //get the index

						writer.WriteEndElement(); //end of the Connection Tag
						writer.WriteEndElement(); //end of the Input Tag
					}
				}
				writer.WriteEndElement(); //end of the Inputs Tag
			}

			//output nodes
			if (blockNode.OutputNodes != null && blockNode.OutputNodes.Count > 0)
			{
				writer.WriteStartElement(null, "Outputs", null);
				foreach (ConnectionNode output in blockNode.OutputNodes)
				{
					if (output.ConnectedNodes.Count > 0)
					{
						foreach (ConnectionNode cn in output.ConnectedNodes)
						{
							writer.WriteStartElement(null, "OutputNode", null);
							writer.WriteAttributeString(null, "Type", null, output.NodeType.ToString());

							writer.WriteStartElement(null, "Connection", null);
							writer.WriteAttributeString(null, "ToNode", null, cn.ParentBlock.Name);
							writer.WriteAttributeString(null, "Node", null, cn.ParentBlock.DType.ToString());
							int ind = 0;
							if (!(cn.ParentBlock is ExitBlockNode exit))
								ind = Array.FindIndex(cn.ParentBlock.OutputNodes.ToArray(), x => x == cn);
							if (ind != -1) writer.WriteAttributeString(null, "Ind", null, ind.ToString()); //get the index
							else writer.WriteAttributeString(null, "Ind", null, Array.FindIndex(blockNode.OutputNodes.ToArray(), x => x == output).ToString()); //get the index
							writer.WriteEndElement(); //end of the Connection Tag
							writer.WriteEndElement(); //end of the Output Tag

						}
					}
					else
					{
						writer.WriteStartElement(null, "OutputNode", null);
						writer.WriteAttributeString(null, "Type", null, output.NodeType.ToString());

						writer.WriteStartElement(null, "Connection", null);
						writer.WriteAttributeString(null, "ToNode", null, "");
						writer.WriteAttributeString(null, "Node", null, "");
						writer.WriteAttributeString(null, "Ind", null, ""); //get the index

						writer.WriteEndElement(); //end of the Connection Tag
						writer.WriteEndElement(); //end of the Input Tag
					}
				}
				writer.WriteEndElement(); //end of the Outputs Tag
			}

			writer.WriteEndElement(); //end of the Nodes Tag

			//the dialogue block holds more data than the other nodes.
			if (blockNode is DialogueNodeBlock dialogueNode)
			{
				writer.WriteStartElement(null, "Timeblock", null);
				writer.WriteAttributeString(null, "Start", null, (dialogueNode.LinkedTimeBlock as TimeBlock)?.StartTime.ToString());
				writer.WriteAttributeString(null, "End", null, (dialogueNode.LinkedTimeBlock as TimeBlock)?.EndTime.ToString());
				writer.WriteAttributeString(null, "LinkedTextBox", null, (dialogueNode.LinkedTimeBlock as TimeBlock)?.LinkedTextBoxName);
				writer.WriteEndElement(); //end of the TimeBlock Tag

				writer.WriteStartElement(null, "Data", null);
				for (int i = 0; i < dialogueNode.OutputNodes.Count; i++)
				{
					writer.WriteStartElement(null, "DiaChoice", null);

					//Sprite
					if (dialogueNode.DialogueSprites.Count > 0 && dialogueNode.DialogueSprites[i] is Sprite sprite)
					{
						writer.WriteStartElement(null, "Sprite", null);
						writer.WriteAttributeString(null, "Name", null, sprite.Name);
						writer.WriteAttributeString(null, "Location", null, sprite.ImgPathLocation);
						writer.WriteAttributeString(null, "Width", null, sprite.GetPropertyData("width").ToString());
						writer.WriteAttributeString(null, "Height", null, sprite.GetPropertyData("height").ToString());
						writer.WriteAttributeString(null, "x", null, sprite.GetPropertyData("x").ToString());
						writer.WriteAttributeString(null, "y", null, sprite.GetPropertyData("y").ToString());
						writer.WriteEndElement(); //end of the Sprite Tag

					}
					else Console.WriteLine("No Sprite Data found for this dialogue block!!!");

					//DialogueChoice
					writer.WriteStartElement(null, "DialogueText", null);
					writer.WriteAttributeString(null, "Text", null, dialogueNode.DialogueTextOptions[i]);
					writer.WriteEndElement(); //end of the DialogueText Tag

					writer.WriteEndElement(); //end of the DiaChoice Tag
				}
				writer.WriteEndElement(); //end of the Data Tag
			}
			writer.WriteEndElement(); //end of the BlockType Tag
		}

	}
}