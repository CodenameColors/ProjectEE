using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AmethystEngine.Components
{
	class PropertyBag
	{
		public String Name { get; set; }
		public ObservableCollection<Tuple<String, object, Control>> Properties { get; set; }

		public PropertyBag()
		{
			Properties = new ObservableCollection<Tuple<string, object, Control>>();
		}

	}
}
