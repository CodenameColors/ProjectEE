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


namespace AmethystEngine.Components
{
  public enum EObjectType
  {
    None,
    Folder,
    File
  };


  public class EditorObject
  {
    public Uri Thumbnail { get; set; }
    public String Name { get; set; }
    public int width { get; set; }
    public EObjectType EditObjType;

    public EditorObject()
    {
      //Thumbnail.Stretch = Stretch.Fill;
    }

    public EditorObject(String desimg, String desName, bool rel = true, EObjectType eot = EObjectType.None)
    {
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
      width = 300;
    }

    public override string ToString()
    {
      return this.Name;
    }
  }
}
