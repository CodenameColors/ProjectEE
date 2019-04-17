using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BixBite.Rendering;

namespace BixBite
{
	public class Level
	{
		//instance variables
		public String LevelName = "";
		public List<SpriteLayer> layers = new List<SpriteLayer>();
		public Property Properties = new Property();

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
