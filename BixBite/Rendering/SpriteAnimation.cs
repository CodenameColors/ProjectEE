using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Rendering
{
	public class SpriteAnimation
	{

		#region Fields

		public SpriteSheet ParentSheet;

		public int FrameWidth;
		public int FrameHeight;

		public int NumberOfRows;

		#endregion

		#region Properties

		public double FPSTimePeriod = 1 / 12.0f;
		public double TimeBetweenFrames = 0.0f;
		public int NumberOfFrames { get; set; }

		public Vector2 RelativeOrigin { get; set; }

		public LinkedList<Vector2> FramePositionOffsets = new LinkedList<Vector2>();
		public LinkedListNode<Vector2> CurrentOffsetNodeData { get; set; }

		#endregion

		public SpriteAnimation()
		{

		}

		public void Update(GameTime gameTime)
		{
			TimeBetweenFrames += gameTime.ElapsedGameTime.Milliseconds;
			if (TimeBetweenFrames > FPSTimePeriod * 1000 && CurrentOffsetNodeData != null)
			{
				CurrentOffsetNodeData = (CurrentOffsetNodeData.Next == null ? FramePositionOffsets.First : CurrentOffsetNodeData.Next);
				TimeBetweenFrames = 0;
			}
		}

		public void Draw( SpriteBatch spriteBatch)
		{
			
		}


	}
}

