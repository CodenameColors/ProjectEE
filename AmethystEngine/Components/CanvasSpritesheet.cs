using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BixBite.Resources;
using DrWPF.Windows.Data;

namespace AmethystEngine.Components
{

	public class CanvasImageProperties
	{

		public String ImageLocation { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public int W { get; set; }
		public int H { get; set; }
		public float SX { get; set; }
		public float SY { get; set; }

		public CanvasImageProperties()
		{

		}

		public CanvasImageProperties(String imageLocation, int width, int height)
		{
			this.ImageLocation = imageLocation;
			this.W = width;
			this.H = height;
		}


	}

	public class CanvasSpritesheet : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}

		public ObservableCollection<CanvasAnimation> AllAnimationOnSheet
		{
			get => _allCanvasAnimations;
			set
			{
				_allCanvasAnimations = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllAnimationOnSheet"));

			}
		}
		private  ObservableCollection<CanvasAnimation> _allCanvasAnimations = new ObservableCollection<CanvasAnimation>();

		public String Name { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }

		public CanvasSpritesheet(String name, int width, int height)
		{
			this.Name = name;
			this.Width = width;
			this.Height = height;
		}

	}


	public class CanvasAnimation : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
		
		public ObservableCollection<CanvasImageProperties> CanvasFrames
		{
			get => _canvasFrames;
			set
			{
				_canvasFrames = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanvasFrames"));
			}
		}
		private ObservableCollection<CanvasImageProperties> _canvasFrames = new ObservableCollection<CanvasImageProperties>();

		public String AnimName { get; set; }
		public uint NumOfFrames { get; set; }


		public CanvasAnimation(String name)
		{
			this.AnimName = name;
		}
	}
}
