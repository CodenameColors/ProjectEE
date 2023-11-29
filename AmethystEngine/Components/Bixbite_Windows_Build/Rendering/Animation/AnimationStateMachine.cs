using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using Microsoft.Xna.Framework;

namespace BixBite.Rendering.Animation
{
	public class AnimationStateMachine
	{

		#region Delegates

		#endregion

		#region Properties

		public Dictionary<String, AnimationState> States = new Dictionary<string, AnimationState>();
		public AnimationState CurrentState = null;

		public List<AnimationState> ObservableAnimationStates
		{
			get => States.Values.ToList();
		}

		/// <summary>
		/// The Screen Draw position and size of the animation state machine
		/// </summary>
		public Rectangle DrawRectangle
		{
			get
			{
				_drawRectangle.X = _xpos + _xoffset;
				_drawRectangle.Y = _ypos + _yoffset;
				_drawRectangle.Width = (int)(_width * _scaleX);
				_drawRectangle.Height = (int)(_height * _scaleY);
				return _drawRectangle;
			}
			set => _drawRectangle = value;
		}

		public int XPos
		{
			get => _xpos;
			set => _xpos = value;
		}
		public int YPos
		{
			get => _ypos;
			set => _ypos = value;
		}

		public int XOffset
		{
			get => _xoffset;
			set => _xoffset = value;
		}
		public int YOffset
		{
			get => _yoffset;
			set => _yoffset = value;
		}

		public virtual float ScaleX
		{
			get => _scaleX;
			set => _scaleX = value;
		}

		public virtual float ScaleY
		{
			get => _scaleY;
			set => _scaleY = value;
		}
		#endregion

		#region fields
		private Rectangle _drawRectangle = new Rectangle();

		private int _xpos = -1;
		private int _ypos = -1;

		private int _xoffset = 0;
		private int _yoffset = 0;

		private float _scaleX = -1f;
		private float _scaleY = -1f;

		private int _width = -1;
		private int _height = -1;
		#endregion

		#region constructors


		#endregion

		#region methods

		public bool AttemptToAddAnimationEvent(AnimationEvent animationEvent)
		{
			return (this.CurrentState != null && this.CurrentState.AttemptToAddAnimationEvent(animationEvent));
		}

		public void SetScreenPosition(int? x, int? y)
		{
			if (x != null)
				this._xpos = (int)x;
			if (y != null)
				this._ypos = (int) y;
		}
		
		public bool ChangeAnimation(String nextDesiredAnimationState)
		{
			bool returnStatus = false;
			if (CurrentState.Connections.Count > 0)
			{
				// we have the connection but do we need to wait to change it?
				AnimationStateConnections desiredConnection =
					CurrentState.Connections.Find(x => x.DestinationAnimationState.StateName == nextDesiredAnimationState);
				if(desiredConnection != null)
				{
					if (desiredConnection.bIsForceFinish)
					{
						// We will be queuing up the animation state.
						CurrentState.bIsAnimationQueued = true;
						CurrentState.NextState = desiredConnection.DestinationAnimationState;
						returnStatus = true;
					}
					else
					{
						this.CurrentState = desiredConnection.DestinationAnimationState;
						returnStatus = true;
					}
				}
			}
			return returnStatus;
		}

		public void ExportAnimationStateMachine(String filePath, String contentPath)
		{

			XmlWriterSettings settings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "  ",
				NewLineChars = "\r\n",
				NewLineHandling = NewLineHandling.Replace

			};
			//settings.Async = true;

			using (XmlWriter writer = XmlWriter.Create(filePath + ".animachine", settings))
			{
				writer.WriteStartElement(null, "AnimationStateMachine", null);
				//writer.WriteAttributeString(null, "Name", null, GetPropertyData("SceneName").ToString());
				//writer.WriteStartElement(null, "Characters", null); //create "Characters" Tag

				writer.WriteStartElement(null, "AnimationStates", null);

				// Write all the animations states
				foreach (AnimationState state in this.States.Values)
				{
					writer.WriteStartElement(null, "AnimationState", null);
					writer.WriteAttributeString(null, "Name", null, state.StateName);
					writer.WriteAttributeString(null, "Frames", null, state.NumOfFrames.ToString());
					writer.WriteAttributeString(null, "Default", null, state.bIsDefaultState.ToString());
					writer.WriteAttributeString(null, "FPS", null, state.FPS.ToString());

					// write all the animation state's CONNECTIONs
					writer.WriteStartElement(null, "Connections", null);
					foreach (AnimationStateConnections connection in state.Connections)
					{
						writer.WriteStartElement(null, "Connection", null);
						writer.WriteAttributeString(null, "OriginState", null, connection.OriginAnimationState.StateName);
						writer.WriteAttributeString(null, "DestinationState", null, connection.DestinationAnimationState.StateName);
						writer.WriteAttributeString(null, "FinishFirst", null, connection.bIsForceFinish.ToString());
						writer.WriteAttributeString(null, "Threshold", null, connection.StateChangeThreshold.ToString());
						writer.WriteFullEndElement(); // End of Tag "Connection"
					}
					writer.WriteEndElement(); // End of Tag "Connections"

					// write all the animation state's LAYERS
					writer.WriteStartElement(null, "Layers", null);
					foreach (AnimationLayer layer in state.AnimationLayers)
					{
						writer.WriteStartElement(null, "Layer", null);
						writer.WriteAttributeString(null, "Name", null, layer.AnimationLayerName);


						// We need to keep track of all the need Animations
						writer.WriteStartElement(null, "Animations", null);
						foreach (Animation animation in layer.PossibleAnimationsForThisLayer.Values)
						{
							writer.WriteStartElement(null, "Animation", null);
							writer.WriteAttributeString(null, "Name", null, animation.AnimationName);
							writer.WriteAttributeString(null, "ParentLayer", null, animation.ParentAnimationLayer.AnimationLayerName);
							writer.WriteAttributeString(null, "ReferenceIndex", null, animation.ReferenceSpritesheetIndex.ToString());

							// We need to keep track of all the need Frames
							writer.WriteStartElement(null, "Frames", null);
							LinkedListNode<AnimationFrameInfo> currentFrame = animation.GetFirstFrame(); 
							while (currentFrame != null)
							{
								writer.WriteStartElement(null, "Frame", null);
								writer.WriteAttributeString(null, "XPos", null, currentFrame.Value.GetDrawRectangle().X.ToString());
								writer.WriteAttributeString(null, "YPos", null, currentFrame.Value.GetDrawRectangle().Y.ToString());
								writer.WriteAttributeString(null, "Width", null, currentFrame.Value.GetDrawRectangle().Width.ToString());
								writer.WriteAttributeString(null, "Height", null, currentFrame.Value.GetDrawRectangle().Height.ToString());
								writer.WriteAttributeString(null, "OriginX", null, currentFrame.Value.OriginPointOffsetX.ToString());
								writer.WriteAttributeString(null, "OriginY", null, currentFrame.Value.OriginPointOffsetY.ToString());
								writer.WriteAttributeString(null, "RenderX", null, currentFrame.Value.RenderPointOffsetX.ToString());
								writer.WriteAttributeString(null, "RenderY", null, currentFrame.Value.RenderPointOffsetY.ToString());
								writer.WriteEndElement(); // End of Tag "Frame"

								currentFrame = currentFrame.Next;
							}
							writer.WriteEndElement(); // End of Tag "Frames"

							// We need to keep track of all the need EVENTS
							writer.WriteStartElement(null, "Events", null);
							foreach(AnimationEvent _event in animation.AnimationEvents)
							{
								writer.WriteStartElement(null, "Event", null);
								writer.WriteAttributeString(null, "PlaceHolder", null, "PLACEHOLDER");
								writer.WriteEndElement(); // End of Tag "Event"
							}
							writer.WriteFullEndElement(); // End of Tag "EVENTS"

							writer.WriteEndElement(); // End of Tag "Animation"
						}
						writer.WriteEndElement(); // End of Tag "EVENTS"

						// We need to keep track of all the need spritesheets
						writer.WriteStartElement(null, "SpriteSheets", null);
						foreach (SpriteSheet spriteSheet in layer.ReferenceSpriteSheets)
						{
							writer.WriteStartElement(null, "SpriteSheet", null);

							String finalPath = spriteSheet.SpriteSheetPath;
							finalPath = finalPath.Replace(contentPath, "{Content}\\");
							writer.WriteAttributeString(null, "Path", null, finalPath);
							
							writer.WriteEndElement(); // End of Tag "spritesheet"
						}
						writer.WriteEndElement(); // End of Tag "spritesheets"


						writer.WriteEndElement(); // End of Tag "Layer"
					}
					writer.WriteEndElement(); // End of Tag "Layers"

					writer.WriteEndElement(); // End of Tag "AnimationState"
				}
				writer.WriteEndElement(); // End of Tag "AnimationStates"


				writer.WriteEndElement(); // End of Tag "AnimationStateMachine"

			}

		}

		public static AnimationStateMachine ImportAnimationStateMachine(String AnimationStateMachineFilePath, String contentPath)
		{
			AnimationStateMachine returnAnimationStateMachine = new AnimationStateMachine();

			XmlReaderSettings settings = new XmlReaderSettings
			{
				//Async = true
			};

			using (XmlReader reader = XmlReader.Create(AnimationStateMachineFilePath, settings))
			{
				while (reader.Read()) //Read until you can't
				{
					//skip to a SpriteSheet node
					while (reader.Name != "AnimationStateMachine")
						reader.Read();
					if (reader.NodeType == XmlNodeType.Element)
					{
						while (reader.Name != "AnimationStates")
							reader.Read();
						if (reader.NodeType == XmlNodeType.Element)
						{
							do
							{
								reader.Read();
								if (reader.Name == "AnimationState" && reader.NodeType == XmlNodeType.Element)
								{
									// Get properties
									String stateName = (reader.GetAttribute("Name"));
									int frames = int.Parse(reader.GetAttribute("Frames"));
									bool bDefault = bool.Parse(reader.GetAttribute("Default"));
									int fps = int.Parse(reader.GetAttribute("FPS"));

									// Create new animation State
									AnimationState animationState = new AnimationState(returnAnimationStateMachine)
										{FPS = fps, bIsDefaultState = bDefault, StateName = stateName, NumOfFrames = frames};

									// We need to get the connections now
									while (reader.Name != "Connections")
										reader.Read();
									if (reader.NodeType == XmlNodeType.Element)
									{
										do
										{
											reader.Read();
											if (reader.Name == "Connection" && reader.NodeType == XmlNodeType.Element)
											{
												String originStateName = (reader.GetAttribute("OriginState"));
												String destinationStateName = (reader.GetAttribute("DestinationState"));
												bool bFinishFirst = bool.Parse(reader.GetAttribute("FinishFirst"));
												int threshold = int.Parse(reader.GetAttribute("Threshold"));

												AnimationStateConnections newAnimationStateConnection = new AnimationStateConnections(animationState)
													{OriginStateName = originStateName, DestinationStateName = destinationStateName
														, StateChangeThreshold = threshold, bIsForceFinish = bFinishFirst};
												
												// TODO: ADD CONNECTION INFO

												animationState.Connections.Add(newAnimationStateConnection);

											}
										} while (reader.Name.Trim() != "Connections");
									}

									// We need to get the Layers now
									while (reader.Name != "Layers")
										reader.Read();
									if (reader.NodeType == XmlNodeType.Element)
									{
										do
										{
											reader.Read();
											if (reader.Name == "Layer" && reader.NodeType == XmlNodeType.Element)
											{
												String layerName = (reader.GetAttribute("Name"));
												AnimationLayer newAnimationLayer = new AnimationLayer(animationState, layerName );

												// We need to get the animations of this layer
												while (reader.Name != "Animations")
													reader.Read();
												if (reader.NodeType == XmlNodeType.Element)
												{
													do
													{
														reader.Read();
														if (reader.Name == "Animation" && reader.NodeType == XmlNodeType.Element)
														{
															String animationName = (reader.GetAttribute("Name"));
															int referenceIndex = int.Parse(reader.GetAttribute("ReferenceIndex"));
															Animation animation = new Animation(newAnimationLayer, animationName);

															// We need to get the Frames of this Animation
															while (reader.Name != "Frames")
																reader.Read();
															if (reader.NodeType == XmlNodeType.Element)
															{
																do
																{
																	reader.Read();
																	if (reader.Name == "Frame" && reader.NodeType == XmlNodeType.Element)
																	{
																		int xPos = int.Parse(reader.GetAttribute("XPos"));
																		int yPos = int.Parse(reader.GetAttribute("YPos"));
																		int width = int.Parse(reader.GetAttribute("Width"));
																		int height = int.Parse(reader.GetAttribute("Height"));
																		int originX = int.Parse(reader.GetAttribute("OriginX"));
																		int originY = int.Parse(reader.GetAttribute("OriginY"));
																		int renderX = int.Parse(reader.GetAttribute("RenderX"));
																		int renderY = int.Parse(reader.GetAttribute("RenderY"));

																		AnimationFrameInfo frameInfo = new AnimationFrameInfo()
																		{
																			OriginPointOffsetX = originX, OriginPointOffsetY = originY,
																			RenderPointOffsetX = renderX, RenderPointOffsetY = renderY
																		};
																		frameInfo.SetRectangle(new Rectangle(xPos, yPos, width, height));
																		animation.AddFrame(frameInfo);
																		animation.SetReferenceSpriteSheetIndex(referenceIndex);
																	}
																} while (reader.Name.Trim() != "Frames");
															}

															// We need to get the Events of this Animation
															while (reader.Name != "Events")
																reader.Read();
															if (reader.NodeType == XmlNodeType.Element)
															{
																do
																{
																	reader.Read();
																	if (reader.Name == "Event" && reader.NodeType == XmlNodeType.Element)
																	{
																		AnimationEvent eventInfo = new AnimationEvent(false);
																		animation.AnimationEvents.Add(eventInfo);
																	}
																} while (reader.Name.Trim() != "Events");
															}
															
															// Set the Default layer
															if (newAnimationLayer.PossibleAnimationsForThisLayer.Count == 0)
															{
																newAnimationLayer.CurrentLayerInformationName = animationName;
															}
															newAnimationLayer.PossibleAnimationsForThisLayer.Add(animationName, animation);
															
														}
													} while (reader.Name.Trim() != "Animations");
												}

												// Each layer has a set of sprite sheets that it uses to link the graphics data
												while (reader.Name != "SpriteSheets")
													reader.Read();
												if (reader.NodeType == XmlNodeType.Element)
												{
													do
													{
														reader.Read();
														if (reader.Name == "SpriteSheet" && reader.NodeType == XmlNodeType.Element)
														{
															SpriteSheet newSpriteSheet = new SpriteSheet();
															newSpriteSheet.SpriteSheetPath = (reader.GetAttribute("Path"));
															newSpriteSheet.SpriteSheetPath = newSpriteSheet.SpriteSheetPath.Replace("{Content}", contentPath);


															newAnimationLayer.ReferenceSpriteSheets.Add(newSpriteSheet);
														}
													} while (reader.Name.Trim() != "SpriteSheets");
												}
												animationState.AnimationLayers.Add(newAnimationLayer);
											}
										} while (reader.Name.Trim() != "Layers");
									}
									returnAnimationStateMachine.States.Add(stateName, animationState);
								}
							} while (reader.Name.Trim() != "AnimationStates");
						}
					}
				}
			}

			return returnAnimationStateMachine;

		}

		#endregion

		#region monogame

		public void Update(GameTime gameTime)
		{
			// Update the current AnimationState
			if (CurrentState != null)
			{
				CurrentState.Update(gameTime);
			}
		}

		#endregion

	}
}
