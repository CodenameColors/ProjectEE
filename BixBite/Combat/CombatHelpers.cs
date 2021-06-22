using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Combat
{

	public enum ESize
	{
		NONE =0,
		Small=1,
		Medium=2,
		Large=3,
		Boss=4,

	}


	public class Item_Keys
	{
		public String Req_Name { get; set; }
		public String Req_Table { get; set; }
		public String Item_ID { get; set; }
		public bool   bDrop { get; set; }

	}


	public class Modifier_Keys
	{
		//public int Row_ID { get; set; }
		public String Req_Name { get; set; }
		public String Req_Table{ get; set; }
		public String Modifier_ID { get; set; }

	}


	public class PartyMember_Keys
	{
		public String Req_Name { get; set; }
		public String Req_Table { get; set; }
		public String PartyMember_ID { get; set; }

	}


	public class Skill_Keys
	{
		public String Req_Name { get; set; }
		public String Req_Table { get; set; }
		public String Skill_ID { get; set; }

	}


	public class Weapon_Keys
	{
		public String Req_Name { get; set; }
		public String Req_Table { get; set; }
		public String Weapon_ID { get; set; }

	}



}
