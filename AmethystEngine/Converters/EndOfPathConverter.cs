using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AmethystEngine.Converters
{
	public class EndOfPathConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string fullPath)
			{
				// Split the path using the directory separator
				string[] pathParts = fullPath.Split(System.IO.Path.DirectorySeparatorChar);

				// Get the last part of the path (file or folder name)
				if (pathParts.Length > 0)
				{
					return pathParts[pathParts.Length - 1];
				}
			}

			// Return the original value if it's not a valid string path
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
