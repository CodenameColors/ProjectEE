using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BixBite.Resources
{
	interface IProperties
	{
		/// <summary>
		/// This structure contains all the property data.
		/// </summary>
		ObservableCollection<Tuple<string, object, Control>> Properties { get; set; }

		/// <summary>
		/// Use this to MANUALLY set the properties dictionary
		/// </summary>
		void UpdateProperties(Dictionary<String, object> newdict);

		/// <summary>
		/// Uses this to clear all the properties in the dictionary
		/// </summary>
		void ClearProperties();

		/// <summary>
		/// Use this to add a property to the dictionary
		/// </summary>
		void AddProperty(String Pname, object data);

		void SetProperty(String PName, object data);

		object GetProperty(String Pname);

		ObservableCollection<Tuple<String, object, Control>> getProperties();

		void setProperties(ObservableCollection<Tuple<String, object, Control>> newprops);
	}
}
