using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Xml;
using BixBite.Rendering.Animation;
using BixBite.Resources;
using DrWPF.Windows.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Rendering
{
	public class SpriteSheet : Sprite
	{

		#region Fields

		private String _name = String.Empty;
		private String _characterName = String.Empty;

		private int _frameWidth = 0;
		private int _frameHeight = 0;
		private SpriteAnimation _nextAnimation = null;

		#endregion

		#region Properties

		public ContentManager contentManager;
		public Rectangle DrawRectangle;

		public List<SpriteAnimation> SpriteAnimations_List
		{
			get => SpriteAnimations.Values.ToList();
		}

		public string SheetName
		{
			get => _name;
			set => _name = value;
		}

		public string CharacterName
		{
			get => _characterName;
			set => _characterName = value;
		}

		private bool _isActive = true;
		public bool IsActive
		{
			get => _isActive;
			set => _isActive = value;
		}

		public ObservableDictionary<String, SpriteAnimation> SpriteAnimations = new ObservableDictionary<string, SpriteAnimation>();
		public SpriteAnimation CurrentAnimation = null; //= new SpriteAnimation();

		#endregion

		public SpriteSheet(string Name, int screenX, int screenY, int w, int h) : base(Name, screenX, screenY, w, h)
		{

		}


		public SpriteSheet(string Name, string imgLoc, int x, int y, int w, int h) : base(Name, imgLoc, x, y, w, h)
		{
			
		}

		public void Onload()
		{

		}

		#region Methods

		public void SetScreenPosition(Vector2 v)
		{
			Screen_pos = v;
			X = v.X;
			Y = v.Y;
		}

		public static SpriteSheet ImportSpriteSheet(String filePath)
		{
			SpriteSheet retSpriteSheet = null;
			XmlReaderSettings settings = new XmlReaderSettings
			{
				//Async = true
			};

			using (XmlReader reader = XmlReader.Create(filePath, settings))
			{
				while (reader.Read()) //Read until you can't
				{
					//skip to a SpriteSheet node
					while (reader.Name != "SpriteSheet")
						reader.Read();
					if(reader.NodeType == XmlNodeType.Element)
					{
						//retSpriteSheet.Name = reader.GetAttribute("Name"); // grab the name
						String characterName = reader.GetAttribute("Character"); // grab the name
						String FileLocation = reader.GetAttribute("File"); // grab the name
						String SheetName = reader.GetAttribute("SheetName"); // grab the name
						int MaxWidth = int.Parse(reader.GetAttribute("Width"));
						int MaxHeight = int.Parse(reader.GetAttribute("Height"));
						int animationCount = int.Parse(reader.GetAttribute("AnimationCount"));
						retSpriteSheet = new SpriteSheet(characterName, 0, 0, MaxWidth, MaxHeight) {SheetName = SheetName, CharacterName = characterName, ImgPathLocation = FileLocation};
						retSpriteSheet.text = retSpriteSheet.contentManager?.Load<Texture2D>((String.Format("{0}", FileLocation)));
						//Skip to the AnimationStates node
						while (reader.Name != "AnimationStates")
							reader.Read();

						//When you get here you SHOULD loop until all animation states have been taken care of
						do
						{
							reader.Read();
							if (reader.Name == "AnimationState" && reader.NodeType == XmlNodeType.Element)
							{
								//If you are here you have found a new animation. So create one to fill with data.
								String animName = (reader.GetAttribute("Name"));
								int startx = int.Parse(reader.GetAttribute("StartX"));
								int starty = int.Parse(reader.GetAttribute("StartY"));
								int numframes = int.Parse(reader.GetAttribute("NumOfFrames"));
								int fwidth = int.Parse(reader.GetAttribute("FrameWidth"));
								int fheight = int.Parse(reader.GetAttribute("FrameHeight"));
								int fps = int.Parse(reader.GetAttribute("FPS"));
								bool isDefault = bool.Parse(reader.GetAttribute("isDefault"));

								SpriteAnimation tempAnimation = new SpriteAnimation(retSpriteSheet, animName,
									new Vector2(startx, starty), fwidth, fheight, numframes, fps)
								{

								};
								//now we need to fill in all the offset/frame position data.
								for (int i = 0; i < numframes; i++)
								{
									tempAnimation.AddFramePosition(new Vector2(startx + (fwidth * i), starty));
								}

								tempAnimation.ResetAnimation(); //set the first position/pointer.
								tempAnimation.bIsDefualt = isDefault;

								//Skip to the AnimationStates node
								while (reader.Name != "Events")
									reader.Read();

								do
								{
									reader.Read();

									if (reader.Name == "SoundEffect" && reader.NodeType == XmlNodeType.Element)
									{
										int fsval = 0;
										int feval = -1;
										String effectName = reader.GetAttribute("Value");
										if(int.TryParse(reader.GetAttribute("FrameStart"), out fsval)) { }
										if(int.TryParse(reader.GetAttribute("FrameEnd"), out feval)) { }
										if (feval == -1) feval = numframes; // If there is none Assume it plays the entire animation.

										AnimationEvent animationEvent = new AudioEvent(fsval,feval, effectName);
										tempAnimation.AddAnimationEvents(animationEvent);

									}

									if (reader.Name == "AllowedAnimName" && reader.NodeType == XmlNodeType.Element)
									{
										String toAnimName = reader.GetAttribute("Name");
										String fromAnimName = tempAnimation.Name;
										bool bfinishFirst = bool.Parse(reader.GetAttribute("bFinishFirst"));

										AnimationEvent animationEvent = new ChangeAnimationEvent(fromAnimName, toAnimName, bfinishFirst);
										tempAnimation.AddAnimationEvents(animationEvent);
									}
								} while (reader.Name != "Events");

								retSpriteSheet.SpriteAnimations.Add(animName, tempAnimation);
							}

						} while (reader.Name.Trim() != "AnimationStates");
					}
				}
			}

			retSpriteSheet.CurrentAnimation = retSpriteSheet.SpriteAnimations.First(x => x.Value.bIsDefualt).Value;
			retSpriteSheet.DrawRectangle = new Rectangle( (int)retSpriteSheet.CurrentAnimation.GetPosition().X,
				(int)retSpriteSheet.CurrentAnimation.GetPosition().Y,
				(int)retSpriteSheet.CurrentAnimation.GetFrameWidth(), 
				(int)retSpriteSheet.CurrentAnimation.GetFrameHeight());

			return retSpriteSheet;
		}

		//public static SpriteSheet ImportSpriteSheet(ContentManager contentManager, String filePath, float scalarX = 1.0f, float scalarY =1.0f)
		//{
		//	SpriteSheet retSpriteSheet = null;
		//	XmlReaderSettings settings = new XmlReaderSettings
		//	{
		//		//Async = true
		//	};

		//	using (XmlReader reader = XmlReader.Create(filePath, settings))
		//	{
		//		while (reader.Read()) //Read until you can't
		//		{
		//			//skip to a SpriteSheet node
		//			while (reader.Name != "SpriteSheet")
		//				reader.Read();
		//			if (reader.NodeType == XmlNodeType.Element)
		//			{
		//				//retSpriteSheet.Name = reader.GetAttribute("Name"); // grab the name
		//				String SpriteSheetName = reader.GetAttribute("Character"); // grab the name
		//				String FileName = reader.GetAttribute("SheetName"); // grab the name
		//				String SheetLocation = reader.GetAttribute("SheetLocation"); // grab the name
		//				int MaxWidth = int.Parse(reader.GetAttribute("Width"));
		//				int MaxHeight = int.Parse(reader.GetAttribute("Height"));

		//				int animationCount = int.Parse(reader.GetAttribute("AnimationCount"));
		//				retSpriteSheet = new SpriteSheet(SpriteSheetName, 0, 0, MaxWidth, MaxHeight) {Name = FileName };
		//				retSpriteSheet.contentManager = contentManager;
		//				retSpriteSheet.text = retSpriteSheet.contentManager?.Load<Texture2D>((String.Format("{0}", SheetLocation)));
		//				//Skip to the AnimationStates node
		//				while (reader.Name != "AnimationStates")
		//					reader.Read();

		//				//When you get here you SHOULD loop until all animation states have been taken care of
		//				do
		//				{
		//					reader.Read();
		//					if (reader.Name == "AnimationState" && reader.NodeType == XmlNodeType.Element)
		//					{
		//						//If you are here you have found a new animation. So create one to fill with data.
		//						String animName = (reader.GetAttribute("Name"));
		//						int startx = int.Parse(reader.GetAttribute("StartX"));
		//						int starty = int.Parse(reader.GetAttribute("StartY"));
		//						int numframes = int.Parse(reader.GetAttribute("NumOfFrames"));
		//						int fwidth = int.Parse(reader.GetAttribute("FrameWidth"));
		//						int fheight = int.Parse(reader.GetAttribute("FrameHeight"));
		//						int fps = int.Parse(reader.GetAttribute("FPS"));
		//						bool isDefault = bool.Parse(reader.GetAttribute("isDefault"));

		//						SpriteAnimation tempAnimation = new SpriteAnimation(retSpriteSheet, animName,
		//							new Vector2(startx, starty), fwidth, fheight, numframes, fps)
		//						{

		//						};
		//						//now we need to fill in all the offset/frame position data.
		//						for (int i = 0; i < numframes; i++)
		//						{
		//							tempAnimation.AddFramePosition(new Vector2(startx + (fwidth * i), starty));
		//						}

		//						tempAnimation.ResetAnimation(); //set the first position/pointer.
		//						tempAnimation.ScalarX = scalarX;
		//						tempAnimation.ScalarY = scalarY;
		//						tempAnimation.bIsDefualt = isDefault;

		//						//Skip to the AnimationStates node
		//						while (reader.Name != "Events")
		//							reader.Read();

		//						do
		//						{
		//							reader.Read();

		//							if (reader.Name == "SoundEffect" && reader.NodeType == XmlNodeType.Element)
		//							{
		//								int fsval = 0;
		//								int feval = -1;
		//								String effectName = reader.GetAttribute("Value");
		//								if (int.TryParse(reader.GetAttribute("FrameStart"), out fsval)) { }
		//								if (int.TryParse(reader.GetAttribute("FrameEnd"), out feval)) { }
		//								if (feval == -1) feval = numframes; // If there is none Assume it plays the entire animation.

		//								AnimationEvent animationEvent = new SoundEffectAnimationEvent(fsval, feval, effectName);
		//								tempAnimation.AddAnimationEvents(animationEvent);

		//							}

		//							if (reader.Name == "AllowedAnimName" && reader.NodeType == XmlNodeType.Element)
		//							{
		//								String toAnimName = reader.GetAttribute("Name");
		//								String fromAnimName = tempAnimation.Name;
		//								bool bfinishFirst = bool.Parse(reader.GetAttribute("bFinishFirst"));

		//								AnimationEvent animationEvent = new ChangeAnimationEvent(fromAnimName, toAnimName, bfinishFirst);
		//								tempAnimation.AddAnimationEvents(animationEvent);
		//							}
		//						} while (reader.Name != "Events");

		//						retSpriteSheet.SpriteAnimations.Add(animName, tempAnimation);
		//					}

		//				} while (reader.Name.Trim() != "AnimationStates");
		//			}
		//		}
		//	}

		//	retSpriteSheet.CurrentAnimation = retSpriteSheet.SpriteAnimations.First(x => x.Value.bIsDefualt).Value;
		//	retSpriteSheet.DrawRectangle = new Rectangle((int)retSpriteSheet.CurrentAnimation.GetPosition().X,
		//		(int)retSpriteSheet.CurrentAnimation.GetPosition().Y,
		//		(int)retSpriteSheet.CurrentAnimation.GetFrameWidth(),
		//		(int)retSpriteSheet.CurrentAnimation.GetFrameHeight());

		//	return retSpriteSheet;
		//}


		public void ChangeAnimation(String newAnimationName)
		{
			//Is this a VALID animation change?
			int cae = CurrentAnimation.GetAnimationEvents().FindIndex(
				x => x is ChangeAnimationEvent && (x as ChangeAnimationEvent).ToAnimationName == newAnimationName);

			if (cae >= 0 && !(CurrentAnimation.GetAnimationEvents()[cae] as ChangeAnimationEvent).bImmediate)
			{
				Vector2 screenpos = CurrentAnimation.GetScreenPosition();
				CurrentAnimation = SpriteAnimations[newAnimationName];
				this.DrawRectangle.X = (int)CurrentAnimation.GetPosition().X;
				this.DrawRectangle.Y = (int)CurrentAnimation.GetPosition().Y;
				this.DrawRectangle.Width = (int)CurrentAnimation.GetFrameWidth();
				this.DrawRectangle.Height = (int)CurrentAnimation.GetFrameHeight();

				CurrentAnimation.SetScreenPosition((int)screenpos.X, (int)screenpos.Y);
			}
			//This animation MUST finish first
			else if (cae >= 0 && ((CurrentAnimation.GetAnimationEvents()[cae] as ChangeAnimationEvent).bImmediate))
			{
				_nextAnimation = SpriteAnimations[newAnimationName];
			}
		}

		public void ChangeAnimation(SpriteAnimation spriteAnimation)
		{
			Vector2 screenpos = CurrentAnimation.GetScreenPosition();
			CurrentAnimation = spriteAnimation;
			this.DrawRectangle.X = (int)CurrentAnimation.GetPosition().X;
			this.DrawRectangle.Y = (int)CurrentAnimation.GetPosition().Y;
			this.DrawRectangle.Width = (int)CurrentAnimation.GetFrameWidth();
			this.DrawRectangle.Height = (int)CurrentAnimation.GetFrameHeight();

			CurrentAnimation.SetScreenPosition((int)screenpos.X, (int)screenpos.Y);

		}

		public static bool ExportSpriteSheet(SpriteSheet desiredSheet, String filepath)
		{
			bool retbool = false;

			XmlWriterSettings settings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "  ",
				NewLineChars = "\r\n",
				NewLineHandling = NewLineHandling.Replace

			};
			//settings.Async = true;

			using (XmlWriter writer = XmlWriter.Create(filepath + ".anim", settings))
			{

				writer.WriteStartElement(null, "SpriteSheet", null);
				writer.WriteAttributeString(null, "Character", null, desiredSheet.CharacterName);
				writer.WriteAttributeString(null, "SheetName", null, desiredSheet.SheetName);
				writer.WriteAttributeString(null, "File", null, desiredSheet.ImgPathLocation);
				writer.WriteAttributeString(null, "AnimationCount", null, desiredSheet.SpriteAnimations.Count.ToString());
				writer.WriteAttributeString(null, "Width", null, desiredSheet.GetPropertyData("width").ToString());
				writer.WriteAttributeString(null, "Height", null, desiredSheet.GetPropertyData("height").ToString());

				//Animation States
				writer.WriteStartElement(null, "AnimationStates", null);

				foreach (SpriteAnimation anim in desiredSheet.SpriteAnimations.Values.ToList())
				{
					writer.WriteStartElement(null, "AnimationState", null);
					writer.WriteAttributeString(null, "Name", null, anim.Name.ToString());
					writer.WriteAttributeString(null, "FPS", null, anim.FPS.ToString());
					writer.WriteAttributeString(null, "StartX", null, anim.StartXPos.ToString());
					writer.WriteAttributeString(null, "StartY", null, anim.StartYPos.ToString());
					writer.WriteAttributeString(null, "NumOfFrames", null, anim.FrameCount.ToString());
					writer.WriteAttributeString(null, "FrameWidth", null, anim.FrameWidth.ToString());
					writer.WriteAttributeString(null, "FrameHeight", null, anim.FrameHeight.ToString());
					writer.WriteAttributeString(null, "isDefault", null, anim.bIsDefualt.ToString());
					writer.WriteEndElement(); //end of the AnimationState Tag

					//Animation Events
					writer.WriteStartElement(null, "Events", null);
					foreach (AnimationEvent animationEvent in anim.GetAnimationEvents())
					{
						if (animationEvent is ChangeAnimationEvent changeanim)
						{
							writer.WriteStartElement(null, "AllowedAnimName", null);
							writer.WriteAttributeString(null, "Name", null, changeanim.ToAnimationName);
							writer.WriteAttributeString(null, "bFinishFirst", null, changeanim.bImmediate.ToString());
							writer.WriteEndElement(); //end of the AllowedAnimName Tag
						}
						else if (animationEvent is AudioEvent soundanim)
						{
							writer.WriteStartElement(null, "SoundEffect", null);
							writer.WriteAttributeString(null, "Value", null, soundanim.SoundEffectName);
							writer.WriteAttributeString(null, "FrameStart", null, soundanim.FrameStart.ToString());
							writer.WriteAttributeString(null, "FrameEnd", null, soundanim.FrameEnd.ToString());
							writer.WriteEndElement(); //end of the SoundEffect Tag
						}
					}
					writer.WriteFullEndElement(); //end of the Events Tag

				}



				writer.WriteEndElement(); //end of the AnimationStates Tag
				writer.WriteEndElement(); //end of the SpriteSheet Tag

			}


			return retbool;
		}

		#endregion

		#region Monogame

		public void LoadContentManager(ContentManager cm)
		{
			this.contentManager = cm;
		}

		public void Update(GameTime gametime)
		{
			CurrentAnimation?.Update(gametime);

			//We queued up an animation change
			if (_nextAnimation != null && CurrentAnimation.bLoopFinished)
			{
				ChangeAnimation(_nextAnimation);
				_nextAnimation = null;
			}

		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if(IsActive)
				CurrentAnimation.Draw(spriteBatch);
		}
		#endregion

		//public void OnLoad(GraphicsDevice graphicsDevice)
		//{
		//	//DEFAULT TESTING I KNOW THAT THERE ARE 2 ROWS/ANIMS
		//	for (int i = 0; i < 2; i++)
		//	{
		//		SpriteAnimation tempSpriteAnimation = new SpriteAnimation(){ FrameWidth = 174, FrameHeight = 324, ParentSheet = this};
		//		for (int j = 174; j < getTexture().Width ; j+= 174)
		//		{
		//			tempSpriteAnimation.FramePositions.AddLast(new LinkedListNode<Vector2>(new Vector2(j, i * 324 )));
		//		}
		//		SpriteAnimations.Add(tempSpriteAnimation);
		//		SpriteAnimations.Last().CurrentFramePosition = SpriteAnimations.Last().FramePositions.First;
		//	}
		//	CurrentAnimation = SpriteAnimations[0];

		//}

		//public void Update(GameTime gameTime)
		//{
		//	CurrentAnimation.Update(gameTime);
		//}

		//public void Draw(SpriteBatch spriteBatch, int i)
		//{
		//	spriteBatch.Draw(text, new Vector2(100, 100), new Rectangle((int)CurrentAnimation.CurrentFramePosition.Value.X, (int)CurrentAnimation.CurrentFramePosition.Value.Y, 174, 324), Color.White, 0.0f,
		//		new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0);
		//	//Draw_Crop(spriteBatch, 0,0, (int)CurrentAnimation.CurrentFramePosition.Value.X, (int)CurrentAnimation.CurrentFramePosition.Value.Y, CurrentAnimation.FrameWidth, CurrentAnimation.FrameHeight);
		//}


		/// <summary>
		/// 
		/// </summary>
		public override void Draw_Crop(SpriteBatch sb, int posx, int posy, int x, int y, int w, int h)
		{
			base.Draw_Crop(sb, posx, posy, x, y, w, h);
		}

		public override void Draw_Crop_Scale(SpriteBatch sb, int posx, int posy, int x, int y, int w, int h, double sx, double sy)
		{
			base.Draw_Crop_Scale(sb, posx, posy, x, y, w, h, sx, sy);
		}
	}
}
