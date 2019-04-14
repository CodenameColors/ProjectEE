using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace AmethystEngine
{
  public class XMLManager<T>
  {
    public Type Type;

    //takes in a path of xml file, and load it. Takes in serialized data and deserializes it for us.
    public T Load(String path)
    {
      T instance;
      byte[] bytearray = Encoding.UTF8.GetBytes(path);
      XmlSerializer serializer = new XmlSerializer(typeof(T));

      //using (TextReader reader = new StreamReader(new MemoryStream(bytearray)))

      using (FileStream fileStream = new FileStream(path, FileMode.Open))
      {
        XmlSerializer xml = new XmlSerializer(this.GetType());
        instance = (T)serializer.Deserialize(fileStream);
      }
      return instance;
    }
    //
    public void save(String path, object obj)
    {
      byte[] bytearray = Encoding.UTF8.GetBytes(path);
      using (TextWriter writer = new StreamWriter(new MemoryStream(bytearray)))
      {
        XmlSerializer xml = new XmlSerializer(Type);
        xml.Serialize(writer, obj);
      }
    }
  }
}