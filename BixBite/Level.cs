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

		/// <summary>
		/// Changes the data of level object and moves it to a different existing layer.
		/// </summary>
		/// <param name="cellx">The desired row of tile array</param>
		/// <param name="celly">The desired column of the tile array</param>
		/// <param name="FromLayer">The current layer </param>
		/// <param name="DesLayer">The desired layer</param>
		/// <param name="DesiredLayerType">What type of layer is the current layer.</param>
		public void ChangeLayer(int cellx, int celly, int FromLayer, int DesLayer, LayerType DesiredLayerType)
		{
			//what type of sprite layer is this?
			if (DesiredLayerType == LayerType.Tile)
			{
				//does this cell contain data?
				int[,] tiledata = (int[,])Layers[FromLayer].LayerObjects;
				if (tiledata[cellx, celly] == 0) return;
				else
				{
					SpriteLayer deslayer = Layers[DesLayer]; //get the desired layer
					((int[,])deslayer.LayerObjects)[cellx, celly] = tiledata[cellx, celly];
					((int[,])Layers[FromLayer].LayerObjects)[cellx, celly] = 0; //clear the data
				}
			}
			else if (DesiredLayerType == LayerType.Sprite)
			{

			}
			else if (DesiredLayerType == LayerType.Gameobject)
			{
			}
		}

		/// <summary>
		/// Moves the data in the array from its original location, to the new location.
		/// </summary>
		/// <param name="cellx">Original Roww number</param>
		/// <param name="celly">Original column number</param>
		/// <param name="shiftx">How many rows to shift the cell over</param>
		/// <param name="shifty">How many columns to shift the cell over</param>
		/// <param name="CurLayer">the int value of the current layer in the array</param>
		/// <param name="layertype">What type of the sprite layer is this layer</param>
		public void Movedata(int cellx, int celly, int shiftx, int shifty, int CurLayer, LayerType layertype)
		{
			//what type of sprite layer is this?
			if (layertype == LayerType.Tile)
			{
				//does this cell contain data?
				int[,] tiledata = (int[,])Layers[CurLayer].LayerObjects;
				if (tiledata[cellx, celly] == 0) return;
				else
				{
					((int[,])Layers[CurLayer].LayerObjects)[cellx + shiftx, celly + shifty] = ((int[,])Layers[CurLayer].LayerObjects)[cellx, celly];
					((int[,])Layers[CurLayer].LayerObjects)[cellx, celly] = 0; //clear the data
				}
			}
			else if (layertype == LayerType.Sprite)
			{

			}
			else if (layertype == LayerType.Gameobject)
			{
			}
		}

		public void ImportLevel()
		{

		}

		public void ExportLevel()
		{

		}

	}
}
