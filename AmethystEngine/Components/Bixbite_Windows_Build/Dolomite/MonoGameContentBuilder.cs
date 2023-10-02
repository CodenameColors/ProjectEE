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
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Content.Pipeline.Builder;
using Color = Microsoft.Xna.Framework.Color;

namespace BixBite.Dolomite
{
	public class MonoGameContentBuilder
	{

		private ContentManager _contentManager;
		private GraphicsDevice _graphicsDevice;
		private PipelineManager _pipelineManager;

		public MonoGameContentBuilder(String projPath)
		{
			_contentManager = new ContentManager(new ServiceProviders());
			GraphicsAdapter graphicsAdapter = new GraphicsAdapter();
			PresentationParameters parameters = new PresentationParameters();
			_graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.HiDef, parameters);
			_pipelineManager = new PipelineManager(projPath, projPath, projPath);
		}

		public bool AttemptToBuildPNGToXNBFile(String pngPath, String monoGameContentFilePath)
		{
			bool returnStatus = false;

			// Step 1: Load Raw Image
			Texture2DContent texture2DContent = new Texture2DContent();
			texture2DContent.Identity = new ContentIdentity(pngPath);
			texture2DContent.Mipmaps.Add(LoadPngAsBitmapContent(_graphicsDevice, pngPath));

			// Step 2: Apply Texture Processor
			TextureProcessor textureProcessor = new TextureProcessor();
			TextureContent processedTexture = textureProcessor.Process(texture2DContent, new PipelineProcessorContext(_pipelineManager, PipelineBuildEvent.Load(pngPath)));

			// Step 3: Save as XNB
			String directoryPath = Directory.GetDirectoryRoot(pngPath);
			using (FileStream stream = new FileStream(directoryPath, FileMode.Create))
			{
				IntermediateSerializer.Serialize(XmlWriter.Create(monoGameContentFilePath), processedTexture, directoryPath);
			}



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
			bitmapContent.SetPixelData(pixels);

			return bitmapContent;
		}
	}
}
