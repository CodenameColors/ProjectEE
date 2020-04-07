using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NodeEditor;

namespace BixBite.Rendering
{
	public class SpriteSheet : Sprite
	{

		#region Fields

		#endregion

		#region Properties

		public List<SpriteAnimation> SpriteAnimations = new List<SpriteAnimation>();
		public SpriteAnimation CurrentAnimation = new SpriteAnimation();

		#endregion

		public SpriteSheet()
		{

		}

		public SpriteSheet(string Name, string imgLoc, int x, int y, int w, int h) : base(Name, imgLoc, x, y, w, h)
		{
			
		}

		public void OnLoad(GraphicsDevice graphicsDevice)
		{
			//DEFAULT TESTING I KNOW THAT THERE ARE 2 ROWS/ANIMS
			for (int i = 0; i < 2; i++)
			{
				SpriteAnimation tempSpriteAnimation = new SpriteAnimation(){ FrameWidth = 155, FrameHeight = 324, ParentSheet = this};
				for (int j = 174; j < getTexture().Width -174; j+= 174)
				{
					tempSpriteAnimation.FramePositionOffsets.AddLast(new LinkedListNode<Vector2>(new Vector2(j, i * 324 )));
				}
				SpriteAnimations.Add(tempSpriteAnimation);
				SpriteAnimations.Last().CurrentOffsetNodeData = SpriteAnimations.Last().FramePositionOffsets.First;
			}
			CurrentAnimation = SpriteAnimations[0];
			
		}

		public void Update(GameTime gameTime)
		{
			CurrentAnimation.Update(gameTime);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(text, new Vector2(100, 100), new Rectangle((int)CurrentAnimation.CurrentOffsetNodeData.Value.X, (int)CurrentAnimation.CurrentOffsetNodeData.Value.Y, 174, 324), Color.White, 0.0f, 
				new Vector2(0,0), new Vector2(1, 1), SpriteEffects.None, 0);
			//Draw_Crop(spriteBatch, 0,0, (int)CurrentAnimation.CurrentOffsetNodeData.Value.X, (int)CurrentAnimation.CurrentOffsetNodeData.Value.Y, CurrentAnimation.FrameWidth, CurrentAnimation.FrameHeight);
		}
		

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
