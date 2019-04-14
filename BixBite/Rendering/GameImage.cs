using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Xml.Serialization;

namespace BixBite.Rendering
{
	class GameImage
	{

		//public float Alpha = 1.0f;
		//public string Text, FontName, Path;
		//public bool IsActive;


		//public Vector2 Position, Scale;
		//[XmlIgnore]
		//public Texture2D Texture;
		//Vector2 origin;
		//public Rectangle SourceRect = Rectangle.Empty;
		//RenderTarget2D RenderTarget;
		//ContentManager content;
		//SpriteFont SpriteFont;

		//public FadeEffect FadeEffect;

		//Dictionary<String, ImageEffect> effecDict = new Dictionary<string, ImageEffect>();
		//public String Effect;

		//public Image()
		//{
		//	FontName = "Orbitron";
		//	Position = Vector2.Zero;
		//	Scale = Vector2.One;
		//	Path = Effect = Text = String.Empty;
		//	Text = "memes";
		//}

		//public void setEffect<T>(ref T Effect) //where T:ImageEffect
		//{
		//	if (Effect == null)
		//	{
		//		Effect = (T)Activator.CreateInstance(typeof(T));
		//	}
		//	else
		//	{
		//		(Effect as ImageEffect).IsActive = true;
		//		var obj = this;
		//		(Effect as ImageEffect).LoadContent(ref obj);

		//	}
		//	effecDict.Add(Effect.GetType().ToString().Replace("GameTest.Components.", ""), (Effect as ImageEffect));
		//}

		//public void ActivateEffect(String Effect)
		//{
		//	if (effecDict.ContainsKey(Effect))
		//	{
		//		effecDict[Effect].IsActive = true;
		//		var obj = this;
		//		effecDict[Effect].LoadContent(ref obj);
		//	}
		//}

		//public void DeactivateEffect(String Effect)
		//{
		//	if (effecDict.ContainsKey(Effect))
		//	{
		//		effecDict[Effect].IsActive = false;
		//		effecDict[Effect].UnloadContent();
		//	}
		//}

		//public void LoadContent()
		//{
		//	content = new ContentManager(ScreenManager.Instance.Content.ServiceProvider, "Content");
		//	if (Path != String.Empty) { Texture = content.Load<Texture2D>(Path); }

		//	SpriteFont = content.Load<SpriteFont>(FontName);
		//	Vector2 dims = Vector2.Zero;
		//	if (Texture != null)
		//	{
		//		dims.X += Texture.Width; dims.Y +=
  // (SpriteFont.MeasureString(Text).Y > Texture.Height ? SpriteFont.MeasureString(Text).Y : Texture.Height);
		//	}
		//	dims.X += SpriteFont.MeasureString(Text).X;

		//	if (SourceRect == Rectangle.Empty) { SourceRect = new Rectangle((int)Position.X, (int)Position.Y, (int)dims.X, (int)dims.Y); }

		//	//set render target and then render with it
		//	RenderTarget = new RenderTarget2D(ScreenManager.Instance.GraphicsDevice, (int)dims.X, (int)dims.Y);
		//	ScreenManager.Instance.GraphicsDevice.SetRenderTarget(RenderTarget);
		//	ScreenManager.Instance.GraphicsDevice.Clear(Color.Transparent);
		//	ScreenManager.Instance.SpriteBatch.Begin();
		//	if (Texture != null) { ScreenManager.Instance.SpriteBatch.Draw(Texture, Vector2.Zero, Color.White); }
		//	ScreenManager.Instance.SpriteBatch.DrawString(SpriteFont, Text, Vector2.Zero, Color.White);
		//	ScreenManager.Instance.SpriteBatch.End();

		//	IsActive = true;

		//	Texture = RenderTarget; ScreenManager.Instance.GraphicsDevice.SetRenderTarget(null);

		//	setEffect<FadeEffect>(ref FadeEffect);

		//	if (Effect != String.Empty)
		//	{
		//		String[] tempS = Effect.Split(':');
		//		foreach (String item in tempS)
		//		{
		//			ActivateEffect(item);
		//		}
		//	}
		//}

		//public void UnloadContent()
		//{
		//	content.Unload();
		//	foreach (var effect in effecDict)
		//	{
		//		effect.Value.UnloadContent();
		//		effect.Value.IsActive = false;
		//	}

		//}

		//public void Update(GameTime gameTime)
		//{
		//	foreach (var effect in effecDict)
		//	{
		//		effect.Value.Update(gameTime);
		//		effect.Value.IsActive = true;
		//	}
		//}

		//public void Draw(SpriteBatch spriteBatch)
		//{
		//	origin = new Vector2(SourceRect.Width / 2, SourceRect.Height / 2);
		//	if (Texture != null)
		//	{
		//		spriteBatch.Draw(Texture, Position + origin, SourceRect, Color.White * Alpha, 0.0f, origin, Scale, SpriteEffects.None, 0.0f);
		//	}
		//	else
		//	{
		//		content = new ContentManager(ScreenManager.Instance.Content.ServiceProvider, "Content");
		//		if (Path != String.Empty) { Texture = content.Load<Texture2D>(Path); }
		//		spriteBatch.Draw(Texture, Position + origin, SourceRect, Color.White * Alpha, 0.0f, origin, Scale, SpriteEffects.None, 0.0f);
		//		//testing remove this later
		//		Alpha = 1.0f;
		//	}
		//}

	}
}
