using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Rendering
{
	public class Camera
	{
		private Matrix transform;
		public Matrix Transform
		{
			get { return transform; }
		}

		private Vector2 center;
		private Viewport viewport;

		public Camera(Viewport newViewport)
		{
			viewport = newViewport;
		}

		public void Update(Vector2 position, int xOffset, int yOffset)
		{
			if (position.X < viewport.Width / 2) //stops camera left side of screen/map
				center.X = viewport.Width / 2;
			else if (position.X > xOffset - (viewport.Width / 2)) //stops camera right side of screen/map
				center.X = xOffset - viewport.Width / 2;
			else //camera follows.
				center.X = position.X;

			if (position.Y < viewport.Height / 2) //stops camera left side of screen/map
				center.Y = viewport.Height / 2;
			else if (position.Y > yOffset - (viewport.Height / 2)) //stops camera right side of screen/map
				center.Y = yOffset - viewport.Height / 2;
			else //camera follows.
				center.Y = position.Y;


			transform = Matrix.CreateTranslation(new Vector3(-center.X + (viewport.Width / 2),
																											 -center.Y + (viewport.Height / 2),0));



		}
	}
}
