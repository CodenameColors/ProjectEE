using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using BixBite.Resources;

namespace AmethystEngine.Components
{
  public enum EObjectType
  {
    None,
    Folder,
    File,
		Sprite,
		GameEvent
  };


  public class EditorObject
  {

		public Uri Thumbnail { get; set; }
    public String Name { get; set; }
    public String ContentPath { get; set; }
		public int Width { get; set; }
    public EObjectType EditObjType;

		


    public EditorObject()
    {
      //Thumbnail.Stretch = Stretch.Fill;
    }

    public EditorObject(String contentPath, String desimg, String desName, bool rel = true, EObjectType eot = EObjectType.None)
    {
			this.ContentPath = contentPath;

			if (rel)
      {
        Thumbnail = new Uri(desimg, UriKind.Relative);
        Name = desName;
        EditObjType = eot;
      }
      else
      {
        Thumbnail = new Uri(desimg, UriKind.Absolute);
        Name = desName;
        EditObjType = eot;
      }
      Width = 300;
    }

		public void SetThumbnail(String desimg, bool rel = true)
		{
			if (rel)
			{
				Thumbnail = new Uri(desimg, UriKind.Relative);
			}
			else
			{
				Thumbnail = new Uri(desimg, UriKind.Absolute);
			}
			Width = 300;
		}

    public override string ToString()
    {
      return this.Name;
    }
  }
}
