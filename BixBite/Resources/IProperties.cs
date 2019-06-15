using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Resources
{
	interface IProperties
	{
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

		ObservableDictionary<String, object> getProperties();

		void setProperties(ObservableDictionary<String, object> newprops);
	}
}
