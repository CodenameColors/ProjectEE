using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Rendering.UI.ProgressBar
{
	public class BaseProgressBar : BaseUI
	{
		#region Delegates

		#endregion

		#region Properties

		public override int XPos
		{
			get => int.Parse(GetPropertyData("XPos").ToString());
			set
			{
				SetProperty("XPos", value);
				SetProperty("OriginX", value);
			}
		}

		public override int YPos
		{
			get => int.Parse(GetPropertyData("YPos").ToString());
			set
			{
				SetProperty("YPos", value);
				SetProperty("OriginY", value);
			}
		}


		public int XOffset
		{
			get => int.Parse(GetPropertyData("XOffset").ToString());
			set => SetProperty("XOffset", value);
		}

		public int YOffset
		{
			get => int.Parse(GetPropertyData("YOffset").ToString());
			set => SetProperty("YOffset", value);
		}

		public bool bBorder
		{
			get => bool.Parse(GetPropertyData("bBorder").ToString());
			set => SetProperty("bBorder", value);
		}

		public bool bInverseDirection
		{
			get => bool.Parse(GetPropertyData("bInverseDirection").ToString());
			set => SetProperty("bInverseDirection", value);
		}

		public bool bHorizontal
		{
			get => bool.Parse(GetPropertyData("bHorizontal").ToString());
			set => SetProperty("bHorizontal", value);
		}

		public virtual int CurrentVal
		{
			get => int.Parse(GetPropertyData("CurrentVal").ToString());
			set => SetProperty("CurrentVal", value);
		}

		public int MaxVal
		{
			get => int.Parse(GetPropertyData("MaxVal").ToString());
			set => SetProperty("MaxVal", value);
		}

		public int SourceWidth
		{
			get => int.Parse(GetPropertyData("SourceWidth").ToString());
			set => SetProperty("SourceWidth", value);
		}

		public int SourceHeight
		{
			get => int.Parse(GetPropertyData("SourceHeight").ToString());
			set => SetProperty("SourceHeight", value);
		}

		public int BorderWidth
		{
			get => int.Parse(GetPropertyData("BorderWidth").ToString());
			set => SetProperty("BorderWidth", value);
		}


		#endregion

		#region Fields
		protected int _maxWidth
		{
			get => int.Parse(GetPropertyData("MaxWidth").ToString());
			set => SetProperty("MaxWidth", value);
		}

		protected int _maxHeight
		{
			get => int.Parse(GetPropertyData("MaxHeight").ToString());
			set => SetProperty("MaxHeight", value);
		}

		protected int _originx
		{
			get => int.Parse(GetPropertyData("OriginX").ToString());
			set => SetProperty("OriginX", value);
		}

		protected int _originy
		{
			get => int.Parse(GetPropertyData("OriginY").ToString());
			set => SetProperty("OriginY", value);
		}

		#endregion

		#region Contructors
		public BaseProgressBar(string UIName, int xPos, int yPos, int width, int height, int zindex, bool border, 
			int borderWidth, int xOff, int yOff, int currentVal, int maxVal, bool bInverse, bool bHorizontal ) 
			: base(UIName, xPos, yPos, xOff, yOff, width, height, zindex)
		{

			

			AddProperty("MaxWidth", width);
			AddProperty("MaxHeight", height);
			AddProperty("XOffset", xOff);
			AddProperty("YOffset", yOff);
			AddProperty("OriginX", xPos);
			AddProperty("OriginY", yPos);
			AddProperty("bBorder", border);
			AddProperty("bInverseDirection", bInverse);
			AddProperty("bHorizontal", bHorizontal);

			AddProperty("BorderWidth", borderWidth);
			AddProperty("CurrentVal", currentVal);
			AddProperty("MaxVal", maxVal);


			if (bInverse)
			{
				AddProperty("SourceWidth", ((int)((currentVal / (float)maxVal) * width)));
				AddProperty("SourceHeight", Height);
			}
			else
			{
				AddProperty("SourceHeight", ((int)((currentVal / (float)maxVal) * Height)));
				AddProperty("SourceWidth", width);
			}
		}
		#endregion

		#region Methods

		#endregion


	}

}
