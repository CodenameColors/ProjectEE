using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Navigation;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using MonoGame.Framework.Content.Pipeline.Builder;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Content.Pipeline.Builder;
using Color = Microsoft.Xna.Framework.Color;

namespace AmethystEngine.Components.Dolomite
{
	public class MonoGameContentBuilder
	{

		private ContentManager _contentManager;
		private GraphicsDevice _graphicsDevice;
		private PipelineManager _pipelineManager;

		public MonoGameContentBuilder(String projPath)
		{
			//_contentManager = new ContentManager(new ServiceProviders());
			//GraphicsAdapter graphicsAdapter = new GraphicsAdapter();
			// PresentationParameters parameters = new PresentationParameters();
			//_graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.HiDef, parameters);
			// _pipelineManager = new PipelineManager(projPath, projPath, projPath);
		}

		public bool AttemptToBuildPNGToXNBFile(String pngPath, String finalDirectory, String monoGameContentFilePath)
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

			string pathA = string.Format("{0}\\source", finalDirectory);
			string pathC = string.Format("{0}\\final", finalDirectory);

			Console.WriteLine("XNB Converter > [INFO] Creating temporary directories.");
			if (!Directory.Exists(pathA)) Directory.CreateDirectory(pathA);
			if (!Directory.Exists(pathC)) Directory.CreateDirectory(pathC);
			Console.WriteLine("XNB Converter > [INFO] Finished creating temporary directories.");

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
			manager.BuildContent((pathA + "\\" + Path.GetFileName(pngPath)), (pathC + "\\" + inputFileName), "TextureImporter", "TextureProcessor", keyValues);

			change the directories to use the EDITOR, and the GAME content -> images.So we don't have to do the final and source shit

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
