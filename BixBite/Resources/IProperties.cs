using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Resources
{
	interface IProperties
	{
		//name, data
		ObservableDictionary<String, object> Properties { get; set; }

		/// <summary>
		/// Use this to MANUALLY set the properties dictionary
		/// </summary>
		void UpdateProperties();

		/// <summary>
		/// Uses this to clear all the properties in the dictionary
		/// </summary>
		void ClearProperties();

		/// <summary>
		/// Use this to add a property to the dictionary
		/// </summary>
		void AddProperty(String Pname, object data, );
	}
}
