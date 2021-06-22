using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BixBite.Resources;

namespace BixBite.Rendering.UI
{
	public abstract class BaseUIComponent  : IProperties
	{
		protected ObservableCollection<Tuple<string, object>> Properties { get; set; }

		public String UIName { get; set; }

		public bool bIsMoving { get; set; }

		/// <summary>
		/// Should we draw this to the screen?
		/// </summary>
		public virtual bool bIsActive { get; set; }
		
		public int ZIndex { get; set; }
		public int XPos { get; set; }
		public int YPos { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }

		//public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

		//public abstract void Update(GameTime gameTime);
		public abstract void SetNewProperties(ObservableCollection<Tuple<string, object>> NewProperties);
		public abstract void ClearProperties();
		public abstract void SetProperty(string Key, object Property);
		public abstract void AddProperty(string Key, object data);
		public abstract object GetPropertyData(string Key);
		public abstract Tuple<string, object> GetProperty(string Key);
		public abstract ObservableCollection<Tuple<string, object>> GetProperties();
		public abstract int GetPropertyIndex(string Key);
	}
}
