using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Combat
{
	public class WeakStrength
	{

		//public List<EWeaponType> StrongAgainst  = new List<EWeaponType>();
		//public List<EWeaponType> WeakAgainst = new List<EWeaponType>();

		//public List<EMagicType> StrongAgainst_Magic = new List<EMagicType>();
		//public List<EMagicType> WeakAgainst_magic = new List<EMagicType>();

		public int ID { get; set; }

		public int physical_weaknesses { get; set; }
		public int physical_strengths { get; set; }

		public int magic_weaknesses { get; set; }
		public int magic_strengths { get; set; }


		public WeakStrength()
		{

		}

		public EWeaponType IsStrongAgainst(EWeaponType desiredEWeaponType)
		{
			throw new NotImplementedException();
			//return EWeaponType.NONE;
		}

		public EWeaponType IsWeakAgainst(EWeaponType desiredEWeaponType)
		{
			throw new NotImplementedException();
			//return EWeaponType.NONE;
		}

		public EMagicType IsStrongAgainst(EMagicType desiredEWeaponType)
		{
			throw new NotImplementedException();
			//return EWeaponType.NONE;
		}

		public EMagicType IsWeakAgainst(EMagicType desiredEWeaponType)
		{
			throw new NotImplementedException();
			//return EWeaponType.NONE;
		}
	}

	public class weaknesses_strengths : WeakStrength
	{
	}

}
