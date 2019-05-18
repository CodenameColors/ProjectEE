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

		/// <summary>
		/// This method is here to add a new sprite layer to the current level object
		/// </summary>
		/// <param name="LayerName">The name of this layer</param>
		/// <param name="deslayertype">The type of layer we are adding</param>
		public void AddLayer(String LayerName, LayerType deslayertype)
		{
			int val = 0;
			for (int i = 0; i < Layers.Count; i++)
			{
				if(Layers[i].LayerName == LayerName)
				{
					val++;
					LayerName += val;
					i = 0; //reset. This will make sure to check ALL layers even  already processed ones.
				}
			}
			Layers.Add(new SpriteLayer(deslayertype, this) { LayerName = LayerName }); //add the layer
		}
		
		/// <summary>
		/// Finds the SpriteLayer in the current levels layers array. 
		/// </summary>
		/// <param name="DesName">The name of the current layer.</param>
		/// <returns>TRUE: index of the SpriteLayer			FALSE: -1</returns>
		public int FindLayerindex(String DesName)
		{
			for(int i =0; i <Layers.Count; i++)
			{
				if(DesName == Layers[i].LayerName)
				{
					return i;
				}
			}
			return -1; //failed. The value doesn't exist.
		}

		public void ImportLevel()
		{

		}

		public void ExportLevel()
		{

		}

	}
}
