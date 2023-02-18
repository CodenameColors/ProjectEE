using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Combat
{
	public enum ETargetingType
	{
		NONE,
		Enemies,
		Allies,
		Buff,
		Debuff,
		All,
	}

	public enum EAfflictionType
	{
		NONE,
		Damage,
		Heal,
		Buff,
		Debuff,
		Special,
	}

}
