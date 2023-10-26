using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using AmethystEngine.Forms;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using MonoGame.Framework.Content.Pipeline.Builder;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace AmethystEngine.Components.Dolomite
{
	public class MonoGameContentBuilder
	{
		private enum EMonoGameAssetType
		{
			NONE  = 0,
			IMAGE = 1,
			SOUND = 2,
			FONT  = 3,
			EFFECT= 4,
		}

		private class MonoGameAssetData
		{
			public String Name { get; set; }
			public String AssetPath { get; set; }

			/// name, value of parameter
			public List<Tuple<String, String>> Parameters  = new List<Tuple<string, string>>();
		}

		private ContentManager _contentManager;
		private GraphicsDevice _graphicsDevice;
		private PipelineManager _pipelineManager;



		/// <summary>
		/// I'm not sure if there is a way to auto change the content file when adding
		/// so for now i'm just going to change the file manually.
		/// </summary>
		/// <param name="editorMonoGameAssetFilePath"></param>
		/// <param name="monogameContentBuilderPath"></param>
		/// <returns></returns>
		private bool UpdateMonogameContentBuilderFiles(String editorMonoGameAssetFilePath, String monogameContentBuilderPath, 
			MonoGameAssetData newAssetData)
		{
			bool updateStatus = false;
			Dictionary<String, String> monoGameParameters = new Dictionary<string, string>();
			List<String> monogameReferences = new List<string>();
			List<MonoGameAssetData> monoGameAssets = new List<MonoGameAssetData>();

			// First read in the global properties. 
			XmlReaderSettings settings = new XmlReaderSettings
			{
				//Async = true
			};

			using (XmlReader reader = XmlReader.Create(editorMonoGameAssetFilePath, settings))
			{
				while (reader.Read())
				{
					//skip to a mgcb node
					while (reader.Name != "mgcb")
						reader.Read();

					//by this time we have found the dialogue scene tag

					//skip to a Global node
					while (reader.Name != "Global")
						reader.Read();

					// At this point we need to read in the parameters. 
					do
					{
						reader.Read();
						if (reader.Name == "Parameter" && reader.NodeType == XmlNodeType.Element)
						{
							monoGameParameters.Add(reader.GetAttribute("Name"), reader.GetAttribute("Value"));
						}
					} while (reader.Name.Trim() != "Global");

					while (reader.Name != "References")
						reader.Read();

					do
					{
						reader.Read();
						if (reader.Name == "Reference" && reader.NodeType == XmlNodeType.Element)
						{
							monogameReferences.Add(reader.GetAttribute("Path"));
						}
					} while (reader.Name.Trim() != "References");

					while (reader.Name != "Content")
						reader.Read();

					do
					{
						reader.Read();
						if (reader.Name == "Asset" && reader.NodeType == XmlNodeType.Element)
						{
							MonoGameAssetData assetData = new MonoGameAssetData();
							assetData.Name = reader.GetAttribute("Name");
							assetData.AssetPath = reader.GetAttribute("Path");
							do
							{
								reader.Read();
								if (reader.Name == "Parameter" && reader.NodeType == XmlNodeType.Element)
								{
									assetData.Parameters.Add(new Tuple<string, string>(
										reader.GetAttribute("Name"), reader.GetAttribute("Value")));
								}
							} while (reader.Name.Trim() != "Asset");
							monoGameAssets.Add(assetData);
						}
					} while (reader.Name.Trim() != "Content");

					// Kick out
					while (!reader.EOF)
						reader.Read();
					break;
				}

			}

			// Now we need to add the NEW ASSET to the arrays.
			if (monoGameAssets.Find(x => x.AssetPath == newAssetData.AssetPath) != null)
			{
				return false;
			}
			monoGameAssets.Add(newAssetData);

			// Now we need to update the actual MonoGame builder file.
			UpdateMGCBFile(monogameContentBuilderPath, monoGameParameters, monogameReferences, monoGameAssets);
			UpdateContentConfigFile(editorMonoGameAssetFilePath, monoGameParameters, monogameReferences, monoGameAssets);

			return updateStatus = true;
		}

		private bool UpdateContentConfigFile(String filePath, Dictionary<String, String> monoGameParameters, List<String> monogameReferences,
			List<MonoGameAssetData> monoGameAssets)
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
				// Start the file
				writer.WriteStartElement(null, "mgcb", null);

				writer.WriteStartElement(null, "Global", null);
				foreach (var parameter in monoGameParameters)
				{
					writer.WriteStartElement(null, "Parameter", null);
					writer.WriteAttributeString(null, "Name", null, parameter.Key);
					writer.WriteAttributeString(null, "Value", null, parameter.Value);
					writer.WriteFullEndElement();//end of Parameter tag
				}
				writer.WriteFullEndElement();//end of mgcb tag

				writer.WriteStartElement(null, "References", null);
				foreach (var reference in monogameReferences)
				{
					writer.WriteStartElement(null, "Reference", null);
					writer.WriteAttributeString(null, "Path", null, reference);
					writer.WriteFullEndElement();//end of Reference tag
				}
				writer.WriteFullEndElement();//end of References tag

				writer.WriteStartElement(null, "Content", null);
				foreach (var asset in monoGameAssets)
				{
					writer.WriteStartElement(null, "Asset", null);
					writer.WriteAttributeString(null, "Type", null, "Image");
					writer.WriteAttributeString(null, "Name", null, asset.Name);
					writer.WriteAttributeString(null, "Path", null, asset.AssetPath);

					foreach (var param in asset.Parameters)
					{
						writer.WriteStartElement(null, "Parameter", null);
						writer.WriteAttributeString(null, "Name", null, param.Item1);
						writer.WriteAttributeString(null, "Value", null, param.Item2);
						writer.WriteFullEndElement();//end of parameter tag
					}
					writer.WriteFullEndElement();//end of Asset tag
				}
				writer.WriteFullEndElement();//end of Content tag

			}
			return true;
		}

		private bool UpdateMGCBFile(String filePath, Dictionary<String, String> monoGameParameters, List<String> monogameReferences,
			List<MonoGameAssetData> monoGameAssets)
		{
			bool fileWriteStatus = false;
			List<String> OutputFileLines = new List<string>();
			string pattern = "[^a-zA-Z0-9 =,]"; // This pattern matches any character that is not a letter, digit, or space.


			// Header
			OutputFileLines.Add("#----------------------------- Global Properties ----------------------------#");
			OutputFileLines.Add("");
			// we need to output all the global parameters!
			foreach (var param in monoGameParameters)
			{
				OutputFileLines.Add(String.Format("/{0}:{1}", param.Key, param.Value ));
			}

			OutputFileLines.Add("");
			OutputFileLines.Add("#----------------------------- References ----------------------------#");
			// Add all reference here!
			foreach (var reference in monogameReferences)
			{
				OutputFileLines.Add(String.Format("/{0}:{1}", "reference", reference));
			}

			OutputFileLines.Add("");
			OutputFileLines.Add("#----------------------------- Content ----------------------------#");
			// Time to add all the assets!
			foreach (var asset in monoGameAssets)
			{
				OutputFileLines.Add( String.Format("#begin {0}",
					((asset.AssetPath).Replace(Path.GetExtension(asset.AssetPath), "")) + ".xnb"));
				
				// UNCOMMENT IF YOU WANT TO MAKE THE MONOGAME FILE BUILDABLE AGAIN!!!
				//foreach (var parameter in asset.Parameters)
				//{
				//	OutputFileLines.Add(String.Format("/{0}:{1}", parameter.Item1, Regex.Replace(parameter.Item2, pattern, "" )));
				//}
				OutputFileLines.Add(String.Format("/copy:{0}",
					((asset.AssetPath).Replace(Path.GetExtension(asset.AssetPath), "")) + ".xnb"));
				OutputFileLines.Add("");

			}

			File.WriteAllLines(filePath, OutputFileLines);
			fileWriteStatus = true;

			return fileWriteStatus;
		}

		public MonoGameContentBuilder(String projPath)
		{
			//_contentManager = new ContentManager(new ServiceProviders());
			//GraphicsAdapter graphicsAdapter = new GraphicsAdapter();
			// PresentationParameters parameters = new PresentationParameters();
			//_graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.HiDef, parameters);
			// _pipelineManager = new PipelineManager(projPath, projPath, projPath);
		}


		public bool AttemptToBuildPNGToXNBFile(String pngPath, String editorDirectory, String finalDirectory, String editorMonoGameAssetFilePath, String monoGameContentFilePath)
		{
			bool returnStatus = false;

			OpaqueDataDictionary keyValues = new OpaqueDataDictionary();
			keyValues.Add("Importer", "TextureImporter");
			keyValues.Add("Processor", "TextureProcessor");
			keyValues.Add("ColorKeyColor", new Color(255, 0, 255, 255));
			keyValues.Add("ColorKeyEnabled", true);
			keyValues.Add("GenerateMipmaps", false);
			keyValues.Add("PremultiplyAlpha", true);
			keyValues.Add("ResizeToPowerOfTwo", false);
			keyValues.Add("MakeSquare", false);
			keyValues.Add("TextureFormat", "Color");
			ContentBuildLogger logger = new Logger();

			string input = pngPath;
			string inputFileName = pngPath.Substring(pngPath.LastIndexOf("\\")+1, pngPath.LastIndexOf(".") - pngPath.LastIndexOf("\\") -1);
			string inputDir = input.Substring(0, input.LastIndexOf("\\"));
			string output = String.Format("{0}\\{1}", finalDirectory, "output\\");

			string pathA = editorDirectory + "\\";
			string pathC = finalDirectory + "\\";

			Console.WriteLine("XNB Converter > [INFO] Creating required Directories.");
			Directory.CreateDirectory(editorDirectory);
			Console.WriteLine("XNB Converter > [INFO] DONE Creating required Directories.");


			Console.WriteLine("XNB Converter > [INFO] Copying Files.");
			if(!File.Exists(String.Format("{0}/{1}", pathA, Path.GetFileName(pngPath))))
				File.Copy(pngPath, String.Format("{0}/{1}", pathA, Path.GetFileName(pngPath)));
			Console.WriteLine("XNB Converter > [INFO] DONE Copying Files.");

			PipelineManager manager = new PipelineManager(pathA, pathC, pathC)
			{
				RethrowExceptions = true,
				CompressContent = true,
				Logger = logger,
				Platform = TargetPlatform.Windows,
				Profile = GraphicsProfile.Reach
			};
			manager.BuildContent((pathA + Path.GetFileName(pngPath)), (pathC + inputFileName), "TextureImporter", "TextureProcessor", keyValues);

			// we need Add the new asset to the amethyst engine config file for assets
			MonoGameAssetData newAssetData = new MonoGameAssetData();
			newAssetData.Name = inputFileName;
			newAssetData.AssetPath = (pathC + Path.GetFileName(pngPath)).Replace(Directory.GetParent(monoGameContentFilePath).FullName, "");
			newAssetData.AssetPath = newAssetData.AssetPath.Replace("\\", "/").Substring(1);
			foreach (var paramKeyValue in keyValues)
			{
				if(paramKeyValue.Key.ToLower() == "importer" || paramKeyValue.Key.ToLower() == "processor")
					newAssetData.Parameters.Add(new Tuple<string, string>(paramKeyValue.Key.ToLower(), paramKeyValue.Value.ToString()));
				else
				{
					if (paramKeyValue.Key.ToLower() == "colorkeycolor")
					{
						Color? color = paramKeyValue.Value as Color?;
						String colorCode = String.Format("{0},{1},{2},{3}", color?.R, color?.G, color?.B, color?.A );
						newAssetData.Parameters.Add(new Tuple<string, string>("processorParam", paramKeyValue.Key.ToLower() + "=" + colorCode));
					}
					else
						newAssetData.Parameters.Add(new Tuple<string, string>("processorParam", paramKeyValue.Key.ToLower() + "=" + paramKeyValue.Value.ToString()));
				}
			}

			UpdateMonogameContentBuilderFiles(editorMonoGameAssetFilePath, monoGameContentFilePath, newAssetData);

			//// Step 1: Load Raw Image
			//Texture2DContent texture2DContent = new Texture2DContent();
			//texture2DContent.Identity = new ContentIdentity(pngPath);
			//texture2DContent.Mipmaps.Add(LoadPngAsBitmapContent(_graphicsDevice, pngPath));

			//// Step 2: Apply Texture Processor
			//TextureProcessor textureProcessor = new TextureProcessor();
			//TextureContent processedTexture = textureProcessor.Process(texture2DContent,
			//	new PipelineProcessorContext(_pipelineManager, PipelineBuildEvent.Load(pngPath)));

			//// Step 3: Save as XNB
			//String directoryPath = Directory.GetDirectoryRoot(pngPath);
			//using (FileStream stream = new FileStream(directoryPath, FileMode.Create))
			//{
			//	IntermediateSerializer.Serialize(XmlWriter.Create(monoGameContentFilePath), manager, directoryPath);
			//}

			//// Step 4: Have celebratory sex with your girlfriend
			//penis.position = setLocation(inside_gf);
			//bool cum = (warmth + GRIP)^moans ? true : false;



			return returnStatus;
		}

		public static BitmapContent LoadPngAsBitmapContent(GraphicsDevice graphicsDevice, string pngFilePath)
		{
			BitmapContent bitmapContent = null;

			using (FileStream stream = new FileStream(pngFilePath, FileMode.Open))
			{
				Texture2D texture = Texture2D.FromStream(graphicsDevice, stream);

				bitmapContent = CreateBitmapContentFromTexture(texture);

				//// Create a RenderTarget2D and draw the texture onto it
				//RenderTarget2D renderTarget = new RenderTarget2D(
				//	graphicsDevice,
				//	texture.Width,
				//	texture.Height,
				//	false,
				//	SurfaceFormat.Color,
				//	DepthFormat.None
				//);

				//graphicsDevice.SetRenderTarget(renderTarget);
				//graphicsDevice.Clear(Microsoft.Xna.Framework.Color.Transparent);

				//SpriteBatch spriteBatch = new SpriteBatch(graphicsDevice);
				//spriteBatch.Begin();
				//if (texture != null)
				//{
				//	spriteBatch.Draw(texture, Vector2.Zero,Microsoft.Xna.Framework.Color.White);
				//	spriteBatch.End();

				//	graphicsDevice.SetRenderTarget(null);

				//	// Extract the pixel data as BitmapContent
				//	byte[] pictureData;
				//	texture.GetData);
				//	bitmapContent.SetPixelData(texture.GetData());

				// Dispose of resources
				texture.Dispose();
				//}

				//renderTarget.Dispose();
			}

			return bitmapContent;
		}

		public static BitmapContent CreateBitmapContentFromTexture(Texture2D texture)
		{
			// Create a new BitmapContent with the same dimensions as the texture
			BitmapContent bitmapContent = new PixelBitmapContent<Color>(texture.Width, texture.Height);

			// Get the pixel data from the texture
			Color[] pixels = new Color[texture.Width * texture.Height];
			texture.GetData(pixels);

			// Convert the Color[] array to a byte array
			byte[] pixelBytes = new byte[pixels.Length * 4]; // 4 bytes per color (RGBA)
			for (int i = 0; i < pixels.Length; i++)
			{
				pixelBytes[i * 4 + 0] = pixels[i].R;
				pixelBytes[i * 4 + 1] = pixels[i].G;
				pixelBytes[i * 4 + 2] = pixels[i].B;
				pixelBytes[i * 4 + 3] = pixels[i].A;
			}

			// Set the pixel data in the BitmapContent
			// bitmapContent.SetPixelData(pixels);

			return bitmapContent;
		}
	}

	class Logger : ContentBuildLogger
	{
		public Logger()
		{

		}

		public override void LogImportantMessage(string message, params object[] messageArgs)
		{
			string a = "";
			for (int i = 0; i < messageArgs.Length; i++) a += messageArgs[i] + " ";
			Console.WriteLine("XNB Converter > [IMPORTANT]" + message + " " + a);
		}

		public override void LogMessage(string message, params object[] messageArgs)
		{
			string a = "";
			for (int i = 0; i < messageArgs.Length; i++) a += messageArgs[i] + " ";
			Console.WriteLine("XNB Converter > [INFO]" + message + " " + a);
		}

		public override void LogWarning(string helpLink, ContentIdentity contentIdentity, string message,
			params object[] messageArgs)
		{
			string a = "";
			for (int i = 0; i < messageArgs.Length; i++) a += messageArgs[i] + " ";
			Console.WriteLine("XNB Converter > [WARNING]" + message + " " + a);
		}
	}

}
