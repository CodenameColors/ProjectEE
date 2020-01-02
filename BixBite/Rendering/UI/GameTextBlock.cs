using BixBite.Resources;
using System;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Rendering.UI
{
	public class GameTextBlock : GameUI, IProperties
	{


		#region Fields

		private SpriteFont _font;

		private Texture2D _texture;

		#endregion

		#region Properties

		public event EventHandler Click;

		public Color PenColour { get; set; }

		public Vector2 Position { get; set; }

		public Rectangle Rectangle
		{
			get
			{
				return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
			}
		}

		public string Text { get; set; }

		#endregion


		public String TBName { get; set; } 

		public GameTextBlock(string UIName, int Width, int Height, int xoff, int yoff, int Zindex ,string BackgroundPath = "#00000000", String Text= "") 
			: base(UIName, Width, Height, Zindex ,BackgroundPath)
		{
			TBName = UIName;
			AddProperty("Xoffset", xoff);
			AddProperty("YOffset", yoff);
			AddProperty("ContentText", Text);
			AddProperty("Font", "Ariel");
			AddProperty("FontSize", 24);
			AddProperty("FontColor", "Black");
			AddProperty("FontStyle", "Normal");
			AddProperty("TextSpeed", 1.0);
			AddProperty("TextTime", 1.0);//number of seconds to "type" the text to the screen
			AddProperty("Image", "");
		}

		public override void PropertyCallback(object sender, RoutedEventArgs e)
		{

		}




	}
}
