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
		public ObservableCollection<SpriteLayer> layers = new ObservableCollection<SpriteLayer>();
		public List<Tuple<String, String>> Properties = new List<Tuple<string, string>>();

		public Level(String desname)
		{
			LevelName = desname;
		}
		public void AddLayer(String LayerName, LayerType deslayertype)
		{
			layers.Add(new SpriteLayer(deslayertype) { LayerName = LayerName });
		}

		public void ImportLevel()
		{

		}

		public void ExportLevel()
		{

		}

	}
}
