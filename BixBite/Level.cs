using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BixBite.Rendering;

namespace BixBite
{
	public class Level
	{
		//instance variables
		public String LevelName { get; set; }
		public ObservableCollection<SpriteLayer> Layers { get; set; }
		public Dictionary<String, object> Properties = new Dictionary<string, object>();

		public Level(String desname)
		{
			LevelName = desname;
			Layers = new ObservableCollection<SpriteLayer>();
		}
		public void AddLayer(String LayerName, LayerType deslayertype)
		{
			Layers.Add(new SpriteLayer(deslayertype) { LayerName = LayerName });
		}

		public void ImportLevel()
		{

		}

		public void ExportLevel()
		{

		}

	}
}
